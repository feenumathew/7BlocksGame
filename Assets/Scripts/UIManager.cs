using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    public static UIManager Instance;
    public GameObject gameOverPanel;

    public GameObject floatingScorePrefab;
    public Canvas uiCanvas;

    void Awake()
    {
        Instance = this;
    }

    
   

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }


    public void ShowScore(Vector3 worldPosition, int score,Color color)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        FloatingScore floatingScore = Instantiate(floatingScorePrefab, uiCanvas.transform).GetComponent<FloatingScore>();
        floatingScore.transform.position = screenPos;
        floatingScore.Initialize(score, color);
       
    }
}
