// File: UIManager.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : BaseManager<UIManager>
{
    public GameObject missionPanel;
    public TMP_Text missionText;
    public TMP_Text timerText;

    public GameObject gameOverPanel;
    public GameObject missionCompletePanel;
    public AudioClip missionCompleteSFX;
    public AudioClip gameOverSFX;

    private AudioSource audioSource;

    public Button mainMenuButton;
    public Button playAgainButton;

    private void Start()
    {
        missionPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        missionCompletePanel.SetActive(false); 
        mainMenuButton.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        HandleMissionDisplay();
    }

    private void HandleMissionDisplay()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            missionPanel.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            missionPanel.SetActive(false);
        }
    }

    public void ShowMission(string missionName, float time)
    {
        missionText.text = "MISSION: " + missionName;
        UpdateMissionTimer(time);
    }

    public void UpdateMissionTimer(float time)
    {
        timerText.text = "TIME: " + Mathf.Ceil(time);
    }

    public void ShowMissionComplete()
    {
        StartCoroutine(ShowPanelTemporary(missionCompletePanel, missionCompleteSFX));
    }

    public void ShowGameOver()
    {
        StartCoroutine(ShowGameOverPanel());
    }

    private IEnumerator ShowGameOverPanel()
    {
        yield return StartCoroutine(ShowPanelTemporary(gameOverPanel, gameOverSFX));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        mainMenuButton.gameObject.SetActive(true);
        playAgainButton.gameObject.SetActive(true);

        mainMenuButton.onClick.RemoveAllListeners();
        playAgainButton.onClick.RemoveAllListeners();

        mainMenuButton.onClick.AddListener(LoadMenuScene);
        playAgainButton.onClick.AddListener(ReloadCurrentScene);
    }

    private IEnumerator ShowPanelTemporary(GameObject groupPanel, AudioClip clip)
    {
        CanvasGroup canvasGroup = groupPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = groupPanel.AddComponent<CanvasGroup>();
        }

        groupPanel.SetActive(true);
        PlaySFX(clip);

        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, 0.5f));
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, 0.5f));
    }



private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        canvasGroup.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void LoadMenuScene()
    {
        ResetAllResources();
        HideGameOverUI();
        SceneManager.LoadScene("MenuGameTheLastRefuge", LoadSceneMode.Single);
    }

    private void ReloadCurrentScene()
    {
        ResetAllResources();
        HideGameOverUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void HideGameOverUI()
    {
        gameOverPanel.SetActive(false);
        missionCompletePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void ResetAllResources()
    {
        ResourceManager.Instance.ResetResourcesToDefault();
    }
}