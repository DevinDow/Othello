using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Othello
{
    public abstract class ComputerPlayer_Recursive : ComputerPlayer
    {
        public static bool LogEachRecursiveTurn = false;
        public static bool LogRecursiveChoiceOptions = false;

        protected ComputerPlayer_Recursive(bool amIWhite) : base(amIWhite) { }

        /// <summary>
        /// does all the recusion of several Turns to return a Score for this BoardState's outlook
        /// </summary>
        /// <param name="boardState">the BoardState to find a Score for</param>
        /// <param name="turn">which recursive Turn Depth</param>
        /// <returns>Score after recursing several Turns</returns>
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
            List<Coord> bestChoices = new List<Coord>();

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
                if (LogEachRecursiveTurn) // Log computerChoice with its computerChoiceScore & computerBoardState
                    Debug.Print(" - {0} choice: #1=" +
                            LogChoice(boardState.WhitesTurn, computerChoice, computerChoiceScore, computerBoardState),
                            LevelName);

                // next Turn Depth = 2 (unless Turn Skipped due to no leagl Moves)
                int nextTurn = 2;
                if (computerBoardState.WhitesTurn == boardState.WhitesTurn) // Turn Skipped due to no legal moves
                {
                    if (LogEachRecursiveTurn)
                        Debug.Print("- SKIPPED Turn #2={0}", boardState.WhitesTurn ? 'W' : 'B');
                    nextTurn++; // depth should go down to same Player to compare equally
                }

                // find minMaxScoreAfterSeveralTurns by starting recursion
                int minMaxScoreAfterSeveralTurns = FindMinMaxScoreAfterSeveralTurns(computerBoardState, nextTurn);
                if (LogEachRecursiveTurn) // Log computerChoice's minMaxScoreAfterSeveralTurns
                    Debug.Print(" - {0} choice: #1=" +
                            LogChoice(boardState.WhitesTurn, computerChoice, computerChoiceScore) +
                            " minMaxScoreAfterSeveralTurns={1:+#;-#;+0}\n\n",
                            LevelName, minMaxScoreAfterSeveralTurns);

                // remember the list of bestComputerChoices that attain maxComputerScoreAfterSeveralTurns
                if (minMaxScoreAfterSeveralTurns > maxComputerScoreAfterSeveralTurns) // remember maxComputerScoreAfterSeveralTurns and start a new List of Moves that attain it
                {
                    maxComputerScoreAfterSeveralTurns = minMaxScoreAfterSeveralTurns;
                    bestChoices = new List<Coord>();
                }

                if (minMaxScoreAfterSeveralTurns >= maxComputerScoreAfterSeveralTurns) // add choice to bestComputerChoices
                {
                    if (!bestChoices.Contains(computerChoice))
                        bestChoices.Add(computerChoice);
                }
            }

            if (LogRecursiveChoiceOptions)
                Debug.Print("** {0} bestChoices count={1}, maxComputerScoreAfterSeveralTurns={2:+#;-#;+0}.  Choose the highest scoring Move.",
                        LevelName, bestChoices.Count, maxComputerScoreAfterSeveralTurns);

            // find finalComputerChoices from equal bestComputerChoices based on the one with best computerChoiceScore
            int maxComputerScore = -int.MaxValue;
            List<Coord> finalComputerChoices = new List<Coord>();
            foreach (Coord computerChoice in bestChoices)
            {
                BoardState computerBoardState = boardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogRecursiveChoiceOptions)
                    Debug.Print("{0} Top Computer choice: " +
                            LogChoice(computerBoardState.WhitesTurn, computerChoice, computerChoiceScore), // computerBoardState),
                            LevelName);
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

            if (LogRecursiveChoiceOptions)
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

        /// <summary>
        /// W->(1,1) resulting Score=+100
        /// resulting BoardState:Turn=B
        ///  W.......
        ///  WWB.....
        /// </summary>
        /// <param name="whitesTurn"></param>
        /// <param name="coord"></param>
        /// <param name="score"></param>
        /// <param name="boardState"></param>
        /// <returns>formatted string</returns>
        protected string LogChoice(bool whitesTurn, Coord coord, int score, BoardState boardState = null)
        {
            // W->(1,1) resulting Score=+100
            string s = string.Format("{0}->{1} Score={2:+#;-#;+0}",
                    whitesTurn ? 'W' : 'B', coord, score);

            // resulting BoardState: Turn=B
            //  W.......
            //  WWB.....
            if (boardState != null)
                s += string.Format("\nresulting BoardState:{0}",
                    boardState);

            return s;
        }


        /// <summary>
        /// returns a weighted value for a Coord
        /// Beginner values all Coords equally
        /// higher Levels weight Coord values by location as more valuable or dangerous
        /// </summary>
        /// <param name="coord">Coord to get weighted Score of</param>
        /// <returns>weighted Score of coord</returns>
        protected override int WeightedCoordValue(Coord coord, int emptyCount)
        {
            const int numEmptyToConsiderBoardMostlyFilled = 5;
            bool boardMostlyFilled = emptyCount <= numEmptyToConsiderBoardMostlyFilled;
            if (boardMostlyFilled)
                return 100; // after board is mostly full, Expert/Ultimate should just try to win the game

            // Higher Levels value Coords differently
            // Corners are highest, then Ends.
            // Coords before Corners & Ends are devalued since they lead to Opponent getting Corners & Ends.
            // And Coords before those are valuable since they lead to Opponent getting those devalued Coords.

            // My Recursive Algorithms perform worse when Negatives throw a wrench in comparing BoardStates, esp near the end of the Game.
            // 2000   2 200 200
            //    2   1   3   3
            //  200   3  50  30
            //  200   3  30  10
            switch (coord.x)
            {
                // Edge COLs
                case 1:
                case 8:
                    switch (coord.y)
                    {
                        case 1:
                        case 8:
                            return 2000; // Corner
                        case 2:
                        case 7:
                            return 2; // leads to Corner
                        default:
                            return 200; // Edge
                    }
                // COL before Edge
                case 2:
                case 7:
                    switch (coord.y)
                    {
                        case 1:
                        case 8:
                            return 2; // leads to Corner
                        case 2:
                        case 7:
                            return 1; // leads to Corner
                        default:
                            return 3; // leads to Edge
                    }
                // COL before COL before Edge
                case 3:
                case 6:
                    switch (coord.y)
                    {
                        case 1:
                        case 8:
                            return 200; // Edge
                        case 2:
                        case 7:
                            return 3; // leads to Edge
                        case 3:
                        case 6:
                            return 50; // leads to Opponent getting devalued Coord near Corner
                        default:
                            return 30; // leads to Opponent getting devalued Coord near Edge
                    }
                // middle COLs
                default:
                    switch (coord.y)
                    {
                        case 1:
                        case 8:
                            return 200; // Edge
                        case 2:
                        case 7:
                            return 3; // leads to Edge
                        case 3:
                        case 6:
                            return 30; // leads to Opponent getting devalued Coord near Edge
                        default:
                            return 10; // Middles
                    }
            }
        }
    }
}
