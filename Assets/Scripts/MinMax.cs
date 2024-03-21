using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{
    const int AITurnIndex = 1;

    public static (float, Tuple<Vector2, Vector2>) getScore(
        Board currentBoardState,
        int currentDepth,
        float alphaMax,
        float betaMin,
        int currentTurnIndex)
    {
        float bestScore;
        Tuple<Vector2, Vector2> bestLine = null;

        // Base case when a leaf node is reached
        // return the AI score - human score
        if (currentDepth == 0 ||
            currentBoardState.AvailableLines.Count == 0)
        {
            return (
                currentBoardState.Score[AITurnIndex] -
                currentBoardState.Score[AITurnIndex - 1],
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

        HashSet<Tuple<Vector2, Vector2>> availableLines =
            currentBoardState.AvailableLines;

        foreach (var line in availableLines)
        {
            //// Use copy constructor to create a new board for
            //// minmax score calculation
            Board nextBoardState = new Board(currentBoardState);
            int nextTurnIndex = nextBoardState.MakeMove(line, currentTurnIndex, false);

            (float nextMoveScore, var _) = getScore(
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
                    bestLine = line;
                }
                alphaMax = Math.Max(alphaMax, nextMoveScore);
            }
            else
            {
                if (nextMoveScore < bestScore)
                {
                    bestScore = nextMoveScore;
                    bestLine = line;
                }
                betaMin = Math.Min(betaMin, nextMoveScore);
            }

            // Alpha beta pruning
            if (betaMin <= alphaMax)
                break;
        }

        // Reach here after evaluating all leaf nodes of 
        // a root node
        return (bestScore, bestLine);
    }
}
