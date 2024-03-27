using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MiniMaxBitboard
{
    const int AITurnIndex = 1;

    public static (float, int, int) GetScore(
        Bitboard currentBoardState,
        int currentDepth,
        float alphaMax,
        float betaMin,
        int currentTurnIndex)
    {
        float bestScore;
        int bestBox = -1;
        int bestLine = -1;

        // Base case when a leaf node is reached
        // return the AI score - human score
        if (currentDepth == 0 ||
            currentBoardState.numAvailableLines == 0)
        {
            return (
                currentBoardState.Score[AITurnIndex] -
                currentBoardState.Score[AITurnIndex - 1],
                bestBox,
                bestLine);
        }

        // Depends on whos turn, initialize best score
        // Since we want to get max when AI's turn,
        // initialize bestscore to negative large number
        // and vice versa
        if (currentTurnIndex == AITurnIndex)
            bestScore = -100000;

        else
            bestScore = 100000;

        Bitboard nextBoardState;
        for (int i = 0; i < currentBoardState.Boxes.Length; i++)
        {
            // iterateRange is to prevent us from going through the same line multiple times
            Vector2 iterateRange;
            // Check all 4 lines if first box
            if (i == 0) iterateRange = new Vector2(0, 3);

            // Check top, right, bottom lines if on first row
            else if (i < currentBoardState.Width) iterateRange = new Vector2(1, 3);

            // Check left, top, right lines if on first column
            else if (i % (currentBoardState.Width - 1) == 0) iterateRange = new Vector2(0, 2);

            // Check top and right lines otherwise
            else iterateRange = new Vector2(1, 2);

            for (int j = (int) iterateRange.x; j < (int) iterateRange.y; j++)
            {

                if (currentBoardState.IsLineConnected(i, j)) continue;

                //// Use copy constructor to create a new board for
                //// minmax score calculation
                nextBoardState = new Bitboard(currentBoardState);
                int nextTurnIndex = nextBoardState.MakeMove(i, j, currentTurnIndex, false);

                (float nextMoveScore, int _, int _) = GetScore(
                    nextBoardState,
                    currentDepth - 1,
                    alphaMax,
                    betaMin,
                    nextTurnIndex);

                // If AI's turn, get the highest score,
                // if human's turn get the lowest score
                // i.e. Assume human makes the best move
                // in order to calculate AI's best move
                if (currentTurnIndex == AITurnIndex)
                {
                    if (nextMoveScore > bestScore)
                    {
                        bestScore = nextMoveScore;
                        bestBox = i;
                        bestLine = j;
                    }
                    alphaMax = Math.Max(alphaMax, nextMoveScore);
                }
                else
                {
                    if (nextMoveScore < bestScore)
                    {
                        bestScore = nextMoveScore;
                        bestBox = i;
                        bestLine = j;
                    }
                    betaMin = Math.Min(betaMin, nextMoveScore);
                }

                // Alpha beta pruning
                if (betaMin <= alphaMax)
                    break;
            }

        }
        // Reach here after evaluating all leaf nodes of 
        // a root node
        return (bestScore, bestBox, bestLine);
    }
}
