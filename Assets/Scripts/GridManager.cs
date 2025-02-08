using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 7;
    public int gridHeight = 7;
    public Transform[,] grid;

    public Transform gridBg;
    public GameObject highLightBG;


    // New: Reference to your grid cell prefab.
    public GameObject gridCellPrefab;

    private BlockSpawner blockSpawner;

    public List<BlockHighLight> blockHighLights;



    void Start()
    {
        blockSpawner = FindObjectOfType<BlockSpawner>();
        grid = new Transform[gridWidth, gridHeight];
        CreateVisibleGrid();
        blockSpawner.CreateSpawnPoints(gridWidth, gridHeight);
        gridBg.localScale = new Vector3(gridWidth * 1.05f, gridHeight * 1.05f, 1);
    }

    // Instantiates the grid cell prefab at every cell coordinate.
    void CreateVisibleGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3 cellPosition = new Vector3(x, y, 0);
                Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);
            }
        }
    }



    // Rounds a vector to the nearest integer values.
    public Vector2 RoundVector2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    // Check if a position is inside the grid.
    public bool InsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    public void AddToGrid(Transform block)
    {
        Vector2 pos = RoundVector2(block.position);
        grid[(int)pos.x, (int)pos.y] = block;
        CheckAndClear();
    }

    public void CheckAndClear()
    {
        List<Transform> blocksToClear = new List<Transform>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] != null)
                {
                    Block blockComponent = grid[x, y].GetComponent<Block>();
                    int horizontalUBBlocks = CountHorizontalUB(x, y);
                    int verticalUBBLocks = CountVerticalUB(x, y);
                    if (blockComponent.number == horizontalUBBlocks)
                    {

                        blocksToClear.Add(grid[x, y]);
                        
                        AddHorizontalUBHighlight(x, y, horizontalUBBlocks);
                    }
                    else if( blockComponent.number == verticalUBBLocks)
                    {
                        blocksToClear.Add(grid[x, y]);
                        AddVerticalUBHighlight(x, y, verticalUBBLocks);
                    }
                }
            }
        }

        if (blocksToClear.Count > 0)
        {
            StartCoroutine(ClearCoRo(blocksToClear));
        }
        else
        {
            blockSpawner.SpawnBlock();
        }

    }

    void AddHorizontalUBHighlight(int x, int y, int horizontalUBCount)
    {
         Vector2 center = new Vector2(x,y);
        int startIndex ;
        for (startIndex = x; startIndex >= 0; startIndex--)
        {
            if (grid[startIndex,y] == null)
                break;
        }
        startIndex++;
        center = new Vector2(startIndex + (horizontalUBCount - 1) / 2f,y);
         if (blockHighLights.Count == 0 || 
         !blockHighLights.Any(a => a.center == center && a.horizontalUBCount == horizontalUBCount &&
          a.verticalUBCount == 1))
        {
            BlockHighLight blockHighLight = Instantiate(highLightBG).GetComponent<BlockHighLight>();
            blockHighLight.Setup(center,horizontalUBCount,1);
            blockHighLights.Add(blockHighLight);
        }
    }

   void AddVerticalUBHighlight(int x, int y, int verticalUBCount)
    {
         Vector2 center = new Vector2(x,y);
        int startIndex ;
        for (startIndex = y; startIndex >= 0; startIndex--)
        {
            if (grid[x,startIndex] == null)
                break;
        }
        startIndex++;
        center = new Vector2(x,startIndex + (verticalUBCount - 1) / 2f);
         if (blockHighLights.Count == 0 || 
         !blockHighLights.Any(a => a.center == center && a.horizontalUBCount == 1 &&
          a.verticalUBCount == verticalUBCount))
        {
            BlockHighLight blockHighLight = Instantiate(highLightBG).GetComponent<BlockHighLight>();
            blockHighLight.Setup(center,1,verticalUBCount);
            blockHighLights.Add(blockHighLight);
        }
    }

    IEnumerator ClearCoRo(List<Transform> blocksToClear)
    {
        foreach(BlockHighLight blockHighLight in blockHighLights)
            blockHighLight.EnableHighLight();
        yield return new WaitForSeconds(.4f);
        foreach (Transform t in blocksToClear)
        {
            Vector2 pos = RoundVector2(t.position);
            grid[(int)pos.x, (int)pos.y] = null;
            Destroy(t.gameObject);
        }
        foreach(BlockHighLight blockHighLight in blockHighLights)
            blockHighLight.DestroyHighLight();
        blockHighLights.Clear();
        StartCoroutine(UpdateGridAfterClear());

    }

    int CountHorizontalUB(int x, int y)
    {

        int count = 0;
        for (int i = x; i >= 0; i--)
        {
            if (grid[i, y] == null)
                break;
            count++;
        }
        for (int i = x + 1; i < gridWidth; i++)
        {
            if (grid[i, y] == null)
                break;
            count++;
        }
        return count;
    }

    int CountVerticalUB(int x, int y)
    {
        int count = 0;
        for (int i = y; i >= 0; i--)
        {
            if (grid[x, i] == null)
                break;
            count++;
        }
        for (int i = y + 1; i < gridHeight; i++)
        {
            if (grid[x, i] == null)
                break;
            count++;
        }
        return count;
    }

    IEnumerator UpdateGridAfterClear()
    {
        yield return null;
        bool gridUpdate = true;
        while (gridUpdate)
        {
            gridUpdate = false;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 1; y < gridHeight; y++)
                {
                    if (grid[x, y] != null && grid[x, y - 1] == null)
                    {
                        gridUpdate = true;
                        grid[x, y - 1] = grid[x, y];
                        grid[x, y] = null;
                        grid[x, y - 1].position += new Vector3(0, -1, 0);
                    }
                }
            }
        }

        CheckAndClear();
    }
}


