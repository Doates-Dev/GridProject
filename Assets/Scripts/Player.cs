using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 currentGridPosition;

    private bool isSelected = false;

    // Each player has their own movement flag
    private bool hasMovedThisTurn = false;

    private PlayerManager playerManager;


    // ==========================================
    // SETUP
    // ==========================================

    public void SetPlayerManager(PlayerManager manager)
    {
        playerManager = manager;
    }


    public void SetGridPosition(Vector2 position)
    {
        currentGridPosition = position;
    }


    public Vector2 GetGridPosition()
    {
        return currentGridPosition;
    }


    // ==========================================
    // MOVEMENT STATUS
    // ==========================================

    public bool HasMovedThisTurn()
    {
        return hasMovedThisTurn;
    }


    public void SetHasMoved()
    {
        hasMovedThisTurn = true;
    }


    public void ResetMovement()
    {
        hasMovedThisTurn = false;
        isSelected = false;
    }


    // ==========================================
    // CLICK PLAYER
    // ==========================================

    private void OnMouseDown()
    {
        if (GameManager.Instance.State !=
        GameManager.Gamestate.PlayerMove)
        {
            return;
        }

        if (hasMovedThisTurn)
        {
            Debug.Log(gameObject.name + " has already moved!");
            return;
        }

        // Try to select this player
        if (!GameManager.Instance.SelectObject(this))
        {
            return;
        }

        isSelected = true;

        Debug.Log(gameObject.name + " selected");
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
    // MOVE PLAYER
    // ==========================================

    private void Move(Vector2 direction)
    {
        // Players move ONE square
        Vector2 newGridPosition =
            currentGridPosition + direction;
        if (GridManager.Instance.IsRestrictedPosition(newGridPosition))
        {
            Debug.Log("Players cannot move onto this tile!");
            return;
        }


        Tile targetTile =
            GridManager.Instance.GetTileAtPosition(
                newGridPosition
            );


        // Outside grid
        if (targetTile == null)
        {
            Debug.Log("Cannot move there!");
            return;
        }


        // Tile occupied
        if (GridManager.Instance.IsPositionOccupied(
            newGridPosition))
        {
            Debug.Log("Tile is occupied!");
            return;
        }


        // Free old position
        GridManager.Instance.FreePosition(
            currentGridPosition
        );


        // Occupy new position
        GridManager.Instance.OccupyPosition(
            newGridPosition
        );


        // Update grid position
        currentGridPosition = newGridPosition;


        // Update world position
        Vector3 position =
            targetTile.transform.position;

        position.z = -1f;

        transform.position = position;


        // This player has now moved
        hasMovedThisTurn = true;

        isSelected = false;

        GameManager.Instance.DeselectObject(this);


        // Tell PlayerManager
        playerManager.PlayerMoved(this);
    }
}