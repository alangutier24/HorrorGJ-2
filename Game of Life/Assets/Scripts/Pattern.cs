using UnityEngine;

[CreateAssetMenu(fileName = "New Pattern", menuName = "Game Of Life/Pattern")]
public class Pattern : ScriptableObject
{
   public Vector2Int[] cells;

   public Vector3Int Center()
    {
        if(cells == null || cells.Length == 0)
            return Vector3Int.zero;

        Vector2Int min = cells[0];
        Vector2Int max = cells[0];
        foreach (var cell in cells)
        {
            min = Vector2Int.Min(min, cell);
            max = Vector2Int.Max(max, cell);
        }
        Vector2Int center = (min + max) / 2;
        return new Vector3Int(center.x, center.y, 0); 
    }
}
