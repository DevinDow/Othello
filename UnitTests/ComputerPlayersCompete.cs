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
        private const int RUNS = 20;

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

        public void WhiteVsBlackRuns(LevelEnum whiteLevel, LevelEnum blackLevel, out int whiteWins, out int blackWins)
        {
            whiteWins = blackWins = 0;
            for (int i = 0; i < RUNS; i++)
            {
                BoardState boardState = ComputerVsComputer(whiteLevel, blackLevel);
                if (boardState.WhiteCount > boardState.BlackCount)
                    whiteWins++;
                if (boardState.BlackCount > boardState.WhiteCount)
                    blackWins++;
            }
            Debug.Print("{0} {1} - {2} {3}", whiteLevel, whiteWins, blackLevel, blackWins);
        }

        public BoardState ComputerVsComputer(LevelEnum whiteLevel, LevelEnum blackLevel)
        { 
            ComputerPlayer.LogDecisions = false;
            BoardState boardState = new BoardState();
            ComputerPlayer white = new ComputerPlayer(whiteLevel, true);
            ComputerPlayer black = new ComputerPlayer(blackLevel, false);
            //Debug.Print("White={0} Black={1}", whiteLevel, blackLevel);

            while (true)
            {
                ComputerPlayer currentPlayer = boardState.WhitesTurn ? white : black;
                currentPlayer.BoardState = boardState;
                Coord? choice = currentPlayer.ChooseNextMove();
                if (choice != null)
                {
                    boardState.PlacePieceAndFlipPiecesAndChangeTurns(choice.Value);
                    //Debug.Print(boardState.ToString());
                    //Debug.Print("White={0} Black={1}", boardState.WhiteCount, boardState.BlackCount);
                }

                if (boardState.endOfGame)
                {
                    if (boardState.WhiteCount > boardState.BlackCount)
                        Debug.Print("{0} wins {1}-{2}", whiteLevel, boardState.WhiteCount, boardState.BlackCount);
                    else if (boardState.BlackCount > boardState.WhiteCount)
                        Debug.Print("{0} wins {1}-{2}", blackLevel, boardState.BlackCount, boardState.WhiteCount);
                    else
                        Debug.Print("TIE {0}-{1}", boardState.WhiteCount, boardState.BlackCount);
                    return boardState;
                }
            }
        }
    }
}
