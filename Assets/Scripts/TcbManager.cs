using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// =========================================================
// 1. 数据结构定义 (完整保留)
// =========================================================

[Serializable]
public class AdminData
{
    public string _id;
    public string name;
    public int level;
}

[Serializable]
public class UserProfileData
{
    public string nickname;
    public string role;
}

[Serializable]
public class DbResponseWrapper<T>
{
    public List<T> data;
    public string requestId;
}

[Serializable]
public class SingleDocResponse<T>
{
    public T data;
}

[Serializable]
public class Mode1Content
{
    public int row;
    public int col;
    public string text;
    public int groupId; // 兼容旧数据
    public string hanzi; // 兼容旧数据
    public string pinyin; // 兼容旧数据
    public string english; // 兼容旧数据
}

[Serializable]
public class Mode2Content
{
    public int sentenceId;
    public int wordOrder;
    public string wordText;
    public string fullSentence;
}

[Serializable]
public class LevelData
{
    public string id;
    public string chapter;
    public int mode;
    public int level;
    public List<Mode1Content> content_mode_1 = new List<Mode1Content>();
    public List<Mode2Content> content_mode_2 = new List<Mode2Content>();
    public string editorStatus = "Working";
}

[Serializable]
public class LevelDataCollection
{
    public List<LevelData> levels = new List<LevelData>();
}

// =========================================================
// 2. TcbManager 主类
// =========================================================

public class TcbManager : MonoBehaviour
{
    public static TcbManager instance;

    // 全局静态状态
    public static bool isLoggedIn = false;
    public static bool IsAdmin = false;
    public static int AdminLevel = 0;
    public static string CurrentUid = "";
    public static string CurrentNickname = "";

    public static LevelDataCollection AllLevels;

    // 持久化 Key
    private const string PREF_AUTO_LOGIN_UID = "AutoLogin_UID";
    private const string PREF_AUTO_LOGIN_NICKNAME = "AutoLogin_Nickname";
    private const string PREF_IS_ADMIN = "AutoLogin_IsAdmin";

    // 事件系统：用于通知 UI 更新
    public event Action OnLoginSuccess;
    public event Action<string> OnLoginFailed;
    public event Action<bool> OnAuthStateChanged; // 参数为 isLoggedIn
    public event Action<string> OnStatusMessageChanged;

    private Dictionary<string, Action<string>> dbSuccessCallbacks = new Dictionary<string, Action<string>>();
    private Dictionary<string, Action<string>> dbErrorCallbacks = new Dictionary<string, Action<string>>();

    // =========================================================
    // 3. 生命周期与初始化
    // =========================================================

    void Awake()
    {
        // 【关键修复】强制时间正常流逝，防止从暂停状态返回时卡死
        Time.timeScale = 1f;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
#if UNITY_EDITOR
        Debug.LogWarning("--- UNITY EDITOR 模式 ---");
        IsAdmin = true; 
        AdminLevel = 999; 
        CurrentUid = "test_editor_user_001"; 
        CurrentNickname = "Editor Admin";
        if (LevelManager.instance != null) LevelManager.IsAdmin = true;
        
        if (AllLevels == null) AllLevels = new LevelDataCollection();
        isLoggedIn = true;
        
        // 触发事件
        OnAuthStateChanged?.Invoke(true);
#else
        // 【核心修复】真机/WebGL 模式下，手动触发一次场景加载逻辑
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Debug.Log("[TcbManager] 手动触发主菜单初始化...");
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
#endif
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 每次切场景都确保时间正常
        Time.timeScale = 1f;

        if (scene.name == "MainMenu")
        {
            Debug.Log($"[TcbManager] 进入主菜单。当前登录状态: {isLoggedIn}");

            // 检查自动登录
            if (!isLoggedIn)
            {
                CheckAutoLogin();
            }
            else
            {
                // 触发事件，通知 UI 更新
                OnAuthStateChanged?.Invoke(true);
            }
        }
    }



    private void CheckAutoLogin()
    {
        string savedUid = PlayerPrefs.GetString(PREF_AUTO_LOGIN_UID, "");
        if (!string.IsNullOrEmpty(savedUid))
        {
            Debug.Log($"[TcbManager] 检测到自动登录 UID: {savedUid}");
            CurrentUid = savedUid;
            CurrentNickname = PlayerPrefs.GetString(PREF_AUTO_LOGIN_NICKNAME, "学员");
            IsAdmin = PlayerPrefs.GetInt(PREF_IS_ADMIN, 0) == 1;

            if (LevelManager.instance != null) LevelManager.IsAdmin = IsAdmin;

            // 标记为已登录
            isLoggedIn = true;
            
            // 触发事件
            OnAuthStateChanged?.Invoke(true);

            // 触发静默校验（不挡UI）
            SilentReauth();
            
            // 加载关卡数据
            if (AllLevels == null || AllLevels.levels.Count == 0)
            {
                LoadLevelsSilent();
            }
        }
    }

