using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    private List<Enemy> enemies = new List<Enemy>();

    private int enemiesMoved = 0;

    public void SpawnEnemies()
    {
        enemies.Clear();

        // 1 enemy on left
        SpawnEnemy(1, 7);
        SpawnEnemy(1, 7);

        // 2 enemies on right
        SpawnEnemy(9, 14);
        SpawnEnemy(9, 14);
    }

    private void SpawnEnemy(int minX, int maxX)
    {
        Vector2 gridPosition =
            GridManager.Instance.GetRandomAvailablePosition(minX, maxX);

        Tile tile =
            GridManager.Instance.GetTileAtPosition(gridPosition);

        if (tile == null)
            return;

        Vector3 spawnPosition = tile.transform.position;
        spawnPosition.z = -1f;

        GameObject enemyObject = Instantiate(
            enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );

        Enemy enemy = enemyObject.GetComponent<Enemy>();

        enemy.SetGridPosition(gridPosition);
        enemy.SetEnemyManager(this);

        enemies.Add(enemy);
    }

    public void EnemyMoved(Enemy enemy)
    {
        enemiesMoved++;

        enemy.SetHasMoved();

        Debug.Log(
            enemy.gameObject.name +
            " moved. " +
            enemiesMoved +
            "/" +
            enemies.Count
        );

        if (enemiesMoved >= enemies.Count)
        {
            Debug.Log("All enemies have moved!");

            GameManager.Instance.EnemyTurnFinished();
        }
    }

    public void StartEnemyTurn()
    {
        enemiesMoved = 0;

        foreach (Enemy enemy in enemies)
        {
            enemy.ResetMovement();
        }

        Debug.Log("Enemy turn started");
    }
}