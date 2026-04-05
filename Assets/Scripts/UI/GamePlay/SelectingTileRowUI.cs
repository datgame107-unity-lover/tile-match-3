// Scripts/UI/SelectingTileRowUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectingTileRowUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform container;

    [Header("Settings")]
    [SerializeField] private int maxSlots = 9;

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
        slot.Show();
        _slots.Add(slot);
    }

    private void OnTilesRemoved(TilesRemovedEvent evt)
    {
        for (int i = _slots.Count - 1; i >= 0; i--)
        {
            if (_slots[i].TileData == evt.tileData)
            {
                _slots[i].Clear();
                _slots.RemoveAt(i);
            }
        }
    }
}


