using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    private Tuple<Vector2, Vector2> _lineCoords;
    private bool _connected;

    public Tuple<Vector2, Vector2> LineCoords => _lineCoords;
    public bool Connected 
    { 
        get { return _connected; } 
        set { _connected = value; }
    }

    public Line(Vector2 p1, Vector2 p2)
    {
        _lineCoords = Tuple.Create(p1, p2);
        _connected = false;
    }

    // Copy constructor of Line
    public Line(Line otherLine)
    {
        _lineCoords = otherLine._lineCoords;
        _connected = otherLine._connected;
    }
}
