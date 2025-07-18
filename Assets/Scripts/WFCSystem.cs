using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles Wave Function Collapse (WFC) logic for a grid-based tilemap system.
/// Initializes tile compatibility, collapses a tile grid by entropy, and propagates constraints
/// </summary>
public class WFCSystem : MonoBehaviour
{
    public List<WFCTile> tileSet; // List of all tile definitions
    public System.Random rng = new(); // Random num generator, used in collapsing

    // Compatability lookup tables, used in propagation
    private Dictionary<WFCType, List<WFCTile>> upCompat = new();
    private Dictionary<WFCType, List<WFCTile>> rightCompat = new();
    private Dictionary<WFCType, List<WFCTile>> downCompat = new();
    private Dictionary<WFCType, List<WFCTile>> leftCompat = new();

    /// <summary>
    /// Initializes tile set and builds directional compatibility lookups
    /// </summary>
    void Awake()
    {
        tileSet = WFCTileSet.GetTileSet();
        BuildCompatibilityLookup();
    }

    /// <summary>
    /// Creates a grid of given size and executes the wave function collapse algorithm
    /// </summary>
    /// <param name="rows">Grid height</param>
    /// <param name="cols">Grid width</param>
    /// <returns>Fully/partially collapsed grid of WFCTiles</returns>
    public WFCTile[,] RunWFC(int rows, int cols)
    {
        List<WFCTile>[,] wave = new List<WFCTile>[rows, cols];

        // Initialize wave with full superposition
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                wave[r, c] = new List<WFCTile>(tileSet);

        // Collapse and propagate until stable
        while (true)
        {
            Vector2Int? tile = FindLowestEntropy(wave, rows, cols);
            if (tile == null) break;

            Collapse(wave, tile.Value);
            Propagate(wave, tile.Value, rows, cols);
        }

        // Convert to a collapsed result grid
        WFCTile[,] result = new WFCTile[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                result[r, c] = wave[r, c].Count == 1 ? wave[r, c][0] : null;

        return result;
    }

    /// <summary>
    /// Builds compatibility lookup tables for each direction, based on tile edge constraints (definitions in tile set)
    /// </summary>
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

        // Add what types each tile can be adjacent to on each side (NWSE)
        foreach (WFCTile tile in tileSet)
        {
            upCompat[tile.down].Add(tile); // This tile can go above a tile with down = tile.down
            rightCompat[tile.left].Add(tile); // This tile can go to the right of a tile whose right = tile.left
            downCompat[tile.up].Add(tile); // This tile can go below a tile with up = tile.up
            leftCompat[tile.right].Add(tile); // This tile can go to the left of a tile with right = tile.right
        }
    }

    /// <summary>
    /// Gets all valid neighbors for a tile in a given direction
    /// </summary>
    /// <param name="tile">The tile to match against</param>
    /// <param name="direction">Direction from tile to neighbor</param>
    /// <returns>List of compatible WFCTiles</returns>
    public List<WFCTile> GetCompatibleNeighbors(WFCTile tile, Vector2Int direction)
    {
        if (direction == Vector2Int.up) return upCompat[tile.up];
        if (direction == Vector2Int.right) return rightCompat[tile.right];
        if (direction == Vector2Int.down) return downCompat[tile.down];
        if (direction == Vector2Int.left) return leftCompat[tile.left];
        return new List<WFCTile>(); // or throw error
    }

    /// <summary>
    /// Collapses a tile in the wave to a single randomly chosen tile
    /// </summary>
    /// <param name="wave">The wave grid (superpositions)</param>
    /// <param name="pos">Position to collapse</param>
    void Collapse(List<WFCTile>[,] wave, Vector2Int pos)
    {
        List<WFCTile> options = wave[pos.x, pos.y];
        if (options.Count <= 1) return;

        int i = rng.Next(options.Count);
        WFCTile chosen = options[i];
        wave[pos.x, pos.y] = new List<WFCTile> { chosen };
    }

    /// <summary>
    /// Propagates constraints from a collapsed tile to its neighbors
    /// </summary>
    /// <param name="wave">Wave grid</param>
    /// <param name="start">Starting tile (collapsed)</param>
    /// <param name="rows">Grid height</param>
    /// <param name="cols">Grid width</param>
    void Propagate(List<WFCTile>[,] wave, Vector2Int start, int rows, int cols)
    {
        Queue<Vector2Int> queue = new(); // BFS-style queue of postitions that need constraint propagation
        queue.Enqueue(start);

        // Four cardinal directions for adjacency
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        while (queue.Count > 0)
        {
            // Dequeue the next tile to propagate from
            Vector2Int current = queue.Dequeue();
            List<WFCTile> currentOptions = wave[current.x, current.y];

            // Check each neighboring tile
            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (!InBounds(neighbor, rows, cols)) continue;

                List<WFCTile> neighborOptions = wave[neighbor.x, neighbor.y];
                int before = neighborOptions.Count;

                // Remove neighbor options not compatible with any of the current tile's options
                neighborOptions.RemoveAll(n =>
                    !IsCompatibleWithAny(n, -dir, currentOptions) // flip dir because testing neighbor -> current
                );

                // If any options removed from neighbor, enqueue it to propagate further
                if (neighborOptions.Count < before)
                    queue.Enqueue(neighbor);
            }
        }
    }

    /// <summary>
    /// Finds the tile with the lowest entropy (fewest remaining tile options)
    /// </summary>
    /// <param name="wave">Wave grid</param>
    /// <param name="rows">Grid height</param>
    /// <param name="cols">Grid width</param>
    /// <returns>Position of tile to collapse, or null if complete</returns>
    Vector2Int? FindLowestEntropy(List<WFCTile>[,] wave, int rows, int cols)
    {
        int min = int.MaxValue; // Tracks current minimum entropy
        List<Vector2Int> candidates = new(); // Holds all tiles that match that minimum

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                int count = wave[r, c].Count;

                // Skip collapsed tiles (1 option) and invalid ones (0 options)
                if (count == 1 || count == 0) continue;

                if (count < min)
                {
                    // New minimum found, clear old candidates and store this one
                    min = count;
                    candidates.Clear();
                    candidates.Add(new Vector2Int(r, c));
                }
                else if (count == min)
                    // Tie for lowest entropy, add to candidate list
                    candidates.Add(new Vector2Int(r, c));
            }

        if (candidates.Count == 0) return null; // No candidates found (wave fully collapsed/unsolvable)
        return candidates[rng.Next(candidates.Count)]; // Randomly pick a candidate
    }

    /// <summary>
    /// Checks whether any tile in the given list is compatible with a target tile from a given direction
    /// </summary>
    /// <param name="neighborTile">Tile to test for compatibility</param>
    /// <param name="directionToCurrent">Direction to the current tile</param>
    /// <param name="possibleCurrentTiles">The current tile's list of options</param>
    /// <returns>True if any are compatible</returns>
    bool IsCompatibleWithAny(WFCTile neighborTile, Vector2Int directionToCurrent, List<WFCTile> possibleCurrentTiles)
    {
        foreach (WFCTile t in possibleCurrentTiles)
        {
            if (GetCompatibleNeighbors(t, directionToCurrent).Contains(neighborTile))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks whether a given tile is within bounds of the grid
    /// </summary>
    bool InBounds(Vector2Int pos, int rows, int cols)
    {
        return pos.x >= 0 && pos.x < rows && pos.y >= 0 && pos.y < cols;
    }
}
