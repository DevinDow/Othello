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
        [TestMethod]
        public void BegVsInt()
        {
            BoardState boardState = ComputerVsComputer(LevelEnum.Beginner, LevelEnum.Intermediate);
            Assert.IsTrue(boardState.BlackCount > boardState.WhiteCount);
        }

        [TestMethod]
        public void IntVsBeg()
        {
            BoardState boardState = ComputerVsComputer(LevelEnum.Intermediate, LevelEnum.Beginner);
            Assert.IsTrue(boardState.WhiteCount > boardState.BlackCount);
        }

        [TestMethod]
        public void AdvVsInt()
        {
            BoardState boardState = ComputerVsComputer(LevelEnum.Advanced, LevelEnum.Intermediate);
            Assert.IsTrue(boardState.WhiteCount > boardState.BlackCount);
        }

        [TestMethod]
        public void IntVsAdv()
        {
            BoardState boardState = ComputerVsComputer(LevelEnum.Intermediate, LevelEnum.Advanced);
            Assert.IsTrue(boardState.BlackCount > boardState.WhiteCount);
        }

        public BoardState ComputerVsComputer(LevelEnum whiteLevel, LevelEnum blackLevel)
        { 
            ComputerPlayer.LogDecisions = false;
            BoardState boardState = new BoardState();
            ComputerPlayer white = new ComputerPlayer(whiteLevel, true);
            ComputerPlayer black = new ComputerPlayer(blackLevel, false);
            Debug.Print("White={0} Black={1}", whiteLevel, blackLevel);

            while (true)
            {
                ComputerPlayer currentPlayer = boardState.WhitesTurn ? white : black;
                currentPlayer.BoardState = boardState;
                Coord? choice = currentPlayer.ChooseNextMove();
                if (choice != null)
                {
                    boardState.PlacePieceAndFlipPiecesAndChangeTurns(choice.Value);
                    Debug.Print(boardState.ToString());
                    Debug.Print("White={0} Black={1}", boardState.WhiteCount, boardState.BlackCount);
                }

                if (boardState.endOfGame)
                    return boardState;
            }
        }
    }
}
