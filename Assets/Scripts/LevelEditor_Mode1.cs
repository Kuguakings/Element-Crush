using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Level Editor Mode 1 Controller - 模式 1: WordMatch3 (消消乐)
/// 负责处理所有与 WordMatch3 模式相关的编辑功能
/// </summary>
public class LevelEditor_Mode1 : MonoBehaviour
{
    #region 引用
    private LevelEditor_Core core;
    #endregion

    #region UI 变量 (模式 1)
    private CanvasGroup chapterSelectPanel_M1_CG;
    private Transform chapterButtonContainer_M1;
    private GameObject chapterButtonPrefab_M1;

    private CanvasGroup levelSelectPanel_M1_CG;
    private Transform levelButtonContainer_M1;
    private GameObject levelButtonPrefab_M1;
    private Button addLevelButton_M1;
    private CanvasGroup addLevelPopupPanel_M1_CG;
    private TMP_InputField newLevelInputfield_M1;
    private Button addLevelConfirmButton_M1;
    private Button addLevelCancelButton_M1;

    private CanvasGroup contentEditorPanel_M1_CG;
    private Transform gridCellContainer_M1;
    private GameObject cellPrefab_M1;
    private TMP_InputField rowsInputField_M1;
    private TMP_InputField columnsInputField_M1;
    private Button regenerateGridButton_M1;
    private Button batchPasteButton_M1;
    private Button saveButton_M1;
    private Button testPlayButton_M1;
    private Button publishButton_M1;
    private Button contentEditorBackButton_M1;

    private CanvasGroup deleteRowPopupPanel_M1_CG;
    private Button confirmDeleteRowButton_M1;
    private Button cancelDeleteRowButton_M1;
    #endregion

    #region 数据变量 (模式 1)
    private string currentEditingChapter_M1 = "";
    private LevelData currentEditingLevel_M1 = null;
    private bool isDirty_M1 = false;
    private List<Mode1Content> parsedPasteData = null;
    private int pendingDeleteRowIndex_M1 = -1;
    private GameObject currentEditingCell_M1;
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
        chapterSelectPanel_M1_CG = GameObject.Find("ChapterSelectPanel_M1")?.GetComponent<CanvasGroup>();
        chapterButtonContainer_M1 = GameObject.Find("ChapterButtonContainer_M1")?.transform;
        GameObject prefabObj = GameObject.Find("ChapterButtonPrefab_M1");
        if (prefabObj != null)
        {
            chapterButtonPrefab_M1 = prefabObj;
            chapterButtonPrefab_M1.SetActive(false);
        }

        // 关卡选择面板
        levelSelectPanel_M1_CG = GameObject.Find("LevelSelectPanel_M1")?.GetComponent<CanvasGroup>();
        levelButtonContainer_M1 = GameObject.Find("LevelButtonContainer_M1")?.transform;
        GameObject levelPrefabObj = GameObject.Find("LevelButtonPrefab_M1");
        if (levelPrefabObj != null)
        {
            levelButtonPrefab_M1 = levelPrefabObj;
            levelButtonPrefab_M1.SetActive(false);
        }
        addLevelButton_M1 = GameObject.Find("AddLevelButton_M1")?.GetComponent<Button>();

        // 添加关卡弹窗
        addLevelPopupPanel_M1_CG = GameObject.Find("AddLevelPopupPanel_M1")?.GetComponent<CanvasGroup>();
        newLevelInputfield_M1 = GameObject.Find("NewLevelInputfield_M1")?.GetComponent<TMP_InputField>();
        addLevelConfirmButton_M1 = GameObject.Find("AddLevelConfirmButton_M1")?.GetComponent<Button>();
        addLevelCancelButton_M1 = GameObject.Find("AddLevelCancelButton_M1")?.GetComponent<Button>();

