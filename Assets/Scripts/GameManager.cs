using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Gamestate State;
    public MonoBehaviour SelectedObject;

    private void Awake()
    {
        Instance = this;
        Debug.Log("GameManager Awake");
    }

    private void Start()
    {
        Debug.Log("GameManager Start");

        UpdateGamestate(Gamestate.Start);
    }

    public void UpdateGamestate(Gamestate newState)
    {
        Debug.Log("UpdateGamestate called: " + newState);

        State = newState;

        switch (newState)
        {
            case Gamestate.Start:

                Debug.Log("Start state - Generating Grid");

                // Generate grid
                GridManager.Instance.GenerateGrid();

                // Spawn ball
                BallManager ballManager =
                    FindFirstObjectByType<BallManager>();

                ballManager.SpawnRandomBall();

                // Spawn enemies
                EnemyManager enemyManager =
                    FindFirstObjectByType<EnemyManager>();

                enemyManager.SpawnEnemies();

                // Spawn players
                PlayerManager playerManager =
                    FindFirstObjectByType<PlayerManager>();

                playerManager.SpawnPlayers();

                // Start player turn
                StartPlayerTurn();

                break;


            case Gamestate.PlayerMove:

                Debug.Log("PLAYER MOVE");

                break;


            case Gamestate.PlayerBallMove:

                Debug.Log("PLAYER BALL MOVE");

                Ball ball = FindFirstObjectByType<Ball>();

                if (ball != null && ball.IsPlayerNearBall())
                {
                    Debug.Log("Player is near the ball.");
                    Debug.Log("Ball can be moved by player.");
                }
                else
                {
                    Debug.Log("No player is near the ball.");

                    // Skip ball movement
                    UpdateGamestate(Gamestate.EnemyMove);
                }

                break;


            case Gamestate.EnemyMove:

                Debug.Log("ENEMY MOVE");

                EnemyManager enemyManager2 =
                    FindFirstObjectByType<EnemyManager>();

                enemyManager2.StartEnemyTurn();

                break;


            case Gamestate.EnemyBallMove:

                Debug.Log("ENEMY BALL MOVE");

                Ball ball2 = FindFirstObjectByType<Ball>();

                if (ball2 != null && ball2.IsEnemyNearBall())
                {
                    Debug.Log("Enemy is near the ball.");
                    Debug.Log("Ball can be moved by enemy.");
                }
                else
                {
                    Debug.Log("No enemy is near the ball.");

                    // Skip ball movement
                    StartPlayerTurn();
                }

                break;


            case Gamestate.Victory:

                Debug.Log("VICTORY!");

                break;


            case Gamestate.Lose:

                Debug.Log("LOSE!");

                break;
        }
    }


    // ==========================================
    // PLAYER TURN
    // ==========================================

    public void StartPlayerTurn()
    {
        PlayerManager playerManager =
            FindFirstObjectByType<PlayerManager>();

        playerManager.ResetPlayerMoves();

        UpdateGamestate(Gamestate.PlayerMove);
    }


    public void PlayerTurnFinished()
    {
        Debug.Log("All players have moved!");

        UpdateGamestate(Gamestate.PlayerBallMove);
    }


    // ==========================================
    // PLAYER BALL
    // ==========================================

    public void PlayerBallFinished()
    {
        Debug.Log("Player finished moving the ball.");

        UpdateGamestate(Gamestate.EnemyMove);
    }


    // ==========================================
    // ENEMY TURN
    // ==========================================

    public void EnemyTurnFinished()
    {
        Debug.Log("All enemies have moved!");

        UpdateGamestate(Gamestate.EnemyBallMove);
    }


    // ==========================================
    // ENEMY BALL
    // ==========================================

    public void EnemyBallFinished()
    {
        Debug.Log("Enemy finished moving the ball.");

        StartPlayerTurn();
    }


    // ==========================================
    // GAME STATES
    // ==========================================

    public enum Gamestate
    {
        Start,
        PlayerMove,
        PlayerBallMove,
        EnemyMove,
        EnemyBallMove,
        Victory,
        Lose
    }
    public bool SelectObject(MonoBehaviour obj)
    {
        if (SelectedObject != null && SelectedObject != obj)
        {
            Debug.Log("Another object is already selected.");
            return false;
        }

        SelectedObject = obj;
        return true;
    }


    public void DeselectObject(MonoBehaviour obj)
    {
        if (SelectedObject == obj)
        {
            SelectedObject = null;
        }
    }
}