using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, Tile> _tiles;

    // Keeps track of tiles that already contain an object
    private HashSet<Vector2> _occupiedPositions = new HashSet<Vector2>();

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();

        // Clear occupied positions in case the grid is regenerated
        _occupiedPositions.Clear();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var spawnedTile = Instantiate(
                    _tilePrefab,
                    new Vector3(x, y),
                    Quaternion.identity
                );

                spawnedTile.name = $"Tile {x} {y}";

                var isOffset =
                    (x % 2 == 0 && y % 2 != 0) ||
                    (x % 2 != 0 && y % 2 == 0);

                spawnedTile.Init(isOffset);

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3(
            (float)_width / 2 - 0.5f,
            (float)_height / 2 - 0.5f,
            -10
        );

       // _tiles[new Vector2(0, 2)].SetColor(Color.black);
       // _tiles[new Vector2(0, 6)].SetColor(Color.black);
      //  _tiles[new Vector2(15, 2)].SetColor(Color.black);
       // _tiles[new Vector2(15, 6)].SetColor(Color.black);
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile))
            return tile;

        return null;
    }
    public bool IsRestrictedPosition(Vector2 position)
    {
        return position == new Vector2(0, 2) ||
               position == new Vector2(0, 6) ||
               position == new Vector2(15, 2) ||
               position == new Vector2(15, 6);
    }

    // Get a random unoccupied position within a specific part of the board
    public Vector2 GetRandomAvailablePosition(int minX, int maxX)
    {
        List<Vector2> availablePositions = new List<Vector2>();

        for (int x = minX; x < maxX; x++)
        {
            for (int y = 2; y <= 5; y++)
            {
                Vector2 position = new Vector2(x, y);

                if (!_occupiedPositions.Contains(position))
                {
                    availablePositions.Add(position);
                }
            }
        }

        if (availablePositions.Count == 0)
        {
            Debug.LogError("No available spawn positions!");
            return new Vector2(-1, -1);
        }

        Vector2 randomPosition =
            availablePositions[Random.Range(0, availablePositions.Count)];
        GridManager.Instance.OccupyPosition(randomPosition);

        _occupiedPositions.Add(randomPosition);

        return randomPosition;
    }

    // Frees a tile when an object moves away from it
    public void FreePosition(Vector2 position)
    {
        _occupiedPositions.Remove(position);
    }

    // Changes an object's occupied tile when it moves
    public void MoveOccupiedPosition(Vector2 oldPosition, Vector2 newPosition)
    {
        _occupiedPositions.Remove(oldPosition);
        _occupiedPositions.Add(newPosition);
    }
    public bool OccupyPosition(Vector2 position)
    {
        if (_occupiedPositions.Contains(position))
        {
            return false;
        }

        _occupiedPositions.Add(position);
        return true;
    }

    public bool IsPositionOccupied(Vector2 position)
    {
        return _occupiedPositions.Contains(position);
    }
}