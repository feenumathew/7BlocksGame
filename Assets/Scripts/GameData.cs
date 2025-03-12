using UnityEngine;

public class GameData : MonoBehaviour
{
    public int currentLevel = 0;
    public int currentScore = 0;

    public bool clearHighScore = false;



    void Start()
    {
        if(clearHighScore)
            PlayerPrefs.SetInt("HighScore", 0);
    }

    public static int HighScore
    {
        get
        {
            return PlayerPrefs.GetInt("HighScore",0);
        }
        set
        {
            PlayerPrefs.SetInt("HighScore", value);
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (currentScore > HighScore)
            {
                HighScore = currentScore;
            }
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application quit.");
        if (currentScore > HighScore)
        {
            HighScore = currentScore;
        }
    }




}