        // 内容编辑面板
        contentEditorPanel_M1_CG = GameObject.Find("ContentEditorPanel_M1")?.GetComponent<CanvasGroup>();
        gridCellContainer_M1 = GameObject.Find("GridCellContainer_M1")?.transform;
        GameObject cellPrefabObj = GameObject.Find("CellPrefab_M1");
        if (cellPrefabObj != null)
        {
            cellPrefab_M1 = cellPrefabObj;
            cellPrefab_M1.SetActive(false);
        }
        rowsInputField_M1 = GameObject.Find("RowsInputField_M1")?.GetComponent<TMP_InputField>();
        columnsInputField_M1 = GameObject.Find("ColumnsInputField_M1")?.GetComponent<TMP_InputField>();
        regenerateGridButton_M1 = GameObject.Find("RegenerateGridButton_M1")?.GetComponent<Button>();
        batchPasteButton_M1 = GameObject.Find("BatchPasteButton_M1")?.GetComponent<Button>();
        saveButton_M1 = GameObject.Find("SaveButton_M1")?.GetComponent<Button>();
        testPlayButton_M1 = GameObject.Find("TestPlayButton_M1")?.GetComponent<Button>();
        publishButton_M1 = GameObject.Find("PublishButton_M1")?.GetComponent<Button>();
        contentEditorBackButton_M1 = GameObject.Find("ContentEditorBackButton_M1")?.GetComponent<Button>();

