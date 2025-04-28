using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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

    void Start()
    {
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
        int randomIndex = Random.Range(0, missions.Count);
        currentMission = missions[randomIndex];
        progress = 0;
        timer = currentMission.timeLimit;
        missionActive = true;
        missionState = MissionState.InProgress;

        Debug.Log($"NEW MISSION: {currentMission.missionName} - {currentMission.description}");

        UIManager.Instance.ShowMission(currentMission.missionName, timer);

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

        if (missionState == MissionState.Completed)
        {
            Debug.Log("MISSION COMPLETE AFTER TIMER!");
            missionActive = false;
            UIManager.Instance.ShowMissionComplete();
            StartCoroutine(PrepareNextMission());
        }
        else
        {
            Debug.Log("MISSION FAILED AFTER TIMER!");
            MissionFailed();
        }
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

    public MissionData GetCurrentMission() => currentMission;
}
