using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector2 currentGridPosition;

    private bool isSelected = false;

    private bool hasMovedThisTurn = false;

    private EnemyManager enemyManager;
    [SerializeField] private GameObject highlight;


    // ==========================================
    // SETUP
    // ==========================================

    public void SetEnemyManager(EnemyManager manager)
    {
        enemyManager = manager;
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

    //work pls
    public void SetHasMoved()
    {
        hasMovedThisTurn = true;

        UpdateHighlight();
    }


    public void ResetMovement()
    {
        hasMovedThisTurn = false;
        isSelected = false;

        UpdateHighlight();
    }


    // ==========================================
    // CLICK ENEMY
    // ==========================================

    private void OnMouseDown()
    {
        if (GameManager.Instance.State !=
        GameManager.Gamestate.EnemyMove)
        {
            return;
        }

        if (hasMovedThisTurn)
        {
            Debug.Log(gameObject.name + " has already moved!");
            return;
        }

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


        if (Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector2.up;
        }


        else if (Input.GetKeyDown(KeyCode.S) ||
                 Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector2.down;
        }


        else if (Input.GetKeyDown(KeyCode.A) ||
                 Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector2.left;
        }


        else if (Input.GetKeyDown(KeyCode.D) ||
                 Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector2.right;
        }


        else if (Input.GetKeyDown(KeyCode.Q))
        {
            direction = new Vector2(-1, 1);
        }


        else if (Input.GetKeyDown(KeyCode.E))
        {
            direction = new Vector2(1, 1);
        }


        else if (Input.GetKeyDown(KeyCode.Z))
        {
            direction = new Vector2(-1, -1);
        }


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
    // MOVE ENEMY
    // ==========================================

    private void Move(Vector2 direction)
    {
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


        if (targetTile == null)
        {
            Debug.Log("Cannot move there!");
            return;
        }


        if (GridManager.Instance.IsPositionOccupied(
            newGridPosition))
        {
            Debug.Log("Tile is occupied!");
            return;
        }


        GridManager.Instance.FreePosition(
            currentGridPosition
        );


        GridManager.Instance.OccupyPosition(
            newGridPosition
        );


        currentGridPosition = newGridPosition;


        Vector3 position =
            targetTile.transform.position;

        position.z = -1f;

        transform.position = position;


        hasMovedThisTurn = true;

        isSelected = false;

        GameManager.Instance.DeselectObject(this);

        // Tell EnemyManager
        enemyManager.EnemyMoved(this);
    }
    private void UpdateHighlight()
    {
        if (highlight == null)
            return;

        bool shouldHighlight =
            !hasMovedThisTurn &&
            GameManager.Instance.State == GameManager.Gamestate.EnemyMove;

        highlight.SetActive(shouldHighlight);
    }
}