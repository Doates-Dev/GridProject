using UnityEngine;

public class Ball : MonoBehaviour
{
    private Vector2 currentGridPosition;

    private bool isSelected = false;


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
        // PLAYER BALL MOVE
        if (GameManager.Instance.State ==
            GameManager.Gamestate.PlayerBallMove)
        {
            if (!IsPlayerNearBall())
            {
                Debug.Log(
                    "No player is close enough to move the ball."
                );

                return;
            }

            isSelected = true;

            Debug.Log("Player selected ball.");

            return;
        }


        // ENEMY BALL MOVE
        if (GameManager.Instance.State ==
            GameManager.Gamestate.EnemyBallMove)
        {
            if (!IsEnemyNearBall())
            {
                Debug.Log(
                    "No enemy is close enough to move the ball."
                );

                return;
            }

            isSelected = true;

            Debug.Log("Enemy selected ball.");

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
        for (int i = 1; i <= 3; i++)
        {
            Vector2 checkPosition =
                startingPosition + direction * i;

            // Check if we're outside the grid
            Tile checkTile =
                GridManager.Instance.GetTileAtPosition(checkPosition);

            if (checkTile == null)
            {
                Debug.Log("Ball reached the edge of the grid.");
                break;
            }

            // Check if something is occupying this tile
            if (GridManager.Instance.IsPositionOccupied(checkPosition))
            {
                Debug.Log(
                    "Ball blocked at " +
                    checkPosition +
                    ". Stopping before obstacle."
                );

                // Stop on the tile before the obstacle
                break;
            }

            // This tile is clear
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

}