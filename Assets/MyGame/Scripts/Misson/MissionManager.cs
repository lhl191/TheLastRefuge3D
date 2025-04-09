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
        if (missionActive && currentMission.timeLimit > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0 && progress < currentMission.requiredAmount)
            {
                MissionFailed(); // Hết thời gian nhưng chưa xong => Chết
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
        Debug.Log($" MISSION SUCCES: {currentMission.missionName}!");
        missionActive = false;
        missionState = MissionState.Completed;
        AssignNewMission();
    }

    public void MissionFailed()
    {
        Debug.Log($" TIME OUT! MISSION FAILED! : {currentMission.missionName}...");

        // Tìm PlayerController và gọi KillPlayer()
        ThirdPersonController player = FindFirstObjectByType<ThirdPersonController>();
        if (player != null)
        {
            player.KillPlayer();
        }

        missionActive = false;
        missionState = MissionState.Failed;
        GameManager.Instance.OnPlayerMissionFailed();
    }

    public MissionData GetCurrentMission() => currentMission;
}
