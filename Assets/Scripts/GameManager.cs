using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Gamestate State;
    public MonoBehaviour SelectedObject;
    private HashSet<Player> playersWhoMovedBall = new HashSet<Player>();
    private HashSet<Enemy> enemiesWhoMovedBall = new HashSet<Enemy>();

    private Label victoryText;
    private Label loseText;

    private Button restartButton;
    public AudioSource src;
    public AudioClip KickSound, NetSound;
    // Start is called before the first frame update

    private int playerScore = 0;
    private int enemyScore = 0;

    

    public int GetPlayerScore()
    {

        return playerScore;
    }

    public int GetEnemyScore()
    {
        return enemyScore;
    }
    public void PlayerScored()
    {
        playerScore++;

        Debug.Log("PLAYER SCORED! Score: " +
                  playerScore + " - " + enemyScore);

        if (playerScore >= 3)
        {
            UpdateGamestate(Gamestate.Victory);
            return;
        }

        ResetAfterGoal();
    }
    private void ResetAfterGoal()
    {
        Debug.Log("Goal scored! Resetting for next round.");

        SelectedObject = null;

        PlayerManager playerManager =
            FindFirstObjectByType<PlayerManager>();

        EnemyManager enemyManager =
            FindFirstObjectByType<EnemyManager>();

        BallManager ballManager =
            FindFirstObjectByType<BallManager>();

        // Remove old ball
        ballManager.ClearBall();

        // Clear ALL old occupancy data
        GridManager.Instance.ClearOccupiedPositions();

        // Reset players
        playerManager.ResetPlayersToStart();

        // Reset enemies
        enemyManager.ResetEnemiesToStart();

        // Clear ball movement records
        playersWhoMovedBall.Clear();
        enemiesWhoMovedBall.Clear();

        // Spawn fresh ball
        ballManager.SpawnRandomBall();

        // Start new round
        StartPlayerTurn();
    }

    public void EnemyScored()
    {
        enemyScore++;

        Debug.Log("ENEMY SCORED! Score: " +
                  playerScore + " - " + enemyScore);

        if (enemyScore >= 3)
        {
            UpdateGamestate(Gamestate.Lose);
            return;
        }

        ResetAfterGoal();
    }
    public void KickSoundMethod()
    {
        src.clip = KickSound;
        src.Play();

    }
    public void NetSoundMethod()
    {
        src.clip = NetSound;
        src.Play();

    }

    private void Awake()
    {
        Instance = this;
        Debug.Log("GameManager Awake");
    }

    private void Start()
    {
        Debug.Log("GameManager Start");

        UIDocument uiDocument =
            FindFirstObjectByType<UIDocument>();

        if (uiDocument != null)
        {
            victoryText =
                uiDocument.rootVisualElement.Q<Label>(
                    "VictoryText"
                );

            loseText =
                uiDocument.rootVisualElement.Q<Label>(
                    "LoseText"
                );

            restartButton =
                uiDocument.rootVisualElement.Q<Button>(
                    "RestartButton"
                );

            if (victoryText != null)
            {
                victoryText.style.display =
                    DisplayStyle.None;
            }

            if (loseText != null)
            {
                loseText.style.display =
                    DisplayStyle.None;
            }

            if (restartButton != null)
            {
                restartButton.style.display =
                    DisplayStyle.None;

                restartButton.clicked += RestartGame;
            }
        }

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

                GridManager.Instance.GenerateGrid();

                BallManager ballManager =
                    FindFirstObjectByType<BallManager>();

                ballManager.SpawnRandomBall();

                EnemyManager enemyManager =
                    FindFirstObjectByType<EnemyManager>();

                enemyManager.SpawnEnemies();

                PlayerManager playerManager =
                    FindFirstObjectByType<PlayerManager>();

                playerManager.SpawnPlayers();

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
                NetSoundMethod();

                if (victoryText != null)
                {
                    victoryText.style.display =
                        DisplayStyle.Flex;
                }

                if (loseText != null)
                {
                    loseText.style.display =
                        DisplayStyle.None;
                }

                if (restartButton != null)
                {
                    restartButton.style.display =
                        DisplayStyle.Flex;
                }

                break;


            case Gamestate.Lose:

                Debug.Log("LOSE!");
                NetSoundMethod();

                if (victoryText != null)
                {
                    victoryText.style.display =
                        DisplayStyle.None;
                }

                if (loseText != null)
                {
                    loseText.style.display =
                        DisplayStyle.Flex;
                }

                if (restartButton != null)
                {
                    restartButton.style.display =
                        DisplayStyle.Flex;
                }

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

        playersWhoMovedBall.Clear();

        UpdateGamestate(Gamestate.PlayerMove);

        playerManager.ResetPlayerMoves();
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
        KickSoundMethod();

        Ball ball = FindFirstObjectByType<Ball>();

        if (ball != null && ball.IsEligiblePlayerNearBall())
        {
            Debug.Log("An eligible player is still near the ball.");

            UpdateGamestate(Gamestate.PlayerBallMove);
        }
        else
        {
            Debug.Log("No eligible players are near the ball.");

            UpdateGamestate(Gamestate.EnemyMove);
        }
    }
    public void StartEnemyBallTurn()
    {
        enemiesWhoMovedBall.Clear();

        Debug.Log("Enemy ball turn started.");

        UpdateGamestate(Gamestate.EnemyBallMove);
    }


    // ==========================================
    // ENEMY TURN
    // ==========================================

    public void EnemyTurnFinished()
    {
        Debug.Log("All enemies have moved!");

        // Start enemy ball phase
        StartEnemyBallTurn();
    }


    // ==========================================
    // ENEMY BALL
    // ==========================================

    public void EnemyBallFinished()
    {
        Debug.Log("Enemy finished moving the ball.");
        KickSoundMethod();

        Ball ball = FindFirstObjectByType<Ball>();

        if (ball != null && ball.IsEligibleEnemyNearBall())
        {
            Debug.Log("An eligible enemy is still near the ball.");

            UpdateGamestate(Gamestate.EnemyBallMove);
        }
        else
        {
            Debug.Log("No eligible enemies are near the ball.");

            StartPlayerTurn();
        }
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
    public bool HasPlayerMovedBall(Player player)
    {
        return playersWhoMovedBall.Contains(player);
    }


    public void PlayerMovedBall(Player player)
    {
        playersWhoMovedBall.Add(player);
    }


    public bool HasEnemyMovedBall(Enemy enemy)
    {
        return enemiesWhoMovedBall.Contains(enemy);
    }


    public void EnemyMovedBall(Enemy enemy)
    {
        enemiesWhoMovedBall.Add(enemy);
    }
    
    public void RestartGame()
    {
        Debug.Log("RESTARTING GAME");
        

        // Clear selected object
        SelectedObject = null;

        // Clear ball movement records
        playersWhoMovedBall.Clear();
        enemiesWhoMovedBall.Clear();


        // Get managers
        PlayerManager playerManager =
            FindFirstObjectByType<PlayerManager>();

        EnemyManager enemyManager =
            FindFirstObjectByType<EnemyManager>();

        BallManager ballManager =
            FindFirstObjectByType<BallManager>();


        // Destroy existing objects
        if (playerManager != null)
        {
            playerManager.ClearPlayers();
        }

        if (enemyManager != null)
        {
            enemyManager.ClearEnemies();
        }

        if (ballManager != null)
        {
            ballManager.ClearBall();
        }


        // Hide end-game UI
        if (victoryText != null)
        {
            victoryText.style.display =
                DisplayStyle.None;
        }

        if (loseText != null)
        {
            loseText.style.display =
                DisplayStyle.None;
        }

        if (restartButton != null)
        {
            restartButton.style.display =
                DisplayStyle.None;
        }


        // Start a completely new game
        UpdateGamestate(Gamestate.Start);
    }
}