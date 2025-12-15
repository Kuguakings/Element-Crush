using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Level Editor Mode 2 Controller - 模式 2: WordLinkUp (连连看)
/// 负责处理所有与 WordLinkUp 模式相关的编辑功能
/// </summary>
public class LevelEditor_Mode2 : MonoBehaviour
{
    #region 引用
    private LevelEditor_Core core;
    #endregion

    #region UI 变量 (模式 2)
    private CanvasGroup chapterSelectPanel_M2_CG;
    private Transform chapterButtonContainer_M2;
    private GameObject chapterButtonPrefab_M2;

    private CanvasGroup levelSelectPanel_M2_CG;
    private Transform levelButtonContainer_M2;
    private GameObject levelButtonPrefab_M2;
    private Button addLevelButton_M2;
    private CanvasGroup addLevelPopupPanel_M2_CG;
    private TMP_InputField newLevelInputfield_M2;
    private Button addLevelConfirmButton_M2;
    private Button addLevelCancelButton_M2;

    private CanvasGroup contentEditorPanel_M2_CG;
    private Transform sentenceInputContainer_M2;
    private GameObject m2SentenceInputPrefab;
    private Button addSentenceButton_M2;
    private Button globalSplitButton_M2;
    private Button globalSaveButton_M2;
    private Button contentEditorBackButton_M2;

    private TMP_InputField detail_SentenceIdInput;
    private TMP_InputField detail_FullSentenceInput;
    private Transform wordListContainer_M2;
    private GameObject m2WordRowPrefab;
    private Button addWordButton_M2;
    private Button resplitButton_M2;
    private Button detailSaveButton_M2;
    private Button deleteRowButton_M2;
    private Button mergeUpButton_M2;
    private Button mergeDownButton_M2;

    private Button saveButton_M2;
    private Button testPlayButton_M2;
    private Button publishButton_M2;

    private CanvasGroup batchPastePopupPanel_M2_CG;
    private TMP_InputField pasteInputField_M2;
    private Transform pastePreviewContainer_M2;
    private Button pasteConfirmButton_M2;
    private Button pasteCancelButton_M2;
    private Button batchPasteButton_M2;
    #endregion

    #region 数据变量 (模式 2)
    private string currentEditingChapter_M2 = "";
    private LevelData currentEditingLevel_M2 = null;
    private bool isDirty_M2 = false;
    private M2_SentenceInputRow currentSelectedSentenceRow = null;
    private M2_WordRow currentSelectedWordRow = null;
    private List<Mode2Content> parsedPasteSentences = null;
    #endregion

    #region 初始化
    public void Initialize(LevelEditor_Core coreRef)
    {
        core = coreRef;
        BindUIComponents();
        BindButtons();
    }

