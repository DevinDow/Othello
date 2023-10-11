﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Othello
{
    public class ComputerPlayer_Ultimate : ComputerPlayer_Expert
    {
        private const int ULTIMATE_TURNS_DEPTH = 11;
        private const int ULTIMATE_TURNS_DEPTH_TO_START_USING_EXPERT = 6; // Ultimate recurses for every Legal Move, but that is excessively slow and less critical at deeper Depths

        public static bool LogEachUltimateTurn = false;
        public static bool LogEachUltimateLegalMoveResponse = false;
        
        public ComputerPlayer_Ultimate(bool amIWhite) : base(amIWhite)
        {
            LevelName = "Ultimate";
        }

        override protected int FindMinMaxScoreAfterSeveralTurns(BoardState boardState, int turn)
        {
            return FindMinMaxScoreForHighestScoringMove(boardState, turn);
        }

        /// <summary>
        /// Recusively find optimal choice by trying every LegalMove for Computer Turn and lowest Score possible for each Opponent Turn
        /// on My/Computer's Turn: recurse for every LegalMove
        /// on Opponent's Turn: assume Opponent will choose lowest Score for Me/Computer
        /// return the minMaxScore after several Turns/recusions
        /// </summary>
        /// <param name="boardState">BoardState for current Turn</param>
        /// <param name="turns">how many Turns to recursively try</param>
        /// <returns>minMaxScore after several Turns/recusions</returns>
        private int FindMinMaxScoreForAllMyLegalMoves(BoardState boardState, int turn)
        {
            bool myTurn = boardState.WhitesTurn ^ !AmIWhite;
            int nextTurn = turn + 1;
            List<Coord> legalMoves = boardState.LegalMoves();
            if (legalMoves.Count == 0)
                return ScoreEndOfGame(boardState);

            if (myTurn) // find recursive Score for each legalMove
            {
                int maxRecursiveScore = -int.MaxValue;
                Coord maxRecursiveResponse;
                BoardState maxRecursiveResponseBoardState;
                foreach (Coord legalMove in legalMoves)
                {
                    BoardState legalMoveBoardState = boardState.Clone();
                    legalMoveBoardState.PlacePieceAndFlipPiecesAndChangeTurns(legalMove);

                    int recusiveScore;
                    if (legalMoveBoardState.endOfGame) // return ScoreEndOfGame()
                        recusiveScore = ScoreEndOfGame(legalMoveBoardState);
                    /*else if (turn >= ULTIMATE_TURNS_DEPTH) // return ScoreBoard()
                        recusiveScore = ScoreBoard(legalMoveBoardState);*/
                    else // recurse
                    {
                        if (LogEachUltimateTurn)
                            Debug.Print("       - Ultimate LegalMove: {0}={1}->{2}\nresulting BoardState:{3}",
                                    turn, boardState.WhitesTurn ? 'W' : 'B', legalMove, legalMoveBoardState);

                        if (legalMoveBoardState.WhitesTurn == boardState.WhitesTurn) // turn skipped due to no legal moves
                            nextTurn++; // depth should go down to same Player to compare equally

                        /*if (nextTurn > ULTIMATE_TURNS_DEPTH)
                            recusiveScore = ScoreBoard(legalMoveBoardState);
                        else // recurse to return resulting minMaxScore after nextTurn*/
                        {
                            if (nextTurn < ULTIMATE_TURNS_DEPTH_TO_START_USING_EXPERT)
                                recusiveScore = FindMinMaxScoreForAllMyLegalMoves(legalMoveBoardState, nextTurn);
                            else
                                recusiveScore = FindMinMaxScoreForHighestScoringMove(legalMoveBoardState, nextTurn);
                        }
                    }

                    // Log each legalMove
                    if (LogEachUltimateTurn)
                        Debug.Print("- Ultimate LegalMove: {0}={1}->{2} recusiveScore={3:+#;-#;+0}\nlegalMoveBoardState:{4}",
                                turn, boardState.WhitesTurn ? 'W' : 'B', legalMove, recusiveScore, legalMoveBoardState);

                    if (recusiveScore > maxRecursiveScore) // is this ultimately the best resulting recusiveScore to bubble-up?
                    {
                        maxRecursiveScore = recusiveScore;
                        maxRecursiveResponse = legalMove;
                        maxRecursiveResponseBoardState = legalMoveBoardState;
                    }
                }
                return maxRecursiveScore; // ultimately bubble-up the maxRecursiveScore until we figure out which first Move results in the best eventual Board Score
            }

            else // Opponent's Turn
            {
                // find best Opponent Response using min Board Score
                int minRecursiveScore = int.MaxValue;
                Coord minRecursiveResponse = new Coord();
                BoardState minRecursiveResponseBoardState = null;
                foreach (Coord legalMove in legalMoves)
                {
                    BoardState legalMoveBoardState = boardState.Clone();
                    legalMoveBoardState.PlacePieceAndFlipPiecesAndChangeTurns(legalMove);

                    int responseScore;
                    if (legalMoveBoardState.endOfGame)
                        responseScore = ScoreEndOfGame(legalMoveBoardState);
                    else
                        responseScore = ScoreBoard(legalMoveBoardState);

                    // Log each legalMove response
                    if (LogEachUltimateTurn && LogEachUltimateLegalMoveResponse)
                        Debug.Print("       - Ultimate Opponent LegalMove: {0}={1}->{2} resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                                turn, boardState.WhitesTurn ? 'W' : 'B', legalMove, responseScore, legalMoveBoardState);

                    if (responseScore < minRecursiveScore) // opponent's Turn chooses lowest Score for me
                    {
                        minRecursiveScore = responseScore;
                        minRecursiveResponse = legalMove;
                        minRecursiveResponseBoardState = legalMoveBoardState;
                    }
                }

                // Log the chosen minMaxResponse
                if (LogEachUltimateTurn)
                    Debug.Print("- Ultimate Opponent's best Response {0}={1}->{2}: resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                            turn, boardState.WhitesTurn ? 'W' : 'B', minRecursiveResponse, minRecursiveScore, minRecursiveResponseBoardState);

                if (minRecursiveResponseBoardState.endOfGame)
                    return minRecursiveScore;

                /*if (turn >= ULTIMATE_TURNS_DEPTH)
                    return minScore;*/

                if (minRecursiveResponseBoardState.WhitesTurn == boardState.WhitesTurn) // turn skipped due to no legal moves
                    nextTurn++; // depth should go down to same Player to compare equally

                // recurse to return resulting minMaxScore after nextTurn
                if (nextTurn < ULTIMATE_TURNS_DEPTH_TO_START_USING_EXPERT)
                    return FindMinMaxScoreForAllMyLegalMoves(minRecursiveResponseBoardState, nextTurn);
                else
                    return FindMinMaxScoreForHighestScoringMove(minRecursiveResponseBoardState, nextTurn);
            }
        }
    }
}