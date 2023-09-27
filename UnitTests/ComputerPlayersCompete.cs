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
            ComputerPlayer.LogDecisions = false;
            BoardState boardState = new BoardState();
            ComputerPlayer blackBeg = new ComputerPlayer(LevelEnum.Beginner, false);
            ComputerPlayer whiteInt = new ComputerPlayer(LevelEnum.Intermediate, true);

            while (true)
            {
                ComputerPlayer currentPlayer = boardState.WhitesTurn ? whiteInt : blackBeg;
                currentPlayer.BoardState = boardState;
                Coord? choice = currentPlayer.ChooseNextMove();
                if (choice != null)
                {
                    boardState.PlacePieceAndFlipPiecesAndChangeTurns(choice.Value);
                    Debug.Print(boardState.ToString());
                    Debug.Print("White={0} Black={1}", boardState.WhiteCount, boardState.BlackCount);
                }

                if (boardState.endOfGame)
                    break;
            }

            Assert.IsTrue(boardState.WhiteCount > boardState.BlackCount);
        }
    }
}
