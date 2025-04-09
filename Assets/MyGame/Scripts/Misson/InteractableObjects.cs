using UnityEngine;

public class InteractableObjects : MonoBehaviour
{
    public enum ObjectType { Tree, Rock, Fruit, Animal, Beast, EnemyPlayer, Item }
    public ObjectType objectType;

    public GameObject interactUI; 

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && interactUI != null)
        {
            interactUI.SetActive(true); // Hiện thông báo khi Player đến gần
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && interactUI != null)
        {
            interactUI.SetActive(false); // Ẩn thông báo khi Player đi xa
        }
    }

    public void Interact()
    {
        Debug.Log($" INTERACT WITH: (ĐANG TƯƠNG TÁC VỚI) : {objectType}");

        MissionData mission = MissionManager.Instance.GetCurrentMission();
        if (mission == null) return;

        if (interactUI != null)
        {
            interactUI.SetActive(false); // Ẩn UI trước khi hủy object
        }

        if (IsCorrectInteraction(mission.missionType))
        {
            Debug.Log(" CORRECT ACTION! UPDATE PROGRESS !! ");
            MissionManager.Instance.UpdateProgress();
        }
        else
        {
            Debug.Log(" WRONG ACTION !! NO QUEST PROGRESS !!");
            MissionManager.Instance.WrongAction();
        }

        Destroy(gameObject);
    }


    private bool IsCorrectInteraction(MissionData.MissionType missionType)
    {
       
        return (objectType == ObjectType.Tree || objectType == ObjectType.Rock || objectType == ObjectType.Fruit) && missionType == MissionData.MissionType.Collect
            || objectType == ObjectType.Animal && missionType == MissionData.MissionType.HuntAnimal
            || objectType == ObjectType.Beast && missionType == MissionData.MissionType.KillBeast
            || objectType == ObjectType.EnemyPlayer && missionType == MissionData.MissionType.KillPlayer
            || objectType == ObjectType.Item && missionType == MissionData.MissionType.FindItem;

    }
    

}