using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    private int woodAmount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddWood(int amount)
    {
        woodAmount += amount;
    }

    public int GetWoodAmount()
    {
        return woodAmount;
    }
}

