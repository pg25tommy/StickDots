using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Dot
{
    public Vector2 dotCoord;
    public List<Vector2> neighbours;

    public Dot(int row, int col)
    {
        dotCoord = new Vector2(row, col);
        neighbours = new List<Vector2>();

    }
}
