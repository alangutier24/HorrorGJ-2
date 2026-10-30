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


    HashSet<Vector3Int> aliveCells = new HashSet<Vector3Int>();
    HashSet<Vector3Int> deadCells = new HashSet<Vector3Int>();
    HashSet<Vector3Int> cellsToCheck = new HashSet<Vector3Int>();

    void Start()
    {
        foreach(var cell in initialState)
        {
            currentState.SetTile(cell, aliveTile);
            aliveCells.Add(cell);
        }
        StartCoroutine(UpdateStatesCoroutine());
    }


    void UpdateStates()
    {
        nextState.ClearAllTiles();
        HashSet<Vector3Int> newAliveCells = new HashSet<Vector3Int>();
        foreach(var cell in aliveCells)
        {
            int aliveNeighbors = 0;
            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    if(x == 0 && y == 0) continue;
                    Vector3Int neighbor = cell + new Vector3Int(x, y, 0);
                    if(IsAlive(neighbor))
                    {
                        aliveNeighbors++;
                    }
                    else
                    {
                        cellsToCheck.Add(neighbor);
                    }
                }
            }
            if(aliveNeighbors == 2 || (aliveNeighbors == 3 && IsAlive(cell)))
            {
                nextState.SetTile(cell, aliveTile);
                newAliveCells.Add(cell);
            }
            else
            {
                nextState.SetTile(cell, deadTile);
                newAliveCells.Remove(cell);
            }
            
        }

        foreach(var cell in cellsToCheck)
        {
            int aliveNeighbors = 0;
            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    if(x == 0 && y == 0) continue;
                    Vector3Int neighbor = cell + new Vector3Int(x, y, 0);
                    if(IsAlive(neighbor))
                    {
                        aliveNeighbors++;
                    }
                }
            }
            if(aliveNeighbors == 3)
            {
                nextState.SetTile(cell, aliveTile);
                newAliveCells.Add(cell);
            }
        }

        aliveCells = newAliveCells;

        Tilemap temp = currentState;
        currentState = nextState;
        nextState = temp;
    }

    bool IsAlive(Vector3Int cell)
    {
        return currentState.GetTile(cell) == aliveTile;
    }

    IEnumerator UpdateStatesCoroutine()
    {
        while(enabled)
        {
            UpdateStates();
            yield return new WaitForSeconds(0.05f);
        }
    }

}
