# LevelEditor 重构指南 - 第三阶段

## 概述

由于 LevelEditorManager.cs 文件非常大（1848 行），完整迁移需要大量工作。
本文档提供了重构的框架和指导，帮助你完成剩余的迁移工作。

## 已完成

✅ LevelEditor_Core.cs - 总控脚本已创建

- 权限检查
- 模式切换
- WebGL 回调分发
- 公共工具方法

## 待完成步骤

### 1. 创建 LevelEditor_Mode1.cs

这个文件需要包含所有 M1\_ 开头的方法和变量。

#### 需要迁移的变量（从 LevelEditorManager.cs）

从第 31-76 行迁移所有 "模式 1 UI 引用" 部分的变量：

- chapterSelectPanel_M1
- hsk1Button_M1, hsk2Button_M1, hsk3Button_M1, hsk4Button_M1
- chapterPanelBackButton_M1
- levelSelectPanel_M1
- levelPanelBackButton_M1, addLevelButton_M1
- levelListContainer_M1, levelInEditorButtonPrefab_M1
- contentEditorPanel_M1
- contentEditorBackButton_M1, addRowButton_M1
- contentListContainer_M1, m1EditorCellPrefab
- saveButton_M1, testPlayButton_M1, publishButton_M1
- publishButtonCG
- batchPasteButton_M1
- addLevelPopupPanel_M1, newLevelInputfield_M1
- addLevelConfirmButton_M1, addLevelCancelButton_M1
- deleteRowPopupPanel, deleteConfirmButton, deleteCancelButton
- batchPastePopupPanel, pasteInputField
- pastePreviewContainer, pasteConfirmImportButton, pasteCancelButton

#### 需要迁移的私有变量

- currentEditingChapter_M1
- currentEditingLevel_M1
- parsedPasteData
- isDirty_M1
- m1_cellIndexToDelete
- currentEditingCell_M1
- 所有相关的 CanvasGroup 引用

#### 需要迁移的方法（从 LevelEditorManager.cs）

从第 462 行开始的所有 M1\_ 方法：

- M1_OnClick_SelectChapter (462 行)
- M1_OnClick_BackToChapterSelect (470 行)
- M1_PopulateLevelList (486 行)
- M1_OnClick_AddLevel (520 行)
- M1_OnClick_AddLevel_Cancel (533 行)
- M1_OnClick_AddLevel_Confirm (545 行)
- M1_OnClick_SelectLevel (590 行)
- M1_OnClick_ContentEditor_Back (610 行)
- M1_PopulateContentEditor (620 行)
- M1_OnClick_AddRow (633 行)
- M1_AddCellRow (653 行)
- M1_SaveGridToCurrentLevelData (690 行左右)
- M1_OnClick_SaveLevel (722 行)
- M1_OnClick_TestPlay (738 行)
- M1_OnClick_Publish (769 行)
- M1_OnClick_BatchPaste (850 行)
- M1_OnPasteInputChanged (876 行)
- M1_AddCellRow_Preview (890 行)
- M1_ParsePastedCSV (910 行)
- M1_OnClick_Paste_ConfirmImport (932 行)
- M1_OnClick_Paste_Cancel (943 行)
- M1_OnRequestDeleteRow (961 行)
- M1_ConfirmDeleteRow
- M1_CancelDeleteRow
- OnAnyCellChanged
- MarkLevelAsDirty
- UpdateEditorButtonStates

#### 需要添加的新方法

```csharp
public void Initialize(LevelEditor_Core core)
{
    this.core = core;
    // 初始化 CanvasGroups
    // 绑定按钮事件
}

public void EnterMode()
{
    // 显示章节选择面板
    // 调用 M1_OnClick_BackToChapterSelect(true)
}

public void SelectLevelInstant(LevelData levelData)
{
    // 用于从试玩返回时直接打开关卡
    M1_OnClick_SelectLevel(levelData, true);
}

public void OnPasteReceived(string pastedText)
{
    // WebGL 回调处理
    parsedPasteData = M1_ParsePastedCSV(pastedText);
    M1_OnClick_Paste_ConfirmImport();
}

public void OnCellEditReceived(string newText)
{
    // WebGL 回调处理
    // 更新 currentEditingCell_M1 的内容
}
```

#### 修改要点

1. 所有 `statusText.text = ...` 改为 `core.SetStatus(...)`
2. 所有 `this` 或 `LevelEditorManager` 改为使用 `this`（Mode1）
3. 添加 `public LevelEditor_Core core;` 变量

### 2. 创建 LevelEditor_Mode2.cs

这个文件需要包含所有 M2\_ 开头的方法和变量。

#### 需要迁移的变量（从 LevelEditorManager.cs）

从第 78-141 行迁移所有 "模式 2 UI 引用" 部分的变量：

- chapterSelectPanel_M2
- hsk1Button_M2, hsk2Button_M2, hsk3Button_M2, hsk4Button_M2
- chapterPanelBackButton_M2
- levelSelectPanel_M2
- levelPanelBackButton_M2, addLevelButton_M2
- levelListContainer_M2
- contentEditorPanel_M2
- detail_SentenceIdInput, detail_FullSentenceInput
- contentEditorBackButton_M2
- addSentenceButton_M2, globalAutoSplitButton_M2
- batchPasteButton_M2, addWordButton_M2
- detailSaveButton_M2, resplitButton_M2
- mergeUpButton_M2, mergeDownButton_M2, deleteRowButton_M2
- sentenceInputContainer_M2, m2SentenceInputPrefab
- wordListContainer_M2, m2WordRowPrefab
- saveButton_M2, testPlayButton_M2, publishButton_M2
- publishButtonCG_M2
- batchPastePopupPanel_M2, pasteInputField_M2
- pastePreviewContainer_M2
- pasteConfirmImportButton_M2, pasteCancelButton_M2
- addLevelPopupPanel_M2, newLevelInputfield_M2
- addLevelConfirmButton_M2, addLevelCancelButton_M2

