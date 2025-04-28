using UnityEngine;

public class StoneResource : MonoBehaviour
{
    public int hitPoints;
    public int stonePerHit;

    private void Start()
    {
        hitPoints = ConfigManager.Instance.defaultStoneHitPoints;
        stonePerHit = ConfigManager.Instance.defaultStonePerHit;
    }

    public void MineStone()
    {
        hitPoints--;

        ResourceManager.Instance.AddStone(stonePerHit);
        Debug.Log("Khai thác đá! Đá hiện tại: " + ResourceManager.Instance.GetStoneAmount());
       
        if (hitPoints <= 0)
        {
            MissionManager.Instance.UpdateProgress();
            Destroy(gameObject); 
        }
    }
}

