using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    public Tuple<Vector2, Vector2> lineCoords;
    public bool connected;

    public Line(Vector2 p1, Vector2 p2)
    {
        lineCoords = Tuple.Create(p1, p2);
        connected = false;
    }

    // Copy constructor of Line
    public Line(Line otherLine)
    {
        lineCoords = otherLine.lineCoords;
        connected = otherLine.connected;
    }
}
