using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{

    private GridManager gridManager;
    private BlockSpawner blockSpawner;

    private TextMeshPro numberText;

    private SpriteRenderer spriteRenderer;

   
    public int number;

    public bool blockPlaced = false;


    void Setup()
    {
        gridManager = FindObjectOfType<GridManager>();
        blockSpawner = FindObjectOfType<BlockSpawner>();
        numberText = GetComponentInChildren<TextMeshPro>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }

    public void Initialize(int num)
    {
        Setup();
        SetNumber(num);
        SetColor(num);
    }

    // Call this to initialize the block’s number.
    public void SetNumber(int num)
    {
        number = num;
        if (numberText != null)
            numberText.text = num.ToString();
       
    }

    void SetColor(int num)
    {
        switch(num)
        {
            case 1:
            {
                spriteRenderer.color = Color.black;
                break;
            }
            case 2:
            {
                spriteRenderer.color = Color.gray;
                break;
            }
            case 3:
            {
                spriteRenderer.color = Color.red;
                break;
            }
            case 4:
            {
                spriteRenderer.color = Color.yellow;
                break;
            }
            case 5:
            {
                spriteRenderer.color = Color.magenta;
                break;
            }
            case 6:
            {
                spriteRenderer.color = Color.green;
                break;
            }
            case 7:
            {
                spriteRenderer.color = Color.blue;
                break;
            }
            default:
            {
                spriteRenderer.color = Color.cyan;
                break;
            }
        }
    }
   

    void Update()
    {
        if(blockPlaced)
            return;


        // Allow horizontal movement.
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);
            if (!IsValidCol())
                transform.position += new Vector3(1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);
            if (!IsValidCol())
                transform.position += new Vector3(-1, 0, 0);
        }
        else if(Input.GetKeyDown(KeyCode.Return)||Input.GetKeyDown(KeyCode.KeypadEnter) )
        {
            MoveDown();
        }
    }

    bool IsValidCol()
    {
         Vector2 pos = gridManager.RoundVector2(transform.position);
         return ((int)pos.x >= 0 && (int)pos.x < gridManager.gridWidth);
    }

    // Move the block down by one unit.
    void MoveDown()
    {

        Vector3 finalPos = transform.position;

        finalPos = new Vector3(transform.position.x,gridManager.gridHeight-1,0);

        while (IsValidPosition(finalPos))
        {
            finalPos += new Vector3(0, -1, 0);
        }

        finalPos += new Vector3(0, 1, 0);
        transform.position = finalPos;
        gridManager.AddToGrid(transform);
        blockPlaced = true;
       
    }

    // Check if the block is in a valid position (inside the grid and not overlapping another block).
    bool IsValidPosition(Vector3 pos)
    {
        pos = gridManager.RoundVector2(pos);
        if (!gridManager.InsideGrid(pos))
            return false;

        // Check if the grid cell is already occupied.
        if (gridManager.grid[(int)pos.x, (int)pos.y] != null)
            return false;

        return true;
    }
}
