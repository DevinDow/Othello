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
        private const int RUNS = 21; // number of times to play each other so that Test judges by which wins most of the time instead of judging by a fluke win by a lower Level

        [TestMethod]
        public void BegVsInt()
        {
            int begWins, intWins;
            WhiteVsBlackRuns(LevelEnum.Beginner, LevelEnum.Intermediate, out begWins, out intWins);
            Assert.IsTrue(intWins > begWins);
        }

        [TestMethod]
        public void IntVsBeg()
        {
            int intWins, begWins;
            WhiteVsBlackRuns(LevelEnum.Intermediate, LevelEnum.Beginner, out intWins, out begWins);
            Assert.IsTrue(intWins > begWins);
        }

        [TestMethod]
        public void AdvVsInt()
        {
            int advWins, intWins;
            WhiteVsBlackRuns(LevelEnum.Advanced, LevelEnum.Intermediate, out advWins, out intWins);
            Assert.IsTrue(advWins > intWins);
        }

        [TestMethod]
        public void IntVsAdv()
        {
            int intWins, advWins;
            WhiteVsBlackRuns(LevelEnum.Intermediate, LevelEnum.Advanced, out intWins, out advWins);
            Assert.IsTrue(advWins > intWins);
        }

        [TestMethod]
        public void ExpVsAdv()
        {
            int expWins, advWins;
            WhiteVsBlackRuns(LevelEnum.Expert, LevelEnum.Advanced, out expWins, out advWins);
            Assert.IsTrue(expWins > advWins);
        }

        [TestMethod]
        public void AdvVsExp()
        {
            int advWins, expWins;
            WhiteVsBlackRuns(LevelEnum.Advanced, LevelEnum.Expert, out advWins, out expWins);
            Assert.IsTrue(expWins > advWins);
        }


        [TestMethod]
        public void UltVsExp()
        {
            int ultWins, expWins;
            WhiteVsBlackRuns(LevelEnum.Ultimate, LevelEnum.Expert, out ultWins, out expWins);
            Assert.IsTrue(ultWins > expWins);
        }

        [TestMethod]
        public void ExpVsUlt()
        {
            int expWins, ultWins;
            WhiteVsBlackRuns(LevelEnum.Expert, LevelEnum.Ultimate, out expWins, out ultWins);
            Assert.IsTrue(ultWins > expWins);
        }

        /// <summary>
        /// run multiple Games of whiteLevel vs blackLevel to tally how many times each wins
        /// </summary>
        /// <param name="whiteLevel">Level for White</param>
        /// <param name="blackLevel">Level for Black</param>
        /// <param name="whiteWins">returns how many times White won</param>
        /// <param name="blackWins">returns how many times Black won</param>
        public void WhiteVsBlackRuns(LevelEnum whiteLevel, LevelEnum blackLevel, out int whiteWins, out int blackWins)
        {
            // don't Log Decisions for Computer vs Computer, but reset it when done
            bool prevLogDecisions = ComputerPlayer.LogDecisions;
            ComputerPlayer.LogDecisions = false; // don't log every Decision, esp. Expert going DEPTH Responses for every Legal Move available

            Debug.Print("White={0} Black={1}", whiteLevel, blackLevel);

            SortedDictionary<int, int> results = new SortedDictionary<int, int>(); // sort results by Pieces diff
            whiteWins = blackWins = 0;
            for (int i = 0; i < RUNS; i++)
            {
                BoardState boardState = ComputerVsComputer(whiteLevel, blackLevel);
                AddResult(results, boardState);
                //Debug.Print(boardState.ToString()); // log final BoardState
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
            // print results
            foreach (int diff in results.Keys)
            {
                if (diff > 0)
                    Debug.Print("{0} won by {1} pieces {2} time(s)", whiteLevel, diff, results[diff]);
                else if (diff == 0)
                    Debug.Print("Tied {0} time(s)", results[diff]);
                else
                    Debug.Print("{0} won by {1} pieces {2} time(s)", blackLevel, -diff, results[diff]);
            }
            Debug.Print("{0} {1} - {2} {3}", whiteLevel, whiteWins, blackLevel, blackWins);

            // reset Log Decisions for other Tests
            ComputerPlayer.LogDecisions = prevLogDecisions;
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

                if (boardState.endOfGame)
                    return boardState;
            }
        }
    }
}
