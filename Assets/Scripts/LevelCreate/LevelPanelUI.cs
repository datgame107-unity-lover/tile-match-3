// Scripts/LevelCreate/LevelPanelUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanelUI : MonoBehaviour
{
    [Header("Refs")]
    public BoardController board;
    public Transform listContainer;
    public GameObject levelItemPrefab;
    public Button newButton;
    public ConfirmDialogUI confirmDialog;

    private LevelJsonService _jsonService;
    
    private int _editingIndex = -1;
    private bool _isDirty = false;
    private bool _isNewUnsaved = false;

    private void Awake()
    {
        _jsonService = new LevelJsonService();
    }

    private void OnEnable()
    {
        if (newButton != null)
        {
            newButton.onClick.RemoveListener(OnClickNew);
            newButton.onClick.AddListener(OnClickNew);
        }
        RefreshList();
    }

    // ── NEW ──────────────────────────────────────────────
    private void OnClickNew()
    {
        TryLeave(() =>
        {
            _editingIndex = GetNextIndex();
            _isNewUnsaved = true;
            _isDirty = false;
            board.ClearBoard();
            SetNewButtonInteractable(false);
            RefreshList();
            Debug.Log($"[LevelPanel] New level {_editingIndex}");
        });
    }

    // ── CLICK ITEM ───────────────────────────────────────
    public void OnClickItem(int index)
    {
        if (index == _editingIndex)
        {
            // click lại level đang edit → bỏ chọn, về board trống
            TryLeave(() =>
            {
                _editingIndex = -1;
                _isNewUnsaved = false;
                _isDirty = false;
                board.ClearBoard();
                SetNewButtonInteractable(true);
                RefreshList();
                Debug.Log("[LevelPanel] Deselected — board cleared");
            });
            return;
        }

        TryLeave(() => LoadLevel(index));
    }

    // ── EXPORT ───────────────────────────────────────────
    public void OnExport()
    {
        if (_editingIndex < 0)
        {
            Debug.LogWarning("[LevelPanel] Chưa chọn level");
            return;
        }
        var data = board.Export(_editingIndex);
        if (data.tiles.Count == 0)
            Debug.LogWarning($"[LevelPanel] Board trống — vẫn save level {_editingIndex}");

        _jsonService.Save(data);
        _isNewUnsaved = false;
        _isDirty = false;
        SetNewButtonInteractable(true);
        RefreshList();
        Debug.Log($"[LevelPanel] Saved level {_editingIndex} — {data.tiles.Count} tiles");
    }

    // ── DELETE ───────────────────────────────────────────
    public void OnDelete(int index)
    {
        confirmDialog.Show(
            message: $"Xóa level {index}?",
            onConfirm: () =>
            {
                if (!_isNewUnsaved || index != _editingIndex)
                    _jsonService.Delete(index);

                if (_editingIndex == index)
                {
                    _editingIndex = -1;
                    _isNewUnsaved = false;
                    _isDirty = false;
                    board.ClearBoard();
                    SetNewButtonInteractable(true);
                }
                RefreshList();
                Debug.Log($"[LevelPanel] Deleted level {index}");
            },
            onCancel: null
        );
    }

    // ── DIRTY ────────────────────────────────────────────
    public void MarkDirty()
    {
        _isDirty = true;
        RefreshList();
    }

    // ── Private ──────────────────────────────────────────
    private void LoadLevel(int index)
    {
        var data = _jsonService.Load(index);
        if (data == null)
        {
            Debug.LogWarning($"[LevelPanel] Level {index} not found");
            return;
        }
        
        _editingIndex = index;
        _isNewUnsaved = false;
        _isDirty = false;
        board.LoadIntoBoard(data);
        SetNewButtonInteractable(false);
        RefreshList();
        Debug.Log($"[LevelPanel] Loaded level {index}");
    }

    private void TryLeave(System.Action onConfirm)
    {
        if (_editingIndex >= 0 && _isDirty)
        {
            confirmDialog.Show(
                message: $"Level {_editingIndex} chưa save. Lưu lại không?",
                onConfirm: () => { OnExport(); onConfirm?.Invoke(); },
                onCancel: () => { _isDirty = false; onConfirm?.Invoke(); }
            );
        }
        else
        {
            onConfirm?.Invoke();
        }
    }

    private void RefreshList()
    {
        foreach (Transform t in listContainer)
            Destroy(t.gameObject);

        if (_isNewUnsaved && _editingIndex >= 0)
            SpawnItem(_editingIndex, isEditing: true, isDirty: _isDirty);

        foreach (var idx in _jsonService.GetAllLevelIndices())
            SpawnItem(idx,
                isEditing: idx == _editingIndex,
                isDirty: idx == _editingIndex && _isDirty);
    }

    private void SpawnItem(int index, bool isEditing, bool isDirty)
    {
        var go = Instantiate(levelItemPrefab, listContainer);
        var item = go.GetComponent<LevelItemUI>();
        if (item != null) item.Init(index, isEditing, isDirty, this);
    }

    private int GetNextIndex()
    {
        var indices = _jsonService.GetAllLevelIndices();
        int next = 1;
        while (indices.Contains(next)) next++;
        return next;
    }

    private void SetNewButtonInteractable(bool value)
    {
        if (newButton != null) newButton.interactable = value;
    }
}