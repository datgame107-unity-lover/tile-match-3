// Scripts/Core/GridConfig.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Định nghĩa grid cố định cho Endless mode.
/// Gắn vào ScriptableObject hoặc truyền thẳng vào BuildRandom.
/// </summary>
[CreateAssetMenu(menuName = "Game/Grid Config")]
public class GridConfig : ScriptableObject
{
    [Header("Grid")]
    public int   columns   = 6;
    public int   rows      = 7;
    public float cellSize  = 0.9f;
    public float colliderSize = 0.8f;
    public Vector2 origin  = Vector2.zero; // góc dưới trái

    public List<Vector2> GetAllPositions()
    {
        var positions = new List<Vector2>();
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < columns; c++)
                positions.Add(origin + new Vector2(c * cellSize, r * cellSize));
        return positions;
    }
}