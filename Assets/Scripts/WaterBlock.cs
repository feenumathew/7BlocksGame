using UnityEngine;

public class WaterBlock : Block
{

    public WaterBlockState waterBlockState = WaterBlockState.Water;


    [SerializeField] private Sprite orgSprite;
    [SerializeField] private Sprite breakSprite;

       public void SetWaterBlockState(WaterBlockState state)
    {
        waterBlockState = state;
        switch (state)
        {
            case WaterBlockState.Water:
                numberText.gameObject.SetActive(false);
               
                break;
            case WaterBlockState.Break:
                spriteRenderer.sprite = breakSprite;
                break;
            case WaterBlockState.Number:
                spriteRenderer.sprite = orgSprite;
                numberText.gameObject.SetActive(true);
                numberText.transform.SetParent(null);
                transform.localScale = Vector3.one*3.33f;
                numberText.transform.SetParent(spriteRenderer.transform);
                numberText.transform.localPosition = Vector3.zero;
                spriteRenderer.color = Color.white;
                SetColor(Number);
                break;
        }
    }

    public override int Number
    {
        get
        {
            if (waterBlockState == WaterBlockState.Number)
            {
                return base.Number;
            }
            else
            {
                return -1;
            }
        }
        set => base.Number = value;
    }

    public void BreakWaterBlock()
    {
        if (waterBlockState == WaterBlockState.Water)
        {
            SetWaterBlockState(WaterBlockState.Break);
        }
        else if (waterBlockState == WaterBlockState.Break)
        {
            SetWaterBlockState(WaterBlockState.Number);
           
        }
        else if (waterBlockState == WaterBlockState.Number)
        {
            //Do nothing
        }
    }

    public override void Initialize(int num, GridCell gridCell)
    {
        Setup();
        SetNumber(num);
        SpawnedBlockMoved(gridCell);
        SetWaterBlockState(WaterBlockState.Water);

    }


}

public enum WaterBlockState
{
    Water,
    Break,
    Number
}
