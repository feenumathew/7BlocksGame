using UnityEngine;

public class BlockHighLight : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;

    public Vector2 center;
    public int horizontalUBCount = 0;
    public int verticalUBCount = 0;


    public void Setup(Vector2 center, int horizontalUBCount, int verticalUBCount)
    {
        gameObject.transform.position = center;
        int number = Mathf.Max(horizontalUBCount,verticalUBCount);

        SetColor(number);
        spriteRenderer.size = new Vector2(4 + (horizontalUBCount-1) * 2.8f, 4 + (verticalUBCount-1) * 2.8f);
    }


    public void EnableHighLight()
    {
        spriteRenderer.enabled = true;
    }

    public void DestroyHighLight()
    {
        Destroy(gameObject);
    }


    void SetColor(int num)
    {
        spriteRenderer.color = Block.ColorForNum(num);
       
    }

}