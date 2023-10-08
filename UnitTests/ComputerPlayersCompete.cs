using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTests
{
    [TestClass]
    public class ComputerPlayersCompete
    {
        private int RUNS; // number of times to play each other so that Test judges by which wins most of the time instead of judging by a fluke win by a lower Level
        private int RUNS_INT = 21;
        private int RUNS_ADV = 21;
        private int RUNS_EXP = 11;
        private int RUNS_ULT = 1;


        [TestMethod]
        public void IntVsBeg()
        {
            RUNS = RUNS_INT;
            WhiteVsBlackRuns(LevelEnum.Intermediate, LevelEnum.Beginner, true);
        }

        [TestMethod]
        public void BegVsInt()
        {
            RUNS = RUNS_INT;
            WhiteVsBlackRuns(LevelEnum.Beginner, LevelEnum.Intermediate, false);
        }

        [TestMethod]
        public void AdvVsInt()
        {
            RUNS = RUNS_ADV;
            WhiteVsBlackRuns(LevelEnum.Advanced, LevelEnum.Intermediate, true);
        }

        [TestMethod]
        public void IntVsAdv()
        {
            RUNS = RUNS_ADV;
            WhiteVsBlackRuns(LevelEnum.Intermediate, LevelEnum.Advanced, false);
        }

        [TestMethod]
        public void ExpVsAdv()
        {
            RUNS = RUNS_EXP;
            WhiteVsBlackRuns(LevelEnum.Expert, LevelEnum.Advanced, true);
        }

        [TestMethod]
        public void AdvVsExp()
        {
            RUNS = RUNS_EXP;
            WhiteVsBlackRuns(LevelEnum.Advanced, LevelEnum.Expert, false);
        }


        [TestMethod]
        public void UltVsExp()
        {
            RUNS = RUNS_ULT;
            WhiteVsBlackRuns(LevelEnum.Ultimate, LevelEnum.Expert, true);
        }

        [TestMethod]
        public void ExpVsUlt()
        {
            RUNS = RUNS_ULT;
            WhiteVsBlackRuns(LevelEnum.Expert, LevelEnum.Ultimate, false);
        }

        /// <summary>
        /// run multiple Games of whiteLevel vs blackLevel to tally how many times each wins
        /// </summary>
        /// <param name="whiteLevel">Level for White</param>
        /// <param name="blackLevel">Level for Black</param>
        /// <param name="whiteShouldWin">whitch Color to Assert Win-Count for</param>
        public void WhiteVsBlackRuns(LevelEnum whiteLevel, LevelEnum blackLevel, bool whiteShouldWin)
        {
            // turn off LogDecisions for Computer vs Computer when doing multiple Runs so the Scores can be easily seen
            bool prevLogDecisions = ComputerPlayer.LogDecisions; // cache to reset LogDecisions for other Tests
            ComputerPlayer.LogDecisions = false; // comment this out to see Decisions for Computer vs Computer

            Debug.Print("White={0} Black={1}", whiteLevel, blackLevel);

            // track Wins by each Color and also track wonByResults to log more useful information
            SortedDictionary<int, int> wonByResults = new SortedDictionary<int, int>(); // sort results by Pieces diff
            int whiteWins = 0;
            int blackWins = 0;
            for (int i = 0; i < RUNS; i++)
            {
                BoardState boardState = ComputerVsComputer(whiteLevel, blackLevel);
                AddResult(wonByResults, boardState);
                
                // log the final boardState when a single run
                if (RUNS == 1)
                    Debug.Print(boardState.ToString()); // log final BoardState

                if (boardState.WhiteCount > boardState.BlackCount)
                {
                    //Debug.Print("{0} wins {1}-{2}", whiteLevel, boardState.WhiteCount, boardState.BlackCount);
                    whiteWins++;
                }
                else if (boardState.BlackCount > boardState.WhiteCount)
                {
                    //Debug.Print("{0} wins {1}-{2}", blackLevel, boardState.BlackCount, boardState.WhiteCount);
                    blackWins++;
                }
                else
                {
                    //Debug.Print("TIE {0}-{1}", boardState.WhiteCount, boardState.BlackCount);
                }
            }

            // log wonByResults
            foreach (int diff in wonByResults.Keys)
            {
                if (diff > 0)
                    Debug.Print("{0} won by {1} pieces {2} time(s)", whiteLevel, diff, wonByResults[diff]);
                else if (diff == 0)
                    Debug.Print("Tied {0} time(s)", wonByResults[diff]);
                else
                    Debug.Print("{0} won by {1} pieces {2} time(s)", blackLevel, -diff, wonByResults[diff]);
            }
            Debug.Print("{0} {1} - {2} {3}", whiteLevel, whiteWins, blackLevel, blackWins);

            // reset LogDecisions for other Tests
            ComputerPlayer.LogDecisions = prevLogDecisions;

            if (whiteShouldWin)
                Assert.IsTrue(whiteWins > blackWins, "{0} ({1} wins) should have beaten {2} ({3} wins)", whiteLevel, whiteWins, blackLevel, blackWins);
            else
                Assert.IsTrue(blackWins > whiteWins, "{0} ({1} wins) should have beaten {2} ({3} wins)", blackLevel, blackWins, whiteLevel, whiteWins);
        }

        private void AddResult(SortedDictionary<int, int> results, BoardState boardState)
        {
            int diff = boardState.WhiteCount - boardState.BlackCount;
            if (results.ContainsKey(diff))
                results[diff]++;
            else
                results[diff] = 1;
        }

        /// <summary>
        /// run a Game of whiteLevel vs blackLevel
        /// </summary>
        /// <param name="whiteLevel">Level for White</param>
        /// <param name="blackLevel">Level for Black</param>
        /// <returns>BoardState after end of Game</returns>
        public BoardState ComputerVsComputer(LevelEnum whiteLevel, LevelEnum blackLevel)
        {
            BoardState boardState = new BoardState();
            ComputerPlayer white = new ComputerPlayer(whiteLevel, true);
            ComputerPlayer black = new ComputerPlayer(blackLevel, false);

            while (true) // repeat currentPlayer making a Move until endOfGame
            {
                // which Player's turn is it (boardState.WhitesTurn knows if a Player is skipped due to no Legal Moves)
                ComputerPlayer currentPlayer = boardState.WhitesTurn ? white : black;
                currentPlayer.BoardState = boardState;
                Coord? choice = currentPlayer.ChooseNextMove();
                if (choice != null)
                {
                    boardState.PlacePieceAndFlipPiecesAndChangeTurns(choice.Value);
                    //Debug.Print(boardState.ToString()); // log BoardState after every Move
                    //Debug.Print("White={0} Black={1}", boardState.WhiteCount, boardState.BlackCount);
                }

                // break when End Of Game
                if (boardState.endOfGame)
                {
                    // log BoardState after endOfGame
                    if (ComputerPlayer.LogDecisions)
                    {
                        Debug.Print("\n");
                        Debug.Print(boardState.ToString());
                        Debug.Print("White={0} Black={1}", boardState.WhiteCount, boardState.BlackCount);
                    }

                    return boardState;
                }
            }
        }
    }
}
