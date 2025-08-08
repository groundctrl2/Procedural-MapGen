using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single tile used in the Wave Function Collapse algorithm
/// </summary>
public class WFCTile
{
    public Sprite sprite;
    public WFCType type, up, right, down, left;

    // Static weight map for each WFCType
    public static readonly Dictionary<WFCType, float> TypeWeights = new()
    {
        { WFCType.grass, 1.0f },
        { WFCType.sand, 0.4f },
        { WFCType.water, 0.1f },
    };

    /// <summary>
    /// Returns the predefined weight associated with this tile's type
    /// </summary>
    public float GetWeight() => TypeWeights[type];

    /// <summary>
    /// Determines whether the specified object is equal to the current tile.
    /// Two tiles are considered equal if they have the same type and the same edge values (up, right, down, left).
    /// </summary>
    /// <param name="obj">The object to compare with the current tile.</param>
    /// <returns>
    /// True if the specified object is a WFCTile with matching type and edge values; otherwise, false.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (obj is WFCTile other)
        {
            return type == other.type &&
                   up == other.up &&
                   right == other.right &&
                   down == other.down &&
                   left == other.left;
        }
        return false;
    }

    /// <summary>
    /// Returns a hash code for the current tile.
    /// The hash code is based on the tile's type and edge values, ensuring that each unique configuration has a unique hash.
    /// </summary>
    /// <returns>
    /// A hash code that uniquely represents this tile's configuration.
    /// </returns>
    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 31 + type.GetHashCode();
        hash = hash * 31 + up.GetHashCode();
        hash = hash * 31 + right.GetHashCode();
        hash = hash * 31 + down.GetHashCode();
        hash = hash * 31 + left.GetHashCode();
        return hash;
    }

}
