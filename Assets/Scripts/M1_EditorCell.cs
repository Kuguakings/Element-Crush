// M1_EditorCell.cs (V2 - �������������༭����)
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // ������ ���� ������

[RequireComponent(typeof(TMPro.TMP_InputField))]
public class M1_EditorCell : MonoBehaviour, IPointerClickHandler
{
    private LevelEditor_Mode1 mode1Controller;
    private TMP_InputField myInputField;
    
    public int row;
    public int col;
    public TMP_Text labelText; // ������ ���� ������

    // ������ ���� Awake() ������
    void Awake()
    {
        // ��ȡ���Լ�����������
        myInputField = GetComponent<TMP_InputField>();
    }

    public void Setup(LevelEditor_Mode1 controller, int row, int col, string text)
    {
        this.mode1Controller = controller;
        this.row = row;
        this.col = col;
        
        if (myInputField != null)
        {
            myInputField.text = text;
            myInputField.onValueChanged.AddListener(OnTextChanged);
        }
        
        UpdateLabel();
    }
    
    private void OnTextChanged(string newText)
    {
        if (mode1Controller != null)
        {
            mode1Controller.MarkLevelAsDirty();
        }
    }
    
    public void UpdateLabel()
    {
        if (labelText != null)
        {
            labelText.text = $"{row},{col}";
        }
    }
    
    public string GetText()
    {
        return myInputField != null ? myInputField.text : "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (mode1Controller == null || myInputField == null) return;

        // 右键 - 删除行
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            mode1Controller.OnRequestDeleteRow(row);
        }
        // 左键 - 编辑单元格
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            mode1Controller.OnRequestEditCell(this.gameObject, myInputField.text);
        }
    }
}