using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TopHUDUIController : MonoBehaviour
{
    private Transform  canvasUI;
    private Button settingsButton;
    private TextMeshProUGUI scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        canvasUI = GameObject.Find("CanvasUI").transform;
        settingsButton = Helper.FindChildByNameContains(transform, "Settings").GetComponentInChildren<Button>();
        scoreText = Helper.FindChildByNameContains(transform, "Score").GetComponentInChildren<TextMeshProUGUI>();
        settingsButton.onClick.AddListener(SettingsClicked);
    }

    void Start()
    {
        Initialize();
        GameManager.Instance.OnScoreChange += UpdateScore;
    }

    void Initialize()
    {
        if(GameManager.Instance.gameState == GameState.Home)
        {
            scoreText.text = GameData.HighScore.ToString();
        }
    }
    private void SettingsClicked()
    {
        Helper.FindChildByNameContains(canvasUI, "SettingsPopup").gameObject.SetActive(true);
    }

    public void UpdateScore(int newScore)
    {
        scoreText.text = newScore.ToString();
    }
}
