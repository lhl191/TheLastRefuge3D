using UnityEngine;

public class FruitResource : MonoBehaviour
{
    public int fruitAmountPerPick;

    private void Start()
    {
        // Lấy giá trị từ ConfigManager
        fruitAmountPerPick = ConfigManager.Instance.defaultFruitPerPick;
    }

    public void PickFruit()
    {
        // Cộng trái cây
        ResourceManager.Instance.AddFruit(fruitAmountPerPick);
        Debug.Log("Nhặt hoa quả! Hoa quả hiện tại: " + ResourceManager.Instance.GetFruitAmount());

        Destroy(gameObject); // Hoa quả đã được nhặt, xóa đối tượng
    }
}

