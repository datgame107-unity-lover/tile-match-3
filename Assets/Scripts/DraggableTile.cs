using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableTile : MonoBehaviour,
    IBeginDragHandler,IDragHandler, IEndDragHandler
{
    private ScrollRect parentScroll;
    public TileDataSO tileData;
    private void Awake()
    {
        parentScroll = GetComponentInParent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("BEGIN DRAG");

        if (LevelEditorManager.Instance == null)
        {
            Debug.LogError("LevelEditorManager.Instance == NULL");
            return;
        }

        var editor = LevelEditorManager.Instance;

        if (tileData == null)
        {
            Debug.LogError("tileData == NULL");
            return;
        }

        if (editor.shadow == null)
        {
            Debug.LogError("editor.shadow == NULL");
            return;
        }

        var sr = editor.shadow.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("Shadow has NO SpriteRenderer");
            return;
        }

        editor.isDragging = true;
        editor.selectedTile = tileData;

        editor.shadow.SetActive(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var editor = LevelEditorManager.Instance;

        editor.TryPlaceSelectedTile(eventData.position);

        editor.isDragging = false;
        editor.selectedTile = null;
        editor.shadow.SetActive(false);

        if (parentScroll != null)
            parentScroll.enabled = true;

        LevelEditorManager.Instance.Sort();
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}
