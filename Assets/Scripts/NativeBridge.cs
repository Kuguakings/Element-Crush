using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// NativeBridge: 游戏与 WebGL 浏览器环境交互的统一接口
/// 所有与 JavaScript 的交互都通过这个单例类进行
/// </summary>
public class NativeBridge : MonoBehaviour
{
    private static NativeBridge _instance;
    
    public static NativeBridge Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("NativeBridge");
                _instance = go.AddComponent<NativeBridge>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region DllImport 定义 (仅在此处出现)

    // === TcbManager 相关 ===
    [DllImport("__Internal")] private static extern void JsRegisterUser(string e, string p, string o, string s, string r);
    [DllImport("__Internal")] private static extern void JsLoginUser(string e, string p, string o, string s, string r);
    [DllImport("__Internal")] private static extern void JsLogoutUser();
    [DllImport("__Internal")] private static extern void JsCheckAdminStatus(string u, string o, string s, string r);
    [DllImport("__Internal")] private static extern void JsGetLevels(string o, string s, string r);
    [DllImport("__Internal")] private static extern void JsUploadNewLevel(string d, string j, string o, string s, string r);
    [DllImport("__Internal")] private static extern void JsGetUserProfile(string u, string o, string s, string r);
    [DllImport("__Internal")] private static extern void JsCreateUserProfile(string u, string n, string o, string s, string r);
    [DllImport("__Internal")] private static extern void JsUpdateUsername(string u, string n, string o, string s, string r);
    [DllImport("__Internal")] private static extern void JsDbGetCollection(string coll, string reqId, string o, string s, string e);
    [DllImport("__Internal")] private static extern void JsDbSetDocument(string coll, string docId, string json, string reqId, string o, string s, string e);
    [DllImport("__Internal")] private static extern void JsDbAddDocument(string coll, string json, string reqId, string o, string s, string e);
    [DllImport("__Internal")] private static extern void JsDbDeleteDocument(string coll, string docId, string reqId, string o, string s, string e);
    [DllImport("__Internal")] private static extern void JsDbGetDocument(string coll, string docId, string reqId, string o, string s, string e);

    // === 原生输入框相关 (HtmlInputBridge, UserProfileManager, LevelEditorManager, AnnouncementManager) ===
    [DllImport("__Internal")] private static extern void JsShowNativePrompt(string existingText, string objectName, string callbackSuccess);

    #endregion

    #region 公开封装方法

    // === 用户认证相关 ===
    
    /// <summary>
    /// 注册新用户
    /// </summary>
    public void RegisterUser(string email, string password, string onSuccessCallback, string onSuccessObj, string onRejectObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsRegisterUser(email, password, onSuccessCallback, onSuccessObj, onRejectObj);
#else
        Debug.Log($"[NativeBridge] 模拟注册用户: {email}");
#endif
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    public void LoginUser(string email, string password, string onSuccessCallback, string onSuccessObj, string onRejectObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsLoginUser(email, password, onSuccessCallback, onSuccessObj, onRejectObj);
#else
        Debug.Log($"[NativeBridge] 模拟登录用户: {email}");
#endif
    }

    /// <summary>
    /// 用户登出
    /// </summary>
    public void LogoutUser()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsLogoutUser();
#else
        Debug.Log("[NativeBridge] 模拟登出用户");
#endif
    }

    /// <summary>
    /// 检查管理员状态
    /// </summary>
    public void CheckAdminStatus(string userId, string onSuccessCallback, string onSuccessObj, string onRejectObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsCheckAdminStatus(userId, onSuccessCallback, onSuccessObj, onRejectObj);
#else
        Debug.Log($"[NativeBridge] 模拟检查管理员状态: {userId}");
#endif
    }

    // === 关卡相关 ===

    /// <summary>
    /// 获取所有关卡
    /// </summary>
    public void GetLevels(string onSuccessCallback, string onSuccessObj, string onRejectObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsGetLevels(onSuccessCallback, onSuccessObj, onRejectObj);
#else
        Debug.Log("[NativeBridge] 模拟获取关卡列表");
#endif
    }

