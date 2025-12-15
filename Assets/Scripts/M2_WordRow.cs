// M2_WordRow.cs (V3.0.4 - Toggle Logic Update)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ������ģʽ2 V3.0.4������
/// ������ M2_WordRow_Prefab (�в�������༭����) ��
/// </summary>
public class M2_WordRow : MonoBehaviour
{
    [Header("UI ����")]
    public TextMeshProUGUI orderText;
    public TMP_InputField wordInput;
    public Button moveUpButton;
    public Button moveDownButton;
    public Toggle selectionToggle; // <--- ��������Toggle ����

    [Header("ѡ�и���")]
    public Image selectionHighlight;
    // public Button clickReceiverButton; // <--- �����Ƴ����ɵĵ�����հ�ť

    private LevelEditor_Mode2 mode2Controller;
    private bool isManagerUpdatingToggle = false;

    public void Setup(LevelEditor_Mode2 controller, int order, string word)
    {
        this.mode2Controller = controller;

        // 1. �������
        wordInput.text = word;

        // 2. �󶨰�ť�¼�
        if (moveUpButton != null)
        {
            moveUpButton.onClick.AddListener(OnMoveUp);
        }
        if (moveDownButton != null)
        {
            moveDownButton.onClick.AddListener(OnMoveDown);
        }

        // ���������� Toggle �¼����滻�ɵ� clickReceiverButton �߼�
        if (selectionToggle != null)
        {
            selectionToggle.onValueChanged.AddListener(OnToggleChanged);
        }

        // 3. �󶨡����顱
        if (wordInput != null)
        {
            wordInput.onValueChanged.AddListener(OnWordChanged);
        }

        // 4. Ĭ�����ظ���
        SetSelected(false);

        // 5. V3.0.3 ˢ�� Bug �޸� (����)
        UpdateVisuals(order, false, false);
    }

    private void OnWordChanged(string s)
    {
        if (mode2Controller != null)
        {
            mode2Controller.MarkLevelAsDirty();
        }
    }

    // ��������Toggle �¼�������
    private void OnToggleChanged(bool isOn)
    {
        if (isManagerUpdatingToggle) return;

        if (mode2Controller != null)
        {
            mode2Controller.OnSelectWordRowToggle(this, isOn);
        }
    }

    private void OnMoveUp()
    {
        if (mode2Controller != null)
        {
            mode2Controller.OnRequestMoveWord(this, -1);
        }
    }

    private void OnMoveDown()
    {
        if (mode2Controller != null)
        {
            mode2Controller.OnRequestMoveWord(this, 1);
        }
    }

    // ���޸ġ�SetSelected ���ڿ��� Toggle ״̬����
    public void SetSelected(bool isSelected)
    {
        // 1. ����
        isManagerUpdatingToggle = true;

        // 2. ���� Toggle ״̬
        if (selectionToggle != null)
        {
            selectionToggle.isOn = isSelected;
        }

        // 3. ���Ƹ��� (ʹ�� .enabled �޸������������е� Bug)
        if (selectionHighlight != null)
        {
            selectionHighlight.enabled = isSelected;
        }

        // 4. ����
        isManagerUpdatingToggle = false;
    }

    public void UpdateVisuals(int order, bool isFirst, bool isLast)
    {
        orderText.text = order.ToString();

        if (moveUpButton != null) moveUpButton.interactable = !isFirst;
        if (moveDownButton != null) moveDownButton.interactable = !isLast;
    }

    public string GetWord()
    {
        return wordInput.text;
    }
}