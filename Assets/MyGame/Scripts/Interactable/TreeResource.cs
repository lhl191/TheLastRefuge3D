using System.Resources;
using UnityEngine;

public class TreeResource : MonoBehaviour
{
    public int hitPoints = 3;
    public int woodPerHit = 3;

    public void ChopTree()
    {
        hitPoints--;

        // Cộng gỗ
        ResourceManager.Instance.AddWood(woodPerHit);
        Debug.Log("Chặt cây! Gỗ hiện tại: " + ResourceManager.Instance.GetWoodAmount());

        if (hitPoints <= 0)
        {
            Destroy(gameObject); // Hoặc chơi animation cây ngã rồi destroy
        }
    }
}

