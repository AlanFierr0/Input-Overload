using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkerGenerator : MonoBehaviour
{
    public enum grid
    {
        FLOOR,
        WALL,
        EMPTY
    }
    public grid[,] map;
    public List<WalkerObject> walkers;
    public Tilemap tilemap;
    public Tile Floor;
    public Tile Wall;
    public int width;
    public int height;

    public int maxWalkers;
    public int tileCount = 0;
    public float fillPercent;
    public float waitTime;
    const float hardTimeout = 5f; 

    private void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        // if hard timeout is reached, stop all coroutines
        if (Time.timeSinceLevelLoad > hardTimeout)
        {
            StopAllCoroutines();
        }
    }

    void GenerateMap()
    {
        map = new grid[width, height];
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                map[x, y] = grid.EMPTY;
            }
        }
        walkers = new List<WalkerObject>();
        Vector3Int center = new Vector3Int(width / 2, height / 2, 0);
        WalkerObject newWalker = new WalkerObject(new Vector2(center.x, center.y), GetDirection(), 0.5f);
        map[center.x, center.y] = grid.FLOOR;
        tilemap.SetTile(center, Floor);
        walkers.Add(newWalker);
        tileCount++;
        StartCoroutine(CreateFloors());
    }

    Vector2 GetDirection()
    {
        int dir = Random.Range(0, 4);
        switch (dir)
        {
            case 0:
                return Vector2.up;
            case 1:
                return Vector2.down;
            case 2:
                return Vector2.left;
            case 3:
                return Vector2.right;
            default:
                return Vector2.up;
        }
    }

    IEnumerator CreateFloors()
    {
        while ((float)tileCount / (float)map.Length < fillPercent)
        {
            bool hasCreatedFloor = false;
            foreach (WalkerObject walker in walkers)
            {
                Vector3Int currentPos = new Vector3Int((int)walker.position.x, (int)walker.position.y, 0);

                if (map[currentPos.x, currentPos.y] != grid.FLOOR)
                {
                    tilemap.SetTile(currentPos, Floor);
                    tileCount++;
                    map[currentPos.x, currentPos.y] = grid.FLOOR;
                    hasCreatedFloor = true;
                }
            }

            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();

            if (hasCreatedFloor)
            {
                yield return new WaitForSeconds(waitTime);
            }
        }
        StartCoroutine(CreateWalls());
    }

    void ChanceToRemove()
    {
        int updatedCount = walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (walkers.Count > 1 && Random.Range(0f, 1f) < 0.05f)
            {
                walkers.RemoveAt(i);
                break;
            }
        }
    }

    void ChanceToRedirect()
    {
        foreach (WalkerObject walker in walkers)
        {
            if (Random.Range(0f, 1f) < walker.chanceToChangeDirection)
            {
                walker.direction = GetDirection();
            }
        }
    }

    void ChanceToCreate()
    {
        int updatedCount = walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (Random.Range(0f, 1f) < walkers[i].chanceToChangeDirection && walkers.Count < maxWalkers)
            {
                Vector2 dir = GetDirection();
                Vector2 pos = walkers[i].position;

                WalkerObject newWalker = new WalkerObject(pos, dir, 0.5f);
                walkers.Add(newWalker);
            }
        }
    }

    void UpdatePosition()
    {
        foreach (WalkerObject walker in walkers)
        {
            walker.position += walker.direction;
            walker.position.x = Mathf.Clamp(walker.position.x, 1, width - 2);
            walker.position.y = Mathf.Clamp(walker.position.y, 1, height - 2);
        }
    }

    IEnumerator CreateWalls()
    {
        for (int x = 0; x < map.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < map.GetLength(1) - 1; y++)
            {
                if (map[x, y] == grid.FLOOR)
                {
                    bool hasCreatedWall = false;
                    if (map[x + 1, y] == grid.EMPTY)
                    {
                        Vector3Int pos = new Vector3Int(x + 1, y, 0);
                        tilemap.SetTile(pos, Wall);
                        map[x + 1, y] = grid.WALL;
                        hasCreatedWall = true;
                    }
                    if (map[x - 1, y] == grid.EMPTY)
                    {
                        Vector3Int pos = new Vector3Int(x - 1, y, 0);
                        tilemap.SetTile(pos, Wall);
                        map[x - 1, y] = grid.WALL;
                        hasCreatedWall = true;
                    }
                    if (map[x, y + 1] == grid.EMPTY)
                    {
                        Vector3Int pos = new Vector3Int(x, y + 1, 0);
                        tilemap.SetTile(pos, Wall);
                        map[x, y + 1] = grid.WALL;
                        hasCreatedWall = true;
                    }
                    if (map[x, y - 1] == grid.EMPTY)
                    {
                        Vector3Int pos = new Vector3Int(x, y - 1, 0);
                        tilemap.SetTile(pos, Wall);
                        map[x, y - 1] = grid.WALL;
                        hasCreatedWall = true;
                    }
                    if (hasCreatedWall)
                    {
                        yield return new WaitForSeconds(waitTime / 2);
                    }
                }
            }
        }
    }
}