    private void BindUIComponents()
    {
        // 章节选择面板
        chapterSelectPanel_M2_CG = GameObject.Find("ChapterSelectPanel_M2")?.GetComponent<CanvasGroup>();
        chapterButtonContainer_M2 = GameObject.Find("ChapterButtonContainer_M2")?.transform;
        GameObject prefabObj = GameObject.Find("ChapterButtonPrefab_M2");
        if (prefabObj != null)
        {
            chapterButtonPrefab_M2 = prefabObj;
            chapterButtonPrefab_M2.SetActive(false);
        }

        // 关卡选择面板
        levelSelectPanel_M2_CG = GameObject.Find("LevelSelectPanel_M2")?.GetComponent<CanvasGroup>();
        levelButtonContainer_M2 = GameObject.Find("LevelButtonContainer_M2")?.transform;
        GameObject levelPrefabObj = GameObject.Find("LevelButtonPrefab_M2");
        if (levelPrefabObj != null)
        {
            levelButtonPrefab_M2 = levelPrefabObj;
            levelButtonPrefab_M2.SetActive(false);
        }
        addLevelButton_M2 = GameObject.Find("AddLevelButton_M2")?.GetComponent<Button>();

        // 添加关卡弹窗
        addLevelPopupPanel_M2_CG = GameObject.Find("AddLevelPopupPanel_M2")?.GetComponent<CanvasGroup>();
        newLevelInputfield_M2 = GameObject.Find("NewLevelInputfield_M2")?.GetComponent<TMP_InputField>();
        addLevelConfirmButton_M2 = GameObject.Find("AddLevelConfirmButton_M2")?.GetComponent<Button>();
        addLevelCancelButton_M2 = GameObject.Find("AddLevelCancelButton_M2")?.GetComponent<Button>();

        // 内容编辑面板
        contentEditorPanel_M2_CG = GameObject.Find("ContentEditorPanel_M2")?.GetComponent<CanvasGroup>();
        sentenceInputContainer_M2 = GameObject.Find("SentenceInputContainer_M2")?.transform;
        GameObject sentencePrefabObj = GameObject.Find("M2_SentenceInputPrefab");
        if (sentencePrefabObj != null)
        {
            m2SentenceInputPrefab = sentencePrefabObj;
            m2SentenceInputPrefab.SetActive(false);
        }
        addSentenceButton_M2 = GameObject.Find("AddSentenceButton_M2")?.GetComponent<Button>();
        globalSplitButton_M2 = GameObject.Find("GlobalSplitButton_M2")?.GetComponent<Button>();
        globalSaveButton_M2 = GameObject.Find("GlobalSaveButton_M2")?.GetComponent<Button>();
        contentEditorBackButton_M2 = GameObject.Find("ContentEditorBackButton_M2")?.GetComponent<Button>();

        // 详情面板
        detail_SentenceIdInput = GameObject.Find("Detail_SentenceIdInput")?.GetComponent<TMP_InputField>();
        detail_FullSentenceInput = GameObject.Find("Detail_FullSentenceInput")?.GetComponent<TMP_InputField>();
        wordListContainer_M2 = GameObject.Find("WordListContainer_M2")?.transform;
        GameObject wordPrefabObj = GameObject.Find("M2_WordRowPrefab");
        if (wordPrefabObj != null)
        {
            m2WordRowPrefab = wordPrefabObj;
            m2WordRowPrefab.SetActive(false);
        }
        addWordButton_M2 = GameObject.Find("AddWordButton_M2")?.GetComponent<Button>();
        resplitButton_M2 = GameObject.Find("ResplitButton_M2")?.GetComponent<Button>();
        detailSaveButton_M2 = GameObject.Find("DetailSaveButton_M2")?.GetComponent<Button>();
        deleteRowButton_M2 = GameObject.Find("DeleteRowButton_M2")?.GetComponent<Button>();
        mergeUpButton_M2 = GameObject.Find("MergeUpButton_M2")?.GetComponent<Button>();
        mergeDownButton_M2 = GameObject.Find("MergeDownButton_M2")?.GetComponent<Button>();

        // 全局按钮
        saveButton_M2 = GameObject.Find("SaveButton_M2")?.GetComponent<Button>();
        testPlayButton_M2 = GameObject.Find("TestPlayButton_M2")?.GetComponent<Button>();
        publishButton_M2 = GameObject.Find("PublishButton_M2")?.GetComponent<Button>();

        // 批量粘贴弹窗
        batchPastePopupPanel_M2_CG = GameObject.Find("BatchPastePopupPanel_M2")?.GetComponent<CanvasGroup>();
        pasteInputField_M2 = GameObject.Find("PasteInputField_M2")?.GetComponent<TMP_InputField>();
        pastePreviewContainer_M2 = GameObject.Find("PastePreviewContainer_M2")?.transform;
        pasteConfirmButton_M2 = GameObject.Find("PasteConfirmButton_M2")?.GetComponent<Button>();
        pasteCancelButton_M2 = GameObject.Find("PasteCancelButton_M2")?.GetComponent<Button>();
        batchPasteButton_M2 = GameObject.Find("BatchPasteButton_M2")?.GetComponent<Button>();
    }

    private void BindButtons()
    {
        // 关卡选择面板
        if (addLevelButton_M2 != null) addLevelButton_M2.onClick.AddListener(OnClick_AddLevel);
        if (addLevelConfirmButton_M2 != null) addLevelConfirmButton_M2.onClick.AddListener(OnClick_AddLevel_Confirm);
        if (addLevelCancelButton_M2 != null) addLevelCancelButton_M2.onClick.AddListener(OnClick_AddLevel_Cancel);

        // 内容编辑面板
        if (addSentenceButton_M2 != null) addSentenceButton_M2.onClick.AddListener(OnClick_AddSentence);
        if (globalSplitButton_M2 != null) globalSplitButton_M2.onClick.AddListener(OnClick_GlobalSplit);
        if (globalSaveButton_M2 != null) globalSaveButton_M2.onClick.AddListener(OnClick_GlobalSave);
        if (contentEditorBackButton_M2 != null) contentEditorBackButton_M2.onClick.AddListener(OnClick_ContentEditor_Back);

        // 详情面板
        if (addWordButton_M2 != null) addWordButton_M2.onClick.AddListener(OnClick_AddWord);
        if (resplitButton_M2 != null) resplitButton_M2.onClick.AddListener(OnRequestAutoSplit_Current);
        if (detailSaveButton_M2 != null) detailSaveButton_M2.onClick.AddListener(OnClick_DetailSave);
        if (deleteRowButton_M2 != null) deleteRowButton_M2.onClick.AddListener(OnClick_DeleteWord);
        if (mergeUpButton_M2 != null) mergeUpButton_M2.onClick.AddListener(OnClick_MergeUp);
        if (mergeDownButton_M2 != null) mergeDownButton_M2.onClick.AddListener(OnClick_MergeDown);

        // 全局按钮
        if (saveButton_M2 != null) saveButton_M2.onClick.AddListener(OnClick_Save);
        if (testPlayButton_M2 != null) testPlayButton_M2.onClick.AddListener(OnClick_TestPlay);
        if (publishButton_M2 != null) publishButton_M2.onClick.AddListener(OnClick_Publish);

        // 批量粘贴
        if (batchPasteButton_M2 != null) batchPasteButton_M2.onClick.AddListener(OnClick_BatchPaste);
        if (pasteConfirmButton_M2 != null) pasteConfirmButton_M2.onClick.AddListener(OnClick_Paste_ConfirmImport);
        if (pasteCancelButton_M2 != null) pasteCancelButton_M2.onClick.AddListener(OnClick_Paste_Cancel);
    }
    #endregion

