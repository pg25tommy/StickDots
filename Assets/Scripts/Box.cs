using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box
{
    public List<Line> lines = new List<Line>();

    // TODO: Use enum
    // -1: Not yet captured
    // 0: Captured by player
    // 1: Captured by AI
    public int capturedBy = -1;
    public int numConnectedLines = 0;

    public Box(Vector2 coordUppderLeft)
    {

        // Always declare line from top dot to bottom dot,
        // left dot to right dot

        // upper left to lower left
        lines.Add(new Line(
            new Vector2(coordUppderLeft.x, coordUppderLeft.y), 
            new Vector2(coordUppderLeft.x + 1, coordUppderLeft.y)));
        // upper left to upper right
        lines.Add(new Line(
            new Vector2(coordUppderLeft.x, coordUppderLeft.y),
            new Vector2(coordUppderLeft.x, coordUppderLeft.y + 1)));
        // upper right to lower right
        lines.Add(new Line(
            new Vector2(coordUppderLeft.x, coordUppderLeft.y + 1),
            new Vector2(coordUppderLeft.x + 1, coordUppderLeft.y + 1)));
        // lower left to lower right
        lines.Add(new Line(
            new Vector2(coordUppderLeft.x + 1, coordUppderLeft.y),
            new Vector2(coordUppderLeft.x + 1, coordUppderLeft.y + 1)));
    }

    public Box(Box otherBox)
    {
        
        lines = new List<Line>();
        foreach (Line line in otherBox.lines)
        {
            // Use copy constructor of Line
            lines.Add(new Line(line));
        }

        capturedBy = otherBox.capturedBy;
        numConnectedLines = otherBox.numConnectedLines;
    }

    //public void ConnectDots(Line lineToConnect)
    //{
    //    lineToConnect.connected = true;
    //    numConnectedLines += 1;
    //}

    public void ConnectDots(Tuple<Vector2, Vector2> lineToConnect)
    {
        foreach (Line line in lines)
        {
            if (line.lineCoords == lineToConnect)
            {
                line.connected = true;
            }
        }
    }
}
