using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCRenderer : MonoBehaviour
{
    public GameObject parent;
    public WFCSystem wfcSystem;
    public GameObject tilePrefab; // prefab with SpriteRenderer attached
    public float tileSize = 1f; // world size of each tile

    public Sprite grassSprite;
    public Sprite sandSprite;
    public Sprite waterSprite;

    public int rows = 10;
    public int cols = 18;

    void Start()
    {
        //WFCTile[,] grid = GenerateGrid();

        WFCTile[,] grid = wfcSystem.RunWFC(rows, cols);
        Render(grid);
    }

    WFCTile[,] GenerateGrid()
    {
        WFCTile[,] grid = new WFCTile[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {

                float rand = Random.value;
                Sprite sprite;

                if (rand < 0.3f) sprite = grassSprite;
                else if (rand < 0.6f) sprite = sandSprite;
                else sprite = waterSprite;


                grid[r, c] = new WFCTile
                {
                    sprite = sprite,
                    type = WFCType.grass,
                    up = WFCType.grass,
                    right = WFCType.grass,
                    down = WFCType.grass,
                    left = WFCType.grass
                };

                //Debug.Log($"Collapsed to tile: {chosenTile}");
            }
        }

        return grid;
    }

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
                obj.transform.parent = parent.transform;

                // Set sprite
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                sr.sprite = tile.sprite;
            }
    }
}
