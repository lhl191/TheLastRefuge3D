using UnityEngine;

public class TreeResource : MonoBehaviour
{
    public int hitPoints;
    public int woodPerHit;

    private void Start()
    {
        hitPoints = ConfigManager.Instance.defaultHitPoints;
        woodPerHit = ConfigManager.Instance.defaultWoodPerHit;
    }

    public void ChopTree()
    {
        hitPoints--;

        ResourceManager.Instance.AddWood(woodPerHit);
        Debug.Log("Chặt cây! Gỗ hiện tại: " + ResourceManager.Instance.GetWoodAmount());

        if (hitPoints <= 0)
        {
            MissionManager.Instance.UpdateProgress();
            Destroy(gameObject);
        }
    }
}
