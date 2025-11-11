using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Shapes/Heart Shape")]
public class HeartShapeSO : BaseShapeSO
{
    public override Vector2[] GetTilePositions()
    {
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < tilesPerLayer; i++)
        {
            float t = (float)i / tilesPerLayer * Mathf.PI * 2f;

            float x = 16 * Mathf.Pow(Mathf.Sin(t), 3);
            float y = 13 * Mathf.Cos(t) - 5 * Mathf.Cos(2 * t) - 2 * Mathf.Cos(3 * t) - Mathf.Cos(4 * t);

            points.Add(new Vector2(x, y) * scale * 0.05f);
        }

        return points.ToArray();
    }
}