        // 删除行确认弹窗
        deleteRowPopupPanel_M1_CG = GameObject.Find("DeleteRowPopupPanel_M1")?.GetComponent<CanvasGroup>();
        confirmDeleteRowButton_M1 = GameObject.Find("ConfirmDeleteRowButton_M1")?.GetComponent<Button>();
        cancelDeleteRowButton_M1 = GameObject.Find("CancelDeleteRowButton_M1")?.GetComponent<Button>();
    }

    private void BindButtons()
    {
        // 关卡选择面板
        if (addLevelButton_M1 != null) addLevelButton_M1.onClick.AddListener(OnClick_AddLevel);
        if (addLevelConfirmButton_M1 != null) addLevelConfirmButton_M1.onClick.AddListener(OnClick_AddLevel_Confirm);
        if (addLevelCancelButton_M1 != null) addLevelCancelButton_M1.onClick.AddListener(OnClick_AddLevel_Cancel);

        // 内容编辑面板
        if (regenerateGridButton_M1 != null) regenerateGridButton_M1.onClick.AddListener(OnClick_RegenerateGrid);
        if (batchPasteButton_M1 != null) batchPasteButton_M1.onClick.AddListener(OnClick_BatchPaste);
        if (saveButton_M1 != null) saveButton_M1.onClick.AddListener(OnClick_Save);
        if (testPlayButton_M1 != null) testPlayButton_M1.onClick.AddListener(OnClick_TestPlay);
        if (publishButton_M1 != null) publishButton_M1.onClick.AddListener(OnClick_Publish);
        if (contentEditorBackButton_M1 != null) contentEditorBackButton_M1.onClick.AddListener(OnClick_ContentEditor_Back);

        // 删除行确认弹窗
        if (confirmDeleteRowButton_M1 != null) confirmDeleteRowButton_M1.onClick.AddListener(ConfirmDeleteRow);
        if (cancelDeleteRowButton_M1 != null) cancelDeleteRowButton_M1.onClick.AddListener(CancelDeleteRow);
    }
    #endregion

    #region 模式进入/退出
    public void EnterMode()
    {
        core.SetStatus("进入模式 1 (WordMatch3) - 请选择章节");
        PopulateChapterList();
        
        // 显示章节选择面板
        if (chapterSelectPanel_M1_CG != null)
        {
            chapterSelectPanel_M1_CG.gameObject.SetActive(true);
            chapterSelectPanel_M1_CG.alpha = 1f;
            chapterSelectPanel_M1_CG.interactable = true;
            chapterSelectPanel_M1_CG.blocksRaycasts = true;
        }
    }

    public void ExitMode()
    {
        // 隐藏所有 Mode1 面板
        if (chapterSelectPanel_M1_CG != null) chapterSelectPanel_M1_CG.gameObject.SetActive(false);
        if (levelSelectPanel_M1_CG != null) levelSelectPanel_M1_CG.gameObject.SetActive(false);
        if (contentEditorPanel_M1_CG != null) contentEditorPanel_M1_CG.gameObject.SetActive(false);
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
        if (chapterButtonContainer_M1 == null || chapterButtonPrefab_M1 == null)
        {
            Debug.LogError("[Mode1] chapterButtonContainer_M1 或 chapterButtonPrefab_M1 未找到！");
            return;
        }

        // 清空现有按钮
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in chapterButtonContainer_M1)
        {
            if (child.gameObject != chapterButtonPrefab_M1)
            {
                childrenToDestroy.Add(child.gameObject);
            }
        }
        foreach (GameObject go in childrenToDestroy) { Destroy(go); }

        // 获取所有 Mode1 章节
        var chaptersForMode1 = TcbManager.AllLevels.levels
            .Where(l => l.mode == (long)GameMode.WordMatch3)
            .Select(l => l.chapter)
            .Distinct()
            .OrderBy(c => c);

        foreach (string chapterName in chaptersForMode1)
        {
            GameObject btnGO = Instantiate(chapterButtonPrefab_M1, chapterButtonContainer_M1);
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
        currentEditingChapter_M1 = chapterName;
        StartCoroutine(core.TransitionTo(chapterSelectPanel_M1_CG, levelSelectPanel_M1_CG));
        core.SetStatus($"当前编辑: 模式 1 / {chapterName}");
        PopulateLevelList(chapterName);
    }
    #endregion

    #region 关卡选择
    private void PopulateLevelList(string chapterName)
    {
        if (levelButtonContainer_M1 == null || levelButtonPrefab_M1 == null)
        {
            Debug.LogError("[Mode1] levelButtonContainer_M1 或 levelButtonPrefab_M1 未找到！");
            return;
        }

        // 清空现有按钮
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in levelButtonContainer_M1)
        {
            if (child.gameObject != levelButtonPrefab_M1)
            {
                childrenToDestroy.Add(child.gameObject);
            }
        }
        foreach (GameObject go in childrenToDestroy) { Destroy(go); }

        // 获取该章节的所有关卡
        var levelsForChapter = TcbManager.AllLevels.levels
            .Where(l => l.mode == (long)GameMode.WordMatch3 && l.chapter == chapterName)
            .OrderBy(l => l.level);

        foreach (LevelData levelData in levelsForChapter)
        {
            GameObject btnGO = Instantiate(levelButtonPrefab_M1, levelButtonContainer_M1);
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
            StartCoroutine(core.TransitionTo(levelSelectPanel_M1_CG, contentEditorPanel_M1_CG));
        }

        currentEditingChapter_M1 = levelData.chapter;
        isDirty_M1 = false;
        currentEditingLevel_M1 = levelData;
        core.SetStatus($"正在编辑: {levelData.chapter} - 关卡 {levelData.level}");
        PopulateGridFromLevelData(levelData);
        UpdateEditorButtonStates();
    }

    private void OnClick_AddLevel()
    {
        if (addLevelPopupPanel_M1_CG != null)
        {
            addLevelPopupPanel_M1_CG.gameObject.SetActive(true);
            addLevelPopupPanel_M1_CG.alpha = 1;
            addLevelPopupPanel_M1_CG.interactable = true;
            addLevelPopupPanel_M1_CG.blocksRaycasts = true;
        }
        core.SetStatus($"当前章节: {currentEditingChapter_M1}。请输入新关卡号。");
    }

    private void OnClick_AddLevel_Cancel()
    {
        if (addLevelPopupPanel_M1_CG != null)
        {
            addLevelPopupPanel_M1_CG.alpha = 0;
            addLevelPopupPanel_M1_CG.interactable = false;
            addLevelPopupPanel_M1_CG.blocksRaycasts = false;
            addLevelPopupPanel_M1_CG.gameObject.SetActive(false);
        }
        core.SetStatus($"当前编辑: 模式 1 / {currentEditingChapter_M1} / (请选择关卡)");
    }

    private void OnClick_AddLevel_Confirm()
    {
        int newLevelNum = -1;
        if (!int.TryParse(newLevelInputfield_M1.text, out newLevelNum) || newLevelNum <= 0)
        {
            core.SetStatus("错误：请输入一个有效的正整数！", Color.red);
            return;
        }

        // 检查是否重复
        bool isDuplicate = TcbManager.AllLevels.levels.Any(l =>
            l.mode == (long)GameMode.WordMatch3 &&
            l.chapter == currentEditingChapter_M1 &&
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
            mode = (int)GameMode.WordMatch3,
            chapter = currentEditingChapter_M1,
            level = newLevelNum,
            id = $"m{(int)GameMode.WordMatch3}-{currentEditingChapter_M1}-l{newLevelNum}",
            content_mode_1 = new List<Mode1Content>(),
            content_mode_2 = new List<Mode2Content>(),
            editorStatus = "Working"
        };
        TcbManager.AllLevels.levels.Add(newLevelData);

        // 关闭弹窗
        if (addLevelPopupPanel_M1_CG != null)
        {
            addLevelPopupPanel_M1_CG.alpha = 0;
            addLevelPopupPanel_M1_CG.interactable = false;
            addLevelPopupPanel_M1_CG.blocksRaycasts = false;
            addLevelPopupPanel_M1_CG.gameObject.SetActive(false);
        }

        // 直接进入编辑界面
        OnClick_SelectLevel(newLevelData, false);
    }
    #endregion

    #region 内容编辑器
    private void OnClick_ContentEditor_Back()
    {
        SaveGridToCurrentLevelData();
        StartCoroutine(core.TransitionTo(contentEditorPanel_M1_CG, levelSelectPanel_M1_CG));
        PopulateLevelList(currentEditingChapter_M1);
        core.SetStatus($"当前编辑: 模式 1 / {currentEditingChapter_M1} / (请选择关卡)");
        currentEditingLevel_M1 = null;
        isDirty_M1 = false;
        UpdateEditorButtonStates();
    }

    private void PopulateGridFromLevelData(LevelData levelData)
    {
        if (gridCellContainer_M1 == null)
        {
            Debug.LogError("[Mode1] gridCellContainer_M1 未找到！");
            return;
        }

        // 清空现有单元格
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in gridCellContainer_M1)
        {
            if (child.gameObject != cellPrefab_M1)
            {
                childrenToDestroy.Add(child.gameObject);
            }
        }
        foreach (GameObject go in childrenToDestroy) { Destroy(go); }

        if (levelData.content_mode_1 == null || levelData.content_mode_1.Count == 0)
        {
            Debug.Log("[Mode1] 当前关卡无内容，需手动生成网格或粘贴数据。");
            return;
        }

        int maxRow = levelData.content_mode_1.Max(c => c.row);
        int maxCol = levelData.content_mode_1.Max(c => c.col);

        // 更新输入框
        if (rowsInputField_M1 != null) rowsInputField_M1.text = maxRow.ToString();
        if (columnsInputField_M1 != null) columnsInputField_M1.text = maxCol.ToString();

        // 设置网格布局
        GridLayoutGroup grid = gridCellContainer_M1.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraintCount = maxCol;
        }

        // 生成单元格
        for (int r = 1; r <= maxRow; r++)
        {
            for (int c = 1; c <= maxCol; c++)
            {
                int row = r, col = c;
                Mode1Content cellData = levelData.content_mode_1.FirstOrDefault(cell => cell.row == row && cell.col == col);
                AddCellRow(row, col, cellData?.text ?? "");
            }
        }
    }

    private void AddCellRow(int row, int col, string text)
    {
        if (cellPrefab_M1 == null || gridCellContainer_M1 == null)
        {
            Debug.LogError("[Mode1] cellPrefab_M1 或 gridCellContainer_M1 未找到！");
            return;
        }

        GameObject cellGO = Instantiate(cellPrefab_M1, gridCellContainer_M1);
        cellGO.SetActive(true);

        M1_EditorCell cellUI = cellGO.GetComponent<M1_EditorCell>();
        if (cellUI != null)
        {
            cellUI.Setup(this, row, col, text);
        }
    }

    private void OnClick_RegenerateGrid()
    {
        if (rowsInputField_M1 == null || columnsInputField_M1 == null)
        {
            core.SetStatus("错误：行/列输入框未找到！", Color.red);
            return;
        }

        if (!int.TryParse(rowsInputField_M1.text, out int rows) || rows <= 0)
        {
            core.SetStatus("错误：请输入有效的行数！", Color.red);
            return;
        }
        if (!int.TryParse(columnsInputField_M1.text, out int cols) || cols <= 0)
        {
            core.SetStatus("错误：请输入有效的列数！", Color.red);
            return;
        }

        // 清空现有单元格
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in gridCellContainer_M1)
        {
            if (child.gameObject != cellPrefab_M1)
            {
                childrenToDestroy.Add(child.gameObject);
            }
        }
        foreach (GameObject go in childrenToDestroy) { Destroy(go); }

        // 设置网格布局
        GridLayoutGroup grid = gridCellContainer_M1.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraintCount = cols;
        }

        // 生成新网格
        for (int r = 1; r <= rows; r++)
        {
            for (int c = 1; c <= cols; c++)
            {
                AddCellRow(r, c, "");
            }
        }

        MarkLevelAsDirty();
        core.SetStatus($"已重新生成 {rows}x{cols} 的网格");
    }

    private void SaveGridToCurrentLevelData()
    {
        if (currentEditingLevel_M1 == null)
        {
            Debug.LogWarning("[Mode1] 无当前关卡，跳过保存");
            return;
        }

        currentEditingLevel_M1.content_mode_1.Clear();

        foreach (Transform child in gridCellContainer_M1)
        {
            if (child.gameObject == cellPrefab_M1) continue;

            M1_EditorCell cellUI = child.GetComponent<M1_EditorCell>();
            if (cellUI != null)
            {
                int row = cellUI.row;
                int col = cellUI.col;
                string text = cellUI.GetText();

                currentEditingLevel_M1.content_mode_1.Add(new Mode1Content
                {
                    row = row,
                    col = col,
                    text = text
                });
            }
        }
    }
    #endregion

    #region 批量粘贴
    private void OnClick_BatchPaste()
    {
        Debug.Log("[Mode1] 正在调用 ShowNativePrompt...");
        NativeBridge.Instance.ShowNativePrompt("", core.gameObject.name, "M1_ReceivePastedTextFromHtml");
    }

    public void OnPasteReceived(string pastedText)
    {
        Debug.Log("[Mode1] 成功接收到 JS 文本！");
        core.SetStatus("已接收到数据，正在解析...");

        parsedPasteData = ParsePastedCSV(pastedText);
        if (parsedPasteData == null || parsedPasteData.Count == 0)
        {
            core.SetStatus("解析失败：未识别到有效数据。", Color.red);
            return;
        }

        OnClick_Paste_ConfirmImport();
    }

    private List<Mode1Content> ParsePastedCSV(string csvText)
    {
        List<Mode1Content> parsedList = new List<Mode1Content>();
        string[] lines = csvText.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith("row,col,text")) continue;

            string[] values = line.Split(new[] { ',', '\t' });
            if (values.Length >= 3)
            {
                if (!int.TryParse(values[0].Trim(), out int row)) { continue; }
                if (!int.TryParse(values[1].Trim(), out int col)) { continue; }
                string text = values[2].Trim();

                parsedList.Add(new Mode1Content
                {
                    row = row,
                    col = col,
                    text = text
                });
            }
        }
        return parsedList;
    }

    private void OnClick_Paste_ConfirmImport()
    {
        if (currentEditingLevel_M1 == null)
        {
            core.SetStatus("错误：请先选中一个关卡！", Color.red);
            return;
        }
        if (parsedPasteData == null || parsedPasteData.Count == 0)
        {
            return;
        }

        // 清空现有单元格
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in gridCellContainer_M1)
        {
            if (child.gameObject != cellPrefab_M1)
            {
                childrenToDestroy.Add(child.gameObject);
            }
        }
        foreach (GameObject go in childrenToDestroy) { Destroy(go); }

        // 计算行列数
        int maxRow = parsedPasteData.Max(c => c.row);
        int maxCol = parsedPasteData.Max(c => c.col);

        // 更新输入框
        if (rowsInputField_M1 != null) rowsInputField_M1.text = maxRow.ToString();
        if (columnsInputField_M1 != null) columnsInputField_M1.text = maxCol.ToString();

        // 设置网格布局
        GridLayoutGroup grid = gridCellContainer_M1.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraintCount = maxCol;
        }

        // 创建单元格
        for (int r = 1; r <= maxRow; r++)
        {
            for (int c = 1; c <= maxCol; c++)
            {
                int row = r, col = c;
                Mode1Content cellData = parsedPasteData.FirstOrDefault(cell => cell.row == row && cell.col == col);
                AddCellRow(row, col, cellData?.text ?? "");
            }
        }

        MarkLevelAsDirty();
        core.SetStatus($"成功导入 {parsedPasteData.Count} 个单元格！");
        parsedPasteData = null;
    }
    #endregion

    #region 删除行功能
    public void OnRequestDeleteRow(int rowIndex)
    {
        pendingDeleteRowIndex_M1 = rowIndex;
        if (deleteRowPopupPanel_M1_CG != null)
        {
            deleteRowPopupPanel_M1_CG.gameObject.SetActive(true);
            deleteRowPopupPanel_M1_CG.alpha = 1;
            deleteRowPopupPanel_M1_CG.interactable = true;
            deleteRowPopupPanel_M1_CG.blocksRaycasts = true;
        }

        // 阻止编辑器交互
        if (contentEditorPanel_M1_CG != null)
        {
            contentEditorPanel_M1_CG.interactable = false;
        }

        core.SetStatus($"确认删除第 {rowIndex} 行吗？");
    }

    private void ConfirmDeleteRow()
    {
        if (pendingDeleteRowIndex_M1 <= 0)
        {
            CancelDeleteRow();
            return;
        }

        // 收集要删除的单元格
        List<GameObject> cellsToDelete = new List<GameObject>();
        foreach (Transform child in gridCellContainer_M1)
        {
            if (child.gameObject == cellPrefab_M1) continue;

            M1_EditorCell cellUI = child.GetComponent<M1_EditorCell>();
            if (cellUI != null && cellUI.row == pendingDeleteRowIndex_M1)
            {
                cellsToDelete.Add(child.gameObject);
            }
        }

        // 删除单元格
        foreach (GameObject go in cellsToDelete)
        {
            Destroy(go);
        }

        // 重新排列后续行
        foreach (Transform child in gridCellContainer_M1)
        {
            if (child.gameObject == cellPrefab_M1) continue;

            M1_EditorCell cellUI = child.GetComponent<M1_EditorCell>();
            if (cellUI != null && cellUI.row > pendingDeleteRowIndex_M1)
            {
                cellUI.row--;
                cellUI.UpdateLabel();
            }
        }

        // 更新行数输入框
        if (rowsInputField_M1 != null && int.TryParse(rowsInputField_M1.text, out int currentRows))
        {
            rowsInputField_M1.text = (currentRows - 1).ToString();
        }

        MarkLevelAsDirty();
        core.SetStatus($"已删除第 {pendingDeleteRowIndex_M1} 行");
        CancelDeleteRow();
    }

    private void CancelDeleteRow()
    {
        pendingDeleteRowIndex_M1 = -1;
        if (deleteRowPopupPanel_M1_CG != null)
        {
            deleteRowPopupPanel_M1_CG.alpha = 0;
            deleteRowPopupPanel_M1_CG.interactable = false;
            deleteRowPopupPanel_M1_CG.blocksRaycasts = false;
            deleteRowPopupPanel_M1_CG.gameObject.SetActive(false);
        }

        // 恢复编辑器交互
        if (contentEditorPanel_M1_CG != null)
        {
            contentEditorPanel_M1_CG.interactable = true;
        }

        if (currentEditingLevel_M1 != null)
        {
            core.SetStatus($"正在编辑: {currentEditingLevel_M1.chapter} - 关卡 {currentEditingLevel_M1.level}");
        }
    }
    #endregion

    #region 单元格编辑
    public void OnRequestEditCell(GameObject cell, string currentText)
    {
        currentEditingCell_M1 = cell;
        Debug.Log($"[Mode1] 正在请求编辑单元格，当前文本: {currentText}");
        NativeBridge.Instance.ShowNativePrompt(currentText, core.gameObject.name, "M1_ReceiveCellEditText");
    }

    public void OnCellEditReceived(string newText)
    {
        Debug.Log($"[Mode1] 接收到 JS 发回的单元格文本: {newText}");
        if (currentEditingCell_M1 == null)
        {
            Debug.LogError("[Mode1] 接收到单元格文本，但 currentEditingCell_M1 为 null！");
            core.SetStatus("接收单元格文本时出错：未找到目标单元格。", Color.red);
            return;
        }

        TMP_InputField inputField = currentEditingCell_M1.GetComponent<TMP_InputField>();
        if (inputField != null)
        {
            inputField.text = newText;
            MarkLevelAsDirty();
            core.SetStatus("单元格已更新。");
        }
        else
        {
            core.SetStatus("接收单元格文本时出错：未在目标上找到 TMP_InputField。", Color.red);
        }

        currentEditingCell_M1 = null;
    }
    #endregion

    #region 全局按钮
    private void OnClick_Save()
    {
        core.SetStatus("正在保存 (M1)...");
        SaveGridToCurrentLevelData();

        isDirty_M1 = false;
        currentEditingLevel_M1.editorStatus = "Working";
        UpdateEditorButtonStates();

        if (TcbManager.instance != null)
        {
            TcbManager.instance.UploadNewLevel(currentEditingLevel_M1.id, currentEditingLevel_M1);
        }
    }

    private void OnClick_TestPlay()
    {
        core.SetStatus("正在保存并准备试玩 (M1)...");
        SaveGridToCurrentLevelData();

        if (currentEditingLevel_M1.content_mode_1 == null || currentEditingLevel_M1.content_mode_1.Count == 0)
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
        LevelManager.selectedGameMode = GameMode.WordMatch3;
        LevelManager.instance.LoadLevel(currentEditingLevel_M1);
    }

    private void OnClick_Publish()
    {
        core.SetStatus("正在发布 (M1)...");
        SaveGridToCurrentLevelData();
        currentEditingLevel_M1.editorStatus = "Published";

        if (TcbManager.instance != null)
        {
            TcbManager.instance.UploadNewLevel(currentEditingLevel_M1.id, currentEditingLevel_M1);
        }
        UpdateEditorButtonStates();
    }
    #endregion

    #region 工具方法
    public void MarkLevelAsDirty()
    {
        isDirty_M1 = true;
        UpdateEditorButtonStates();
    }

    private void UpdateEditorButtonStates()
    {
        if (currentEditingLevel_M1 == null) return;

        bool canPublish = (currentEditingLevel_M1.editorStatus == "Tested") && !isDirty_M1;
        if (publishButton_M1 != null)
        {
            publishButton_M1.interactable = canPublish;
        }

        bool canTest = !isDirty_M1 || currentEditingLevel_M1.editorStatus == "Working";
        if (testPlayButton_M1 != null)
        {
            testPlayButton_M1.interactable = canTest;
        }
    }
    #endregion
}
