using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private float updateInterval = 0.05f;
    [SerializeField] private Pattern pattern;

    private HashSet<Vector3Int> aliveCells = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> checkCells = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> nextAliveCells = new HashSet<Vector3Int>();

    void Start()
    {
        SetPattern();
        StartCoroutine(UpdateBoardCorroutine(updateInterval));
    }

    private void SetPattern()
    {
        ClearBoard();
        if (pattern != null)
        {
        
            Vector3Int center = pattern.Center();
            foreach (Vector3Int cell in pattern.cells)
            {
                Vector3Int offsetCell = cell - center;
                currentState.SetTile(offsetCell, aliveTile);
                aliveCells.Add(offsetCell);
            }
        }
    }

    private IEnumerator UpdateBoardCorroutine(float interval)
    {
        while(enabled)
        {
            UpdateBoard();
            yield return new WaitForSeconds(interval);
        }
    }
    private void UpdateBoard()
    {
        nextAliveCells.Clear();
        foreach (Vector3Int cell in aliveCells)
        {
            int aliveNeighbors = 0;
            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    Vector3Int neighbor = cell + new Vector3Int(x, y, 0);
                    if(x == 0 && y == 0) continue;
                    if (IsAlive(neighbor))
                    {
                        aliveNeighbors++;
                    }
                    else
                    {
                        checkCells.Add(neighbor);
                    }
                }
            }
            if (aliveNeighbors == 3 || (aliveNeighbors == 2 && IsAlive(cell)))
            {
                nextState.SetTile(cell, aliveTile);
                nextAliveCells.Add(cell);
            }
            else
            {
                nextState.SetTile(cell, null);
                nextAliveCells.Remove(cell);
            }
        }
        foreach (var cell in checkCells)
        {
            int aliveNeighbors = 0;
            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    Vector3Int neighbor = cell + new Vector3Int(x, y, 0);
                    if(x == 0 && y == 0) continue;
                    if (IsAlive(neighbor))
                    {
                        aliveNeighbors++;
                    }
                }
            }
            if (aliveNeighbors == 3 && !IsAlive(cell))
            {
                nextState.SetTile(cell, aliveTile);
                nextAliveCells.Add(cell);
            }
        }

        aliveCells = new HashSet<Vector3Int>(nextAliveCells);

        Tilemap temp = currentState;
        currentState = nextState;
        nextState = temp;
        nextState.ClearAllTiles();
        checkCells.Clear();
        nextAliveCells.Clear();
    }

    void ClearBoard()
    {
        currentState.ClearAllTiles();
        nextState.ClearAllTiles();
    }

    private bool IsAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == aliveTile;
    }
}
