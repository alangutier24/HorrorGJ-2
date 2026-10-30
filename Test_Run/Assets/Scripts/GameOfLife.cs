using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class GameOfLife : MonoBehaviour
{
    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tilemap nextState;
    
    [SerializeField] private Tile aliveTile;
    [SerializeField] private Tile deadTile;

    [SerializeField] private Vector3Int[] initialState;

    HashSet<Vector3Int> aliveCells;
    HashSet<Vector3Int> deadCells;
    HashSet<Vector3Int> cellsToCheck;

    void Start()
    {
        aliveCells = new HashSet<Vector3Int>();
        deadCells = new HashSet<Vector3Int>();
        cellsToCheck = new HashSet<Vector3Int>();

        foreach (var cell in initialState)
        {
            aliveCells.Add(cell);
            currentState.SetTile(cell, aliveTile);
        }
        StartCoroutine(UpdateLoop());
    }


    private void UpdateStates()
    {
        nextState.ClearAllTiles();
        HashSet<Vector3Int> nextGeneration = new HashSet<Vector3Int>();
        foreach (var cell in aliveCells)
        {
            int aliveNeighbors = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    var neighbor = cell + new Vector3Int(x, y, 0);
                    if(!IsAlive(neighbor))
                    {
                        cellsToCheck.Add(neighbor);
                    }
                    else
                    {
                        aliveNeighbors++;
                    }
                }
            }

            if (aliveNeighbors == 2 || aliveNeighbors == 3 && IsAlive(cell))
            {
                nextState.SetTile(cell, aliveTile);
                nextGeneration.Add(cell);
            }
            else if (aliveNeighbors == 3)
            {
                nextState.SetTile(cell, aliveTile);
                nextGeneration.Add(cell);
            }
            else
            {
                nextState.SetTile(cell, deadTile);
                deadCells.Add(cell);
                nextGeneration.Remove(cell);
                nextState.SetTile(cell, deadTile);
            }
        }

        foreach(var cell in cellsToCheck)
        {
            int aliveNeighbors = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    var neighbor = cell + new Vector3Int(x, y, 0);
                    if (IsAlive(neighbor))
                    {
                        aliveNeighbors++;
                    }
                }
            }

            if (aliveNeighbors == 3 && !IsAlive(cell))
            {
                nextState.SetTile(cell, aliveTile);
                nextGeneration.Add(cell);
                deadCells.Remove(cell);
            }
        }
        aliveCells = nextGeneration;
        
        Tilemap temp = currentState;
        currentState = nextState;
        nextState = temp;
    

    }

    private bool IsAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == aliveTile;
    }

    private IEnumerator UpdateLoop(float updateInterval = 0.05f)
    {
        while (true)
        {
            UpdateStates();
            yield return new WaitForSeconds(updateInterval);
        }
    }

}
