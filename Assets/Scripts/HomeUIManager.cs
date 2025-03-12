using TMPro;
using UnityEngine;

public class HomeUIManager : MonoBehaviour
{
    public GameObject settingsPopup;
    public TextMeshProUGUI highScoreText;

    public TextMeshProUGUI soundText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        highScoreText.text = GameData.HighScore.ToString();
    }


    public void SettingsClicked()
    {
        settingsPopup.SetActive(true);
        if(AudioManager.Instance.IsMuted)
        {
            soundText.text = "Sound Off";
        }
        else
        {
            soundText.text = "Sound On";
        }

    }

    public void LoadScene(string sceneName)
    {
        GameManager.Instance.LoadScene(sceneName);
    }



    public void QuitClicked()
    {
        Application.Quit();
    }
}
