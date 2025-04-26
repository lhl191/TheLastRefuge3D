using System.Collections.Generic;
using UnityEngine;

public class GameManager : BaseManager<GameManager>
{
    [SerializeField] private PlayerHealth player;
    [SerializeField] private List<EnemyHealth> enemies;

    private bool gameEnded = false;

    public void OnPlayerDied()
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log("GAME OVER! Player is Dead!");
    }

    public void OnPlayerMissionFailed()
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log("GAME OVER! Mission Failed!");
    }

    public void OnEnemyDied(EnemyHealth enemy)
    {
        if (gameEnded) return;

        int index = enemies.FindIndex(e => e.GetInstanceID() == enemy.GetInstanceID());
        if (index == -1) return;
        enemies.RemoveAt(index);

        if (!player.isDead && enemies.Count == 0)
        {
            gameEnded = true;
            Debug.Log("YOU WIN! All Player eliminated. Last Survivor!");
        }
    }
}