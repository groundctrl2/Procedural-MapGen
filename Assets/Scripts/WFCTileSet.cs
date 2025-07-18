using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides a predefined set of WFCTile configurations representing valid tile types and edge compatibility
/// This static class acts as a tile palette used by the WFCSystem
/// </summary>
public class WFCTileSet
{
    /// <summary>
    /// Returns a predefined tile set containing combinations of grass, sand, and water tiles
    /// Each tile encodes the types of tiles allowed on its four sides (up, right, down, left)
    /// This serves as the input domain for the wave function collapse algorithm
    /// </summary>
    /// <returns>A list of WFCTile objects representing the available tiles and their edge constraints</returns>
    public static List<WFCTile> GetTileSet()
    {
        return new List<WFCTile> {
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
            // Grass: 3 Sand
            new WFCTile { type = WFCType.grass, up = WFCType.grass, right = WFCType.sand, down = WFCType.sand, left = WFCType.sand },
            new WFCTile { type = WFCType.grass, up = WFCType.sand, right = WFCType.grass, down = WFCType.sand, left = WFCType.sand },
            new WFCTile { type = WFCType.grass, up = WFCType.sand, right = WFCType.sand, down = WFCType.grass, left = WFCType.sand },
            new WFCTile { type = WFCType.grass, up = WFCType.sand, right = WFCType.sand, down = WFCType.sand, left = WFCType.grass },

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
    }
}
