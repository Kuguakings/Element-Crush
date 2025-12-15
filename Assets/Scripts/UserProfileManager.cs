/// <summary>
/// 用户资料管理器 / User Profile Manager
/// 管理主菜单右上角的"用户信息面板" / Manages the "User Info Panel" in the top-right of main menu
/// 显示用户昵称、角色（管理员/学员）/ Displays username, role (admin/student)
/// </summary>
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UserProfileManager : MonoBehaviour, IPointerClickHandler
{
    [Header("UI 组件 / UI Components")]
    public TextMeshProUGUI usernameText; // 用于显示昵称的文本 / Text for displaying username
    public TextMeshProUGUI roleText;     // 用于显示角色(管理员/学员)的文本 / Text for displaying role (admin/student)

    [Header("角色颜色 / Role Colors")]
    public Color superAdminColor = new Color(1f, 0.84f, 0f); // 金色(超级管理员) / Gold (super admin)
    public Color adminColor = new Color(0f, 0.8f, 1f);       // 蓝色(管理员) / Blue (admin)
    public Color userColor = new Color(0.8f, 0.8f, 0.8f);    // 灰色(普通用户) / Gray (normal user)

    void Start()
    {
        // ��ʼ����һ�� UI
        UpdateUI();
    }

    public void UpdateUI()
    {
        // 1. �����û���
        if (!string.IsNullOrEmpty(TcbManager.CurrentNickname))
        {
            usernameText.text = TcbManager.CurrentNickname;
        }
        else
        {
            usernameText.text = "������...";
        }

        // 2. ���½�ɫ/Ȩ����ʾ
        if (TcbManager.IsAdmin)
        {
            if (TcbManager.AdminLevel >= 999)
            {
                roleText.text = "��������Ա";
                roleText.color = superAdminColor;
                usernameText.color = superAdminColor;
            }
            else
            {
                roleText.text = "����Ա";
                roleText.color = adminColor;
                usernameText.color = adminColor;
            }
        }
        else
        {
            roleText.text = "ѧԱ";
            roleText.color = userColor;
            usernameText.color = Color.white; // ��ͨ�û����ְ�ɫ
        }
    }

    /// <summary>
    /// 处理点击事件 / Handle Click Event
    /// 右键点击打开改名对话框 / Right click to open rename dialog
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 只有右键才触发改名 / Only right click triggers rename
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("右键点击用户头像，打开改名窗口... / Right clicked user avatar, opening rename window...");
            string currentName = TcbManager.CurrentNickname;

            NativeBridge.Instance.ShowNativePrompt(currentName, gameObject.name, "OnReceiveNewName");
        }
    }

    /// <summary>
    /// 接收JavaScript回调的新名字 / Receive New Name from JavaScript Callback
    /// 由WebGL的JavaScript代码调用 / Called by WebGL JavaScript code
    /// </summary>
    /// <param name="newName">用户输入的新名字 / User's input new name</param>
    public void OnReceiveNewName(string newName)
    {
        // 检查有效性 / Check validity
        if (string.IsNullOrWhiteSpace(newName)) return;

        // 限制长度，防止 UI 溢出 / Limit length to prevent UI overflow
        if (newName.Length > 12) newName = newName.Substring(0, 12);

        // 先更新UI，让用户立即看到反馈 / Update UI first for immediate user feedback
        usernameText.text = newName;

        // 同步到后端数据库 / Sync to backend database
        if (TcbManager.instance != null)
        {
            TcbManager.instance.RequestUpdateUsername(newName);
        }
    }
}