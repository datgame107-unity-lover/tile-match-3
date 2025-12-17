using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [SerializeField] private GameObject tileSelectGlowPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void OnEnable()
    {
        EventManager.OnTileSelected += HandleTileSelected;
    }

    private void OnDisable()
    {
            EventManager.OnTileSelected -= HandleTileSelected;
    }

    private void HandleTileSelected(Tile tile)
    {
        // VFX Spawn
        var fx = Instantiate(tileSelectGlowPrefab, tile.transform.position, Quaternion.identity, tile.transform);
        fx.transform.localPosition = Vector3.zero;
    }
}
