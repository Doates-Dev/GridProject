using UnityEngine;

public class Ball : MonoBehaviour
{
    private Vector2 currentGridPosition;

    private bool isSelected = false;

    private Player ballMovingPlayer;
    private Enemy ballMovingEnemy;


   

    // ==========================================
    // SETUP
    // ==========================================





    public void SetGridPosition(Vector2 position)
    {
        currentGridPosition = position;

        UpdateWorldPosition();
    }


    public Vector2 GetGridPosition()
    {
        return currentGridPosition;
    }


    // ==========================================
    // CLICK BALL
    // ==========================================

    private void OnMouseDown()
    {
        
        // ==========================================
        // PLAYER BALL MOVE
        // ==========================================

        if (GameManager.Instance.State ==
            GameManager.Gamestate.PlayerBallMove)
        {
            Player[] players =
                FindObjectsByType<Player>(
                    FindObjectsSortMode.None
                );

            foreach (Player player in players)
            {
                // This player has already moved the ball
                if (GameManager.Instance.HasPlayerMovedBall(player))
                    continue;

                // Is this player beside the ball?
                if (IsWithinOneTile(player.GetGridPosition()))
                {
                    ballMovingPlayer = player;
                    isSelected = true;

                    Debug.Log(
                        player.gameObject.name +
                        " selected the ball."
                    );

                    return;
                }
            }

            Debug.Log("No eligible player is near the ball.");

            return;
        }


        // ==========================================
        // ENEMY BALL MOVE
        // ==========================================

        if (GameManager.Instance.State ==
            GameManager.Gamestate.EnemyBallMove)
        {
            Enemy[] enemies =
                FindObjectsByType<Enemy>(
                    FindObjectsSortMode.None
                );

            foreach (Enemy enemy in enemies)
            {
                // This enemy has already moved the ball
                if (GameManager.Instance.HasEnemyMovedBall(enemy))
                    continue;

                // Is this enemy beside the ball?
                if (IsWithinOneTile(enemy.GetGridPosition()))
                {
                    ballMovingEnemy = enemy;
                    isSelected = true;

                    Debug.Log(
                        enemy.gameObject.name +
                        " selected the ball."
                    );

                    return;
                }
            }

            Debug.Log("No eligible enemy is near the ball.");

            return;
        }
    }


    // ==========================================
    // INPUT
    // ==========================================

    private void Update()
    {
        if (!isSelected)
            return;


        Vector2 direction = Vector2.zero;


        // UP
        if (Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector2.up;
        }


        // DOWN
        else if (Input.GetKeyDown(KeyCode.S) ||
                 Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector2.down;
        }


        // LEFT
        else if (Input.GetKeyDown(KeyCode.A) ||
                 Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector2.left;
        }


        // RIGHT
        else if (Input.GetKeyDown(KeyCode.D) ||
                 Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector2.right;
        }


        // UP LEFT
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            direction = new Vector2(-1, 1);
        }


        // UP RIGHT
        else if (Input.GetKeyDown(KeyCode.E))
        {
            direction = new Vector2(1, 1);
        }


