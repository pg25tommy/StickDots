using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box
{
    // Coords for the 4 dots
    public Vector2 dotUpperLeft;
    public Vector2 dotUpperRight;
    public Vector2 dotBottomLeft;
    public Vector2 dotBottomRight;

    // The line is simply the coords of the two dots
    public Tuple<Vector2, Vector2> lineLeft;
    public Tuple<Vector2, Vector2> lineTop;
    public Tuple<Vector2, Vector2> lineRight;
    public Tuple<Vector2, Vector2> lineBottom;

    // Dictionary to check where the line is connected,
    // with tuple of tuples of ints as key (i.e. coords of two dots)
    // and bool as value (connected or not)
    public Dictionary<Tuple<Vector2, Vector2>, bool> lineConnectedDict =
        new Dictionary<Tuple<Vector2, Vector2>, bool>();

    // TODO: Use enum
    // -1: Not yet captured
    // 0: Captured by player
    // 1: Captured by AI
    public int capturedBy = -1;

    public int numConnectedLines = 0;

    public Box(Vector2 coordUppderLeft)
    {
        dotUpperLeft = coordUppderLeft;
        dotUpperRight = new Vector2(coordUppderLeft.x, coordUppderLeft.y + 1);
        dotBottomLeft = new Vector2(coordUppderLeft.x + 1, coordUppderLeft.y);
        dotBottomRight = new Vector2(coordUppderLeft.x + 1, coordUppderLeft.y + 1);

        // Always declare line from top dot to bottom dot,
        // left dot to right dot
        lineLeft = Tuple.Create(dotUpperLeft, dotBottomLeft);
        lineTop = Tuple.Create(dotUpperLeft, dotUpperRight);
        lineRight = Tuple.Create(dotUpperRight, dotBottomRight);
        lineBottom = Tuple.Create(dotBottomLeft, dotBottomRight);

        lineConnectedDict.Add(lineLeft, false);
        lineConnectedDict.Add(lineTop, false);
        lineConnectedDict.Add(lineRight, false);
        lineConnectedDict.Add(lineBottom, false);
    }

    public Box(Box otherBox)
    {
        dotUpperLeft = otherBox.dotUpperLeft;
        dotUpperRight = otherBox.dotUpperRight;
        dotBottomLeft = otherBox.dotBottomLeft;
        dotBottomRight = otherBox.dotBottomRight;

        // Always declare line from top dot to bottom dot,
        // left dot to right dot
        lineLeft = otherBox.lineLeft;
        lineTop = otherBox.lineTop;
        lineRight = otherBox.lineRight;
        lineBottom = otherBox.lineBottom;

        lineConnectedDict.Add(lineLeft, otherBox.lineConnectedDict[lineLeft]);
        lineConnectedDict.Add(lineTop, otherBox.lineConnectedDict[lineTop]);
        lineConnectedDict.Add(lineRight, otherBox.lineConnectedDict[lineRight]);
        lineConnectedDict.Add(lineBottom, otherBox.lineConnectedDict[lineBottom]);

        capturedBy = otherBox.capturedBy;
        numConnectedLines = otherBox.numConnectedLines;
    }

    public void AddLinesToSet(
        HashSet<Tuple<Vector2, Vector2>> availableLines)
    {
        availableLines.Add(lineLeft);
        availableLines.Add(lineTop);
        availableLines.Add(lineRight);
        availableLines.Add(lineBottom);
    }

    public void ConnectDots(
        Tuple<Vector2, Vector2> lineToConnect,
        int turnIndex)
    {
        lineConnectedDict[lineToConnect] = true;
        numConnectedLines += 1;
    }
}
