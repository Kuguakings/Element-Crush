using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// LevelEditor_Core: 关卡编辑器总控
/// 职责：
/// 1. 权限检查
/// 2. 模式切换导航  
/// 3. WebGL回调分发
/// 4. 公共工具（statusText等）
/// </summary>
public class LevelEditor_Core : MonoBehaviour
{
    [Header("子控制器引用")]
    public LevelEditor_Mode1 mode1Controller;
    public LevelEditor_Mode2 mode2Controller;

    [Header("公共 UI")]
    public Button backToMenuButton;
    public TextMeshProUGUI statusText;

    [Header("主面板导航")]
    public GameObject mainPanel;
    public TMP_Dropdown modeSelectDropdown;
    public GameObject mode1EditorPanel;
    public GameObject mode2EditorPanel;

    [Header("动画参数")]
    public float panelFadeDuration = 0.3f;

    [Header("编辑器测试")]
    [Tooltip("【仅编辑器模式】: 用于在未从 MainMenu 启动时自动创建 LevelManager。")]
    public GameObject levelManagerPrefab;

    private CanvasGroup mainPanelCG;
    private CanvasGroup mode1EditorPanelCG;
    private CanvasGroup mode2EditorPanelCG;

    void Start()
    {
        // 1. 编辑器测试模式修复
#if UNITY_EDITOR
        Debug.LogWarning("【编辑器测试模式】：自动将权限设为 Admin。");
        TcbManager.IsAdmin = true;

        if (TcbManager.AllLevels == null)
        {
            Debug.LogWarning("【编辑器测试模式】：TcbManager.AllLevels 为空，已自动初始化。");
            TcbManager.AllLevels = new LevelDataCollection();
        }

        if (LevelManager.instance == null)
        {
            Debug.LogWarning("【编辑器测试模式】：LevelManager 实例未找到，正在从 Prefab 自动创建。");
            if (levelManagerPrefab != null)
            {
                GameObject lmGO = Instantiate(levelManagerPrefab);
                LevelManager newLM = lmGO.GetComponent<LevelManager>();
                if (newLM != null)
                {
                    newLM.ManuallyTriggerFadeOut();
                }
            }
            else
            {
                Debug.LogError("【编辑器测试模式】: 严重错误! LevelManager Prefab 未在 LevelEditor_Core 中设置! 试玩功能将失败。");
            }
        }
#endif

        // 2. 初始化 CanvasGroups
        mainPanelCG = mainPanel.GetComponent<CanvasGroup>() ?? mainPanel.AddComponent<CanvasGroup>();
        mode1EditorPanelCG = mode1EditorPanel.GetComponent<CanvasGroup>() ?? mode1EditorPanel.AddComponent<CanvasGroup>();
        mode2EditorPanelCG = mode2EditorPanel.GetComponent<CanvasGroup>() ?? mode2EditorPanel.AddComponent<CanvasGroup>();

        // 3. 权限检查
        if (!TcbManager.IsAdmin)
        {
            SetStatus("错误：您没有管理员权限，无法使用此编辑器！", Color.red);
            if (modeSelectDropdown != null) modeSelectDropdown.gameObject.SetActive(false);
            if (mode1EditorPanel != null) mode1EditorPanel.gameObject.SetActive(false);
            if (mode2EditorPanel != null) mode2EditorPanel.gameObject.SetActive(false);
            return;
        }
        else
        {
            SetStatus("管理员您好，请选择您要创建的关卡模式。", Color.white);
        }

        // 4. 按钮绑定
        if (backToMenuButton != null) 
            backToMenuButton.onClick.AddListener(GoToMainMenu);

        // 5. 设置模式选择下拉框
        SetupModeDropdown();

        // 6. 初始化子控制器
        if (mode1Controller != null) 
            mode1Controller.Initialize(this);
        if (mode2Controller != null) 
            mode2Controller.Initialize(this);

        // 7. 处理从试玩返回的情况
        HandleReturnFromTestPlay();
    }

    private void SetupModeDropdown()
    {
        if (modeSelectDropdown == null) return;

        modeSelectDropdown.ClearOptions();
        modeSelectDropdown.AddOptions(new System.Collections.Generic.List<string> 
        { 
            "请选择模式...", 
            "模式 1: 单词消消乐", 
            "模式 2: 词语连连看" 
        });
        modeSelectDropdown.onValueChanged.AddListener(OnModeSelected);

        if (mode1EditorPanel != null) mode1EditorPanel.gameObject.SetActive(false);
        if (mode2EditorPanel != null) mode2EditorPanel.gameObject.SetActive(false);
    }

    private void OnModeSelected(int index)
    {
        if (index == 0)
        {
            ResetToModeSelect();
        }
        else if (index == 1)
        {
            // 进入模式1
            StartCoroutine(TransitionTo(mainPanelCG, mode1EditorPanelCG, () => {
                if (mode1Controller != null)
                    mode1Controller.EnterMode();
            }));
        }
        else if (index == 2)
        {
            // 进入模式2
            StartCoroutine(TransitionTo(mainPanelCG, mode2EditorPanelCG, () => {
                if (mode2Controller != null)
                    mode2Controller.EnterMode();
            }));
        }
    }

    public void ResetToModeSelect()
    {
        CanvasGroup panelToHide = null;

        if (mode1EditorPanelCG.alpha > 0) 
            panelToHide = mode1EditorPanelCG;
        else if (mode2EditorPanelCG.alpha > 0) 
            panelToHide = mode2EditorPanelCG;

        if (panelToHide != null && panelToHide != mainPanelCG)
        {
            StartCoroutine(TransitionTo(panelToHide, mainPanelCG));
        }
        else
        {
            ShowPanelInstant(mainPanelCG);
        }

        if (modeSelectDropdown != null) 
            modeSelectDropdown.value = 0;
        
        SetStatus("请选择您要创建的关卡模式。");
    }

