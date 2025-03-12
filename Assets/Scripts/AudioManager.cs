using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [Range(0f, 1f)]
    public const float defaultMasterVolume = .75f;
    [HideInInspector]
    public float masterVolume;

    public bool IsMuted = false;
   
    private AudioSource bgmSource;
    private AudioSource sfxSource;
    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Initialize()
    {
        SetMuted(IsMutedFromPrefs());

        masterVolume = MasterVolumeFromPrefs();

        GameObject bgmObj = new GameObject("BGM Source");
        bgmObj.transform.parent = transform;
        bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = masterVolume;

        GameObject sfxObj = new GameObject("SFX Source");
        sfxObj.transform.parent = transform;
        sfxSource = sfxObj.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = masterVolume;
    }

    public bool IsMutedFromPrefs()
    {
        return PlayerPrefs.GetInt("IsMuted", 0) == 1;
    }

    public float MasterVolumeFromPrefs()
    {
        return PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);
    }

    public void SetMuted(bool muted)
    {
        IsMuted = muted;
        PlayerPrefs.SetInt("IsMuted", muted ? 1 : 0);
        if (bgmSource != null)
            bgmSource.mute = muted;
        if (sfxSource != null)
            sfxSource.mute = muted;
    }
    

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        if (bgmSource != null)
            bgmSource.volume = masterVolume;
        if (sfxSource != null)
            sfxSource.volume = masterVolume;
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null || clip == null)
            return;
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = masterVolume;
        bgmSource.Play();
    }

    public void PlayBGM(string clipName, bool loop = true)
    {
        AudioClip clip = Resources.Load<AudioClip>(clipName);
        if (clip != null)
            PlayBGM(clip, loop);
        else
            Debug.LogError("BGM not found: " + clipName);
    }

    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f, float duckedBgmMultiplier = 0.5f, float fadeDuration = 0.1f)
    {
        if (clip == null)
            return;
        StartCoroutine(PlaySFXWithDuckingCoroutine(clip, volumeScale, duckedBgmMultiplier, fadeDuration));
    }

    public void PlaySFX(string clipName, float volumeScale = 1f, float duckedBgmMultiplier = 0.5f, float fadeDuration = 0.1f)
    {
        AudioClip clip = Resources.Load<AudioClip>(clipName);
        if (clip != null)
            PlaySFX(clip, volumeScale, duckedBgmMultiplier, fadeDuration);
        else
            Debug.LogError("SFX not found: " + clipName);
    }

    IEnumerator PlaySFXWithDuckingCoroutine(AudioClip clip, float volumeScale, float duckedBgmMultiplier, float fadeDuration)
    {
        float originalBgmVolume = bgmSource.volume;
        float targetVolume = originalBgmVolume * duckedBgmMultiplier;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(originalBgmVolume, targetVolume, elapsed / fadeDuration);
            yield return null;
        }
        bgmSource.volume = targetVolume;
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale) * masterVolume);
        yield return new WaitForSeconds(clip.length);
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(targetVolume, originalBgmVolume, elapsed / fadeDuration);
            yield return null;
        }
        bgmSource.volume = originalBgmVolume;
    }
}
