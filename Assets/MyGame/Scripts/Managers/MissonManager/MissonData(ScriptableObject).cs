using UnityEngine;

[CreateAssetMenu(fileName = "NewMission", menuName = "Missions/MissionData")]
public class MissionData : ScriptableObject
{
    public string missionName;
    public string description;
    public MissionType missionType;
    public int requiredAmount;
    public float timeLimit;
  

    public enum MissionType
    {
        Survive,
        Collect,
        HuntAnimal,
        KillBeast,
        KillPlayer,
        StealthSurvive,
        CollectableBranch,
        FindItem
    }
}