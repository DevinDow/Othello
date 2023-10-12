using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Othello
{
    public abstract class ComputerPlayer_Recursive : ComputerPlayer_AdvancedWeighting
    {
        public static bool LogEachRecursiveTurn = false;

        protected ComputerPlayer_Recursive(bool amIWhite) : base(amIWhite) { }

        protected abstract int FindMinMaxScoreAfterSeveralTurns(BoardState boardState, int turn);

        /// <summary>
        /// loop through all of Computer's Legal Moves
        /// collect the ones that maximize Score after several Turns
        /// if multiple Choices tie after several Turns then choose the ones with the highest first-move Score
        /// </summary>
        /// <returns>list of equal Computer Choices</returns>
        protected override List<Coord> findBestChoices(BoardState boardState)
        {
            int maxComputerScoreAfterSeveralTurns = -int.MaxValue;
            List<Coord> bestComputerChoices = new List<Coord>();

            // try every Legal Move
            List<Coord> legalComputerMoves = boardState.LegalMoves();
            foreach (Coord computerChoice in legalComputerMoves)
            {
                if (LogEachRecursiveTurn) // re-Log initial BoardState before each legal Move
                    Debug.Print("initial BoardState:{0}", boardState);

                // Turn Depth = 1
                BoardState computerBoardState = boardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogEachRecursiveTurn) // Log computerBoardState & computerChoiceScore for computerChoice
                    Debug.Print(" - {0} choice: #1={1}->{2} resulting Board's Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                            LevelName, boardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);

                // next Turn Depth = 2 (unless Turn Skipped due to no leagl Moves)
                int nextTurn = 2;
                if (computerBoardState.WhitesTurn == boardState.WhitesTurn) // Turn Skipped due to no legal moves
                {
                    if (LogEachRecursiveTurn)
                        Debug.Print("- SKIPPED Turn #2={0}",
                                boardState.WhitesTurn ? 'W' : 'B');
                    nextTurn++; // depth should go down to same Player to compare equally
                }

                // find minMaxScoreAfterSeveralTurns by starting recursion
                int minMaxScoreAfterSeveralTurns = FindMinMaxScoreAfterSeveralTurns(computerBoardState, nextTurn);
                if (LogEachRecursiveTurn) // Log minMaxScoreAfterSeveralTurns for computerChoice
                    Debug.Print(" - {0} choice: #1={1}->{2} resulting Board's Score={3:+#;-#;+0} minMaxScoreAfterSeveralTurns={4}\n\n",
                            LevelName, boardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, minMaxScoreAfterSeveralTurns);

                // remember the list of bestComputerChoices that attain maxComputerScoreAfterSeveralTurns
                if (minMaxScoreAfterSeveralTurns > maxComputerScoreAfterSeveralTurns) // remember maxComputerScoreAfterSeveralTurns and start a new List of Moves that attain it
                {
                    maxComputerScoreAfterSeveralTurns = minMaxScoreAfterSeveralTurns;
                    bestComputerChoices = new List<Coord>();
                }

                if (minMaxScoreAfterSeveralTurns >= maxComputerScoreAfterSeveralTurns) // add choice to bestComputerChoices
                {
                    if (!bestComputerChoices.Contains(computerChoice))
                        bestComputerChoices.Add(computerChoice);
                }
            }

            if (LogEachRecursiveTurn)
                Debug.Print("** {0} bestComputerChoices count={1}, maxComputerScoreAfterSeveralTurns={2:+#;-#;+0}.  Choose the highest scoring Move.",
                        LevelName, bestComputerChoices.Count, maxComputerScoreAfterSeveralTurns);

            // find finalComputerChoices from equal bestComputerChoices based on the one with best computerChoiceScore
            int maxComputerScore = -int.MaxValue;
            List<Coord> finalComputerChoices = new List<Coord>();
            foreach (Coord computerChoice in bestComputerChoices)
            {
                BoardState computerBoardState = boardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogEachRecursiveTurn)
                    Debug.Print("{0} Top Computer choice: {1}->{2} resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                            LevelName, computerBoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);
                if (computerChoiceScore > maxComputerScore)
                {
                    maxComputerScore = computerChoiceScore;
                    finalComputerChoices = new List<Coord>();
                }
                if (computerChoiceScore >= maxComputerScore)
                {
                    finalComputerChoices.Add(computerChoice);
                }
            }

            if (LogDecisions)
                Debug.Print("finalComputerChoices count={0}, maxComputerScore={1} maxComputerScoreAfterSeveralTurns={2}",
                        finalComputerChoices.Count, maxComputerScore, maxComputerScoreAfterSeveralTurns);

            return finalComputerChoices;
        }

        /// <summary>
        /// End-of-Game Score should just be a comparison of Counts
        /// MULTIPLIER helps it fit in with & out-weigh other Scores
        /// </summary>
        /// <param name="boardState">the BoardState to calculate Score for</param>
        /// <returns>a high Score if won, a low negative Score if lost</returns>
        protected int ScoreEndOfGame(BoardState boardState)
        {
            int endOfGameScore;
            const int MULTIPLIER = 10000;
            if (AmIWhite)
                endOfGameScore = MULTIPLIER * (boardState.WhiteCount - boardState.BlackCount);
            else
                endOfGameScore = MULTIPLIER * (boardState.BlackCount - boardState.WhiteCount);

            return endOfGameScore;
        }
    }
}
