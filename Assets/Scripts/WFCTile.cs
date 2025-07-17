using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCTile
{
    public Sprite sprite;
    public WFCType type, up, right, down, left;

    public override string ToString()
    {
        return $"{type} (U:{up}, R:{right}, D:{down}, L:{left})";
    }
}
