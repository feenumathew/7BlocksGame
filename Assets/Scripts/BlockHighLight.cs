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

}