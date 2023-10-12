using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Othello
{
    public class ComputerPlayer_Expert : ComputerPlayer_Recursive
    {
        private const int EXPERT_TURNS_DEPTH = 11;

        public static bool LogEachExpertTurn = false;
        public static bool LogEachExpertLegalMoveResponse = false;
        
        public ComputerPlayer_Expert(bool amIWhite) : base(amIWhite)
        {
            LevelName = "Expert";
        }

        override protected int FindMinMaxScoreAfterSeveralTurns(BoardState boardState, int turn)
        {
            return FindMinMaxScoreForHighestScoringMove(boardState, turn);
        }

        /// <summary>
        /// Recusively find optimal choice by finding highest/lowest Score possible on each Turn
        /// on My/Computer's Turn: maximize my Score
        /// on Opponent's Turn: assume Opponent will choose lowest Score for Me/Computer
        /// return the minMaxScore after several Turns/recusions
        /// </summary>
        /// <param name="boardState">BoardState for current Turn</param>
        /// <param name="turns">how many Turns to recursively try</param>
        /// <returns>minMaxScore after several Turns/recusions</returns>
        protected int FindMinMaxScoreForHighestScoringMove(BoardState boardState, int turn)
        {
            bool myTurn = boardState.WhitesTurn ^ !AmIWhite;
            int minMaxScore = myTurn ? -int.MaxValue : int.MaxValue;
            Coord minMaxResponse = new Coord();
            BoardState minMaxResponseBoardState = null;

            List<Coord> legalMoves = boardState.LegalMoves();
            if (legalMoves.Count == 0) // game over
            {
                return ScoreEndOfGame(boardState);
            }

            // find best/worst Score for every leagalMove response
            foreach (Coord response in legalMoves)
            {
                BoardState responseBoardState = boardState.Clone();
                responseBoardState.PlacePieceAndFlipPiecesAndChangeTurns(response);

                int responseScore;
                if (responseBoardState.endOfGame)
                {
                    responseScore = ScoreEndOfGame(responseBoardState);
                }
                else
                {
                    responseScore = ScoreBoard(responseBoardState);
                }
                // Log each legalMove response
                if (LogEachExpertTurn && LogEachExpertLegalMoveResponse)
                    Debug.Print("       - Expert LegalMove Response: #{0}=" +
                            LogChoice(boardState.WhitesTurn, response, responseScore, responseBoardState),
                            turn);

                if (myTurn)
                {
                    if (responseScore > minMaxScore) // my Turn goes for highest Score for me
                    {
                        minMaxScore = responseScore;
                        minMaxResponse = response;
                        minMaxResponseBoardState = responseBoardState;
                    }
                }
                else
                {
                    if (responseScore < minMaxScore) // opponent's Turn chooses lowest Score for me
                    {
                        minMaxScore = responseScore;
                        minMaxResponse = response;
                        minMaxResponseBoardState = responseBoardState;
                    }
                }
            }

            // Log the chosen minMaxResponse
            if (LogEachExpertTurn)
                Debug.Print("- Expert response #{0}=" + 
                        LogChoice(boardState.WhitesTurn, minMaxResponse, minMaxScore, minMaxResponseBoardState),
                        turn);

            if (turn >= EXPERT_TURNS_DEPTH)
                return minMaxScore;

            int nextTurn = turn + 1;
            if (minMaxResponseBoardState.WhitesTurn == boardState.WhitesTurn) // turn skipped due to no legal moves
            {
                if (LogEachExpertTurn)
                    Debug.Print("- SKIPPED Turn #{0}={1}", nextTurn, boardState.WhitesTurn ? 'W' : 'B');
                nextTurn++; // depth should go down to same Player to compare equally
            }

            // recurse to return resulting minMaxScore after levelsLeft more Turns
            return FindMinMaxScoreForHighestScoringMove(minMaxResponseBoardState, nextTurn);
        }
    }
}
