using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Board
{
    // Num of dots on verticle axis
    private int height;
    // Num of dots on horizontal axis
    private int width;

    private float[] score = { 0, 0 };

    private Box[][] boxes;

    // HashSet for unique elements
    // (i.e. no repeating when adding the same element)
    private HashSet<Tuple<Vector2, Vector2>> availableLines =
        new HashSet<Tuple<Vector2, Vector2>>();
    private HashSet<Tuple<Vector2, Vector2>> connectedLines =
        new HashSet<Tuple<Vector2, Vector2>>();

    // All boxes where 3 lines are already connected
    private Queue<Tuple<Vector2, Vector2>> lastLineForBoxesWithThreeConnections =
        new Queue<Tuple<Vector2, Vector2>>();

    public float[] Score => score;
    public HashSet<Tuple<Vector2, Vector2>> AvailableLines => availableLines;
    public HashSet<Tuple<Vector2, Vector2>> ConnectedLines => connectedLines;
    public Queue<Tuple<Vector2, Vector2>> LastLineForBoxesWithThreeConnections =>
        lastLineForBoxesWithThreeConnections;

    public Board(int h, int w)
    {
        height = h;
        width = w;
        boxes = new Box[width - 1][];
        InitializeBoxes();
    }

    // Copy constructor
    // Need to copy board to make future moves in minmax algorithm
    public Board(Board originalBoardState)
    {
        height = originalBoardState.height;
        width = originalBoardState.width;
        score = new[] { originalBoardState.score[0], originalBoardState.score[1] };
        boxes = new Box[width - 1][];
        boxes = InitializeBoxes(originalBoardState.boxes);

        // Deep copy of the hashsets
        availableLines = new HashSet<Tuple<Vector2, Vector2>>
            (originalBoardState.availableLines);
        connectedLines = new HashSet<Tuple<Vector2, Vector2>>
            (originalBoardState.connectedLines);

        // Deep copy of boxes w/ 3 lines connected
        lastLineForBoxesWithThreeConnections.Clear();
        foreach (var line in originalBoardState.lastLineForBoxesWithThreeConnections)
        {
            lastLineForBoxesWithThreeConnections.Enqueue(line);
        }
    }

    private Box[][] InitializeBoxes()
    {
        for (int i = 0; i < width - 1; i++)
        {
            boxes[i] = new Box[height - 1];
            for (int j = 0; j < height - 1; j++)
            {
                // Use lower left dot coordinate as box coordinate
                boxes[i][j] = new Box(new Vector2(i, j));
                AddAllLinesOfBoxToAvailableSet(boxes[i][j]);
            }
        }
        return boxes;
    }

    private Box[][] InitializeBoxes(Box[][] originalBoxes)
    {
        for (int i = 0; i < width - 1; i++)
        {
            boxes[i] = new Box[height - 1];
            for (int j = 0; j < height - 1; j++)
            {
                // Use copy constructor of box
                boxes[i][j] = new Box(originalBoxes[i][j]);
            }
        }
        return boxes;
    }

    private void AddAllLinesOfBoxToAvailableSet(Box box)
    {
        foreach (Line line in box.Lines)
        {
            availableLines.Add(line.LineCoords);
        }
    }

    public int MakeMove(
        // NOTE: Always declare line from top dot to bottom dot,
        // left dot to right dot
        Tuple<Vector2, Vector2> lineToConnect,
        int turnIndex,
        bool playCaptureAnimIfCaptured)
    {
        if (connectedLines.Contains(lineToConnect))
            return -1;

        connectedLines.Add(lineToConnect);
        availableLines.Remove(lineToConnect);

        // Connect the line and get the number of connected line for each box
        // Note: 1 move will affect 2 boxes
        int[] numConnectedLines = CheckBothBoxConnections(
            true, turnIndex, lineToConnect, playCaptureAnimIfCaptured);

        bool captured = CheckIfEitherBoxCaptured(numConnectedLines);

        if (captured)
            score[turnIndex] += 1;

        // Flip the index if the person making the move didn't capture a box
        if (!captured)
            turnIndex = 1 - turnIndex;

        return turnIndex;
    }

    private bool CheckIfEitherBoxCaptured(int[] numConnectedLines)
    {
        foreach (int connections in numConnectedLines)
        {
            if (connections == 4) return true;
        }
        return false;
    }

    public int[] CheckBothBoxConnections(bool toConnect, int turnIndex,
        Tuple<Vector2, Vector2> lineToConnect, bool toPlayCaptureAnimIfCaptured)
    {
        int[] numConnectionsEachBox = new int[2];
        int firstDotX = (int)lineToConnect.Item1.x;
        int firstDotY = (int)lineToConnect.Item1.y;

        bool isHorizontal = IsHorizontalLine(lineToConnect);

        // Iterate x (i.e. left & right boxes that share line) if line is vertical
        // iterate y (i.e. top & bottom boxesthat share line) if line is horizontal
        int varToChange = (isHorizontal ? firstDotY : firstDotX);
        int index = 0;
        for (int i = varToChange; i >= varToChange - 1; i--)
        {

            if (i < 0 ||
                isHorizontal && i >= height - 1 ||
                !isHorizontal && i >= width - 1)
                continue;

            // Check left and right boxes if line is vertical
            // top and bottom boxes if line is horizontal
            Box box = isHorizontal ? boxes[firstDotX][i] : boxes[i][firstDotY];

            if (toConnect)
                box.ConnectDots(lineToConnect);

            numConnectionsEachBox[index] = box.NumConnectedLines;

            if (box.NumConnectedLines == 3)
                AddLastLineToQueue(box);

            // If capture box this round
            // Note that if this function is called from MinMax,
            // we don't want to play animation
            else if (
                toPlayCaptureAnimIfCaptured &&
                box.NumConnectedLines == 4 && 
                box.CapturedBy == -1)
            {
                box.CapturedBy = turnIndex;
                Vector3 boxCoordAndCapturedBy = isHorizontal ?
                    new Vector3(firstDotX, i, turnIndex) :
                    new Vector3(i, firstDotY, turnIndex);
                GameManager.Instance.CaptureBox(boxCoordAndCapturedBy);
            }

            index++;
        }

        return numConnectionsEachBox;
    }

    private void AddLastLineToQueue(Box box)
    {
        // box already has 3 connected lines, get the one that is not connected
        foreach (Line line in box.Lines)
        {
            // Add to list if the line is not connected
            if (!line.Connected)
                lastLineForBoxesWithThreeConnections.Enqueue(line.LineCoords);
        }
    }

    private bool IsHorizontalLine(Tuple<Vector2, Vector2> line)
    {
        // If the x coord of first dot equals to  
        // x coord of the second dot, then vertical line
        if (line.Item1.x == line.Item2.x)
            return false;
        return true;
    }
}
