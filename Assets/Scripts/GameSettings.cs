using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptable Objects/Game Settings")]
public class GameSettings : ScriptableObject
{
    public int gridSize = 7;
    [Min(1)]
    public int blockSpawnVerticalOffsetInUnits = 2;
     [Min(1)]
    public int waterBlockSpawnVerticalOffsetInUnits = 1;
    public int levelIncreaseGap = 10;
    public int blockClearBasePoints = 5;
    public int blockClearScoreMultiplier = 2;
    public  float blockMoveUpSpeed = 2f;
    public float blockFallSpeedInit = 30;
    public float blockFallSpeedNormal = 15;
    private static GameSettings instance;
    public static GameSettings Instance
    {
        get {
            if (instance == null)
            {
                instance = Resources.Load<GameSettings>("GameSettings");
                if (instance == null)
                {
                    Debug.LogError("GameSettings asset not found in Resources folder!");
                }
            }
            return instance;
        }
    }
}