        // DOWN LEFT
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            direction = new Vector2(-1, -1);
        }


        // DOWN RIGHT
        else if (Input.GetKeyDown(KeyCode.C))
        {
            direction = new Vector2(1, -1);
        }


        if (direction != Vector2.zero)
        {
            Move(direction);
        }
    }


    // ==========================================
    // MOVE BALL
    // ==========================================

    private void Move(Vector2 direction)
    {
        // Remember where the ball started
        Vector2 startingPosition = currentGridPosition;

        // This will be the furthest position the ball can reach
        Vector2 finalPosition = startingPosition;

        // Check up to 3 spaces
        for (int i = 1; i <= 2; i++)
        {
            Vector2 checkPosition =
                startingPosition + direction * i;

            // Restricted tile
            if (GridManager.Instance.IsRestrictedPosition(checkPosition))
            {
                Debug.Log(
                    "Ball cannot enter restricted tile: " +
                    checkPosition
                );

                break;
            }

            Tile checkTile =
                GridManager.Instance.GetTileAtPosition(checkPosition);

            if (checkTile == null)
            {
                Debug.Log("Ball reached the edge of the grid.");
                break;
            }

            if (GridManager.Instance.IsPositionOccupied(checkPosition))
            {
                Debug.Log(
                    "Ball blocked at " +
                    checkPosition +
                    ". Stopping before obstacle."
                );

                break;
            }

            finalPosition = checkPosition;
            


        }


        // Ball couldn't move
        if (finalPosition == startingPosition)
        {
            Debug.Log("Ball cannot move - obstacle immediately in front.");
            isSelected = false;

            GameManager.Instance.DeselectObject(this);

            return;
        }


        // Free the ball's old position
        GridManager.Instance.FreePosition(
            startingPosition
        );


        // Occupy the ball's new position
        GridManager.Instance.OccupyPosition(
            finalPosition
        );


        // Update grid position
        currentGridPosition = finalPosition;


        // Move visually
        UpdateWorldPosition();


        // Deselect ball
        isSelected = false;

        GameManager.Instance.DeselectObject(this);


        // ==========================================
        // RECORD WHO MOVED THE BALL
        // ==========================================

        if (ballMovingPlayer != null)
        {
            GameManager.Instance.PlayerMovedBall(ballMovingPlayer);

            Debug.Log(
                ballMovingPlayer.gameObject.name +
                " has used their ball movement."
            );

            ballMovingPlayer = null;
        }

        if (ballMovingEnemy != null)
        {
            GameManager.Instance.EnemyMovedBall(ballMovingEnemy);

            Debug.Log(
                ballMovingEnemy.gameObject.name +
                " has used their ball movement."
            );

            ballMovingEnemy = null;
        }


        // ==========================================
        // CHECK VICTORY
        // ==========================================

        if (IsVictoryPosition())
        {
            Debug.Log("BALL ENTERED VICTORY AREA!");

            GameManager.Instance.UpdateGamestate(
                GameManager.Gamestate.Victory
            );

            return;
        }


        // ==========================================
        // CHECK LOSE
        // ==========================================

        if (IsLosePosition())
        {
            Debug.Log("BALL ENTERED LOSE AREA!");

            GameManager.Instance.UpdateGamestate(
                GameManager.Gamestate.Lose
            );

            return;
        }



        // ==========================================
        // CONTINUE GAME
        // ==========================================

        if (GameManager.Instance.State ==
            GameManager.Gamestate.PlayerBallMove)
        {
            GameManager.Instance.PlayerBallFinished();
        }
        else if (GameManager.Instance.State ==
                 GameManager.Gamestate.EnemyBallMove)
        {
            GameManager.Instance.EnemyBallFinished();
        }
    }


    // ==========================================
    // CHECK PLAYER DISTANCE
    // ==========================================

    public bool IsPlayerNearBall()
    {
        Player[] players =
            FindObjectsByType<Player>(
                FindObjectsSortMode.None
            );


        foreach (Player player in players)
        {
            if (IsWithinOneTile(
                player.GetGridPosition()))
            {
                return true;
            }
        }


        return false;
    }


    // ==========================================
    // CHECK ENEMY DISTANCE
    // ==========================================

    public bool IsEnemyNearBall()
    {
        Enemy[] enemies =
            FindObjectsByType<Enemy>(
                FindObjectsSortMode.None
            );


        foreach (Enemy enemy in enemies)
        {
            if (IsWithinOneTile(
                enemy.GetGridPosition()))
            {
                return true;
            }
        }


        return false;
    }


    // ==========================================
    // DISTANCE CHECK
    // ==========================================

    private bool IsWithinOneTile(
        Vector2 otherPosition)
    {
        float distanceX =
            Mathf.Abs(
                otherPosition.x -
                currentGridPosition.x
            );


        float distanceY =
            Mathf.Abs(
                otherPosition.y -
                currentGridPosition.y
            );


        // Includes diagonal positions
        return distanceX <= 1 &&
               distanceY <= 1;
    }


    // ==========================================
    // WORLD POSITION
    // ==========================================

    private void UpdateWorldPosition()
    {
        Tile tile =
            GridManager.Instance.GetTileAtPosition(
                currentGridPosition
            );


        if (tile != null)
        {
            Vector3 position =
                tile.transform.position;

            position.z = -1f;

            transform.position = position;
        }
    }
    private bool IsVictoryPosition()
    {
        return currentGridPosition == new Vector2(15, 3) ||
               currentGridPosition == new Vector2(15, 4) ||
               currentGridPosition == new Vector2(15, 5);
    }
    private bool IsLosePosition()
    {
        return currentGridPosition == new Vector2(0, 3) ||
               currentGridPosition == new Vector2(0, 4) ||
               currentGridPosition == new Vector2(0, 5);
    }
    public bool IsEligiblePlayerNearBall()
    {
        Player[] players =
            FindObjectsByType<Player>(
                FindObjectsSortMode.None
            );

        foreach (Player player in players)
        {
            // Ignore players who already moved the ball
            if (GameManager.Instance.HasPlayerMovedBall(player))
                continue;

            if (IsWithinOneTile(player.GetGridPosition()))
            {
                return true;
            }
        }

        return false;
    }
    public bool IsEligibleEnemyNearBall()
    {
        Enemy[] enemies =
            FindObjectsByType<Enemy>(
                FindObjectsSortMode.None
            );

        foreach (Enemy enemy in enemies)
        {
            // Ignore enemies who already moved the ball
            if (GameManager.Instance.HasEnemyMovedBall(enemy))
                continue;

            if (IsWithinOneTile(enemy.GetGridPosition()))
            {
                return true;
            }
        }

        return false;
    }
    

}