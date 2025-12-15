// M2_SentenceInputRow.cs (V3.0.4 - �����ȶ���)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class M2_SentenceInputRow : MonoBehaviour
{
    [Header("UI ����")]
    public TMP_InputField sentenceIdInput;
    public TMP_InputField fullSentenceInput;
    public Toggle selectionToggle; // ���¡�Toggle ����

    [Header("ѡ�и���")]
    public Image selectionHighlight;

    private LevelEditor_Mode2 mode2Controller;
    private int internal_sentenceId;
    private string internal_fullSentence; // ���¡�����ʵʱ��¼�����˽�б���

    private bool isManagerUpdatingToggle = false; // ���¡���ֹ Toggle �¼�ѭ����������

    void Awake()
    {
        if (fullSentenceInput != null)
        {
            fullSentenceInput.lineType = TMP_InputField.LineType.MultiLineSubmit;
        }
    }

    public void Setup(LevelEditor_Mode2 controller, int id, Mode2Content data)
    {
        this.mode2Controller = controller;
        this.internal_sentenceId = id;

        // 1. ������ݲ���ʼ���ڲ�����
        sentenceIdInput.text = id.ToString();
        if (data != null)
        {
            fullSentenceInput.text = data.fullSentence;
            internal_fullSentence = data.fullSentence; // ���޸��㡿
        }
        else
        {
            fullSentenceInput.text = "";
            internal_fullSentence = ""; // ���޸��㡿
        }

        // 2. �󶨡����顱�͡�ʵʱ��¼��
        if (fullSentenceInput != null)
        {
            fullSentenceInput.onValueChanged.AddListener(OnSentenceChanged);
        }

        // 3. �� Toggle �¼�
        if (selectionToggle != null)
        {
            selectionToggle.onValueChanged.AddListener(OnToggleChanged);
        }

        SetToggleState(false);
    }

    private void OnToggleChanged(bool isOn)
    {
        if (isManagerUpdatingToggle) return; // ���� Manager �������¼�

        if (mode2Controller != null)
        {
            mode2Controller.OnSentenceToggleChanged(this, isOn);
        }
    }

    /// <summary>
    /// �����ӱ��༭ʱ����ʵʱ��¼����ֵ��
    /// </summary>
    private void OnSentenceChanged(string s)
    {
        internal_fullSentence = s; // ���޸��㡿: ʵʱ��¼��������
        if (mode2Controller != null)
        {
            mode2Controller.MarkLevelAsDirty();
        }
    }

    /// <summary>
    /// �� Manager ���������� Toggle ״̬ (����������)
    /// </summary>
    public void SetToggleState(bool isOn)
    {
        isManagerUpdatingToggle = true;

        if (selectionToggle != null)
        {
            selectionToggle.isOn = isOn;
        }

        // �޸��ˡ����������С��� Bug
        if (selectionHighlight != null)
        {
            selectionHighlight.enabled = isOn;
        }

        isManagerUpdatingToggle = false;
    }

    public int GetSentenceId()
    {
        return int.TryParse(sentenceIdInput.text, out int id) ? id : internal_sentenceId;
    }

    /// <summary>
    /// ���¡�: ����ʵʱ��¼���������ݣ��޸��ˡ���ȡ���ַ������� Bug
    /// </summary>
    public string GetFullSentence()
    {
        return internal_fullSentence; // ���޸��㡿: �����ڲ�����
    }

    /// <summary>
    /// �������º���������: �� Manager ���ã������ڱ������������ӵ��ı�
    /// </summary>
    public void SetFullSentenceText(string text)
    {
        // 1. �����ڲ����� (ȷ�� GetFullSentence() ��������ֵ)
        this.internal_fullSentence = text;

        // 2. �����Ӿ��ֶ� (��������)
        if (fullSentenceInput != null)
        {
            fullSentenceInput.text = text;
        }
        // ע��: ���ﲻ��Ҫ MarkLevelAsDirty����Ϊ�������ڱ��档
    }
}