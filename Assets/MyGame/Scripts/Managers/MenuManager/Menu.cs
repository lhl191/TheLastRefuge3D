using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingPanel;

    [Header("Audio")]
    public Slider volumeSlider;
    public AudioSource backgroundMusic; 

    [Header("SFX")]
    public AudioSource sfxAudioSource; 
    public AudioClip clickSound;

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }

        if (backgroundMusic != null)
        {
            backgroundMusic.gameObject.SetActive(true); // <- bật MusicPlayer nếu nó đang disable
            if (!backgroundMusic.isPlaying)
            {
                backgroundMusic.Play();
            }
        }
    }

    public void StartGame()
    {
        PlayClickSound();
        SceneManager.LoadScene("TheLastRefuge", LoadSceneMode.Single);
    }

    public void OpenSetting()
    {
        PlayClickSound();
        settingPanel.SetActive(true);
    }

    public void CloseSetting()
    {
        PlayClickSound();
        settingPanel.SetActive(false);
    }

    public void ExitGame()
    {
        PlayClickSound();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }



public void SetVolume(float value)
    {
        AudioListener.volume = value;
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = value; 
        }
    }
    public void PlayClickSound()
    {
        if (sfxAudioSource != null && clickSound != null)
        {
            sfxAudioSource.PlayOneShot(clickSound);
        }
    }
}
