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
        // number of times to play each other so that the Test judges by which wins most of the time instead of judging by a fluke win by a lower Level
        private int RUNS_INT = 21;
        private int RUNS_ADV = 21;
        private int RUNS_EXP = 11;
        private int RUNS_ULT = 1;


        [TestMethod]
        public void IntVsBeg()
        {
            BlackVsWhiteRuns(new ComputerPlayer_Intermediate(false), new ComputerPlayer_Beginner(true), true, RUNS_INT);
        }

        [TestMethod]
        public void BegVsInt()
        {
            BlackVsWhiteRuns(new ComputerPlayer_Beginner(false), new ComputerPlayer_Intermediate(true), false, RUNS_INT);
        }
        
        [TestMethod]
        public void AdvVsInt()
        {
            BlackVsWhiteRuns(new ComputerPlayer_Advanced(false), new ComputerPlayer_Intermediate(true), true, RUNS_ADV);
        }

        [TestMethod]
        public void IntVsAdv()
        {
            BlackVsWhiteRuns(new ComputerPlayer_Intermediate(false), new ComputerPlayer_Advanced(true), false, RUNS_ADV);
        }

        [TestMethod]
        public void ExpVsAdv()
        {
            BlackVsWhiteRuns(new ComputerPlayer_Expert(false), new ComputerPlayer_Advanced(true), true, RUNS_EXP);
        }

        [TestMethod]
        public void AdvVsExp()
        {
            BlackVsWhiteRuns(new ComputerPlayer_Advanced(false), new ComputerPlayer_Expert(true), false, RUNS_EXP);
        }

        [TestMethod]
        public void UltVsAdv()
        {
            BlackVsWhiteRuns(new ComputerPlayer_Ultimate(false), new ComputerPlayer_Advanced(true), true, RUNS_ULT);
        }

        [TestMethod]
        public void AdvVsUlt()
        {
            BlackVsWhiteRuns(new ComputerPlayer_Advanced(false), new ComputerPlayer_Ultimate(true), false, RUNS_ULT);
        }

        [TestMethod]
        public void UltVsExp()
        {
            BlackVsWhiteRuns(new ComputerPlayer_Ultimate(false), new ComputerPlayer_Expert(true), true, RUNS_ULT);
        }

        [TestMethod]
        public void ExpVsUlt()
        {
            BlackVsWhiteRuns(new ComputerPlayer_Expert(false), new ComputerPlayer_Ultimate(true), false, RUNS_ULT);
        }

        /// <summary>
        /// run multiple Games of black vs white to tally how many times each wins
        /// </summary>
        /// <param name="black"></param>
        /// <param name="white"></param>
        /// <param name="blackShouldWin">whitch Color to Assert Win-Count for</param>
        public void BlackVsWhiteRuns(ComputerPlayer black, ComputerPlayer white, bool blackShouldWin, int runs)
        {
            // turn off LogDecisions for Computer vs Computer when doing multiple Runs so the Scores can be easily seen
            bool prevLogDecisions = ComputerPlayer.LogDecisions; // cache to reset LogDecisions for other Tests
            ComputerPlayer.LogDecisions = false; // comment this out to see Decisions for Computer vs Computer

            Debug.Print("Black={0} White={1}", black.LevelName, white.LevelName);

            // track Wins by each Color and also track wonByResults to log more useful information
            SortedDictionary<int, int> wonByResults = new SortedDictionary<int, int>(); // sort results by Pieces diff
            int blackWins = 0;
            int whiteWins = 0;
            for (int i = 0; i < runs; i++)
            {
                BoardState boardState = ComputerVsComputer(black, white);
                AddResult(wonByResults, boardState);
                
                // log the final boardState when a single run
                if (runs == 1)
                    Debug.Print(boardState.ToString()); // log final BoardState

                if (boardState.BlackCount > boardState.WhiteCount)
                {
                    //Debug.Print("{0} wins {1}-{2}", blackLevel, boardState.BlackCount, boardState.WhiteCount);
                    blackWins++;
                }
                else if (boardState.WhiteCount > boardState.BlackCount)
                {
                    //Debug.Print("{0} wins {1}-{2}", whiteLevel, boardState.WhiteCount, boardState.BlackCount);
                    whiteWins++;
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
                    Debug.Print("{0} won by {1} pieces {2} time(s)", white.LevelName, diff, wonByResults[diff]);
                else if (diff == 0)
                    Debug.Print("Tied {0} time(s)", wonByResults[diff]);
                else
                    Debug.Print("{0} won by {1} pieces {2} time(s)", black.LevelName, -diff, wonByResults[diff]);
            }
            Debug.Print("{0} {1} - {2} {3}", white.LevelName, whiteWins, black.LevelName, blackWins);

            // reset LogDecisions for other Tests
            ComputerPlayer.LogDecisions = prevLogDecisions;

            if (blackShouldWin)
                Assert.IsTrue(blackWins > whiteWins, "{0} ({1} wins) should have beaten {2} ({3} wins)", black.LevelName, blackWins, white.LevelName, whiteWins);
            else
                Assert.IsTrue(whiteWins > blackWins, "{0} ({1} wins) should have beaten {2} ({3} wins)", white.LevelName, whiteWins, black.LevelName, blackWins);
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
        /// <param name="black">Level for Black</param>
        /// <param name="white">Level for White</param>
        /// <returns>BoardState after end of Game</returns>
        public static BoardState ComputerVsComputer(ComputerPlayer black, ComputerPlayer white)
        {
            BoardState boardState = new BoardState();

            while (true) // repeat currentPlayer making a Move until endOfGame
            {
                // which Player's turn is it (boardState.WhitesTurn knows if a Player is skipped due to no Legal Moves)
                ComputerPlayer currentPlayer = boardState.WhitesTurn ? white : black;
                Coord? choice = currentPlayer.ChooseNextMove(boardState);
                if (choice != null)
                {
                    boardState.PlacePieceAndFlipPiecesAndChangeTurns(choice.Value);
                    //Debug.Print(boardState.ToString()); // log BoardState after every Move
                    //Debug.Print("Black={0} White={1}", boardState.BlackCount, boardState.WhiteCount);
                }

                // break when End Of Game
                if (boardState.endOfGame)
                {
                    // log BoardState after endOfGame
                    if (ComputerPlayer.LogDecisions)
                    {
                        Debug.Print("\n");
                        Debug.Print(boardState.ToString());
                        Debug.Print("Black={0} White={1}", boardState.BlackCount, boardState.WhiteCount);
                    }

                    return boardState;
                }
            }
        }
    }
}