#### 需要迁移的私有变量

- currentEditingChapter_M2
- currentEditingLevel_M2
- parsedPasteSentences
- currentSelectedSentenceRow
- currentSelectedWordRow
- isDirty_M2
- 所有相关的 CanvasGroup 引用

#### 需要迁移的方法

搜索 LevelEditorManager.cs 中所有以 M2\_ 开头的方法：

- M2_OnClick_SelectChapter
- M2_OnClick_BackToChapterSelect
- M2_PopulateLevelList
- M2_OnClick_AddLevel
- M2_OnClick_AddLevel_Cancel
- M2_OnClick_AddLevel_Confirm
- M2_OnClick_SelectLevel
- M2_OnClick_ContentEditor_Back
- M2_PopulateSentenceEditor
- M2_OnClick_AddSentence
- M2_OnClick_GlobalSplit
- M2_OnSentenceSelected
- M2_PopulateWordList
- M2_UpdateContextualButtons
- M2_OnWordSelected
- M2_OnClick_AddWord
- M2_OnClick_DetailSave
- M2_RequestAutoSplit_Current
- M2_OnClick_MergeUp
- M2_OnClick_MergeDown
- M2_OnClick_DeleteWord
- M2_OnClick_BatchPaste
- M2_OnPasteInputChanged
- M2_ParsePastedCSV
- M2_OnClick_Paste_ConfirmImport
- M2_OnClick_Paste_Cancel
- OnClick_Save_M2
- OnClick_TestPlay_M2
- OnClick_Publish_M2

#### 需要添加的新方法

```csharp
public void Initialize(LevelEditor_Core core)
{
    this.core = core;
    // 初始化 CanvasGroups
    // 绑定按钮事件
}

public void EnterMode()
{
    // 显示章节选择面板
    // 调用 M2_OnClick_BackToChapterSelect(true)
}

public void SelectLevelInstant(LevelData levelData)
{
    // 用于从试玩返回时直接打开关卡
    M2_OnClick_SelectLevel(levelData, true);
}

public void OnPasteReceived(string pastedText)
{
    // WebGL 回调处理
    parsedPasteSentences = M2_ParsePastedCSV(pastedText);
    M2_OnClick_Paste_ConfirmImport();
}
```

### 3. 修改辅助脚本

#### M1_EditorCell.cs

将所有 `LevelEditorManager` 改为 `LevelEditor_Mode1`：

```csharp
// 旧代码
private LevelEditorManager editorManager;
public void Setup(LevelEditorManager manager)

// 新代码
private LevelEditor_Mode1 mode1Controller;
public void Setup(LevelEditor_Mode1 controller)
```

#### M2_WordRow.cs 和 M2_SentenceInputRow.cs

同样将 `LevelEditorManager` 改为 `LevelEditor_Mode2`：

```csharp
// 旧代码
private LevelEditorManager editorManager;
public void Setup(LevelEditorManager manager, ...)

// 新代码
private LevelEditor_Mode2 mode2Controller;
public void Setup(LevelEditor_Mode2 controller, ...)
```

## 迁移技巧

### 查找方法范围

使用 VS Code 的折叠功能查看方法结构：

1. 按 Ctrl+K, Ctrl+0 折叠所有代码
2. 按 Ctrl+F 搜索方法名
3. 展开对应的方法复制完整代码

### 批量替换

1. 在新文件中，全局搜索替换：

   - `statusText.text =` → `core.SetStatus(`
   - `if (statusText != null) statusText.text =` → `core.SetStatus(`
   - 添加相应的 `)`

2. 检查所有 `this` 引用，确保指向正确的对象

### CanvasGroup 初始化

在 Initialize 方法中添加：

```csharp
chapterSelectPanel_M1_CG = chapterSelectPanel_M1.GetComponent<CanvasGroup>()
    ?? chapterSelectPanel_M1.AddComponent<CanvasGroup>();
// ... 对所有面板重复此操作
```

### 按钮绑定示例

```csharp
if (hsk1Button_M1 != null)
    hsk1Button_M1.onClick.AddListener(() => OnClick_SelectChapter("HSK1"));
if (hsk2Button_M1 != null)
    hsk2Button_M1.onClick.AddListener(() => OnClick_SelectChapter("HSK2"));
// ... 等等
```

## 测试检查清单

- [ ] 编译无错误
- [ ] 模式选择下拉框工作正常
- [ ] 可以进入模式 1 和模式 2
- [ ] 可以选择章节
- [ ] 可以创建和编辑关卡
- [ ] 保存、试玩、发布功能正常
- [ ] 从试玩返回后能正确恢复状态
- [ ] WebGL 输入框集成工作正常

## 场景设置

1. 在场景中创建一个空物体 "LevelEditor"
2. 挂载 LevelEditor_Core 脚本
3. 创建两个子物体 "Mode1Controller" 和 "Mode2Controller"
4. 分别挂载 LevelEditor_Mode1 和 LevelEditor_Mode2
5. 在 Core 的 Inspector 中：
   - 将 Mode1Controller 拖到 mode1Controller 字段
   - 将 Mode2Controller 拖到 mode2Controller 字段
   - 绑定所有公共 UI 引用
6. 在 Mode1Controller 和 Mode2Controller 的 Inspector 中：
   - 绑定各自的 UI 引用

## 注意事项

1. 保留原 LevelEditorManager.cs 作为参考，不要立即删除
2. 所有 using 语句都需要复制到新文件
3. 公共方法签名必须保持一致
4. CanvasGroup 的管理需要特别注意
5. 确保所有事件监听器都正确绑定
