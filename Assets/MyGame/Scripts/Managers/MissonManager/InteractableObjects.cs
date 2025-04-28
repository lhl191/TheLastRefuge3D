// File: Assets/MyGame/Scripts/Interactables/InteractableObjects.cs

using UnityEngine;

public class InteractableObjects : MonoBehaviour
{
    public enum ObjectType { Tree, Rock, Fruit, Animal, Beast, EnemyPlayer, Item, CollectableBranch }
    public ObjectType objectType;

    public GameObject interactUI;

    private bool isPlayerNearby = false;

    void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactUI != null)
                interactUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    void Update()
    {
        if (!isPlayerNearby) return;

        if (RequiresPressE() && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private bool RequiresPressE()
    {
        return objectType == ObjectType.Fruit || objectType == ObjectType.Item;
    }

    public void Interact()
    {
        Debug.Log($"INTERACT WITH: {objectType}");

        MissionData mission = MissionManager.Instance.GetCurrentMission();
        if (mission == null) return;

        if (interactUI != null)
            interactUI.SetActive(false);

        string weaponType = WeaponManager.GetWeaponTypeString();

        switch (objectType)
        {
            case ObjectType.Tree:
                GetComponent<TreeResource>()?.ChopTree();
                break;

            case ObjectType.Rock:
                GetComponent<StoneResource>()?.MineStone();
                break;

            case ObjectType.Fruit:
                GetComponent<FruitResource>()?.PickFruit();
                break;

            case ObjectType.Animal:
                AnimalHealth animalHealth = GetComponent<AnimalHealth>();
                if (animalHealth != null)
                {
                    animalHealth.TakeDamage(animalHealth.maxHealth, weaponType);
                }
                break;

            case ObjectType.Beast:
                BossHealth bossHealth = GetComponent<BossHealth>();
                if (bossHealth != null)
                {
                    bossHealth.TakeDamage(bossHealth.maxHealth, weaponType);
                }
                break;

            case ObjectType.EnemyPlayer:
                EnemyHealth enemyHealth = GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(enemyHealth.maxHealth, weaponType);
                }
                break;

            case ObjectType.Item:
                HandleMysteryBox();
                break;
            case ObjectType.CollectableBranch:
         
                PickBranch();
                break;
        }

        if (IsCorrectInteraction(mission.missionType))
        {
            Debug.Log("CORRECT ACTION! UPDATE PROGRESS!!");
            MissionManager.Instance.UpdateProgress();
        }
        else
        {
            Debug.Log("WRONG ACTION!! NO QUEST PROGRESS!!");
            MissionManager.Instance.WrongAction();
        }

     
        if (objectType != ObjectType.Item)
        {
            Destroy(gameObject);
        }
    }

    private void PickBranch()
    {
        Debug.Log("Picked up a branch!");
    }
    private void HandleMysteryBox()
    {
        Debug.Log("\ud83c\udff1 Opened Mystery Box!");

        MysteryBoxReward reward = GetComponent<MysteryBoxReward>();
        if (reward != null)
        {
            reward.SpawnReward();
        }
    }

    private bool IsCorrectInteraction(MissionData.MissionType missionType)
    {
        return ((objectType == ObjectType.Tree || objectType == ObjectType.Rock || objectType == ObjectType.Fruit || objectType == ObjectType.CollectableBranch)
                && missionType == MissionData.MissionType.Collect)
            || (objectType == ObjectType.Animal && missionType == MissionData.MissionType.HuntAnimal)
            || (objectType == ObjectType.Beast && missionType == MissionData.MissionType.KillBeast)
            || (objectType == ObjectType.EnemyPlayer && missionType == MissionData.MissionType.KillPlayer)
            || (objectType == ObjectType.Item && missionType == MissionData.MissionType.FindItem);
    }

}