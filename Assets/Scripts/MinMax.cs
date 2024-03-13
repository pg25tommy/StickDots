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

        if (currentDepth == 0 ||
            currentBoardState.availableLines.Count == 0)
        {
            return (
                currentBoardState.score[AITurnIndex] -
                currentBoardState.score[AITurnIndex - 1],
                bestLine);
        }

        if (currentTurnIndex == AITurnIndex)
            bestScore = -100000;

        else
            bestScore = 100000;

        HashSet<Tuple<Vector2, Vector2>> availableLines =
            currentBoardState.availableLines;

        foreach (var line in availableLines)
        {
            //// Use copy constructor to create a new board for
            //// minmax score calculation
            Board nextBoardState = new Board(currentBoardState);
            int nextTurnIndex = nextBoardState.MakeMove(line, currentTurnIndex);

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

        return (bestScore, bestLine);
    }
}
