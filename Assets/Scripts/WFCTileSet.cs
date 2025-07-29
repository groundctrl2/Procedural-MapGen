using System.Collections.Generic;

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
            new WFCTile { type = WFCType.grass, up = WFCType.water, right = WFCType.grass, down = WFCType.sand, left = WFCType.grass},

            // Sand: 
            new WFCTile { type = WFCType.sand, up = WFCType.grass, right = WFCType.sand, down = WFCType.water, left = WFCType.sand },

            // Water: 
            new WFCTile { type = WFCType.water, up = WFCType.sand, right = WFCType.water, down = WFCType.grass, left = WFCType.water },
        };
    }
}