    #region 模式进入/退出
    public void EnterMode()
    {
        core.SetStatus("进入模式 2 (WordLinkUp) - 请选择章节");
        PopulateChapterList();
        
        // 显示章节选择面板
        if (chapterSelectPanel_M2_CG != null)
        {
            chapterSelectPanel_M2_CG.gameObject.SetActive(true);
            chapterSelectPanel_M2_CG.alpha = 1f;
            chapterSelectPanel_M2_CG.interactable = true;
            chapterSelectPanel_M2_CG.blocksRaycasts = true;
        }
    }

    public void ExitMode()
    {
        // 隐藏所有 Mode2 面板
        if (chapterSelectPanel_M2_CG != null) chapterSelectPanel_M2_CG.gameObject.SetActive(false);
        if (levelSelectPanel_M2_CG != null) levelSelectPanel_M2_CG.gameObject.SetActive(false);
        if (contentEditorPanel_M2_CG != null) contentEditorPanel_M2_CG.gameObject.SetActive(false);
    }

    /// <summary>
    /// 从试玩返回时直接进入编辑关卡（不经过章节和关卡选择）
    /// </summary>
    public void SelectLevelInstant(LevelData levelData)
    {
        OnClick_SelectLevel(levelData, instant: true);
    }
    #endregion

    #region 章节选择
    private void PopulateChapterList()
    {
        if (chapterButtonContainer_M2 == null || chapterButtonPrefab_M2 == null)
        {
            Debug.LogError("[Mode2] chapterButtonContainer_M2 或 chapterButtonPrefab_M2 未找到！");
            return;
        }

        // 清空现有按钮
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in chapterButtonContainer_M2)
        {
            if (child.gameObject != chapterButtonPrefab_M2)
            {
                childrenToDestroy.Add(child.gameObject);
            }
        }
        foreach (GameObject go in childrenToDestroy) { Destroy(go); }

        // 获取所有 Mode2 章节
        var chaptersForMode2 = TcbManager.AllLevels.levels
            .Where(l => l.mode == (long)GameMode.WordLinkUp)
            .Select(l => l.chapter)
            .Distinct()
            .OrderBy(c => c);

        foreach (string chapterName in chaptersForMode2)
        {
            GameObject btnGO = Instantiate(chapterButtonPrefab_M2, chapterButtonContainer_M2);
            btnGO.SetActive(true);

            Button btn = btnGO.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                string capturedName = chapterName;
                btn.onClick.AddListener(() => OnClick_SelectChapter(capturedName));
            }

