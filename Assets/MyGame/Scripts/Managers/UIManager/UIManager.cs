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
    public TMP_Text progressText;

    public GameObject gameOverPanel;
    public GameObject missionCompletePanel;
    public TMP_Text victoryReasonText;
    public AudioClip missionCompleteSFX;
    public AudioClip gameOverSFX;

    private AudioSource audioSource;

    public Button mainMenuButton;
    public Button playAgainButton;

    public GameObject pausePanel;
    public Button resumeButton;

    protected override void Awake()
    {
        base.Awake();
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(LoadMenuScene);
        audioSource = gameObject.AddComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (Instance != this) return;
        ResetUI();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetUI();

        var slider = GameObject.FindWithTag("HealthBar");
        var player = Object.FindFirstObjectByType<PlayerHealth>();
        if (player != null && slider != null)
        {
            player.AssignHealthBar(slider.GetComponent<Slider>());
        }
    }

    private void Update()
    {
        HandleMissionDisplay();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausePanel();
        }

        if (missionCompletePanel.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            ReloadCurrentScene();
        }
    }


    private void HandleMissionDisplay()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            missionPanel.SetActive(true);

            var mission = MissionManager.Instance.GetCurrentMission();
            if (mission != null)
            {
                UIManager.Instance.ShowMission(
                    mission.missionName,
                    mission.timeLimit,
                    MissionManager.Instance.GetProgress(),
                    mission.requiredAmount
                );
            }
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            missionPanel.SetActive(false);
        }
    }

    public void ShowMission(string missionName, float time, int currentProgress = 0, int required = 0)
    {
        missionText.text = "MISSION: " + missionName;
        UpdateMissionTimer(time);
        UpdateMissionProgress(currentProgress, required);
    }

    public void UpdateMissionTimer(float time)
    {
        timerText.text = "TIME: " + Mathf.Ceil(time);
    }

    public void UpdateMissionProgress(int current, int total)
    {
        if (progressText == null) return;

        progressText.text = $"Progress: {current}/{total}";
        progressText.gameObject.SetActive(true);
    }

    public void ShowMissionComplete()
    {
        StartCoroutine(ShowPanelTemporary(missionCompletePanel, missionCompleteSFX));
    }

    public void ShowGameOver()
    {
        StartCoroutine(ShowGameOverPanel());
    }
    private void TogglePausePanel()
    {
        bool isActive = pausePanel.activeSelf;
        pausePanel.SetActive(!isActive);

        Time.timeScale = isActive ? 1f : 0f; 
        Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isActive;
    }

    private void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        gameOverPanel.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
        playAgainButton.gameObject.SetActive(true);

        CanvasGroup cg = gameOverPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }
    }

    public void ShowMissionCompleteSuccess()
    {
        victoryReasonText.text = "MISSION SUCCESS!";
        StartCoroutine(ShowVictoryPanel());
    }

    public void ShowVictory()
    {
        victoryReasonText.text = "ALL ENEMIES DEFEATED ! VICTORY !!!";
        StartCoroutine(ShowVictoryPanel());
    }

    private IEnumerator ShowVictoryPanel()
    {
        yield return StartCoroutine(ShowPanelTemporary(missionCompletePanel, missionCompleteSFX));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        mainMenuButton.gameObject.SetActive(true);
        playAgainButton.gameObject.SetActive(true);

        mainMenuButton.onClick.RemoveAllListeners();
        playAgainButton.onClick.RemoveAllListeners();

        mainMenuButton.onClick.AddListener(LoadMenuScene);
        playAgainButton.onClick.AddListener(ReloadCurrentScene);

        missionCompletePanel.SetActive(true);

        CanvasGroup cg = missionCompletePanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }
    }

    private IEnumerator ShowPanelTemporary(GameObject groupPanel, AudioClip clip)
    {
        CanvasGroup canvasGroup = groupPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = groupPanel.AddComponent<CanvasGroup>();
        }

        groupPanel.SetActive(true);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        PlaySFX(clip);
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, 0.5f));
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, 0.5f));

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        groupPanel.SetActive(false);
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
        ResourceManager.Instance?.ResetResourcesToDefault();
        WeaponManager.CurrentWeapon = WeaponManager.WeaponType.NoWeapon;
        MissionManager.Instance?.ResetMissionState();
        GameManager.Instance?.ResetGameState();

        var player = Object.FindFirstObjectByType<PlayerHealth>();
        if (player != null) player.ResetHealth();

        ResetUI();
        SceneManager.LoadScene("MenuGameTheLastRefuge", LoadSceneMode.Single);
    }

    public void ReloadCurrentScene()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.ResetMissionState();
            MissionManager.Instance.StopAllCoroutines();
        }

        ResourceManager.Instance?.ResetResourcesToDefault();
        WeaponManager.CurrentWeapon = WeaponManager.WeaponType.NoWeapon;
        GameManager.Instance?.ResetGameState();

        var player = Object.FindFirstObjectByType<PlayerHealth>();
        if (player != null) player.ResetHealth();

        ResetUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetUI()
    {
        missionPanel.SetActive(false);
        missionCompletePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        mainMenuButton.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);

        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}