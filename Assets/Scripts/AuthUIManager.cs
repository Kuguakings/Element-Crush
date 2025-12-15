using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// AuthUIManager: 专门管理登录界面的 UI 交互
/// 职责：处理登录/注册界面的输入、按钮点击、状态显示、面板切换
/// </summary>
public class AuthUIManager : MonoBehaviour
{
    [Header("UI 元素")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button registerButton;
    public Button loginButton;
    public TextMeshProUGUI statusText;

    [Header("面板控制")]
    public CanvasGroup loginCanvasGroup;
    public CanvasGroup mainMenuObjectGroup;
    public Button levelEditorButton;

    [Header("动画参数")]
    public float panelFadeDuration = 0.3f;

    private bool isInitialized = false;

    void Start()
    {
        InitializeUI();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // 订阅 TcbManager 事件
        if (TcbManager.instance != null)
        {
            TcbManager.instance.OnLoginSuccess += HandleLoginSuccess;
            TcbManager.instance.OnLoginFailed += HandleLoginFailed;
            TcbManager.instance.OnAuthStateChanged += HandleAuthStateChanged;
            TcbManager.instance.OnStatusMessageChanged += HandleStatusMessageChanged;
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        // 取消订阅事件
        if (TcbManager.instance != null)
        {
            TcbManager.instance.OnLoginSuccess -= HandleLoginSuccess;
            TcbManager.instance.OnLoginFailed -= HandleLoginFailed;
            TcbManager.instance.OnAuthStateChanged -= HandleAuthStateChanged;
            TcbManager.instance.OnStatusMessageChanged -= HandleStatusMessageChanged;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Debug.Log("[AuthUIManager] 进入主菜单，重新初始化 UI...");
            InitializeUI();
            UpdateUIState();
        }
    }

    private void InitializeUI()
    {
        Debug.Log("[AuthUIManager] 初始化 UI...");
        
        // 绑定 UI 组件
        BindUIComponentsSafe();
        
        // 设置按钮监听
        if (loginButton != null)
        {
            loginButton.onClick.RemoveAllListeners();
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }
        
        if (registerButton != null)
        {
            registerButton.onClick.RemoveAllListeners();
            registerButton.onClick.AddListener(OnRegisterButtonClicked);
        }

        isInitialized = true;
        
        // 初始化后立即更新 UI 状态
        UpdateUIState();
    }

    private void BindUIComponentsSafe()
    {
        // 如果已经绑定，跳过
        if (loginButton != null && emailInput != null && passwordInput != null)
        {
            return;
        }

        Debug.Log("[AuthUIManager] 正在安全绑定 UI 组件...");
        
        try
        {
            // 1. 查找 LoginPanel
            if (loginCanvasGroup == null)
            {
                var allCGs = Resources.FindObjectsOfTypeAll<CanvasGroup>();
                foreach (var cg in allCGs)
                {
                    if (cg.gameObject.scene.IsValid() && cg.gameObject.name == "LoginPanel")
                    {
                        loginCanvasGroup = cg;
                        break;
                    }
                }
            }

            // 2. 绑定 LoginPanel 内的元素
            if (loginCanvasGroup != null)
            {
                GameObject panelObj = loginCanvasGroup.gameObject;
                var allInputs = panelObj.GetComponentsInChildren<TMP_InputField>(true);

                // 优先按名字找
                emailInput = allInputs.FirstOrDefault(x => x.name == "Input_Account");
                passwordInput = allInputs.FirstOrDefault(x => x.name == "Input_Password");

                // 暴力兜底：按顺序找
                if (emailInput == null && allInputs.Length > 0)
                {
                    emailInput = allInputs[0];
                    Debug.LogWarning($"[AuthUIManager] 未找到 Input_Account，自动使用第一个输入框: {emailInput.name}");
                }
                if (passwordInput == null && allInputs.Length > 1)
                {
                    passwordInput = allInputs[1];
                    Debug.LogWarning($"[AuthUIManager] 未找到 Input_Password，自动使用第二个输入框: {passwordInput.name}");
                }

                var allBtns = panelObj.GetComponentsInChildren<Button>(true);
                loginButton = allBtns.FirstOrDefault(x => x.name == "Btn_Login");
                registerButton = allBtns.FirstOrDefault(x => x.name == "Btn_Register");
                
                var allTexts = panelObj.GetComponentsInChildren<TextMeshProUGUI>(true);
                statusText = allTexts.FirstOrDefault(x => x.name == "Text_Status");
            }
            else
            {
                Debug.LogError("[AuthUIManager] 【严重】找不到 LoginPanel！");
            }

            // 3. 查找 MainPanel
            if (mainMenuObjectGroup == null)
            {
                var allCGs = Resources.FindObjectsOfTypeAll<CanvasGroup>();
                foreach (var cg in allCGs)
                {
                    if (cg.gameObject.scene.IsValid() && cg.gameObject.name == "MainPanel")
                    {
                        mainMenuObjectGroup = cg;
                        break;
                    }
                }
            }

            // 4. 查找 LevelEditorButton
            if (levelEditorButton == null)
            {
                var allBtns = Resources.FindObjectsOfTypeAll<Button>();
                foreach (var btn in allBtns)
                {
                    if (btn.gameObject.scene.IsValid() && btn.gameObject.name == "LevelEditor_Button")
                    {
                        levelEditorButton = btn;
                        break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[AuthUIManager] UI 绑定异常: " + e.Message);
        }
    }

    private void OnLoginButtonClicked()
    {
        Debug.Log("[AuthUIManager] 点击了登录按钮...");
        TryAuthenticate(false);
    }

    private void OnRegisterButtonClicked()
    {
        Debug.Log("[AuthUIManager] 点击了注册按钮...");
        TryAuthenticate(true);
    }

    private void TryAuthenticate(bool isRegister)
    {
        // 确保输入框已绑定
        if (emailInput == null || passwordInput == null)
        {
            BindUIComponentsSafe();
        }

        if (emailInput == null || passwordInput == null)
        {
            UpdateStatusText("UI 错误：找不到输入框");
            return;
        }

        string email = emailInput.text;
        string password = passwordInput.text;

        // 验证输入
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            UpdateStatusText("账号或密码不能为空");
            return;
        }

        // 显示正在连接状态
        UpdateStatusText("正在连接...");

        // 调用 TcbManager 进行认证
        if (TcbManager.instance != null)
        {
            if (isRegister)
            {
                TcbManager.instance.Register(email, password);
            }
            else
            {
                TcbManager.instance.Login(email, password);
            }
        }
        else
        {
            UpdateStatusText("错误：找不到 TcbManager");
            Debug.LogError("[AuthUIManager] TcbManager.instance is null!");
        }
    }

    // === 事件处理方法 ===

    private void HandleLoginSuccess()
    {
        Debug.Log("[AuthUIManager] 收到登录成功事件");
        UpdateStatusText("登录成功，加载数据...");
    }

    private void HandleLoginFailed(string error)
    {
        Debug.LogError($"[AuthUIManager] 收到登录失败事件: {error}");
        UpdateStatusText("错误：" + error);
    }

    private void HandleAuthStateChanged(bool isLoggedIn)
    {
        Debug.Log($"[AuthUIManager] 认证状态改变: {isLoggedIn}");
        UpdateUIState();
    }

    private void HandleStatusMessageChanged(string message)
    {
        UpdateStatusText(message);
    }

    // === UI 更新方法 ===

    public void UpdateUIState()
    {
        if (!isInitialized) return;

        // 双重保险：如果引用丢了，再找一次
        if (loginCanvasGroup == null || mainMenuObjectGroup == null)
        {
            BindUIComponentsSafe();
        }

        bool isLoggedIn = TcbManager.isLoggedIn;

        if (isLoggedIn)
        {
            Debug.Log("[AuthUIManager] 已登录，隐藏登录框，显示主菜单。");
            
            // 已登录：关登录页，开主页
            SetCanvasGroupState(loginCanvasGroup, false);
            SetCanvasGroupState(mainMenuObjectGroup, true);

            // 更新用户资料显示
            var profile = FindObjectOfType<UserProfileManager>();
            if (profile != null) profile.UpdateUI();

            // 根据管理员状态显示/隐藏关卡编辑器按钮
            if (levelEditorButton)
            {
                levelEditorButton.gameObject.SetActive(TcbManager.IsAdmin);
            }
        }
        else
        {
            Debug.Log("[AuthUIManager] 未登录，显示登录框。");
            
            // 未登录：开登录页，关主页
            SetCanvasGroupState(loginCanvasGroup, true);
            SetCanvasGroupState(mainMenuObjectGroup, false);

            UpdateStatusText("请输入账号密码");
        }
    }

    public void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    private void SetCanvasGroupState(CanvasGroup cg, bool visible)
    {
        if (cg == null) return;
        
        cg.alpha = visible ? 1 : 0;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
        cg.gameObject.SetActive(visible);
    }

    /// <summary>
    /// 执行面板切换动画（从登录界面到主菜单）
    /// </summary>
    public void TransitionToMainMenu()
    {
        if (loginCanvasGroup != null && mainMenuObjectGroup != null)
        {
            StartCoroutine(TransitionTo(loginCanvasGroup, mainMenuObjectGroup));
        }
    }

    private IEnumerator TransitionTo(CanvasGroup from, CanvasGroup to)
    {
        yield return StartCoroutine(FadeCanvasGroup(from, 1, 0, panelFadeDuration));
        yield return StartCoroutine(FadeCanvasGroup(to, 0, 1, panelFadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float dur)
    {
        if (cg == null) yield break;
        
        if (start > 0 || end > 0)
        {
            cg.gameObject.SetActive(true);
        }
        
        cg.interactable = false;
        float t = 0f;
        
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, end, t / dur);
            yield return null;
        }
        
        cg.alpha = end;
        bool isVisible = (end > 0.01f);
        cg.interactable = isVisible;
        cg.blocksRaycasts = isVisible;
        
        if (!isVisible)
        {
            cg.gameObject.SetActive(false);
        }
    }
}
