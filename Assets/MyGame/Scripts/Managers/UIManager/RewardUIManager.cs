// Assets/MyGame/Scripts/UI/RewardUIManager.cs

using UnityEngine;
using UnityEngine.UI;

public class RewardUIManager : BaseManager<RewardUIManager>
{
    [Header("UI Components")]
    public GameObject rewardPanel;
    public Image rewardImage;
    public Button okButton;

    protected override void Awake()
    {
        base.Awake();

        if (rewardPanel != null)
            rewardPanel.SetActive(false);

        if (okButton != null)
            okButton.onClick.AddListener(HideReward);
    }

    public void ShowReward(Sprite rewardSprite)
    {
        if (rewardPanel != null && rewardImage != null)
        {
            rewardImage.sprite = rewardSprite;
            rewardPanel.SetActive(true);

            // 🔥 Unlock + Hiện chuột khi hiện Reward
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void HideReward()
    {
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(false);

            // 🔥 Khoá + Ẩn chuột lại khi tắt Reward
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

}