    /// <summary>
    /// 上传新关卡
    /// </summary>
    public void UploadNewLevel(string levelData, string levelJson, string onSuccessCallback, string onSuccessObj, string onRejectObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsUploadNewLevel(levelData, levelJson, onSuccessCallback, onSuccessObj, onRejectObj);
#else
        Debug.Log($"[NativeBridge] 模拟上传新关卡: {levelData}");
#endif
    }

    // === 用户档案相关 ===

    /// <summary>
    /// 获取用户档案
    /// </summary>
    public void GetUserProfile(string userId, string onSuccessCallback, string onSuccessObj, string onRejectObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsGetUserProfile(userId, onSuccessCallback, onSuccessObj, onRejectObj);
#else
        Debug.Log($"[NativeBridge] 模拟获取用户档案: {userId}");
#endif
    }

    /// <summary>
    /// 创建用户档案
    /// </summary>
    public void CreateUserProfile(string userId, string username, string onSuccessCallback, string onSuccessObj, string onRejectObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsCreateUserProfile(userId, username, onSuccessCallback, onSuccessObj, onRejectObj);
#else
        Debug.Log($"[NativeBridge] 模拟创建用户档案: {userId}, {username}");
#endif
    }

    /// <summary>
    /// 更新用户名
    /// </summary>
    public void UpdateUsername(string userId, string newUsername, string onSuccessCallback, string onSuccessObj, string onRejectObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsUpdateUsername(userId, newUsername, onSuccessCallback, onSuccessObj, onRejectObj);
#else
        Debug.Log($"[NativeBridge] 模拟更新用户名: {userId}, {newUsername}");
#endif
    }

    // === 数据库操作相关 ===

    /// <summary>
    /// 获取集合
    /// </summary>
    public void DbGetCollection(string collectionName, string requestId, string onSuccessCallback, string onSuccessObj, string onErrorObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsDbGetCollection(collectionName, requestId, onSuccessCallback, onSuccessObj, onErrorObj);
#else
        Debug.Log($"[NativeBridge] 模拟获取集合: {collectionName}");
#endif
    }

    /// <summary>
    /// 设置文档
    /// </summary>
    public void DbSetDocument(string collectionName, string docId, string jsonData, string requestId, string onSuccessCallback, string onSuccessObj, string onErrorObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsDbSetDocument(collectionName, docId, jsonData, requestId, onSuccessCallback, onSuccessObj, onErrorObj);
#else
        Debug.Log($"[NativeBridge] 模拟设置文档: {collectionName}/{docId}");
#endif
    }

    /// <summary>
    /// 添加文档
    /// </summary>
    public void DbAddDocument(string collectionName, string jsonData, string requestId, string onSuccessCallback, string onSuccessObj, string onErrorObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsDbAddDocument(collectionName, jsonData, requestId, onSuccessCallback, onSuccessObj, onErrorObj);
#else
        Debug.Log($"[NativeBridge] 模拟添加文档: {collectionName}");
#endif
    }

    /// <summary>
    /// 删除文档
    /// </summary>
    public void DbDeleteDocument(string collectionName, string docId, string requestId, string onSuccessCallback, string onSuccessObj, string onErrorObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsDbDeleteDocument(collectionName, docId, requestId, onSuccessCallback, onSuccessObj, onErrorObj);
#else
        Debug.Log($"[NativeBridge] 模拟删除文档: {collectionName}/{docId}");
#endif
    }

    /// <summary>
    /// 获取文档
    /// </summary>
    public void DbGetDocument(string collectionName, string docId, string requestId, string onSuccessCallback, string onSuccessObj, string onErrorObj)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsDbGetDocument(collectionName, docId, requestId, onSuccessCallback, onSuccessObj, onErrorObj);
#else
        Debug.Log($"[NativeBridge] 模拟获取文档: {collectionName}/{docId}");
#endif
    }

    // === 原生输入框相关 ===

    /// <summary>
    /// 显示原生输入框
    /// </summary>
    public void ShowNativePrompt(string existingText, string objectName, string callbackSuccess)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsShowNativePrompt(existingText, objectName, callbackSuccess);
#else
        Debug.Log($"[NativeBridge] 模拟显示原生输入框: {objectName}, 现有文本: {existingText}");
#endif
    }

    #endregion
}
