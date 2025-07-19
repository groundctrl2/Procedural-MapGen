using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Renders a grid of WFCTile objects in the Unity scene.
/// Calls WFCSystem to generate the tile data and instantiates GameObjects accordingly.
/// </summary>
public class WFCRenderer : MonoBehaviour
{
    public WFCSystem wfcSystem;
    public GameObject tilePrefab; // prefab with SpriteRenderer attached
    public float tileSize = 1f; // world size of each tile

    public Sprite grassSprite;
    public Sprite sandSprite;
    public Sprite waterSprite;

    public int rows = 25;
    public int cols = 50;

    /// <summary>
    /// Runs WFC generation and renders the resulting grid
    /// </summary>
    void Start()
    {
        WFCTile[,] grid = wfcSystem.RunWFC(rows, cols);
        Render(grid);
    }

    /// <summary>
    /// Renders a 2D array of WFCTile objects by instantiating tile prefabs and assigning sprites
    /// </summary>
    /// <param name="grid">2D array of WFCTile results from WFCSystem</param>
    public void Render(WFCTile[,] grid)
    {
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                WFCTile tile = grid[r, c];
                if (tile == null) continue;

                // Assign sprite
                switch (tile.type)
                {
                    case WFCType.grass:
                        tile.sprite = grassSprite;
                        break;
                    case WFCType.sand:
                        tile.sprite = sandSprite;
                        break;
                    case WFCType.water:
                        tile.sprite = waterSprite;
                        break;
                }

                // Spawn tile
                GameObject obj = Instantiate(tilePrefab);
                obj.transform.position = new Vector3(c * tileSize, -r * tileSize, 0); // negative y for top-down
                obj.transform.parent = this.transform;

                // Set sprite
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                sr.sprite = tile.sprite;
            }
    }
}
