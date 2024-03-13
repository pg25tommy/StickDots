using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    // Num of dots on verticle axis
    public int height;
    // Num of dots on horizontal axis
    public int width;

    public float[] score = { 0, 0 };

    public Box[][] boxes;

    // HashSet for unique elements
    // (i.e. no repeating when adding the same element)
    public HashSet<Tuple<Vector2, Vector2>> availableLines =
        new HashSet<Tuple<Vector2, Vector2>>();
    public HashSet<Tuple<Vector2, Vector2>> connectedLines =
        new HashSet<Tuple<Vector2, Vector2>>();

    // All boxes where
    public Queue<Tuple<Vector2, Vector2>> lastLineForBoxesWithThreeConnections =
        new Queue<Tuple<Vector2, Vector2>>();

    public Board(int h, int w)
    {
        height = h;
        width = w;
        boxes = new Box[height - 1][];
        InitializeBoxes();
    }

    // Copy constructor
    // Need to copy board to make future moves in minmax algorithm
    public Board(Board originalBoardState)
    {
        height = originalBoardState.height;
        width = originalBoardState.width;
        score = new[] { originalBoardState.score[0], originalBoardState.score[1] };
        boxes = new Box[height - 1][];
        boxes = InitializeBoxes(originalBoardState.boxes);

        // Shallow copy of the hashsets
        availableLines =
            new HashSet<Tuple<Vector2, Vector2>>
            (originalBoardState.availableLines);
        connectedLines =
            new HashSet<Tuple<Vector2, Vector2>>
            (originalBoardState.connectedLines);

        // Shallow copy
        lastLineForBoxesWithThreeConnections.Clear();
        foreach (var line in originalBoardState.lastLineForBoxesWithThreeConnections)
        {
            lastLineForBoxesWithThreeConnections.Enqueue(line);
        }
    }

    public Box[][] InitializeBoxes()
    {
        for (int i = 0; i < height - 1; i++)
        {
            boxes[i] = new Box[width - 1];
            for (int j = 0; j < width - 1; j++)
            {
                boxes[i][j] = new Box(new Vector2(i, j));
                boxes[i][j].AddLinesToSet(availableLines);
            }
        }
        return boxes;
    }

    public Box[][] InitializeBoxes(Box[][] originalBoxes)
    {
        for (int i = 0; i < height - 1; i++)
        {
            boxes[i] = new Box[width - 1];
            for (int j = 0; j < width - 1; j++)
            {
                // Use copy constructor of box
                boxes[i][j] = new Box(originalBoxes[i][j]);
            }
        }
        return boxes;
    }

    public int MakeMove(
        // NOTE: Always declare line from top dot to bottom dot,
        // left dot to right dot
        Tuple<Vector2, Vector2> lineToConnect,
        int turnIndex)
    {
        if (connectedLines.Contains(lineToConnect))
            return -1;

        connectedLines.Add(lineToConnect);
        availableLines.Remove(lineToConnect);

        // Connect the line and get the number of connected line for each box
        // Note: 1 move will affect 2 boxes
        int[] numConnectedLines = CheckBothBoxConnections(true, turnIndex, lineToConnect);

        bool captured = CheckIfEitherBoxCaptured(numConnectedLines);

        if (captured)
            score[turnIndex] += 1;

        // Flip the index if the person making the move didn't capture a box
        if (!captured)
            turnIndex = 1 - turnIndex;

        return turnIndex;
    }

    public bool CheckIfEitherBoxCaptured(int[] numConnectedLines)
    {
        foreach (int connections in numConnectedLines)
        {
            if (connections == 4) return true;
        }
        return false;
    }

    public int[] CheckBothBoxConnections(bool toConnect, int turnIndex,
        Tuple<Vector2, Vector2> lineToConnect)
    {
        int[] numConnectionsEachBox = new int[2];
        int firstDotRow = (int)lineToConnect.Item1.x;
        int firstDotCol = (int)lineToConnect.Item1.y;

        bool isHorizontal = IsHorizontalLine(lineToConnect);
        int varToChange = (isHorizontal ? firstDotRow : firstDotCol);
        int index = 0;
        for (int i = varToChange; i >= varToChange - 1; i--)
        {

            if (i < 0 ||
                isHorizontal && i >= height - 1 ||
                !isHorizontal && i >= width - 1)
                continue;

            Box box = isHorizontal ? boxes[i][firstDotCol] : boxes[firstDotRow][i];

            if (toConnect)
                box.ConnectDots(lineToConnect, turnIndex);

            numConnectionsEachBox[index] = box.numConnectedLines;

            if (box.numConnectedLines == 3)
                AddLastLineToList(box);

            index++;
        }

        return numConnectionsEachBox;
    }

    public void AddLastLineToList(Box box)
    {
        foreach (KeyValuePair<Tuple<Vector2, Vector2>, bool> entry in
            box.lineConnectedDict)
        {
            // Add to list if the line is not connected
            if (!entry.Value)
                lastLineForBoxesWithThreeConnections.Enqueue(entry.Key);
        }
    }

    public bool IsHorizontalLine(Tuple<Vector2, Vector2> line)
    {
        // If the row index of first dot equals to the 
        // row index of the second dot, then horizontal line
        if (line.Item1.x == line.Item2.x)
            return true;
        return false;
    }
}
