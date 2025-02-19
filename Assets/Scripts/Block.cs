using UnityEngine;
using TMPro;
using System.Collections;

public class Block : MonoBehaviour
{

    private GridManager gridManager;
    private BlockSpawner blockSpawner;

    private TextMeshPro numberText;

    private SpriteRenderer spriteRenderer;
    public Animator anim;
   
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
    if (blockPlaced)
        return;

    // If running on mobile, process touch input for horizontal dragging.
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);

        // When the finger moves or is stationary, update the block's x-position.
        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            // Convert the touch position from screen space to world space.
            Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
            // Round the x-coordinate to snap to grid columns.
            float targetX = Mathf.Round(touchWorldPos.x);
            // Clamp the target x to valid grid columns.
            targetX = Mathf.Clamp(targetX, 0, gridManager.gridWidth - 1);
            // Update the block's horizontal position while preserving its current y.
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        }
        
        // Optionally, you can trigger a drop if the touch ends quickly (a tap).
        if (touch.phase == TouchPhase.Ended)
        {
            // For example, if the finger didn’t move much, treat it as a drop command.
            // (You can implement a tap-detection threshold if desired.)
            MoveDown();
        }
    }
    // Otherwise, use keyboard input (useful for testing in the Editor).
    else
    {
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
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            MoveDown();
        }
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
        blockPlaced = true;
        //
        Vector3 finalPos = transform.position;
        
        finalPos = new Vector3(transform.position.x,gridManager.gridHeight-1,0);

        while (IsValidPosition(finalPos))
        {
            finalPos += new Vector3(0, -1, 0);
        }

        finalPos += new Vector3(0, 1, 0);
        StartCoroutine(BlockPlacing(finalPos));
       
    }

    IEnumerator BlockPlacing(Vector3 finalPos)
    {
        float t=0;
        Vector3 initPos = transform.position;
        float distance = initPos.y - finalPos.y;
        Debug.Log("distance"+distance);
        while(t<1)
        {
            t += Time.deltaTime*2f*(9f/distance);
            transform.position = Vector3.Lerp(initPos,finalPos,t); 
            yield return null;
        }
        transform.position = finalPos;
        gridManager.AddToGrid(transform);
        
        anim.SetTrigger("Bounce");
    }

    // Check if the block is in a valid position (inside the grid and not overlapping another block).
    bool IsValidPosition(Vector3 pos)
    {
        pos = gridManager.RoundVector2(pos);
        if (!gridManager.InsideGrid(pos))
            return false;

        // Check if the grid cell is already occupied.
        if (gridManager.gridSquare[(int)pos.x, (int)pos.y] != null)
            return false;

        return true;
    }
}
