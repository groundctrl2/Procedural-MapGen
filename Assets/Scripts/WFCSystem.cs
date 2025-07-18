using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
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
    private Dictionary<WFCType, HashSet<WFCTile>> upCompat = new();
    private Dictionary<WFCType, HashSet<WFCTile>> rightCompat = new();
    private Dictionary<WFCType, HashSet<WFCTile>> downCompat = new();
    private Dictionary<WFCType, HashSet<WFCTile>> leftCompat = new();

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
        float[,] entropyGrid = new float[rows, cols];
        bool[,] entropyDirtyGrid = new bool[rows, cols];

        // Initialize wave with full superposition
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                wave[r, c] = new List<WFCTile>(tileSet);

        // Collapse and propagate until stable
        while (true)
        {
            Vector2Int? tile = FindLowestEntropy(wave, entropyGrid, entropyDirtyGrid, rows, cols);
            if (tile == null) break;

            Collapse(wave, tile.Value);
            Propagate(wave, tile.Value, rows, cols, entropyDirtyGrid);
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
            upCompat[t] = new HashSet<WFCTile>();
            rightCompat[t] = new HashSet<WFCTile>();
            downCompat[t] = new HashSet<WFCTile>();
            leftCompat[t] = new HashSet<WFCTile>();
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
    public HashSet<WFCTile> GetCompatibleNeighbors(WFCTile tile, Vector2Int direction)
    {
        if (direction == Vector2Int.up) return upCompat[tile.up];
        if (direction == Vector2Int.right) return rightCompat[tile.right];
        if (direction == Vector2Int.down) return downCompat[tile.down];
        if (direction == Vector2Int.left) return leftCompat[tile.left];
        return new HashSet<WFCTile>(); // or throw error
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
        options.Clear();
        options.Add(chosen); // Keep only the chosen tile in the wave list
    }

    /// <summary>
    /// Propagates constraints from a collapsed tile to its neighbors
    /// </summary>
    /// <param name="wave">Wave grid</param>
    /// <param name="start">Starting tile (collapsed)</param>
    /// <param name="rows">Grid height</param>
    /// <param name="cols">Grid width</param>
    /// <param name="entropyDirtyGrid">2D grid of flags indicating tiles needing entropy recalculated</param>
    void Propagate(List<WFCTile>[,] wave, Vector2Int start, int rows, int cols, bool[,] entropyDirtyGrid)
    {

        if (wave[start.x, start.y].Count <= 1)
            return; // Skip if start tile already collapsed/invalid

        bool[,] visited = new bool[rows, cols]; // Track tiles already enqueued
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
                if (neighborOptions.Count <= 1) continue; // Skip collapsed/invalid tiles
                int before = neighborOptions.Count;

                // Remove options not compatible with current
                for (int i = neighborOptions.Count - 1; i >= 0; i--)
                    if (!IsCompatibleWithAny(neighborOptions[i], -dir, currentOptions)) // flip dir because testing neighbor -> current
                        neighborOptions.RemoveAt(i);

                // If any options removed, enqueue to propagate further
                if (neighborOptions.Count < before && !visited[neighbor.x, neighbor.y])
                {
                    entropyDirtyGrid[neighbor.x, neighbor.y] = true;
                    visited[neighbor.x, neighbor.y] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    /// <summary>
    /// Finds the tile with the lowest Shannon entropy (fewest remaining tile options)
    /// Ignores tiles already collapsed (1 option) or invalid (0 options)
    /// </summary>
    /// <param name="wave">Wave grid (holds tile superpositions)</param>
    /// <param name="entropyGrid">Cached grid of entropy values to update</param>
    /// <param name="entropyDirtyGrid">2D grid of flags indicating tiles needing entropy recalculated</param>
    /// <param name="rows">Grid height</param>
    /// <param name="cols">Grid width</param>
    /// <returns>Position of tile to collapse (with lowest entropy), or null if complete</returns>
    Vector2Int? FindLowestEntropy(List<WFCTile>[,] wave, float[,] entropyGrid, bool[,] entropyDirtyGrid, int rows, int cols)
    {
        float min = float.MaxValue; // Current lowest entropy found
        List<Vector2Int> candidates = new(); // Positions tied for lowest entropy

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                if (entropyDirtyGrid[r, c])
                    UpdateEntropy(wave, entropyGrid, entropyDirtyGrid, r, c);

                if (wave[r, c].Count <= 1) continue; // Skip collapsed/invalid tiles

                float entropy = entropyGrid[r, c];
                if (entropy < min)
                {
                    // New minimum found, clear old candidates and store this one
                    min = entropy;
                    candidates.Clear();
                    candidates.Add(new Vector2Int(r, c));
                }
                else if (Mathf.Approximately(entropy, min))
                    // Equal  entropy, add to candidate list
                    candidates.Add(new Vector2Int(r, c));
            }

        if (candidates.Count == 0) return null; // No candidates found (wave fully collapsed/unsolvable)
        return candidates[rng.Next(candidates.Count)]; // Randomly pick a valid candidate
    }

    /// <summary>
    /// Updates the cached Shannon entropy value (in bits) for a specific tile position in the entropy grid
    /// Entropy is calculated using the weights of the remaining valid tile options
    /// Collapsed or invalid positions are marked with float.MaxValue to exclude them from selection
    /// </summary>
    /// <param name="wave">Wave grid holding tile option superpositions</param>
    /// <param name="entropyGrid">Cached grid of entropy values to update</param>
    /// <param name="entropyDirtyGrid">2D grid of flags indicating tiles needing entropy recalculated</param>
    /// <param name="r">Row index of the tile</param>
    /// <param name="c">Column index of the tile</param>
    void UpdateEntropy(List<WFCTile>[,] wave, float[,] entropyGrid, bool[,] entropyDirtyGrid, int r, int c)
    {
        if (!entropyDirtyGrid[r, c]) return; // Skip if not dirty

        var options = wave[r, c];

        if (options.Count <= 1)
        {
            entropyGrid[r, c] = float.MaxValue; // Treat collapsed/invalid tiles as unusable
            entropyDirtyGrid[r, c] = false;
            return;
        }

        // Calculate total weight of remaining options
        float sum = 0f;
        foreach (var tile in options)
            sum += tile.GetWeight(); // Total weight of all options

        // Apply Shannon entropy formula
        float entropy = 0f;
        foreach (var tile in options)
        {
            float p = tile.GetWeight() / sum; // Normalized weight (probability)
            entropy -= p * Mathf.Log(p, 2);
        }

        entropyGrid[r, c] = entropy; // Store result
        entropyDirtyGrid[r, c] = false; // Clear dirty flag after update
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
