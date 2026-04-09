// Scripts/UI/SelectingTileRowUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Thêm thư viện này để dùng LayoutRebuilder
using DG.Tweening;

public class SelectingTileRowUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform container;

    [Header("Settings")]
    [SerializeField] private int maxSlots = 9; // Tùy chỉnh theo game (thường là 7)

    private readonly List<SelectingSlotUI> _slots = new();

    // ── Unity lifecycle ───────────────────────────────

    private void Awake()
    {
        if (container == null) container = transform;
    }

    private void OnEnable()
    {
        EventBus<TileSelectedEvent>.Subscribe(OnTileSelected);
        EventBus<TilesRemovedEvent>.Subscribe(OnTilesRemoved);
    }

    private void OnDisable()
    {
        EventBus<TileSelectedEvent>.Unsubscribe(OnTileSelected);
        EventBus<TilesRemovedEvent>.Unsubscribe(OnTilesRemoved);
    }

    // ── Handlers ─────────────────────────────────────

    private void OnTileSelected(TileSelectedEvent evt)
    {
        if (_slots.Count >= maxSlots) return;

        var go = Instantiate(slotPrefab, container);
        var slot = go.GetComponent<SelectingSlotUI>();
        if (slot == null) slot = go.AddComponent<SelectingSlotUI>();

        slot.Init(evt.tile.tileData);

        // --- 1. TÌM VỊ TRÍ CHÈN VÀO ---
        // Mặc định insertIndex = _slots.Count (tức là CHÈN VÀO CUỐI CÙNG)
        int insertIndex = _slots.Count;

        // Quét ngược tìm gạch giống nó để xếp cạnh nhau
        for (int i = _slots.Count - 1; i >= 0; i--)
        {
            if (_slots[i].TileData == evt.tile.tileData)
            {
                insertIndex = i + 1; // Nằm ngay bên phải viên giống nó
                break;
            }
        }

        // --- 2. CẬP NHẬT UI ---
        _slots.Insert(insertIndex, slot);
        slot.transform.SetSiblingIndex(insertIndex);

        // ÉP UNITY CẬP NHẬT GIAO DIỆN NGAY LẬP TỨC để gạch không bị giật/nhảy vị trí
        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());

        slot.Show();

        // --- 3. HIỆU ỨNG NHÚN NHẢY ---
        AnimateGroup(evt.tile.tileData);
    }

    private void OnTilesRemoved(TilesRemovedEvent evt)
    {
        // Quét ngược để tránh lỗi khi xóa phần tử trong List
        for (int i = _slots.Count - 1; i >= 0; i--)
        {
            if (_slots[i].TileData == evt.tileData)
            {
                // Lấy reference của GameObject trước khi xóa khỏi list
                GameObject slotToDestroy = _slots[i].gameObject;

                _slots[i].Clear();
                _slots.RemoveAt(i);

                // BẮT BUỘC PHẢI DESTROY ĐỂ KHÔNG BỊ KẸT RÁC UI ĐẨY GẠCH LÊN ĐẦU
                Destroy(slotToDestroy);
            }
        }
    }

    // ── Effects ──────────────────────────────────────

    private void AnimateGroup(TileDataSO tileData)
    {
        foreach (var slot in _slots)
        {
            if (slot.TileData == tileData)
            {
                slot.transform.DOKill(true);
                slot.transform.DOPunchScale(Vector3.one * 0.15f, 0.25f, 5, 1);
            }
        }
    }
}