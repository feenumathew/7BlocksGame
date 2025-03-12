using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockPrefab;  // Assign your block prefab in the Inspector.

    public GameObject waterBlockPrefab;
    public GameObject gridPrefab;
    public Transform top;
    public List<GridCell> topSpawnPoints;    // spawnPoints positioned at the top-center of your grid.

    public List<GridCell> bottomSpawnPoints;
    public int blockSpawnCount = 0;

    private GridManager gridManager;
    private int width;
    private int height;

    private void Awake()
    {
        topSpawnPoints = new List<GridCell>();
        bottomSpawnPoints = new List<GridCell>();
        gridManager = FindFirstObjectByType<GridManager>();
        width = GameSettings.Instance.gridSize;
        height = GameSettings.Instance.gridSize;
    }



    public void SetupSpawnPoints()
    {
        for (int i = 0, j = height + GameSettings.Instance.blockSpawnVerticalOffsetInUnits - 1; i < width; i++)
        {
            topSpawnPoints.Add(gridManager.gridCells[i, j]);
        }
        for (int i = 0, j = -1; i < width; i++)
        {
            bottomSpawnPoints.Add(gridManager.gridCells[i, j]);
        }
    }
    // Call this method to spawn a new block.



    public void StartSpawningBlock()
    {
        StartCoroutine(BlockSpawningLoop());
    }

    IEnumerator BlockSpawningLoop()
    {
        while (true)
        {
            yield return new WaitWhile(() => (gridManager.gridState == GridState.InProcess));

            if (gridManager.IsTopRowFull())
            {
                GameManager.Instance.GameOver();
                yield break;
            }

            blockSpawnCount++;

            if (blockSpawnCount % (GameSettings.Instance.levelIncreaseGap+1) == 0)
            {

                GameManager.Instance.IncreaseLevel();
                SpawnBottomWaterBlocks();
                gridManager.MoveAllBlocksUp();
                gridManager.gridState = GridState.InProcess;
                yield return new WaitWhile(() => (gridManager.gridState == GridState.InProcess));
                SpawnBlock();

            }
            else
            {
                SpawnBlock();
            }
        }
    }




    void SpawnBottomWaterBlocks()
    {
        int blockNum;
        for (int i = 0; i < width; i++)
        {
            blockNum = Helper.GenerateWeightedRandom("RandomBlockNumber", 1, GameSettings.Instance.gridSize);
            GameObject blockObj = Instantiate(waterBlockPrefab, (Vector2)bottomSpawnPoints[i].gridPos, Quaternion.identity);
            WaterBlock block = blockObj.GetComponent<WaterBlock>();
            block.Initialize(blockNum, bottomSpawnPoints[i]);
        }
    }

    private void SpawnBlock()
    {
        gridManager.gridState = GridState.InProcess;
        int r = Helper.GenerateWeightedRandom("RandomSpawnPosition", 0, topSpawnPoints.Count - 1);
        int blockNum = Helper.GenerateWeightedRandom("RandomBlockNumberWithWater", 0, GameSettings.Instance.gridSize);
        GridCell initialCell = topSpawnPoints[r];
        GameObject blockObj;
        if (blockNum == 0)
        {
            blockNum = Helper.GenerateWeightedRandom("RandomBlockNumber", 1, GameSettings.Instance.gridSize);
            blockObj = Instantiate(waterBlockPrefab, (Vector2)initialCell.gridPos, Quaternion.identity);
        }
        else
        {
            blockObj = Instantiate(blockPrefab, (Vector2)initialCell.gridPos, Quaternion.identity);
        }

        Block block = blockObj.GetComponent<Block>();

        block.Initialize(blockNum, initialCell);
    }




}
