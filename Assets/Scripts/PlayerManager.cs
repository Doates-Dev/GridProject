using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private List<Player> players = new List<Player>();

    private int playersMoved = 0;
   

    public void SpawnPlayers()
    {
        players.Clear();

        // 1 player on the left
        SpawnPlayer(1, 7);
        SpawnPlayer(1, 7);
        SpawnPlayer(1, 7);

        // 2 players on the right

        SpawnPlayer(9, 14);
        SpawnPlayer(9, 14); 
        SpawnPlayer(9, 14);

    }

    private void SpawnPlayer(int minX, int maxX)
    {
        Vector2 gridPosition =
            GridManager.Instance.GetRandomAvailablePosition(minX, maxX);

        Tile tile =
            GridManager.Instance.GetTileAtPosition(gridPosition);

        if (tile == null)
            return;

        Vector3 spawnPosition = tile.transform.position;
        spawnPosition.z = -1f;

        GameObject playerObject = Instantiate(
            playerPrefab,
            spawnPosition,
            Quaternion.identity
        );

        Player player = playerObject.GetComponent<Player>();

        player.SetGridPosition(gridPosition);

        // Give this player access to PlayerManager
        player.SetPlayerManager(this);

        players.Add(player);
    }

    public bool CanPlayerMove(Player player)
    {
        // Player has already moved
        if (player.HasMovedThisTurn())
        {
            return false;
        }

        return true;
    }

    public void PlayerMoved(Player player)
    {
        playersMoved++;

        player.SetHasMoved();

        Debug.Log(
            player.gameObject.name +
            " moved. " +
            playersMoved +
            "/" +
            players.Count
        );

        if (playersMoved >= players.Count)
        {
            Debug.Log("All players have moved!");

            GameManager.Instance.PlayerTurnFinished();
        }
    }

    public void ResetPlayerMoves()
    {
        playersMoved = 0;

        foreach (Player player in players)
        {
            player.ResetMovement();
        }
    }
    public void ClearPlayers()
    {
        foreach (Player player in players)
        {
            if (player != null)
            {
                Destroy(player.gameObject);
            }
        }

        players.Clear();
    }
}