using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot: MonoBehaviour
{
    private Vector2 _dotCoord;

    public Vector2 DotCoord
    {
        get { return _dotCoord; }
        set { _dotCoord = value; }
    }
}
