using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_InputField))]
public class HtmlInputBridge : MonoBehaviour, IPointerClickHandler
{
    private TMP_InputField inputField;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        // ��Ϊֻ�������ʱֻ�������ǵĵ������������ֻ�����
        inputField.readOnly = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!inputField.interactable) return;

        string currentText = inputField.text;
        string myGameObjectName = gameObject.name;

        Debug.Log($"[HtmlInputBridge] ���ں���ԭ�� Prompt: {myGameObjectName}");

        NativeBridge.Instance.ShowNativePrompt(currentText, myGameObjectName, "OnHtmlInputSuccess");
    }

    public void OnHtmlInputSuccess(string newText)
    {
        Debug.Log($"[HtmlInputBridge] �յ������ı�: {newText}");
        inputField.text = newText;
        // �����¼���֪ͨ�����ű����ݱ���
        inputField.onValueChanged.Invoke(newText);
        inputField.onEndEdit.Invoke(newText);
    }
}