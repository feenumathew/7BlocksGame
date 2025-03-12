using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Block : MonoBehaviour
{
    private GridManager gridManager;

    private BlockSpawner blockSpawner;

    private UIManager uIManager;
    protected TextMeshPro numberText;
    protected SpriteRenderer spriteRenderer;

    private bool overUI;

    private int spawnPointIndex = 0;

    public GridCell currentCell = null;

    public Vector2Int blockPos;
    public Animator anim;
    private int _number;
    public BlockState blockState = BlockState.Spawned;


    public virtual int Number
    {
        get
        {
            return _number;
        }
        set
        {
            _number = value;
        }
    }

    protected void Setup()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        blockSpawner = FindFirstObjectByType<BlockSpawner>();
        uIManager = FindFirstObjectByType<UIManager>();
        numberText = GetComponentInChildren<TextMeshPro>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void Initialize(int num, GridCell gridCell)
    {
        Setup();
        SetNumber(num);
        SetColor(num);
        SpawnedBlockMoved(gridCell);
    }

    public void SpawnedBlockMoved(GridCell gridCell)
    {
        ClearGridCell();
        spawnPointIndex = gridCell.gridPos.x;
        currentCell = gridCell;
        blockPos = gridCell.gridPos;
        transform.position = (Vector2)blockPos;
        gridCell.AddBlock(this);
    }

    public void BlockPlaced(GridCell gridCell)
    {
        ClearGridCell();
        blockState = BlockState.Placed;
        currentCell = gridCell;
        blockPos = gridCell.gridPos;
        transform.position = (Vector2)blockPos;
        gridCell.AddBlock(this);
    }

    public void ClearGridCell()
    {
        if (currentCell != null)
        {
            currentCell.RemoveBlock();
        }
        currentCell = null;
    }

    public void ClearBlock(int scoreForClear)
    {
        uIManager.ShowScore(transform.position, scoreForClear, ColorForNum(Number));
        ClearGridCell();
        Destroy(gameObject);
    }


    // Call this to initialize the block’s number.
    public void SetNumber(int num)
    {
        Number = num;
        if (numberText != null)
            numberText.text = num.ToString();
    }

    protected void SetColor(int num)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = ColorForNum(num);
    }
    public static Color ColorForNum(int num)
    {
        Color color = Color.magenta;
        switch (num)
        {
            case 1:
                ColorUtility.TryParseHtmlString("#222222", out color);
                break;
            case 2:
                ColorUtility.TryParseHtmlString("#95999B", out color);
                break;
            case 3:
                ColorUtility.TryParseHtmlString("#BA2926", out color);
                break;
            case 4:
                ColorUtility.TryParseHtmlString("#DCA402", out color);
                break;
            case 5:
                ColorUtility.TryParseHtmlString("#694180", out color);
                break;
            case 6:
                ColorUtility.TryParseHtmlString("#02BB52", out color);
                break;
            case 7:
                ColorUtility.TryParseHtmlString("#387CC8", out color);
                break;
            default:
                break;
        }
        return color;
    }

    void Update()
    {
        if (blockState != BlockState.Spawned || (!blockSpawner.topSpawnPoints.Contains(currentCell)))
            return;

        // If running on mobile, process touch input for horizontal dragging.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                overUI = true;
                return;
            }


            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {

                Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
                int targetX = (int)Mathf.Round(touchWorldPos.x);
                targetX = Mathf.Clamp(targetX, 0, gridManager.gridWidth - 1);
                SpawnedBlockMoved(blockSpawner.topSpawnPoints[targetX]);
            }
            if (touch.phase == TouchPhase.Ended)
            {
                if (overUI)
                {
                    overUI = false;
                }
                else
                {
                    MoveDown();
                   
                }

            }
        }
        // Otherwise, use keyboard input (useful for testing in the Editor).
        else
        {

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                spawnPointIndex = Mathf.Clamp(spawnPointIndex - 1, 0, gridManager.gridWidth - 1);
                SpawnedBlockMoved(blockSpawner.topSpawnPoints[spawnPointIndex]);

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                spawnPointIndex = Mathf.Clamp(spawnPointIndex + 1, 0, gridManager.gridWidth - 1);
                SpawnedBlockMoved(blockSpawner.topSpawnPoints[spawnPointIndex]);
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                MoveDown();
            }
        }
    }


    // Move the block down by one unit.
    void MoveDown()
    {


        GridCell finalGridCell = gridManager.gridCells[blockPos.x, gridManager.gridHeight - 1]; //Setting the first available cell

        Vector2Int finalPos = finalGridCell.gridPos;

        while (IsValidGrid(finalGridCell))
        {
            finalPos.y--;

            if (finalPos.y < 0)
            {
                break;
            }
            finalGridCell = gridManager.gridCells[finalPos.x, finalPos.y];
        }

        finalGridCell = gridManager.gridCells[finalPos.x, finalPos.y + 1];

        if (IsValidGrid(finalGridCell))
        {
            blockState = BlockState.Moving;
            StartCoroutine(BlockPlacing(finalGridCell));
        }

    }

    IEnumerator BlockPlacing(GridCell finalGridCell)
    {

        yield return StartCoroutine(MoveBlockCoro(finalGridCell,GameSettings.Instance.blockFallSpeedInit));
       
        yield return StartCoroutine(AnimationWait("BlockMove"));

        gridManager.CheckAndClear();

    }


    public void MoveBlock(GridCell finalGridCell, float speed,Action callback = null)
    {
        StartCoroutine(MoveBlockCoro(finalGridCell, speed,callback));
    }

    public IEnumerator MoveBlockCoro(GridCell finalGridCell, float speed,Action callback = null)
    {
         float t = 0;
        Vector2 initPos  = currentCell.gridPos;
        Vector2 finalPos = finalGridCell.gridPos;
        float distance = Mathf.Abs(finalPos.y - initPos.y);
        while (t < 1)
        {
            t += Time.deltaTime * speed/distance;
            transform.position = Vector2.Lerp(initPos, finalPos, t);
            yield return null;
        }
        transform.position = finalPos;
        BlockPlaced(finalGridCell);
        callback?.Invoke();
    }



    IEnumerator AnimationWait(string animationState)
    {
        anim.Play(animationState,0);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(animationState));
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < .9f)
        {
            yield return null;
        }
        
    }



    // Check if the block is in a valid position (inside the grid and not overlapping another block).
    bool IsValidGrid(GridCell gridCell)
    {
        if (!gridManager.InsideGrid(gridCell))
            return false;

        // Check if the grid cell is already occupied.
        if (gridCell.isOccupied)
            return false;

        return true;
    }
}

public enum BlockState
{

    Spawned,
    Moving,
    Placed
}