    private void SilentReauth()
    {
        NativeBridge.Instance.CheckAdminStatus(CurrentUid, "OnAdminCheckResult_Silent", gameObject.name, "OnAuthError_Silent");
        NativeBridge.Instance.GetUserProfile(CurrentUid, gameObject.name, "OnGetUserProfileSuccess", "OnAuthError_Silent");
    }

    // 静默回调
    public void OnAdminCheckResult_Silent(string jsonOrEmpty) { OnAdminCheckResult(jsonOrEmpty); }
    public void OnAuthError_Silent(string err) { Debug.LogWarning("静默更新失败: " + err); }




    /// <summary>
    /// 注册新用户
    /// </summary>
    public void Register(string email, string password)
    {
        Debug.Log($"[TcbManager] 开始注册: {email}");
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            OnLoginFailed?.Invoke("账号或密码不能为空");
            return;
        }
        
        NativeBridge.Instance.RegisterUser(email, password, "OnLoginOrRegisterSuccess", gameObject.name, "OnAuthError");
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    public void Login(string email, string password)
    {
        Debug.Log($"[TcbManager] 开始登录: {email}");
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            OnLoginFailed?.Invoke("账号或密码不能为空");
            return;
        }
        
        NativeBridge.Instance.LoginUser(email, password, "OnLoginOrRegisterSuccess", gameObject.name, "OnAuthError");
    }

    public void OnLoginOrRegisterSuccess(string uid)
    {
        Debug.Log($"[TcbManager] 登录/注册成功: {uid}");
        
        CurrentUid = uid;
        
        // 保存自动登录信息
        PlayerPrefs.SetString(PREF_AUTO_LOGIN_UID, uid);
        PlayerPrefs.Save();
        
        // 触发事件
        OnLoginSuccess?.Invoke();
        OnStatusMessageChanged?.Invoke("登录成功，加载数据...");

        // 检查管理员状态和用户信息
        NativeBridge.Instance.CheckAdminStatus(uid, "OnAdminCheckResult", gameObject.name, "OnAuthError");
        NativeBridge.Instance.GetUserProfile(uid, gameObject.name, "OnGetUserProfileSuccess", "OnAuthError");
    }

    public void OnGetUserProfileSuccess(string json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            var data = JsonUtility.FromJson<UserProfileData>(json);
            CurrentNickname = data.nickname;
            PlayerPrefs.SetString(PREF_AUTO_LOGIN_NICKNAME, CurrentNickname);
            PlayerPrefs.Save();

            var p = FindObjectOfType<UserProfileManager>();
            if (p) p.UpdateUI();
        }
        else
        {
            string defaultName = "学员_" + CurrentUid.Substring(0, 4);
            NativeBridge.Instance.CreateUserProfile(CurrentUid, defaultName, "OnCreateProfileSuccess", gameObject.name, "OnAuthError");
        }
    }

    public void OnCreateProfileSuccess(string msg)
    {
        NativeBridge.Instance.GetUserProfile(CurrentUid, gameObject.name, "OnGetUserProfileSuccess", "OnAuthError");
    }

    public void RequestUpdateUsername(string newName)
    {
        NativeBridge.Instance.UpdateUsername(CurrentUid, newName, "OnUpdateNameSuccess", gameObject.name, "OnAuthError");
    }
    public void OnUpdateNameSuccess(string msg) { RequestUpdateUsername(CurrentUid); }

    public void OnAdminCheckResult(string jsonOrEmpty)
    {
        if (!string.IsNullOrEmpty(jsonOrEmpty))
        {
            IsAdmin = true;
            if (LevelManager.instance != null) LevelManager.IsAdmin = true;
            try { var d = JsonUtility.FromJson<AdminData>(jsonOrEmpty); AdminLevel = d.level; } catch { AdminLevel = 1; }
            Debug.Log($"[TcbManager] 管理员权限确认: Level {AdminLevel}");
        }
        else
        {
            IsAdmin = false;
            AdminLevel = 0;
            if (LevelManager.instance != null) LevelManager.IsAdmin = false;
            Debug.Log("[TcbManager] 非管理员用户");
        }

        PlayerPrefs.SetInt(PREF_IS_ADMIN, IsAdmin ? 1 : 0);
        PlayerPrefs.Save();

        // 通知其他系统更新 UI
        var p = FindObjectOfType<UserProfileManager>();
        if (p) p.UpdateUI();
        
        // 触发事件，通知 UI 更新
        OnAuthStateChanged?.Invoke(isLoggedIn);

        // 加载关卡数据
        LoadLevelsSilent();
    }

    private void LoadLevelsSilent()
    {
        NativeBridge.Instance.GetLevels(gameObject.name, "OnGetLevelsSuccess", "OnAuthError");
    }

    public void OnGetLevelsSuccess(string jsonString)
    {
        try
        {
            AllLevels = JsonUtility.FromJson<LevelDataCollection>(jsonString);
            if (AllLevels == null) AllLevels = new LevelDataCollection();
            Debug.Log($"[TcbManager] 关卡数据加载成功: {AllLevels.levels.Count} 个关卡");
        }
        catch (Exception e)
        {
            OnAuthError("Data Error: " + e.Message);
            return;
        }

        isLoggedIn = true;
        
        // 触发事件，通知 UI 更新
        OnAuthStateChanged?.Invoke(true);
    }

    public void OnAuthError(string error)
    {
        Debug.LogError($"[TcbManager] Auth Error: {error}");
        
        // 触发事件
        OnLoginFailed?.Invoke(error);
        OnStatusMessageChanged?.Invoke("错误：" + error);
    }

    public void UploadNewLevel(string docId, LevelData data)
    {
        if (!IsAdmin) return;
        string jsonData = JsonUtility.ToJson(data);
        NativeBridge.Instance.UploadNewLevel(docId, jsonData, gameObject.name, "OnUploadSuccess", "OnAuthError");
    }

    public void OnUploadSuccess(string message)
    {
        var editor = FindObjectOfType<LevelEditorManager>();
        if (editor != null) editor.OnUploadSuccessCallback(message);
    }

    // =========================================================
    // 6. 通用数据库操作
    // =========================================================
    public void GetCollectionData<T>(string c, Action<List<T>> s, Action<string> e = null)
    {
        string rId = Guid.NewGuid().ToString();
        RegisterCallbacks(rId, (j) => { try { string w = "{\"data\":" + j + "}"; var r = JsonUtility.FromJson<DbResponseWrapper<T>>(w); s?.Invoke(r.data); } catch (Exception ex) { e?.Invoke(ex.Message); } }, e);
        NativeBridge.Instance.DbGetCollection(c, rId, "OnDbGenericSuccess", gameObject.name, "OnDbGenericError");
    }

    public void SetDocument<T>(string c, string d, T data, Action s = null, Action<string> e = null)
    {
        string rId = Guid.NewGuid().ToString(); TrySetId(data, d);
        RegisterCallbacks(rId, (m) => s?.Invoke(), e);
        NativeBridge.Instance.DbSetDocument(c, d, JsonUtility.ToJson(data), rId, "OnDbGenericSuccess", gameObject.name, "OnDbGenericError");
    }

    public void AddDocument<T>(string c, T data, Action s = null, Action<string> e = null)
    {
        string rId = Guid.NewGuid().ToString(); string d = TryGetId(data); if (string.IsNullOrEmpty(d)) d = Guid.NewGuid().ToString(); TrySetId(data, d);
        RegisterCallbacks(rId, (m) => s?.Invoke(), e);
        NativeBridge.Instance.DbAddDocument(c, JsonUtility.ToJson(data), rId, "OnDbGenericSuccess", gameObject.name, "OnDbGenericError");
    }

    public void DeleteDocument(string c, string d, Action s = null, Action<string> e = null)
    {
        string rId = Guid.NewGuid().ToString();
        RegisterCallbacks(rId, (m) => s?.Invoke(), e);
        NativeBridge.Instance.DbDeleteDocument(c, d, rId, "OnDbGenericSuccess", gameObject.name, "OnDbGenericError");
    }

    public void GetDocument<T>(string c, string d, Action<T> s, Action<string> e = null)
    {
        string rId = Guid.NewGuid().ToString();
        RegisterCallbacks(rId, (j) => { try { s?.Invoke(JsonUtility.FromJson<T>(j)); } catch { s?.Invoke(default(T)); } }, e);
        NativeBridge.Instance.DbGetDocument(c, d, rId, "OnDbGenericSuccess", gameObject.name, "OnDbGenericError");
    }

    private void TrySetId<T>(T data, string id) { try { var f = typeof(T).GetField("_id") ?? typeof(T).GetField("id"); if (f != null) f.SetValue(data, id); } catch { } }
    private string TryGetId<T>(T data) { try { var f = typeof(T).GetField("_id") ?? typeof(T).GetField("id"); if (f != null) return f.GetValue(data) as string; } catch { } return null; }

    public void OnDbGenericSuccess(string p)
    {
        int i = p.IndexOf('|'); if (i < 0) return;
        string id = p.Substring(0, i); string data = p.Substring(i + 1);
        if (dbSuccessCallbacks.ContainsKey(id)) { dbSuccessCallbacks[id]?.Invoke(data); CleanupCallbacks(id); }
    }

    public void OnDbGenericError(string p)
    {
        int i = p.IndexOf('|'); if (i < 0) return;
        string id = p.Substring(0, i); string err = p.Substring(i + 1);
        if (dbErrorCallbacks.ContainsKey(id)) { dbErrorCallbacks[id]?.Invoke(err); CleanupCallbacks(id); }
    }

    private void RegisterCallbacks(string r, Action<string> s, Action<string> e) { dbSuccessCallbacks[r] = s; dbErrorCallbacks[r] = e; }
    private void CleanupCallbacks(string r) { dbSuccessCallbacks.Remove(r); dbErrorCallbacks.Remove(r); }
}