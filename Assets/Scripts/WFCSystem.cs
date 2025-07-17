using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCSystem : MonoBehaviour
{
    public List<WFCTile> WFCTileSet;
    public System.Random rng = new();

    private Dictionary<WFCType, List<WFCTile>> upCompat = new();
    private Dictionary<WFCType, List<WFCTile>> rightCompat = new();
    private Dictionary<WFCType, List<WFCTile>> downCompat = new();
    private Dictionary<WFCType, List<WFCTile>> leftCompat = new();

    void Awake()
    {
        WFCTileSet = new List<WFCTile> {
            // Grass:
            new WFCTile { type = WFCType.grass, up = WFCType.grass, right = WFCType.grass, down = WFCType.grass, left = WFCType.grass},
            // Grass: 1 Sand
            new WFCTile { type = WFCType.grass, up = WFCType.sand, right = WFCType.grass, down = WFCType.grass, left = WFCType.grass },
            new WFCTile { type = WFCType.grass, up = WFCType.grass, right = WFCType.sand, down = WFCType.grass, left = WFCType.grass },
            new WFCTile { type = WFCType.grass, up = WFCType.grass, right = WFCType.grass, down = WFCType.sand, left = WFCType.grass },
            new WFCTile { type = WFCType.grass, up = WFCType.grass, right = WFCType.grass, down = WFCType.grass, left = WFCType.sand },
            // Grass: 2 Sand (must be touching)
            new WFCTile { type = WFCType.grass, up = WFCType.sand, right = WFCType.sand, down = WFCType.grass, left = WFCType.grass },
            new WFCTile { type = WFCType.grass, up = WFCType.sand, right = WFCType.grass, down = WFCType.grass, left = WFCType.sand },
            new WFCTile { type = WFCType.grass, up = WFCType.grass, right = WFCType.sand, down = WFCType.sand, left = WFCType.grass },
            new WFCTile { type = WFCType.grass, up = WFCType.grass, right = WFCType.grass, down = WFCType.sand, left = WFCType.sand },

            // Sand: 
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.sand, down = WFCType.sand, left = WFCType.sand },
            // Sand: 1 Grass
            new WFCTile { type = WFCType.sand, up = WFCType.grass, right = WFCType.sand, down = WFCType.sand, left = WFCType.sand },
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.grass, down = WFCType.sand, left = WFCType.sand },
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.sand, down = WFCType.grass, left = WFCType.sand },
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.sand, down = WFCType.sand, left = WFCType.grass },
            // Sand: 2 Grass
            new WFCTile { type = WFCType.sand, up = WFCType.grass, right = WFCType.grass, down = WFCType.sand, left = WFCType.sand },
            new WFCTile { type = WFCType.sand, up = WFCType.grass, right = WFCType.sand, down = WFCType.grass, left = WFCType.sand },
            new WFCTile { type = WFCType.sand, up = WFCType.grass, right = WFCType.sand, down = WFCType.sand, left = WFCType.grass },
            new WFCTile { type = WFCType.sand, up = WFCType.grass, right = WFCType.grass, down = WFCType.sand, left = WFCType.sand },
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.grass, down = WFCType.grass, left = WFCType.sand },
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.grass, down = WFCType.sand, left = WFCType.grass },
            new WFCTile { type = WFCType.sand, up = WFCType.grass, right = WFCType.sand, down = WFCType.grass, left = WFCType.sand },
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.grass, down = WFCType.grass, left = WFCType.sand },
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.sand, down = WFCType.grass, left = WFCType.grass },
            new WFCTile { type = WFCType.sand, up = WFCType.grass, right = WFCType.sand, down = WFCType.sand, left = WFCType.grass },
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.grass, down = WFCType.sand, left = WFCType.grass },
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.sand, down = WFCType.grass, left = WFCType.grass },
            // Sand: 3 Grass
            new WFCTile { type = WFCType.sand, up = WFCType.grass, right = WFCType.sand, down = WFCType.sand, left = WFCType.sand },
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.grass, down = WFCType.sand, left = WFCType.sand },
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.sand, down = WFCType.grass, left = WFCType.sand },
            new WFCTile { type = WFCType.sand, up = WFCType.sand, right = WFCType.sand, down = WFCType.sand, left = WFCType.grass },

            // Water: 
            new WFCTile { type = WFCType.water, up = WFCType.water, right = WFCType.water, down = WFCType.water, left = WFCType.water },
            // Water: 1 Sand
            new WFCTile { type = WFCType.water, up = WFCType.sand, right = WFCType.water, down = WFCType.water, left = WFCType.water },
            new WFCTile { type = WFCType.water, up = WFCType.water, right = WFCType.sand, down = WFCType.water, left = WFCType.water },
            new WFCTile { type = WFCType.water, up = WFCType.water, right = WFCType.water, down = WFCType.sand, left = WFCType.water },
            new WFCTile { type = WFCType.water, up = WFCType.water, right = WFCType.water, down = WFCType.water, left = WFCType.sand },
            // Water: 2 Sand
            new WFCTile { type = WFCType.water, up = WFCType.water, right = WFCType.water, down = WFCType.sand, left = WFCType.sand },
            new WFCTile { type = WFCType.water, up = WFCType.water, right = WFCType.sand, down = WFCType.water, left = WFCType.sand },
            new WFCTile { type = WFCType.water, up = WFCType.water, right = WFCType.sand, down = WFCType.sand, left = WFCType.water },
            new WFCTile { type = WFCType.water, up = WFCType.water, right = WFCType.water, down = WFCType.sand, left = WFCType.sand },
            new WFCTile { type = WFCType.water, up = WFCType.sand, right = WFCType.water, down = WFCType.water, left = WFCType.sand },
            new WFCTile { type = WFCType.water, up = WFCType.sand, right = WFCType.water, down = WFCType.sand, left = WFCType.water },
            new WFCTile { type = WFCType.water, up = WFCType.water, right = WFCType.sand, down = WFCType.water, left = WFCType.sand },
            new WFCTile { type = WFCType.water, up = WFCType.sand, right = WFCType.water, down = WFCType.water, left = WFCType.sand },
            new WFCTile { type = WFCType.water, up = WFCType.sand, right = WFCType.sand, down = WFCType.water, left = WFCType.water },
            new WFCTile { type = WFCType.water, up = WFCType.water, right = WFCType.sand, down = WFCType.sand, left = WFCType.water },
            new WFCTile { type = WFCType.water, up = WFCType.sand, right = WFCType.water, down = WFCType.sand, left = WFCType.water },
            new WFCTile { type = WFCType.water, up = WFCType.sand, right = WFCType.sand, down = WFCType.water, left = WFCType.water },
            // Water: 3 Sand
            new WFCTile { type = WFCType.water, up = WFCType.water, right = WFCType.sand, down = WFCType.sand, left = WFCType.sand },
            new WFCTile { type = WFCType.water, up = WFCType.sand, right = WFCType.water, down = WFCType.sand, left = WFCType.sand },
            new WFCTile { type = WFCType.water, up = WFCType.sand, right = WFCType.sand, down = WFCType.water, left = WFCType.sand },
            new WFCTile { type = WFCType.water, up = WFCType.sand, right = WFCType.sand, down = WFCType.sand, left = WFCType.water },
        };

        BuildCompatibilityLookup();
    }

    void BuildCompatibilityLookup()
    {
        upCompat.Clear();
        rightCompat.Clear();
        downCompat.Clear();
        leftCompat.Clear();

        foreach (WFCType t in System.Enum.GetValues(typeof(WFCType)))
        {
            upCompat[t] = new List<WFCTile>();
            rightCompat[t] = new List<WFCTile>();
            downCompat[t] = new List<WFCTile>();
            leftCompat[t] = new List<WFCTile>();
        }

        foreach (WFCTile tile in WFCTileSet)
        {
            upCompat[tile.down].Add(tile); // This tile can go above a tile with down = tile.down
            rightCompat[tile.left].Add(tile); // This tile can go to the right of a tile whose right = tile.left
            downCompat[tile.up].Add(tile); // This tile can go below a tile with up = tile.up
            leftCompat[tile.right].Add(tile); // This tile can go to the left of a tile with right = tile.right
        }
    }

    public List<WFCTile> GetCompatibleNeighbors(WFCTile tile, Vector2Int direction)
    {
        if (direction == Vector2Int.up) return upCompat[tile.up];
        if (direction == Vector2Int.right) return rightCompat[tile.right];
        if (direction == Vector2Int.down) return downCompat[tile.down];
        if (direction == Vector2Int.left) return leftCompat[tile.left];
        return new List<WFCTile>(); // or throw error
    }

    public WFCTile[,] RunWFC(int rows, int cols)
    {
        List<WFCTile>[,] wave = new List<WFCTile>[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                wave[r, c] = new List<WFCTile>(WFCTileSet);

        while (true)
        {
            Vector2Int? cell = FindLowestEntropy(wave, rows, cols);
            if (cell == null) break;

            Collapse(wave, cell.Value);
            Propagate(wave, cell.Value, rows, cols);
        }

        WFCTile[,] result = new WFCTile[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                result[r, c] = wave[r, c].Count == 1 ? wave[r, c][0] : null;

        return result;
    }

    void Collapse(List<WFCTile>[,] wave, Vector2Int pos)
    {
        List<WFCTile> options = wave[pos.x, pos.y];
        if (options.Count <= 1) return;

        int i = rng.Next(options.Count);
        WFCTile chosen = options[i];
        wave[pos.x, pos.y] = new List<WFCTile> { chosen };
    }

    Vector2Int? FindLowestEntropy(List<WFCTile>[,] wave, int rows, int cols)
    {
        int min = int.MaxValue;
        List<Vector2Int> candidates = new();

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                int count = wave[r, c].Count;
                if (count == 1 || count == 0) continue;

                if (count < min)
                {
                    min = count;
                    candidates.Clear();
                    candidates.Add(new Vector2Int(r, c));
                }
                else if (count == min)
                    candidates.Add(new Vector2Int(r, c));
            }

        if (candidates.Count == 0) return null;
        return candidates[rng.Next(candidates.Count)];
    }

    void Propagate(List<WFCTile>[,] wave, Vector2Int start, int rows, int cols)
    {
        Queue<Vector2Int> queue = new();
        queue.Enqueue(start);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            List<WFCTile> currentOptions = wave[current.x, current.y];

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (!InBounds(neighbor, rows, cols)) continue;

                List<WFCTile> neighborOptions = wave[neighbor.x, neighbor.y];
                int before = neighborOptions.Count;

                neighborOptions.RemoveAll(n =>
                    !IsCompatibleWithAny(n, -dir, currentOptions)
                );

                if (neighborOptions.Count < before)
                    queue.Enqueue(neighbor);
            }
        }
    }

    bool IsCompatibleWithAny(WFCTile neighborTile, Vector2Int directionToCurrent, List<WFCTile> possibleCurrentTiles)
    {
        foreach (WFCTile t in possibleCurrentTiles)
        {
            if (GetCompatibleNeighbors(t, directionToCurrent).Contains(neighborTile))
                return true;
        }
        return false;
    }

    bool InBounds(Vector2Int pos, int rows, int cols)
    {
        return pos.x >= 0 && pos.x < rows && pos.y >= 0 && pos.y < cols;
    }
}