            TMP_Text btnText = btnGO.GetComponentInChildren<TMP_Text>();
            if (btnText != null) btnText.text = chapterName;
        }
    }

    private void OnClick_SelectChapter(string chapterName)
    {
        currentEditingChapter_M2 = chapterName;
        StartCoroutine(core.TransitionTo(chapterSelectPanel_M2_CG, levelSelectPanel_M2_CG));
        core.SetStatus($"当前编辑: 模式 2 / {chapterName}");
        PopulateLevelList(chapterName);
    }
    #endregion

    #region 关卡选择
    private void PopulateLevelList(string chapterName)
    {
        if (levelButtonContainer_M2 == null || levelButtonPrefab_M2 == null)
        {
            Debug.LogError("[Mode2] levelButtonContainer_M2 或 levelButtonPrefab_M2 未找到！");
            return;
        }

        // 清空现有按钮
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in levelButtonContainer_M2)
        {
            if (child.gameObject != levelButtonPrefab_M2)
            {
                childrenToDestroy.Add(child.gameObject);
            }
        }
        foreach (GameObject go in childrenToDestroy) { Destroy(go); }

        // 获取该章节的所有关卡
        var levelsForChapter = TcbManager.AllLevels.levels
            .Where(l => l.mode == (long)GameMode.WordLinkUp && l.chapter == chapterName)
            .OrderBy(l => l.level);

        foreach (LevelData levelData in levelsForChapter)
        {
            GameObject btnGO = Instantiate(levelButtonPrefab_M2, levelButtonContainer_M2);
            btnGO.SetActive(true);

            Button btn = btnGO.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                LevelData capturedLevel = levelData;
                btn.onClick.AddListener(() => OnClick_SelectLevel(capturedLevel));
            }

            TMP_Text btnText = btnGO.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
            {
                string statusLabel = "";
                switch (levelData.editorStatus)
                {
                    case "Working": statusLabel = "[编辑中]"; break;
                    case "Tested": statusLabel = "[已测试]"; break;
                    case "Published": statusLabel = "[已发布]"; break;
                    default: statusLabel = "[未知]"; break;
                }
                btnText.text = $"关卡 {levelData.level} {statusLabel}";
            }
        }
    }

    private void OnClick_SelectLevel(LevelData levelData, bool instant = false)
    {
        if (!instant)
        {
            StartCoroutine(core.TransitionTo(levelSelectPanel_M2_CG, contentEditorPanel_M2_CG));
        }

        currentEditingChapter_M2 = levelData.chapter;
        isDirty_M2 = false;
        currentEditingLevel_M2 = levelData;
        core.SetStatus($"正在编辑: {levelData.chapter} - 关卡 {levelData.level}");
        PopulateSentenceEditor(currentEditingLevel_M2);
        
        if (sentenceInputContainer_M2.childCount > 0) { }
        else
        {
            ClearWordList();
            UpdateContextualButtons(null);
            SetDetailPanelActive(false);
        }
        UpdateEditorButtonStates();
    }

    private void OnClick_AddLevel()
    {
        if (addLevelPopupPanel_M2_CG != null)
        {
            addLevelPopupPanel_M2_CG.gameObject.SetActive(true);
            addLevelPopupPanel_M2_CG.alpha = 1;
            addLevelPopupPanel_M2_CG.interactable = true;
            addLevelPopupPanel_M2_CG.blocksRaycasts = true;
        }
        core.SetStatus($"当前章节: {currentEditingChapter_M2}。请输入新关卡号。");
    }

    private void OnClick_AddLevel_Cancel()
    {
        if (addLevelPopupPanel_M2_CG != null)
        {
            addLevelPopupPanel_M2_CG.alpha = 0;
            addLevelPopupPanel_M2_CG.interactable = false;
            addLevelPopupPanel_M2_CG.blocksRaycasts = false;
            addLevelPopupPanel_M2_CG.gameObject.SetActive(false);
        }
        core.SetStatus($"当前编辑: 模式 2 / {currentEditingChapter_M2} / (请选择关卡)");
    }

    private void OnClick_AddLevel_Confirm()
    {
        int newLevelNum = -1;
        if (!int.TryParse(newLevelInputfield_M2.text, out newLevelNum) || newLevelNum <= 0)
        {
            core.SetStatus("错误：请输入一个有效的正整数！", Color.red);
            return;
        }

        // 检查是否重复
        bool isDuplicate = TcbManager.AllLevels.levels.Any(l =>
            l.mode == (long)GameMode.WordLinkUp &&
            l.chapter == currentEditingChapter_M2 &&
            l.level == newLevelNum
        );
        if (isDuplicate)
        {
            core.SetStatus($"错误：关卡 {newLevelNum} 已经存在！", Color.red);
            return;
        }

        // 创建新关卡
        LevelData newLevelData = new LevelData
        {
            mode = (int)GameMode.WordLinkUp,
            chapter = currentEditingChapter_M2,
            level = newLevelNum,
            id = $"m{(int)GameMode.WordLinkUp}-{currentEditingChapter_M2}-l{newLevelNum}",
            content_mode_1 = new List<Mode1Content>(),
            content_mode_2 = new List<Mode2Content>(),
            editorStatus = "Working"
        };
        TcbManager.AllLevels.levels.Add(newLevelData);

        // 关闭弹窗
        if (addLevelPopupPanel_M2_CG != null)
        {
            addLevelPopupPanel_M2_CG.alpha = 0;
            addLevelPopupPanel_M2_CG.interactable = false;
            addLevelPopupPanel_M2_CG.blocksRaycasts = false;
            addLevelPopupPanel_M2_CG.gameObject.SetActive(false);
        }

        // 直接进入编辑界面
        OnClick_SelectLevel(newLevelData, false);
    }
    #endregion

    #region 内容编辑器
    private void OnClick_ContentEditor_Back()
    {
        OnClick_GlobalSave();
        StartCoroutine(core.TransitionTo(contentEditorPanel_M2_CG, levelSelectPanel_M2_CG));
        PopulateLevelList(currentEditingChapter_M2);
        core.SetStatus($"当前编辑: 模式 2 / {currentEditingChapter_M2} / (请选择关卡)");
        currentEditingLevel_M2 = null;
        currentSelectedSentenceRow = null;
        currentSelectedWordRow = null;
        isDirty_M2 = false;
        UpdateEditorButtonStates();
    }

    private void PopulateSentenceEditor(LevelData levelData)
    {
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in sentenceInputContainer_M2) { childrenToDestroy.Add(child.gameObject); }
        foreach (GameObject go in childrenToDestroy) { Destroy(go); }
        
        if (levelData.content_mode_2 == null || levelData.content_mode_2.Count == 0) return;
        
        var sentenceGroups = levelData.content_mode_2
            .GroupBy(word => word.sentenceId)
            .OrderBy(group => group.Key);
        
        foreach (var group in sentenceGroups)
        {
            int sentenceId = group.Key;
            Mode2Content firstWord = group.First();
            AddSentenceInputRow(sentenceId, firstWord);
        }
    }

    private void OnClick_AddSentence()
    {
        int newId = 1;
        if (sentenceInputContainer_M2.childCount > 0)
        {
            var lastRow = sentenceInputContainer_M2.GetChild(sentenceInputContainer_M2.childCount - 1).GetComponent<M2_SentenceInputRow>();
            newId = lastRow.GetSentenceId() + 1;
        }
        M2_SentenceInputRow newRow = AddSentenceInputRow(newId, null);
    }

    private M2_SentenceInputRow AddSentenceInputRow(int id, Mode2Content data)
    {
        GameObject rowGO = Instantiate(m2SentenceInputPrefab, sentenceInputContainer_M2);
        M2_SentenceInputRow rowUI = rowGO.GetComponent<M2_SentenceInputRow>();
        if (rowUI != null) rowUI.Setup(this, id, data);
        return rowUI;
    }

    public void OnSentenceToggleChanged(M2_SentenceInputRow toggledRow, bool isToggledOn)
    {
        if (isToggledOn)
        {
            currentSelectedSentenceRow = toggledRow;
            if (detail_SentenceIdInput != null) detail_SentenceIdInput.text = toggledRow.GetSentenceId().ToString();
            if (detail_FullSentenceInput != null) detail_FullSentenceInput.text = toggledRow.GetFullSentence();
            SetDetailPanelActive(true);
            PopulateWordList(toggledRow);
        }
        else
        {
            if (currentSelectedSentenceRow == toggledRow)
            {
                OnClick_DetailSave();
                currentSelectedSentenceRow = null;
                SetDetailPanelActive(false);
                ClearWordList();
            }
        }
    }
    #endregion

    #region 词语列表
    private void PopulateWordList(M2_SentenceInputRow selectedRow)
    {
        ClearWordList();
        int sentenceId = selectedRow.GetSentenceId();
        var words = currentEditingLevel_M2.content_mode_2
            .Where(w => w.sentenceId == sentenceId)
            .OrderBy(w => w.wordOrder)
            .ToList();
        
        if (words.Count == 0)
        {
            OnRequestAutoSplit(selectedRow, false);
        }
        else
        {
            foreach (var wordData in words)
            {
                AddWordRow(wordData.wordOrder, wordData.wordText);
            }
            RenumberWordList();
        }
    }

    public void OnRequestAutoSplit(M2_SentenceInputRow rowToSplit, bool autoSave = false)
    {
        if (rowToSplit == null) return;
        ClearWordList();
        string fullSentence = rowToSplit.GetFullSentence();
        if (string.IsNullOrEmpty(fullSentence)) return;
        
        List<Mode2Content> newWords = new List<Mode2Content>();
        int order = 1;
        foreach (char c in fullSentence)
        {
            AddWordRow(order, c.ToString());
            newWords.Add(new Mode2Content
            {
                sentenceId = rowToSplit.GetSentenceId(),
                wordOrder = order,
                wordText = c.ToString(),
                fullSentence = fullSentence
            });
            order++;
        }
        RenumberWordList();
        if (autoSave) SaveDetailDataToLevel(rowToSplit.GetSentenceId(), newWords);
    }

    private void OnRequestAutoSplit_Current()
    {
        if (currentSelectedSentenceRow == null)
        {
            core.SetStatus("错误：请先在左侧选中一个句子！", Color.red);
            return;
        }
        OnRequestAutoSplit(currentSelectedSentenceRow, false);
        MarkLevelAsDirty();
    }

    private void OnClick_GlobalSplit()
    {
        core.SetStatus("正在拆分所有句子...");
        foreach (Transform child in sentenceInputContainer_M2)
        {
            M2_SentenceInputRow row = child.GetComponent<M2_SentenceInputRow>();
            if (row != null) OnRequestAutoSplit(row, true);
        }
        if (currentSelectedSentenceRow != null)
        {
            PopulateWordList(currentSelectedSentenceRow);
        }
        core.SetStatus("全部拆分完毕！");
        MarkLevelAsDirty();
    }

    private void OnClick_DetailSave()
    {
        if (currentSelectedSentenceRow == null) return;
        
        int sentenceId = currentSelectedSentenceRow.GetSentenceId();
        System.Text.StringBuilder reconstructedSentence = new System.Text.StringBuilder();
        List<Mode2Content> newWords = new List<Mode2Content>();
        
        for (int i = 0; i < wordListContainer_M2.childCount; i++)
        {
            M2_WordRow wordRow = wordListContainer_M2.GetChild(i).GetComponent<M2_WordRow>();
            if (wordRow != null)
            {
                string wordText = wordRow.GetWord();
                reconstructedSentence.Append(wordText);
                newWords.Add(new Mode2Content
                {
                    sentenceId = sentenceId,
                    wordOrder = i + 1,
                    wordText = wordText,
                    fullSentence = ""
                });
            }
        }
        
        string finalSentence = reconstructedSentence.ToString();
        foreach (var word in newWords) word.fullSentence = finalSentence;
        
        SaveDetailDataToLevel(sentenceId, newWords);
        currentSelectedSentenceRow.SetFullSentenceText(finalSentence);
        if (detail_FullSentenceInput != null) detail_FullSentenceInput.text = finalSentence;
        core.SetStatus($"句子 {sentenceId} 保存成功！ (同步完成)");
        MarkLevelAsDirty();
    }

    private void OnClick_GlobalSave()
    {
        OnClick_DetailSave();
        if (currentEditingLevel_M2 == null) return;
        
        foreach (Transform child in sentenceInputContainer_M2)
        {
            M2_SentenceInputRow row = child.GetComponent<M2_SentenceInputRow>();
            if (row != null)
            {
                int sId = row.GetSentenceId();
                string sText = row.GetFullSentence();
                var wordsToUpdate = currentEditingLevel_M2.content_mode_2
                    .Where(w => w.sentenceId == sId);
                foreach (var word in wordsToUpdate) word.fullSentence = sText;
            }
        }
    }

    private void SaveDetailDataToLevel(int sentenceId, List<Mode2Content> newWords)
    {
        if (currentEditingLevel_M2 == null) return;
        
        currentEditingLevel_M2.content_mode_2.RemoveAll(w => w.sentenceId == sentenceId);
        currentEditingLevel_M2.content_mode_2.AddRange(newWords);
        currentEditingLevel_M2.content_mode_2 = currentEditingLevel_M2.content_mode_2
            .OrderBy(w => w.sentenceId)
            .ThenBy(w => w.wordOrder)
            .ToList();
    }

    private void OnClick_AddWord()
    {
        if (currentSelectedSentenceRow == null)
        {
            core.SetStatus("错误：请先在左侧选中一个句子！", Color.red);
            return;
        }
        int newOrder = wordListContainer_M2.childCount + 1;
        AddWordRow(newOrder, "");
        RenumberWordList();
        MarkLevelAsDirty();
    }

    private void AddWordRow(int order, string word)
    {
        GameObject rowGO = Instantiate(m2WordRowPrefab, wordListContainer_M2);
        M2_WordRow rowUI = rowGO.GetComponent<M2_WordRow>();
        if (rowUI != null) rowUI.Setup(this, order, word);
    }

    private void ClearWordList()
    {
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in wordListContainer_M2) { childrenToDestroy.Add(child.gameObject); }
        foreach (GameObject go in childrenToDestroy) { DestroyImmediate(go); }
        currentSelectedWordRow = null;
        UpdateContextualButtons(null);
    }

    private void SetDetailPanelActive(bool isActive)
    {
        if (addWordButton_M2 != null) addWordButton_M2.gameObject.SetActive(isActive);
        if (resplitButton_M2 != null) resplitButton_M2.gameObject.SetActive(isActive);
        if (detailSaveButton_M2 != null) detailSaveButton_M2.gameObject.SetActive(isActive);
        if (!isActive) UpdateContextualButtons(null);
    }
    #endregion

    #region 词语编辑
    public void OnRequestMoveWord(M2_WordRow rowToMove, int direction)
    {
        int currentIndex = rowToMove.transform.GetSiblingIndex();
        int newIndex = currentIndex + direction;
        if (newIndex < 0 || newIndex >= wordListContainer_M2.childCount) return;
        
        rowToMove.transform.SetSiblingIndex(newIndex);
        MarkLevelAsDirty();
        RenumberWordList();
    }

    public void OnSelectWordRowToggle(M2_WordRow toggledRow, bool isToggledOn)
    {
        if (isToggledOn)
        {
            foreach (Transform child in wordListContainer_M2)
            {
                M2_WordRow row = child.GetComponent<M2_WordRow>();
                if (row != null && row != toggledRow) row.SetSelected(false);
            }
            currentSelectedWordRow = toggledRow;
            toggledRow.SetSelected(true);
            UpdateContextualButtons(toggledRow);
        }
        else
        {
            if (currentSelectedWordRow == toggledRow)
            {
                currentSelectedWordRow = null;
                toggledRow.SetSelected(false);
                UpdateContextualButtons(null);
            }
        }
    }

    private void OnClick_DeleteWord()
    {
        if (currentSelectedWordRow == null) return;
        
        DestroyImmediate(currentSelectedWordRow.gameObject);
        currentSelectedWordRow = null;
        MarkLevelAsDirty();
        RenumberWordList();
        UpdateContextualButtons(null);
    }

    private void OnClick_MergeUp()
    {
        if (currentSelectedWordRow == null) return;
        
        int currentIndex = currentSelectedWordRow.transform.GetSiblingIndex();
        if (currentIndex == 0) return;
        
        M2_WordRow targetRow = wordListContainer_M2.GetChild(currentIndex - 1).GetComponent<M2_WordRow>();
        if (targetRow == null) return;
        
        string textToMerge = currentSelectedWordRow.GetWord();
        string targetText = targetRow.GetWord();
        targetRow.wordInput.text = targetText + textToMerge;
        
        DestroyImmediate(currentSelectedWordRow.gameObject);
        currentSelectedWordRow = null;
        MarkLevelAsDirty();
        RenumberWordList();
        UpdateContextualButtons(null);
    }

    private void OnClick_MergeDown()
    {
        if (currentSelectedWordRow == null) return;
        
        int currentIndex = currentSelectedWordRow.transform.GetSiblingIndex();
        if (currentIndex >= wordListContainer_M2.childCount - 1) return;
        
        M2_WordRow targetRow = wordListContainer_M2.GetChild(currentIndex + 1).GetComponent<M2_WordRow>();
        if (targetRow == null) return;
        
        string textToMerge = targetRow.GetWord();
        string currentText = currentSelectedWordRow.GetWord();
        currentSelectedWordRow.wordInput.text = currentText + textToMerge;
        
        DestroyImmediate(targetRow.gameObject);
        MarkLevelAsDirty();
        RenumberWordList();
        UpdateContextualButtons(currentSelectedWordRow);
    }

    private void RenumberWordList()
    {
        int total = wordListContainer_M2.childCount;
        for (int i = 0; i < total; i++)
        {
            M2_WordRow rowUI = wordListContainer_M2.GetChild(i).GetComponent<M2_WordRow>();
            if (rowUI != null)
            {
                bool isFirst = (i == 0);
                bool isLast = (i == total - 1);
                rowUI.UpdateVisuals(i + 1, isFirst, isLast);
            }
        }
    }

    private void UpdateContextualButtons(M2_WordRow selectedRow)
    {
        bool isSelected = (selectedRow != null);
        if (deleteRowButton_M2 != null) deleteRowButton_M2.gameObject.SetActive(isSelected);
        
        int index = isSelected ? selectedRow.transform.GetSiblingIndex() : -1;
        bool canMergeUp = isSelected && (index > 0);
        bool canMergeDown = isSelected && (index < wordListContainer_M2.childCount - 1);
        
        if (mergeUpButton_M2 != null) mergeUpButton_M2.gameObject.SetActive(canMergeUp);
        if (mergeDownButton_M2 != null) mergeDownButton_M2.gameObject.SetActive(canMergeDown);
    }
    #endregion

    #region 批量粘贴
    private void OnClick_BatchPaste()
    {
        Debug.Log("[Mode2] 正在调用 ShowNativePrompt...");
        NativeBridge.Instance.ShowNativePrompt("", core.gameObject.name, "M2_ReceivePastedTextFromHtml");
    }

    public void OnPasteReceived(string pastedText)
    {
        Debug.Log("[Mode2] 成功接收到 JS 文本！");
        core.SetStatus("已接收到数据，正在解析...");

        parsedPasteSentences = ParsePastedCSV(pastedText);
        if (parsedPasteSentences == null || parsedPasteSentences.Count == 0)
        {
            core.SetStatus("解析失败：未识别到有效数据。", Color.red);
            return;
        }

        OnClick_Paste_ConfirmImport();
    }

    private List<Mode2Content> ParsePastedCSV(string csvText)
    {
        List<Mode2Content> parsedList = new List<Mode2Content>();
        string[] lines = csvText.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith("sentenceId,wordOrder,wordText,fullSentenceText")) continue;
            
            string[] values = line.Split(',');
            if (values.Length == 4)
            {
                if (!int.TryParse(values[0].Trim(), out int sId)) { continue; }
                if (!int.TryParse(values[1].Trim(), out int wOrder)) { continue; }
                string wordText = values[2].Trim();
                string fullSentence = values[3].Trim();
                
                parsedList.Add(new Mode2Content
                {
                    sentenceId = sId,
                    wordOrder = wOrder,
                    wordText = wordText,
                    fullSentence = fullSentence
                });
            }
        }
        return parsedList;
    }

    private void OnClick_Paste_ConfirmImport()
    {
        if (currentEditingLevel_M2 == null)
        {
            core.SetStatus("错误：请先选中一个关卡！", Color.red);
            return;
        }
        if (parsedPasteSentences == null || parsedPasteSentences.Count == 0)
        {
            return;
        }

        int maxExistingId = 0;
        if (currentEditingLevel_M2.content_mode_2.Count > 0)
        {
            maxExistingId = currentEditingLevel_M2.content_mode_2.Max(w => w.sentenceId);
        }
        
        foreach (Transform child in sentenceInputContainer_M2)
        {
            M2_SentenceInputRow row = child.GetComponent<M2_SentenceInputRow>();
            if (row != null && row.GetSentenceId() > maxExistingId) maxExistingId = row.GetSentenceId();
        }

        var sentenceGroups = parsedPasteSentences.GroupBy(w => w.sentenceId).OrderBy(g => g.Key);
        int importedSentenceCount = 0;
        
        foreach (var group in sentenceGroups)
        {
            importedSentenceCount++;
            int originalSentenceId = group.Key;
            List<Mode2Content> wordsForThisSentence = group.OrderBy(w => w.wordOrder).ToList();
            Mode2Content firstWord = wordsForThisSentence.First();
            
            bool uiExists = sentenceInputContainer_M2.Cast<Transform>().Any(t => {
                var row = t.GetComponent<M2_SentenceInputRow>();
                return row != null && row.GetSentenceId() == originalSentenceId;
            });
            
            int finalSentenceId = originalSentenceId;
            if (uiExists || finalSentenceId <= maxExistingId) finalSentenceId = ++maxExistingId;
            else maxExistingId = finalSentenceId;
            
            firstWord.sentenceId = finalSentenceId;
            M2_SentenceInputRow newRow = AddSentenceInputRow(finalSentenceId, firstWord);
            
            foreach (var word in wordsForThisSentence) word.sentenceId = finalSentenceId;
            SaveDetailDataToLevel(finalSentenceId, wordsForThisSentence);
        }

        if (currentSelectedSentenceRow != null)
        {
            OnSentenceToggleChanged(currentSelectedSentenceRow, false);
            OnSentenceToggleChanged(currentSelectedSentenceRow, true);
        }
        else if (sentenceInputContainer_M2.childCount > 0)
        {
            M2_SentenceInputRow firstRow = sentenceInputContainer_M2.GetChild(0).GetComponent<M2_SentenceInputRow>();
            if (firstRow != null) OnSentenceToggleChanged(firstRow, true);
        }

        MarkLevelAsDirty();
        core.SetStatus($"成功导入 {importedSentenceCount} 条句子 (共 {parsedPasteSentences.Count} 个词)！");
        parsedPasteSentences = null;
    }

    private void OnClick_Paste_Cancel()
    {
        if (batchPastePopupPanel_M2_CG != null)
        {
            batchPastePopupPanel_M2_CG.alpha = 0;
            batchPastePopupPanel_M2_CG.interactable = false;
            batchPastePopupPanel_M2_CG.blocksRaycasts = false;
            batchPastePopupPanel_M2_CG.gameObject.SetActive(false);
        }
        parsedPasteSentences = null;
        
        if (pasteInputField_M2 != null) pasteInputField_M2.text = "";
        
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in pastePreviewContainer_M2) { childrenToDestroy.Add(child.gameObject); }
        foreach (GameObject go in childrenToDestroy) { Destroy(go); }
        
        if (currentEditingLevel_M2 != null)
        {
            core.SetStatus($"正在编辑: {currentEditingLevel_M2.chapter} - 关卡 {currentEditingLevel_M2.level}");
        }
    }
    #endregion

    #region 全局按钮
    private void OnClick_Save()
    {
        core.SetStatus("正在保存 (M2)...");
        OnClick_GlobalSave();

        isDirty_M2 = false;
        currentEditingLevel_M2.editorStatus = "Working";
        UpdateEditorButtonStates();

        if (TcbManager.instance != null)
        {
            TcbManager.instance.UploadNewLevel(currentEditingLevel_M2.id, currentEditingLevel_M2);
        }
    }

    private void OnClick_TestPlay()
    {
        core.SetStatus("正在保存并准备试玩 (M2)...");
        OnClick_GlobalSave();

        if (currentEditingLevel_M2.content_mode_2 == null || currentEditingLevel_M2.content_mode_2.Count == 0)
        {
            core.SetStatus("试玩失败：关卡内容为空！请添加内容后再试玩。", Color.red);
            return;
        }

        if (LevelManager.instance == null)
        {
            core.SetStatus("试玩失败：LevelManager 实例未找到！", Color.red);
            return;
        }

        LevelManager.isTestPlayMode = true;
        LevelManager.selectedGameMode = GameMode.WordLinkUp;
        LevelManager.instance.LoadLevel(currentEditingLevel_M2);
    }

    private void OnClick_Publish()
    {
        core.SetStatus("正在发布 (M2)...");
        OnClick_GlobalSave();
        currentEditingLevel_M2.editorStatus = "Published";

        if (TcbManager.instance != null)
        {
            TcbManager.instance.UploadNewLevel(currentEditingLevel_M2.id, currentEditingLevel_M2);
        }
        UpdateEditorButtonStates();
    }
    #endregion

    #region 工具方法
    public void MarkLevelAsDirty()
    {
        isDirty_M2 = true;
        UpdateEditorButtonStates();
    }

    private void UpdateEditorButtonStates()
    {
        if (currentEditingLevel_M2 == null) return;

        bool canPublish = (currentEditingLevel_M2.editorStatus == "Tested") && !isDirty_M2;
        if (publishButton_M2 != null)
        {
            publishButton_M2.interactable = canPublish;
        }

        bool canTest = !isDirty_M2 || currentEditingLevel_M2.editorStatus == "Working";
        if (testPlayButton_M2 != null)
        {
            testPlayButton_M2.interactable = canTest;
        }
    }
    #endregion
}
