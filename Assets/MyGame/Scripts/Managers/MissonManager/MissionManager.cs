using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : BaseManager<MissionManager>
{
    public List<MissionData> missions;
    private MissionData currentMission;
    private int progress;
    private float timer;
    private bool missionActive = false;

    public GameObject stealthSoundPrefab;
    private GameObject activeSoundObj;

    public enum MissionState { NotStarted, InProgress, Completed, Failed }
    public MissionState missionState = MissionState.NotStarted;

    private Coroutine missionCoroutine;
    private bool sceneHooked = false;
    private bool hasAssignedMission = false;

    protected override void Awake()
    {
        base.Awake();

        if (!sceneHooked)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            sceneHooked = true;
        }
    }

    void Start()
    {
        if (Instance != this) return;
        ResetMissionState();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == "TheLastRefuge")
        {
            ResetMissionState();
            StartCoroutine(DelayedAssign());
        }
    }

    private IEnumerator DelayedAssign()
    {
        yield return new WaitForSeconds(0.1f);
        AssignNewMission();
    }

    void Update()
    {
        if (!missionActive) return;

        if (currentMission.timeLimit > 0)
        {
            timer -= Time.deltaTime;
            UIManager.Instance.UpdateMissionTimer(timer);

            if (timer <= 0)
            {
                CheckMissionResult();
            }
        }
    }

    public void AssignNewMission()
    {
        if (hasAssignedMission) return;
        hasAssignedMission = true;

        if (missions == null || missions.Count == 0)
        {
            Debug.LogWarning("No missions available to assign!");
            return;
        }

        int randomIndex = Random.Range(0, missions.Count);
        currentMission = missions[randomIndex];

        if (currentMission == null || string.IsNullOrEmpty(currentMission.missionName))
        {
            Debug.LogWarning("Current mission is invalid or has no name.");
            return;
        }

        progress = 0;
        timer = currentMission.timeLimit;
        missionActive = true;
        missionState = MissionState.InProgress;

        Debug.Log($"NEW MISSION: {currentMission.missionName} - {currentMission.description}");

        UIManager.Instance.ShowMission(currentMission.missionName, timer, progress, currentMission.requiredAmount);

        if (currentMission.missionType == MissionData.MissionType.StealthSurvive)
        {
            SpawnStealthSound();
        }
    }

    void SpawnStealthSound()
    {
        if (activeSoundObj != null) Destroy(activeSoundObj);

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        activeSoundObj = Instantiate(stealthSoundPrefab, playerTransform);
        activeSoundObj.transform.localPosition = Vector3.zero;

        AudioSource source = activeSoundObj.GetComponent<AudioSource>();
        if (source != null)
        {
            source.Play();
        }
    }

    public void UpdateProgress()
    {
        if (!missionActive) return;

        progress++;
        Debug.Log($"PROGRESS: {progress}/{currentMission.requiredAmount}");

        UIManager.Instance.UpdateMissionProgress(progress, currentMission.requiredAmount);

        if (progress >= currentMission.requiredAmount)
        {
            MissionCompleted();
        }
    }

    public void WrongAction()
    {
        Debug.Log("WRONG ACTION!! NO QUEST PROGRESS!!");
    }

    void MissionCompleted()
    {
        Debug.Log($"MISSION SUCCESS: {currentMission.missionName}!");
        missionState = MissionState.Completed;
        RemoveStealthSound();
    }

    private void CheckMissionResult()
    {
        if (!missionActive) return;

        var type = currentMission.missionType;

        if (type == MissionData.MissionType.Survive || type == MissionData.MissionType.StealthSurvive)
        {
            MissionCompleted();
            missionActive = false;
            UIManager.Instance.ShowMissionComplete();
            StartCoroutine(StartPrepareNextMission());
            return;
        }

        if (missionState == MissionState.Completed)
        {
            Debug.Log("MISSION COMPLETE AFTER TIMER!");
            missionActive = false;
            UIManager.Instance.ShowMissionComplete();
            StartCoroutine(StartPrepareNextMission());
        }
        else
        {
            Debug.Log("MISSION FAILED AFTER TIMER!");
            MissionFailed();
        }
    }

    private IEnumerator StartPrepareNextMission()
    {
        if (missionCoroutine != null)
        {
            StopCoroutine(missionCoroutine);
            missionCoroutine = null;
        }

        missionCoroutine = StartCoroutine(PrepareNextMission());
        yield return missionCoroutine;
    }

    private IEnumerator PrepareNextMission()
    {
        Debug.Log("WAITING 10 SECONDS BEFORE NEXT MISSION...");
        int countdown = 10;

        while (countdown > 0)
        {
            UIManager.Instance.UpdateMissionTimer(countdown);
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        AssignNewMission();
    }

    public void MissionFailed()
    {
        Debug.Log($"TIME OUT! MISSION FAILED! : {currentMission.missionName}...");

        ThirdPersonController player = FindFirstObjectByType<ThirdPersonController>();
        if (player != null) player.KillPlayer();

        missionActive = false;
        missionState = MissionState.Failed;

        GameManager.Instance.OnPlayerMissionFailed();
        RemoveStealthSound();

        UIManager.Instance.ShowGameOver();
    }

    void RemoveStealthSound()
    {
        if (activeSoundObj != null)
        {
            Destroy(activeSoundObj);
            activeSoundObj = null;
        }
    }

    public void ResetMissionState()
    {
        if (missionCoroutine != null)
        {
            StopCoroutine(missionCoroutine);
            missionCoroutine = null;
        }

        currentMission = null;
        progress = 0;
        timer = 0;
        missionActive = false;
        missionState = MissionState.NotStarted;
        RemoveStealthSound();
        hasAssignedMission = false;
    }

    public MissionData GetCurrentMission() => currentMission;
    public int GetProgress() => progress;
}
