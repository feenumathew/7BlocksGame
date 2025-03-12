using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Home,
    GamePlay,
    GameOver
}

public class GameManager : MonoBehaviour
{

   
    private GameData gameData;
    private GridManager gridManager;
    private BlockSpawner blockSpawner;


    public static GameManager Instance;

   

    public GameState gameState = GameState.Home;

    public Action<int> OnScoreChange;



    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        } 

    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if(scene.name == "GameScene")
        {
            Debug.Log("Game Scene Loaded");
            gameState = GameState.GamePlay;
            SetupForGamePlay();
            StartCoroutine(StartGamePlay());
        }
        else if(scene.name == "HomeScene")
        {
            Debug.Log("Home Scene Loaded");
            gameState = GameState.Home;
        }
    }

    void SetupForGamePlay()
    {
        gameData = FindFirstObjectByType<GameData>();
        gridManager = FindFirstObjectByType<GridManager>();
        blockSpawner = FindFirstObjectByType<BlockSpawner>();
    }

    IEnumerator StartGamePlay()
    {
        gridManager.SetupGrid();
        yield return new WaitForSeconds(.5f);
        blockSpawner.StartSpawningBlock();
    }

    public void IncreaseLevel()
    {
        gameData.currentLevel++;
    }

    public int CurrentScore()
    {
        return gameData.currentScore;
    }

    public void AddScore(int score)
    {
        int currentScore = gameData.currentScore;   
        currentScore += score;
        gameData.currentScore = currentScore;
        OnScoreChange?.Invoke(currentScore);
    }

    public void GameOver()
    {
        gameState = GameState.GameOver;
        UIManager.Instance.ShowGameOverPanel();
    }
    


}
