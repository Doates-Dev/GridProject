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

        List<Vector2> availablePositions =
            new List<Vector2>();

        foreach (Vector2 position in possiblePositions)
        {
            Tile tile =
                GridManager.Instance.GetTileAtPosition(position);

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

        // Try to find a position beside a player
        List<Vector2> playerPositions =
            new List<Vector2>();

        Player[] players =
            FindObjectsByType<Player>(
                FindObjectsSortMode.None
            );

        foreach (Player player in players)
        {
            playerPositions.Add(
                player.GetGridPosition()
            );
        }

        List<Vector2> positionsNearPlayer =
            new List<Vector2>();

        foreach (Vector2 ballPosition in availablePositions)
        {
            foreach (Vector2 playerPosition in playerPositions)
            {
                float distanceX =
                    Mathf.Abs(
                        ballPosition.x -
                        playerPosition.x
                    );

                float distanceY =
                    Mathf.Abs(
                        ballPosition.y -
                        playerPosition.y
                    );

                if (distanceX <= 1 &&
                    distanceY <= 1)
                {
                    positionsNearPlayer.Add(
                        ballPosition
                    );

                    break;
                }
            }
        }

        Vector2 randomPosition;

        // Prefer a position beside a player
        if (positionsNearPlayer.Count > 0)
        {
            randomPosition =
                positionsNearPlayer[
                    Random.Range(
                        0,
                        positionsNearPlayer.Count
                    )
                ];
        }
        else
        {
            // Fallback to any available position
            randomPosition =
                availablePositions[
                    Random.Range(
                        0,
                        availablePositions.Count
                    )
                ];
        }

        Vector3 spawnPosition =
            GridManager.Instance
                .GetTileAtPosition(randomPosition)
                .transform.position;

        spawnPosition.z = -1f;

        ball = Instantiate(
    ballPrefab,
    spawnPosition,
    Quaternion.identity
);

        // Make absolutely sure the ball can be clicked
        CircleCollider2D collider =
            ball.GetComponent<CircleCollider2D>();

        if (collider != null)
        {
            collider.enabled = true;
            Debug.Log("Ball collider enabled.");
        }
        else
        {
            Debug.LogError("Ball has no CircleCollider2D!");
        }

        Ball ballScript = ball.GetComponent<Ball>();

        if (ballScript == null)
        {
            Debug.LogError("ERROR: Spawned ball has no Ball component!");
            return;
        }

        ballScript.enabled = true;
        ballScript.SetGridPosition(randomPosition);

        Debug.Log(
            "Spawned Ball: " +
            ball.name +
            " | Ball script enabled: " +
            ballScript.enabled
        );
    }

    public void ClearBall()
    {
        Ball ballScript =
            FindFirstObjectByType<Ball>();

        if (ballScript != null)
        {
            Vector2 position =
                ballScript.GetGridPosition();

            GridManager.Instance.FreePosition(
                position
            );

            Destroy(ballScript.gameObject);
        }

        ball = null;
    }

}