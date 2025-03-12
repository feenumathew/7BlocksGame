using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{

    private Button soundToggle;
    private Button quitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        soundToggle = Helper.FindChildByNameContains(transform, "SoundToggle").GetComponent<Button>();
        quitButton = Helper.FindChildByNameContains(transform, "QuitGame").GetComponent<Button>();
        soundToggle.onClick.AddListener(ToggleSound);
        quitButton.onClick.AddListener(QuitGame);
    }

    void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        TextMeshProUGUI soundText = soundToggle.GetComponentInChildren<TextMeshProUGUI>();
        if(AudioManager.Instance.IsMutedFromPrefs())
        {
            soundText.text = "Sound Off";
        }
        else
        {
            soundText.text = "Sound On";
        }
    }

    private void QuitGame()
    {
       Application.Quit();
    }

    private void ToggleSound()
    {
        TextMeshProUGUI soundText = soundToggle.GetComponentInChildren<TextMeshProUGUI>();
        if(AudioManager.Instance.IsMuted)
        {

            soundText.text = "Sound On";
            AudioManager.Instance.SetMuted(false);
           
        }
        else
        {
           soundText.text = "Sound Off";
            AudioManager.Instance.SetMuted(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
