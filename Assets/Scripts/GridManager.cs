using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GridState
{
    Ready,
    InProcess  
}

public class GridManager : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public TwoKeyDictionary<int, int, GridCell> gridCells = new TwoKeyDictionary<int, int, GridCell>();
    public GameObject highLightBG;
    public GameObject gridCellPrefab;

    public GridState gridState = GridState.Ready;
    private BlockSpawner blockSpawner;
    private List<BlockHighLight> blockHighLights = new List<BlockHighLight>();

    private int successiveClears = 0;

    void Awake()
    {
        blockSpawner = FindFirstObjectByType<BlockSpawner>();
    }


    public void SetupGrid()
    {
       
        gridHeight = gridWidth = GameSettings.Instance.gridSize;

        CreateGrid(0, gridHeight, 0, gridWidth); //creating playarea grid
        CreateGrid(gridHeight+GameSettings.Instance.blockSpawnVerticalOffsetInUnits-1,1, 0, gridWidth, true); //creating spawn points grid
        CreateGrid(gridHeight,GameSettings.Instance.blockSpawnVerticalOffsetInUnits-1,0,gridWidth,false); //creating top inbetween invisible grids
        CreateGrid(-1 ,1, 0, gridWidth,false); //creating bottom water grid

        blockSpawner.SetupSpawnPoints();
       
    }



    void CreateGrid(int yStart,int yCount, int xStart, int xCount, bool visible = true)
    {
        for (int y = yStart; y < yStart+yCount; y++)
        {
            for (int x = xStart; x < xStart+xCount ; x++)
            {
                Vector3 cellPosition = new Vector2(x, y);
                gridCells.Add(x, y, Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform).GetComponent<GridCell>());
                gridCells[x, y].Initialize(x, y);
                if(!visible)
                    gridCells[x, y].HideGridGeaphic();
            }
        }
        
    }

    public bool IsTopRowFull()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (!gridCells[x, gridHeight-1].isOccupied)
                return false;
        }
        return true;
    }

    public bool IsAnyBlocksOutOfGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
           if (gridCells[x, gridHeight].isOccupied)
                    return true;
        }
        return false;
    }


    public Vector2 RoundVector2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    public bool InsideGrid(GridCell cell)
    {
        return (cell.gridPos.x >= 0 && cell.gridPos.x < gridWidth && cell.gridPos.y >= 0 && cell.gridPos.y < gridHeight);
    }


    public void CheckAndClear()
    {

        List<Block> blocksToClear = new List<Block>();
        List<WaterBlock> waterBlocksToBreak = new List<WaterBlock>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridCells[x, y].isOccupied)
                {
                    Block block = gridCells[x, y].block;
                    List<Block> horizontalUBBlocks = CountHorizontalUB(x, y);  //UB Stands for unbroken blocks
                    List<Block> verticalUBBlocks = CountVerticalUB(x, y);
                    if (block.Number == horizontalUBBlocks.Count)
                    {
                        blocksToClear.Add(block);
                        AddHorizontalUBHighlight(x, y, horizontalUBBlocks.Count);
                        waterBlocksToBreak =  CheckForWaterBlocks(horizontalUBBlocks);
                    }
                    else if (block.Number == verticalUBBlocks.Count)
                    {
                        blocksToClear.Add(block);
                        AddVerticalUBHighlight(x, y, verticalUBBlocks.Count);
                        waterBlocksToBreak =  waterBlocksToBreak.Concat(CheckForWaterBlocks(verticalUBBlocks)).ToList();
                    }
                }
            }
        }

        if (blocksToClear.Count > 0)
        {
            StartCoroutine(ClearBlocksCoRo(blocksToClear,waterBlocksToBreak));
            AudioManager.Instance.PlaySFX("mixkit-game-ball-tap-2073",.5f);
        }
        else
        {
            successiveClears = 0;
            gridState = GridState.Ready;
        }
    }

    List<WaterBlock> CheckForWaterBlocks(List<Block> blocks)
    {
        List<WaterBlock> waterBlocks = new List<WaterBlock>();
        foreach (Block block in blocks)
        {
            if (block.GetType() == typeof(WaterBlock))
            {
                WaterBlock waterBlock = (WaterBlock)block;
                waterBlocks.Add(waterBlock);
            }
        }
        return waterBlocks;
    }



    void AddHorizontalUBHighlight(int x, int y, int horizontalUBCount)
    {
        Vector2 center = new Vector2(x, y);
        int startIndex;
        for (startIndex = x; startIndex >= 0; startIndex--)
        {
            if (!gridCells[startIndex, y].isOccupied)
                break;
        }
        startIndex++;
        center = new Vector2(startIndex + (horizontalUBCount - 1) / 2f, y);
        if (blockHighLights.Count == 0 ||
        !blockHighLights.Any(a => a.center == center && a.horizontalUBCount == horizontalUBCount &&
         a.verticalUBCount == 1))
        {
            BlockHighLight blockHighLight = Instantiate(highLightBG).GetComponent<BlockHighLight>();
            blockHighLight.Setup(center, horizontalUBCount, 1);
            blockHighLights.Add(blockHighLight);
        }
    }

    void AddVerticalUBHighlight(int x, int y, int verticalUBCount)
    {
        Vector2 center = new Vector2(x, y);
        int startIndex;
        for (startIndex = y; startIndex >= 0; startIndex--)
        {
            if (gridCells[x, startIndex] == null)
                break;
        }
        startIndex++;
        center = new Vector2(x, startIndex + (verticalUBCount - 1) / 2f);
        if (blockHighLights.Count == 0 ||
        !blockHighLights.Any(a => a.center == center && a.horizontalUBCount == 1 &&
         a.verticalUBCount == verticalUBCount))
        {
            BlockHighLight blockHighLight = Instantiate(highLightBG).GetComponent<BlockHighLight>();
            blockHighLight.Setup(center, 1, verticalUBCount);
            blockHighLights.Add(blockHighLight);
        }
    }

    IEnumerator ClearBlocksCoRo(List<Block> blocksToClear,List<WaterBlock> waterBlocksToBreak)
    {
        foreach (BlockHighLight blockHighLight in blockHighLights)
            blockHighLight.EnableHighLight();
        yield return new WaitForSeconds(.4f);
        
        foreach (BlockHighLight blockHighLight in blockHighLights)
            blockHighLight.DestroyHighLight();
        blockHighLights.Clear();

        int scoreForClear = GameSettings.Instance.blockClearBasePoints * (int)Mathf.Pow(GameSettings.Instance.blockClearScoreMultiplier, successiveClears);

        foreach (Block block in blocksToClear)
        {
            block.ClearBlock(scoreForClear);
        }
        foreach (WaterBlock waterBlock in waterBlocksToBreak)
        {
            waterBlock.BreakWaterBlock();
        }
        successiveClears++;
        StartCoroutine(UpdateGridAfterClear());

    }

    List<Block> CountHorizontalUB(int x, int y)
    {

        List<Block> blocks = new List<Block>();
        for (int i = x; i >= 0; i--)
        {
            if (gridCells[i, y].isOccupied == false)
                break;
            blocks.Add(gridCells[i, y].block);
        }
        for (int i = x + 1; i < gridWidth; i++)
        {
            if (gridCells[i, y].isOccupied == false)
                break;
            blocks.Add(gridCells[i, y].block);
        }
        return blocks;
    }

    List<Block> CountVerticalUB(int x, int y)
    {

        List<Block> blocks = new List<Block>();
        for (int i = y; i >= 0; i--)
        {
            if (gridCells[x, i].isOccupied == false)
                break;
            blocks.Add(gridCells[x, i].block);
        }
        for (int i = y + 1; i < gridHeight; i++)
        {
            if (gridCells[x, i].isOccupied == false)
                break;
            blocks.Add(gridCells[x, i].block);
        }
        return blocks;
    }

    IEnumerator UpdateGridAfterClear()

    {
        yield return null;
        bool gridUpdate = true;
        Dictionary<Block, GridCell> toBeMovedBlocks = new Dictionary<Block, GridCell>();
        while (gridUpdate)
        {
            gridUpdate = false;

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 1; y < gridHeight; y++)
                {
                    if (gridCells[x, y].isOccupied && !gridCells[x, y - 1].isOccupied)
                    {
                        gridUpdate = true;
                        if (!toBeMovedBlocks.ContainsKey(gridCells[x, y].block))
                            toBeMovedBlocks.Add(gridCells[x, y].block, gridCells[x, y]);
                        gridCells[x, y].block.BlockPlaced(gridCells[x, y - 1]);
                    }
                }
            }

        }
        int blocksToMove = toBeMovedBlocks.Count;

        if (blocksToMove == 0)
        {
            CheckAndClear();
        }
        else
        {
            foreach (KeyValuePair<Block, GridCell> block in toBeMovedBlocks)
            {
                GridCell finalGridCell = block.Key.currentCell;
                block.Key.BlockPlaced(block.Value);
                block.Key.MoveBlock(finalGridCell, GameSettings.Instance.blockFallSpeedNormal, () =>
                {
                    blocksToMove--;
                    if (blocksToMove == 0)
                    {
                        CheckAndClear();
                    }
                });
            }
        }
    }

    public void MoveAllBlocksUp()
    {

        StartCoroutine(MoveAllBlocksUpCoRo());
    }

    IEnumerator MoveAllBlocksUpCoRo()
    {
        yield return new WaitForSeconds(.2f);

        float t = 0;

        List<(Block,GridCell)> allMovableBlocks =  new List<(Block,GridCell)>();

        //adding all blocks inside grid
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = gridHeight - 1; y >= 0; y--)
            {
                if (gridCells[x, y].isOccupied)
                {
                    allMovableBlocks.Add((gridCells[x, y].block,gridCells[x, y + 1]));
                }
            }
        }

        //adding waterblocks

        for (int x = 0; x < gridWidth; x++)
        {
                int y = 0 - GameSettings.Instance.waterBlockSpawnVerticalOffsetInUnits;
                if (gridCells[x, y].isOccupied)
                {
                    allMovableBlocks.Add((gridCells[x, y].block,gridCells[x, y + GameSettings.Instance.waterBlockSpawnVerticalOffsetInUnits ]));
                }
        }

        int blockCount = allMovableBlocks.Count;

        foreach((Block,GridCell) blockGridCellTuple in allMovableBlocks)
        {
            blockGridCellTuple.Item1.MoveBlock(blockGridCellTuple.Item2,GameSettings.Instance.blockMoveUpSpeed,()=>{blockCount--;});
        }

        while(blockCount>0)
            yield return null;


        if(IsAnyBlocksOutOfGrid())
            GameManager.Instance.GameOver();
        else
        {
            CheckAndClear();
        }
    }
    
}