    private void HandleReturnFromTestPlay()
    {
        if (!LevelManager.justReturnedFromTest)
        {
            // 正常启动，显示主面板
            ShowPanelInstant(mainPanelCG);
            SetStatus("管理员您好，请选择您要创建的关卡模式。");
            if (modeSelectDropdown != null) modeSelectDropdown.value = 0;
            return;
        }

        Debug.Log("检测到从试玩返回！");

        if (LevelManager.selectedGameMode == GameMode.WordMatch3)
        {
            ShowPanelInstant(mode1EditorPanelCG);
            
            LevelData dataToOpen = TcbManager.AllLevels.levels.Find(l => l.id == LevelManager.selectedLevelData.id);
            if (dataToOpen != null && mode1Controller != null)
                mode1Controller.SelectLevelInstant(dataToOpen);
            else if (mode1Controller != null)
                mode1Controller.SelectLevelInstant(LevelManager.selectedLevelData);

            if (LevelManager.justCompletedTestPlay)
                SetStatus("试玩通过！现在可以发布了。");
            else
                SetStatus("已从试玩返回。");
        }
        else if (LevelManager.selectedGameMode == GameMode.WordLinkUp)
        {
            ShowPanelInstant(mode2EditorPanelCG);
            
            LevelData dataToOpen = TcbManager.AllLevels.levels.Find(l => l.id == LevelManager.selectedLevelData.id);
            if (dataToOpen != null && mode2Controller != null)
                mode2Controller.SelectLevelInstant(dataToOpen);
            else if (mode2Controller != null)
                mode2Controller.SelectLevelInstant(LevelManager.selectedLevelData);

            if (LevelManager.justCompletedTestPlay)
                SetStatus("试玩通过！现在可以发布了。");
            else
                SetStatus("已从试玩返回。");
        }

        LevelManager.justReturnedFromTest = false;
        LevelManager.justCompletedTestPlay = false;
    }

    // ========================================
    // 公共工具方法
    // ========================================

    public void SetStatus(string message, Color? color = null)
    {
        if (statusText != null)
        {
            statusText.text = message;
            if (color.HasValue)
                statusText.color = color.Value;
        }
        Debug.Log($"[LevelEditor_Core] Status: {message}");
    }

    public void GoToMainMenu()
    {
        Debug.Log("[LevelEditor_Core] 返回主菜单");
        SceneManager.LoadScene("MainMenu");
    }

    // ========================================
    // WebGL 回调分发
    // ========================================

    /// <summary>
    /// WebGL 错误回调
    /// </summary>
    public void OnHtmlInputError(string error)
    {
        Debug.LogError($"[LevelEditor_Core] OnHtmlInputError: {error}");
        SetStatus("操作失败: " + error, Color.red);
    }

    /// <summary>
    /// 接收 M1 从 JS 浮层发回的粘贴数据
    /// </summary>
    public void M1_ReceivePastedTextFromHtml(string pastedText)
    {
        Debug.Log("[LevelEditor_Core] M1 成功接收到 JS 文本，转发给 Mode1Controller");
        if (mode1Controller != null)
            mode1Controller.OnPasteReceived(pastedText);
        else
            SetStatus("错误：Mode1Controller 未初始化", Color.red);
    }

    /// <summary>
    /// 接收 M2 从 JS 浮层发回的粘贴数据
    /// </summary>
    public void M2_ReceivePastedTextFromHtml(string pastedText)
    {
        Debug.Log("[LevelEditor_Core] M2 成功接收到 JS 文本，转发给 Mode2Controller");
        if (mode2Controller != null)
            mode2Controller.OnPasteReceived(pastedText);
        else
            SetStatus("错误：Mode2Controller 未初始化", Color.red);
    }

    /// <summary>
    /// 接收 M1 从 JS 浮层发回的单元格编辑数据
    /// </summary>
    public void M1_ReceiveCellEditText(string newText)
    {
        Debug.Log($"[LevelEditor_Core] M1 接收到单元格编辑文本，转发给 Mode1Controller");
        if (mode1Controller != null)
            mode1Controller.OnCellEditReceived(newText);
        else
            SetStatus("错误：Mode1Controller 未初始化", Color.red);
    }

    /// <summary>
    /// 上传成功回调
    /// </summary>
    public void OnUploadSuccessCallback(string message)
    {
        Debug.Log($"[LevelEditor_Core] 上传成功: {message}");
        SetStatus("上传成功！", Color.green);
    }

    // ========================================
    // 面板切换动画
    // ========================================

    private void ShowPanelInstant(CanvasGroup cg)
    {
        if (cg == null) return;

        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        if (cg.gameObject != null) 
            cg.gameObject.SetActive(true);
    }

    private void HidePanelInstant(CanvasGroup cg)
    {
        if (cg == null) return;

        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        if (cg.gameObject != null) 
            cg.gameObject.SetActive(false);
    }

    public IEnumerator TransitionTo(CanvasGroup from, CanvasGroup to, System.Action onComplete = null)
    {
        yield return StartCoroutine(FadeCanvasGroup(from, 1, 0, panelFadeDuration));
        yield return StartCoroutine(FadeCanvasGroup(to, 0, 1, panelFadeDuration));
        onComplete?.Invoke();
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float dur)
    {
        if (cg == null) yield break;

        if (start > 0 || end > 0)
        {
            if (cg.gameObject != null) 
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
            if (cg.gameObject != null) 
                cg.gameObject.SetActive(false);
        }
    }
}
