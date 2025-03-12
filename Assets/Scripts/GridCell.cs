using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Vector2Int gridPos;
    public bool isOccupied = false;
    public Block block;

    public void Initialize(int x,int y)
    {
        this.gridPos = new Vector2Int(x, y);
    }
    public void HideGridGeaphic()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
    public void AddBlock(Block block)
    {
        this.block = block;
        block.transform.parent = transform;
        isOccupied = true;
    }

    public void RemoveBlock()
    {
        block = null;
        isOccupied = false;
    }

}
