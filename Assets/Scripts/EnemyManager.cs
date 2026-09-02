using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    private List<Enemy> enemies = new List<Enemy>();
    private List<Vector2> enemyStartingPositions = new List<Vector2>();

    private int enemiesMoved = 0;

    public void SpawnEnemies()
    {
        enemies.Clear();
        enemyStartingPositions.Clear();

        // Left side
        SpawnEnemy(1, 7);
        SpawnEnemy(1, 7);
        SpawnEnemy(1, 7);

        // Right side
        SpawnEnemy(9, 14);
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
        enemyStartingPositions.Add(gridPosition);

        GridManager.Instance.OccupyPosition(gridPosition);
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

    public void ResetEnemiesToStart()
    {
        Debug.Log("Resetting enemies to starting positions.");

        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy enemy = enemies[i];

            if (enemy == null)
                continue;

            Vector2 currentPosition =
                enemy.GetGridPosition();

            Vector2 startingPosition =
                enemyStartingPositions[i];

            // Free current position
            GridManager.Instance.OccupyPosition(
    startingPosition
);

            // Set enemy grid position
            enemy.SetGridPosition(
                startingPosition
            );

            // Move enemy visually
            Tile tile =
                GridManager.Instance.GetTileAtPosition(
                    startingPosition
                );

            if (tile != null)
            {
                Vector3 worldPosition =
                    tile.transform.position;

                worldPosition.z = -1f;

                enemy.transform.position =
                    worldPosition;
            }

            // Reset movement
            enemy.ResetMovement();
        }

        enemiesMoved = 0;
    }

    public void ClearEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }

        enemies.Clear();
        enemyStartingPositions.Clear();
    }
}