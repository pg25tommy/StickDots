
using System.Security.Cryptography;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

enum BitIndex
{
    Left        = 0,
    Top         = 1,
    Right       = 2,
    Bottom      = 3,
    Captured    = 4,
    CapturedBy  = 5,
}

public class Bitboard
{
    // Num of dots on verticle axis
    private int height;
    // Num of dots on horizontal axis
    private int width;

    private int totalBoxes;

    private float[] score = { 0, 0 };

    // Each uint represents a box
    // Least Significant Bit (bit 0) is the left line
    // Bit 1 is the top line
    // Bit 2 is the right line
    // Bit 3 is the bottom line
    // Bit 4 is if the box is captured
    // Bit 5 is who captured the box
    private byte[] boxes;

    public int numAvailableLines = 0;

    public float[] Score => score;
    public int Height => height;
    public int Width => width;
    public byte[] Boxes => boxes;

    public Bitboard(int h, int w)
    {
        height = h;
        width = w;
        totalBoxes = (height - 1) * (width - 1);
        // E.g. 4dots x 4dots grid has 9 boxes
        boxes = new byte[totalBoxes];
    }

    // Copy constructor
    // Need to copy board to make future moves in minmax algorithm
    public Bitboard(Bitboard originalBoardState)
    {
        height = originalBoardState.height;
        width = originalBoardState.width;
        totalBoxes = originalBoardState.totalBoxes;
        score = new[] { originalBoardState.score[0], originalBoardState.score[1] };

        boxes = new byte[totalBoxes];
        for (int i = 0; i < totalBoxes; i++)
        {
            boxes[i] = originalBoardState.boxes[i];
        }
    }

    public void ConnectLine(int boxIndex, int lineIndex)
    {
        // Set the bit
        boxes[boxIndex] |= (byte)(1 << lineIndex);

    }

    public bool IsLineConnected(int boxIndex, int lineIndex)
    {
        // Check if corresponding bit of box is set
        return (boxes[boxIndex] & (1 << lineIndex)) != 0;
    }

    public Vector2 GetBoxCoordFromBoxIndex(int boxIndex)
    {
        int lowerLeftY = boxIndex / (width - 1);
        int lowerLeftX = boxIndex - lowerLeftY * (width - 1);

        return new Vector2(lowerLeftX, lowerLeftY);
    }

    public (Vector2 p1, Vector2 p2) GetVectorsFromBoxAndLineIndex(
        int boxIndex, int lineIndex)
    {
        Vector2 lowerLeft = GetBoxCoordFromBoxIndex(boxIndex);

        switch (lineIndex)
        {
            case 0:
                return (
                    new Vector2(lowerLeft.x, lowerLeft.y), 
                    new Vector2(lowerLeft.x, lowerLeft.y + 1));
            case 1:
                return (
                    new Vector2(lowerLeft.x, lowerLeft.y + 1),
                    new Vector2(lowerLeft.x + 1, lowerLeft.y + 1));
            case 2:
                return (
                    new Vector2(lowerLeft.x + 1, lowerLeft.y),
                    new Vector2(lowerLeft.x + 1, lowerLeft.y + 1));
            case 3:
                return (
                    new Vector2(lowerLeft.x, lowerLeft.y),
                    new Vector2(lowerLeft.x + 1, lowerLeft.y));
            default:
                return (new Vector2(-1, -1), new Vector2(-1, -1));
        }
    }

    public int GetBoxIndexFromVectors(Vector2 p1, Vector2 p2)
    {
        int boxIndex = -1;
        // If vertical line
        if (p1.x == p2.x)
        {
            // Always return the left box since
            // GetOtherBoxAndLineIndex() will get the right box
            boxIndex = (int)(p1.x - 1 + p1.y * (width - 1));
        }
        // If horizontal line
        else
        {
            // Always return the bottom box since
            // GetOtherBoxAndLineIndex() will get the top box
            boxIndex = (int)(p1.x + (p1.y - 1) * (width - 1));
        }
        return boxIndex;
    }

    public (int, int) GetOtherBoxAndLineIndex(
        int firstBoxIndex, int firstLineIndex)
    {
        int secondBoxIndex = -1;
        // If horizontal get bottom line of the box above
        if (firstLineIndex == (int)BitIndex.Top || 
            firstLineIndex == (int)BitIndex.Bottom)
        {
            // if not out of bound
            if (firstBoxIndex + width - 1 < totalBoxes)
                secondBoxIndex = firstBoxIndex + width - 1;
            return (secondBoxIndex, (int) BitIndex.Bottom);
        }
        // If vertical get left line of the box to the right
        else
        {
            // if not out of bound
            if (firstBoxIndex + 1 < firstBoxIndex / (width - 1) + width - 1)
                secondBoxIndex = firstBoxIndex + 1;
            return (secondBoxIndex, (int) BitIndex.Left);
        }
    }

    public int MakeMove(int boxIndex, int lineIndex, 
        int turnIndex, bool playCaptureAnimIfCaptured)
    {
        bool capturedEitherBox = false;

        (int otherBoxIndex, int otherLineIndex) = 
            GetOtherBoxAndLineIndex(boxIndex, lineIndex);

        int[] boxIndices = new int[] {boxIndex, otherBoxIndex};
        int[] lineIndices = new int[] {lineIndex, otherLineIndex};
        for (int i = 0; i < 2; i++)
        {
            if (boxIndex == -1) continue;
            ConnectLine(boxIndices[i], lineIndices[i]);
            int numLinesConnected = GetBoxConnections(boxIndices[i]);

            // If box captured this round
            // i.e. all 4 lines connected but flag not yet set
            if (numLinesConnected == 4 && !IsBoxCaptured(boxIndices[i]))
            {
                SetCaptured(boxIndices[i], turnIndex, playCaptureAnimIfCaptured);
                score[turnIndex] += 1;
                capturedEitherBox = true;
            }
        }

        // Flip the index if the person making the move didn't capture a box
        if (!capturedEitherBox)
            turnIndex = 1 - turnIndex;

        return turnIndex;
    }

    private int GetBoxConnections(int boxIndex)
    {
        int numConnections = 0;
        for (int i = 0; i < 4; i++)
        {
            if ((boxes[boxIndex] & (1 << i)) != 0) 
                numConnections++;
        }
        return numConnections;
    }

    private void SetCaptured(int boxIndex, int turnIndex, 
        bool playCaptureAnimIfCaptured)
    {
        boxes[boxIndex] |= (1 << (int)BitIndex.Captured);
        if (playCaptureAnimIfCaptured)
        {
            Vector2 boxCoord = GetBoxCoordFromBoxIndex(boxIndex);
            Vector3 boxCoordAndCapturedBy = new Vector3(
                boxCoord.x, boxCoord.y, turnIndex);
            GameManagerBitboard.Instance.CaptureBox(boxCoordAndCapturedBy);
        }
    }

    private void SetBoxOwner(int boxIndex, int turnIndex)
    {
        boxes[boxIndex] |= (byte)(turnIndex << (int)BitIndex.CapturedBy);
    }

    private bool IsBoxCaptured(int boxIndex)
    {
        // Check if corresponding bit of box is set
        return (boxes[boxIndex] & (1 << (int) BitIndex.Captured)) != 0;
    }
}
