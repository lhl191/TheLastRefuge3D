using UnityEngine;
using TMPro;

public class ResourceUI : BaseManager<ResourceUI>
{
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI fruitText;

    void Update()
    {
        // Cập nhật thông tin tài nguyên
        if (woodText != null)
        {
            woodText.text = " " + ResourceManager.Instance.GetWoodAmount();
        }

        if (fruitText != null)
        {
            fruitText.text = " " + ResourceManager.Instance.GetFruitAmount();
        }

        if (stoneText != null)
        {
            stoneText.text = " " + ResourceManager.Instance.GetStoneAmount();
        }
    }
}
