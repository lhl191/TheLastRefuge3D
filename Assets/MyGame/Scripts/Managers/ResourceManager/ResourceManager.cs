using UnityEngine;

public class ResourceManager : BaseManager<ResourceManager>
{
    private int woodAmount = 0;
    private int fruitAmount = 0;
    private int stoneAmount = 0;

    public void AddWood(int amount)
    {
        woodAmount += amount;
    }

    public void AddStone(int amount)
    {
        stoneAmount += amount;
    }

    public void AddFruit(int amount)
    {
        fruitAmount += amount;
    }

    public int GetWoodAmount() => woodAmount;
    public int GetStoneAmount() => stoneAmount;
    public int GetFruitAmount() => fruitAmount;
}