using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI fruitText;

    void Update()
    {
        if (woodText != null)
        {
            woodText.text = "Wood: " + ResourceManager.Instance.GetWoodAmount();
        }

        if (fruitText != null)
        {
            fruitText.text = "Fruit: " + ResourceManager.Instance.GetFruitAmount();
        }

        if (stoneText != null)
        {
            stoneText.text = "Stone: " + ResourceManager.Instance.GetStoneAmount();
        }
    }
}


