using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseManager<GameManager>
{
    [SerializeField] private PlayerHealth player;
    [SerializeField] private List<EnemyHealth> enemies;

    private bool gameEnded = false;

    private bool sceneHooked = false;

    protected override void Awake()
    {
        base.Awake();

        if (!sceneHooked)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            sceneHooked = true;
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Start()
    {
        StartCoroutine(DelayedReset());
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
     
        StartCoroutine(DelayedReset());
    }
    IEnumerator DelayedReset()
    {
        yield return null; 
        ResetGameState();
    }
    public void ResetGameState()
    {
        gameEnded = false;
        player = Object.FindFirstObjectByType<PlayerHealth>();
        enemies = new List<EnemyHealth>(FindObjectsOfType<EnemyHealth>());

        if (player != null)
        {
            player.ResetHealth();
        }
    }


    public void OnPlayerDied()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("GAME OVER! Player is Dead!");
        UIManager.Instance.ShowGameOver();
    }

    public void OnPlayerMissionFailed()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("GAME OVER! Mission Failed!");
        UIManager.Instance.ShowGameOver();
    }

    public void OnEnemyDied(EnemyHealth enemy)
    {
        if (gameEnded) return;
        if (enemy == null || enemies == null)
        {
            Debug.LogWarning("Enemy or enemy list is null in OnEnemyDied");
            return;
        }

        int index = enemies.FindIndex(e => e.GetInstanceID() == enemy.GetInstanceID());
        if (index == -1)
        {
            Debug.LogWarning("Enemy not found in list.");
            return;
        }

        enemies.RemoveAt(index);

        if (!player.isDead && enemies.Count == 0)
        {
            gameEnded = true;
            Debug.Log("YOU WIN! All Enemies Eliminated. Last Survivor!");
            UIManager.Instance.ShowVictory();
        }
    }


    public void OnPlayerRespawn()
    {
        PlayerHealth player = Object.FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.ResetHealth();
        }

        MissionManager.Instance.ResetMissionState();
    }
}