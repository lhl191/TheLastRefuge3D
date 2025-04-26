using UnityEngine;

public class SurvivalTimer : MonoBehaviour
{
    private float survivalTime;
    private bool isActive;

    void Start()
    {
        MissionData mission = MissionManager.Instance.GetCurrentMission();
        if (mission != null && mission.missionType == MissionData.MissionType.Survive)
        {
            isActive = true;
            survivalTime = mission.timeLimit;
        }
    }

    void Update()
    {
        if (isActive)
        {
            survivalTime -= Time.deltaTime;
            if (survivalTime <= 0)
            {
                isActive = false;
                MissionManager.Instance.MissionFailed(); // Gọi MissionFailed() khi hết thời gian
            }
        }
    }
}
