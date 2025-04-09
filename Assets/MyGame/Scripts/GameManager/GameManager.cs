using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private PlayerHealth player;
    [SerializeField] private List<EnemyHealth> enemies;

    private bool gameEnded = false;

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            
        }

    }
   

    public void OnPlayerDied()
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log(" GAME OVER! Player is Dead!");
        // TODO: Load Game Over UI, disable gameplay
    }

    public void OnPlayerMissionFailed()
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log(" GAME OVER! Mission Failed!");
        // Load Game Over UI, disable gameplay
    }

    public void OnEnemyDied(EnemyHealth enemy)
    {
        if (gameEnded) return;

        int index = enemies.FindIndex(e => e.GetInstanceID() == enemy.GetInstanceID());
        if (index == -1)
        {
            return;
        }
        enemies.RemoveAt(index);
        


        if (!player.isDead && enemies.Count == 0)
        {
            gameEnded = true;
            Debug.Log(" YOU WIN! All Player eliminated. Last Survivor!");
            // Show Win UI
        }


    }
}
