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
    Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left }; // Four cardinal directions for adjacency

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
        MinPriorityQueue pq = new MinPriorityQueue(rows * cols);
        int index;
        Vector2Int current;

        // Initialize wave with full superposition
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                wave[r, c] = new List<WFCTile>(tileSet);

        // Pick random starting tile
        current = new Vector2Int(rng.Next(rows), rng.Next(cols));

        // Insert all tiles into priority queue before anything collapses
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector2Int pos = new Vector2Int(r, c);
                if (pos == current) continue; // Skip the (starting) tile about to collapse
                index = r * cols + c;
                float entropy = CalculateEntropy(wave[r, c]);
                pq.Insert(entropy, index);
            }
        }

        // Collapse and propagate start tile
        Collapse(wave, current);
        Propagate(wave, current, rows, cols, pq);

        // Collapse and propagate until stable
        while (!pq.IsEmpty())
        {
            // Get position of lowest entropy tile
            index = pq.DelMin();
            current = new Vector2Int(index / cols, index % cols);

            // Collapse and propagate
            Collapse(wave, current);
            Propagate(wave, current, rows, cols, pq);
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
            upCompat[tile.up].Add(tile);
            rightCompat[tile.right].Add(tile);
            downCompat[tile.down].Add(tile);
            leftCompat[tile.left].Add(tile);
        }
    }

    /// <summary>
    /// Returns a list of tiles that are compatible with the given tile in the specified direction.
    /// This uses the edge of the current tile and looks for neighbor tiles whose opposite edge matches.
    /// </summary>
    /// <param name="tile">The tile to match against</param>
    /// <param name="direction">Direction from tile to neighbor</param>
    /// <returns>List of compatible WFCTiles</returns>
    public HashSet<WFCTile> GetCompatibleNeighbors(WFCTile tile, Vector2Int direction)
    {
        if (direction == Vector2Int.up) return downCompat[tile.up];
        else if (direction == Vector2Int.right) return leftCompat[tile.right];
        else if (direction == Vector2Int.down) return upCompat[tile.down];
        else if (direction == Vector2Int.left) return rightCompat[tile.left];
        else return new HashSet<WFCTile>(); // or throw error
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

        // Compute total weight
        float totalWeight = 0f;
        foreach (var tile in options)
            totalWeight += tile.GetWeight();

        // Roll a random float in [0, totalWeight)
        float roll = (float)(rng.NextDouble() * totalWeight);

        // Find the tile corresponding to the roll
        float cumulative = 0f;
        foreach (var tile in options)
        {
            cumulative += tile.GetWeight();
            if (roll < cumulative)
            {
                options.Clear();
                options.Add(tile); // Keep only the chosen tile in the wave list
                return;
            }
        }
    }

    /// <summary>
    /// Propagates constraints from a collapsed tile to its neighbors
    /// </summary>
    /// <param name="wave">Wave grid</param>
    /// <param name="start">Starting tile (collapsed)</param>
    /// <param name="rows">Grid height</param>
    /// <param name="cols">Grid width</param>
    /// <param name="pq">Min priority queue storing entropy-index nodes</param>
    void Propagate(List<WFCTile>[,] wave, Vector2Int start, int rows, int cols, MinPriorityQueue pq)
    {
        bool[,] queued = new bool[rows, cols]; // Track tiles already enqueued
        Queue<Vector2Int> queue = new(); // BFS-style queue of postitions that need constraint propagation

        // Queue starting tile
        queue.Enqueue(start);
        queued[start.x, start.y] = true;

        // Queue starting tile's neighbors
        foreach (var dir in directions)
        {
            Vector2Int neighbor = start + dir;
            if (InBounds(neighbor, rows, cols) && !queued[neighbor.x, neighbor.y])
            {
                queue.Enqueue(neighbor);
                queued[neighbor.x, neighbor.y] = true;
            }
        }

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
                if (neighborOptions.Count < before && !queued[neighbor.x, neighbor.y])
                {
                    queued[neighbor.x, neighbor.y] = true;
                    queue.Enqueue(neighbor);

                    // Update priority queue with new entropy
                    int index = neighbor.x * cols + neighbor.y;
                    float entropy = CalculateEntropy(neighborOptions);
                    pq.Update(entropy, index);
                }
            }
        }
    }

    /// <summary>
    /// Calculates the Shannon entropy of a tile's remaining options
    /// </summary>
    /// <param name="options">List of WFCTiles representing current valid options at a tile</param>
    /// <returns>Shannon entropy (in bits) of the tile distribution</returns>
    float CalculateEntropy(List<WFCTile> options)
    {
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

        return entropy;
    }


    /// <summary>
    /// Checks whether any tile in the given list is compatible with a target tile from a given direction. Compatibility is mutual.
    /// </summary>
    /// <param name="neighborTile">Tile to test for compatibility</param>
    /// <param name="directionToCurrent">Direction to the current tile</param>
    /// <param name="possibleCurrentTiles">The current tile's list of options</param>
    /// <returns>True if any are compatible</returns>
    bool IsCompatibleWithAny(WFCTile neighbor, Vector2Int directionToCurrent, List<WFCTile> possibleCurrentTiles)
    {
        foreach (WFCTile possible in possibleCurrentTiles)
        {
            // Check if the possible tile accepts the neighbor
            bool forward = GetCompatibleNeighbors(possible, directionToCurrent).Contains(neighbor);

            // Check if the neighbor accepts the possible tile (reverse direction)
            bool reverse = GetCompatibleNeighbors(neighbor, -directionToCurrent).Contains(possible);

            if (forward && reverse)
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
