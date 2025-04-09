using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text missionText;
    public Text timerText;
    public GameObject gameOverPanel;
    public GameObject missionCompletePanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ShowMission(string missionName, float time)
    {
        missionText.text = "Nhiệm vụ: " + missionName;
        UpdateMissionTimer(time);
    }

    public void UpdateMissionTimer(float time)
    {
        timerText.text = "Thời gian còn lại: " + Mathf.Ceil(time) + " giây";
    }

    public void ShowMissionComplete()
    {
        missionCompletePanel.SetActive(true);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }
}

