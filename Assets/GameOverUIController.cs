using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIController : MonoBehaviour
{
    private Button retryButton;
    private Button homeButton;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI titleText;

     void Awake()
    {
        retryButton = Helper.FindChildByNameContains(transform, "Retry").GetComponent<Button>();
        homeButton = Helper.FindChildByNameContains(transform, "Home").GetComponent<Button>();
        scoreText = Helper.FindChildByNameContains(transform, "ScoreText").GetComponent<TextMeshProUGUI>();
        titleText = Helper.FindChildByNameContains(transform, "TitleText").GetComponent<TextMeshProUGUI>();
        retryButton.onClick.AddListener(RetryGame);
        homeButton.onClick.AddListener(GoHome);
    }

    void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameManager.Instance.gameState = GameState.GameOver;
        int currentScore = GameManager.Instance.CurrentScore();
        int highScore = GameData.HighScore;
        if(currentScore > highScore)
        {
            GameData.HighScore = currentScore;
            titleText.text = "New High Score!!!!";
        }
        
        scoreText.text = "SCORE\n" + GameManager.Instance.CurrentScore().ToString();


    }

    private void GoHome()
    {
        GameManager.Instance.LoadScene("HomeScene");
    }

    private void RetryGame()
    {
        GameManager.Instance.LoadScene("GameScene");
    }
}
