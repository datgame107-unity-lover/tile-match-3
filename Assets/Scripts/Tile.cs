using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileDataSO tileData;
    public Vector3 worldPos;
    public Vector2Int gridPos;
    public int layer;
    public bool isBlocked = true;
    public bool clicked;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale; // Lưu lại scale ban đầu
    }

    private void OnMouseDown()
    {   
        if(isBlocked) {return;}
        // Phóng to khi được click
        transform.DOScale(originalScale * 1.2f, 0.1f).SetEase(Ease.OutQuad);
    }

    private void OnMouseUp()
    {
        // Thu nhỏ về lại ban đầu khi thả chuột
        transform.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad);
    }
}
