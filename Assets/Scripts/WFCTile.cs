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
        { WFCType.sand, 0.7f },
        { WFCType.water, 0.4f },
    };

    /// <summary>
    /// Returns the predefined weight associated with this tile's type
    /// </summary>
    public float GetWeight() => TypeWeights[type];

    public override bool Equals(object obj)
    {
        if (obj is not WFCTile other) return false;
        return type == other.type; // or some other unique identifier
    }

    public override int GetHashCode()
    {
        return type.GetHashCode(); // must match Equals
    }
}
