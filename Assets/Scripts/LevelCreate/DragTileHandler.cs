// Scripts/LevelCreate/DragTileHandler.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class DragTileHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TileDataSO tileData;
    public BoardController board;

    private GameObject _ghost;
    private Camera _cam;

    private void Start() => _cam = Camera.main;

    public void OnBeginDrag(PointerEventData e)
    {
        _ghost = Instantiate(board.tilePrefab);
        _ghost.GetComponent<Tile>().Init(tileData, 0);
        var col = _ghost.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    public void OnDrag(PointerEventData e)
    {
        if (_ghost == null) return;
        var world = _cam.ScreenToWorldPoint(
            new Vector3(e.position.x, e.position.y, -_cam.transform.position.z));
        world.z = 0;
        var snapped = board.Snap(world);
        _ghost.transform.position = new Vector3(snapped.x, snapped.y, 0);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (_ghost == null) return;
        bool placed = board.TryPlace(tileData, _ghost.transform.position);
        if (placed) FindFirstObjectByType<LevelPanelUI>()?.MarkDirty();
        Destroy(_ghost);
        _ghost = null;
    }
}