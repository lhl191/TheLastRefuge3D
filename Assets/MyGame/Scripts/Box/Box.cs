using UnityEngine;

public class MysteryBoxReward : MonoBehaviour
{
    public GameObject rewardPrefab;
    public Transform rewardSpawnPoint;
    public Sprite rewardIcon;

    private Animator animator;
    private bool isOpened = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SpawnReward()
    {
        if (isOpened) return; 
        isOpened = true;

        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

       
        Invoke(nameof(SpawnAndShowReward), 3.0f); 
        Invoke(nameof(DestroyBox), 5.0f);
    }

    private void SpawnAndShowReward()
    {
        if (rewardPrefab != null && rewardSpawnPoint != null)
        {
            Instantiate(rewardPrefab, rewardSpawnPoint.position, rewardSpawnPoint.rotation);
            Debug.Log("🎁 Reward Spawned Successfully!");
        }

        if (rewardIcon != null)
        {
            RewardUIManager.Instance?.ShowReward(rewardIcon);
        }
    }

    private void DestroyBox()
    {
        Destroy(gameObject);
    }
}
   



