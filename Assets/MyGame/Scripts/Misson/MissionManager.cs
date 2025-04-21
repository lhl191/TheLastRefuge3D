using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;
    public List<MissionData> missions;
    private MissionData currentMission;
    private int progress;
    private float timer;
    private bool missionActive = false;

    public GameObject stealthSoundPrefab;
    private GameObject activeSoundObj;

    public enum MissionState { NotStarted, InProgress, Completed, Failed }
    public MissionState missionState = MissionState.NotStarted;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        AssignNewMission();
    }

    void Update()
    {
        if (!missionActive) return;

        if (missionState == MissionState.InProgress && currentMission.timeLimit > 0)
        {
            timer -= Time.deltaTime;

            // Nếu hết thời gian mà chưa hoàn thành
            if (timer <= 0 && progress < currentMission.requiredAmount)
            {
                MissionFailed();
            }

            // Nếu hết thời gian và nhiệm vụ đã hoàn thành → gán nhiệm vụ mới
            if (timer <= 0 && missionState == MissionState.Completed)
            {
                AssignNewMission();
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

        Debug.Log($"🎯 NEW MISSION: {currentMission.missionName} - {currentMission.description}");

        // Nếu là nhiệm vụ stealth → bật âm thanh
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
        Debug.Log($" PROGRESS : {progress}/{currentMission.requiredAmount}");

        if (progress >= currentMission.requiredAmount)
        {
            MissionCompleted();
        }
    }
    public void WrongAction()
    {
        Debug.Log(" WRONG ACTION !! NO QUEST PROGRESS !! ");
    }
    void MissionCompleted()
    {
        Debug.Log($" MISSION SUCCESS: {currentMission.missionName}!");
        missionState = MissionState.Completed;

        RemoveStealthSound(); // 👈 Khi hoàn thành nhiệm vụ
    }


    public void MissionFailed()
    {
        Debug.Log($" TIME OUT! MISSION FAILED! : {currentMission.missionName}...");

        ThirdPersonController player = FindFirstObjectByType<ThirdPersonController>();
        if (player != null) player.KillPlayer();

        missionActive = false;
        missionState = MissionState.Failed;
        GameManager.Instance.OnPlayerMissionFailed();

        RemoveStealthSound(); // 👈 Khi nhiệm vụ fail
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
