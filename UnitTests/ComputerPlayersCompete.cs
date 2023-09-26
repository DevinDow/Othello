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
            BoardState boardState = new BoardState();
            ComputerPlayer blackBeg = new ComputerPlayer(LevelEnum.Beginner, false);
            ComputerPlayer whiteInt = new ComputerPlayer(LevelEnum.Intermediate, true);

            while (true)
            {
                ComputerPlayer currentPlayer = boardState.WhitesTurn ? whiteInt : blackBeg;
                currentPlayer.BoardState = boardState;
                Coord? choice = currentPlayer.ChooseNextMove();
                if (choice != null)
                    boardState.PlacePieceAndFlipPiecesAndChangeTurns(choice.Value);
                else if (boardState.endOfGame)
                    break;
            }
        }
    }
}
