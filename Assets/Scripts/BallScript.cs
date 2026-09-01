using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;

    private GameObject ball;
   

    public void SpawnRandomBall()
    {
        Vector2[] possiblePositions =
        {
        new Vector2(6, 3),
        new Vector2(9, 3),
        new Vector2(9, 5),
        new Vector2(6, 5)
    };

        List<Vector2> availablePositions = new List<Vector2>();

        foreach (Vector2 position in possiblePositions)
        {
            Tile tile = GridManager.Instance.GetTileAtPosition(position);

            if (tile != null &&
                !GridManager.Instance.IsPositionOccupied(position))
            {
                availablePositions.Add(position);
            }
        }

        if (availablePositions.Count == 0)
        {
            Debug.LogError("No available ball positions!");
            return;
        }

        Vector2 randomPosition =
            availablePositions[Random.Range(0, availablePositions.Count)];

        Vector3 spawnPosition =
            GridManager.Instance.GetTileAtPosition(randomPosition).transform.position;

        spawnPosition.z = -1f;

        ball = Instantiate(
            ballPrefab,
            spawnPosition,
            Quaternion.identity
        );

        Ball ballScript = ball.GetComponent<Ball>();
        ballScript.SetGridPosition(randomPosition);
        GridManager.Instance.OccupyPosition(randomPosition);
    }
    public void ClearBall()
    {
        Ball ball = FindFirstObjectByType<Ball>();

        if (ball != null)
        {
            Destroy(ball.gameObject);
        }
    }

}