using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello;
using System;

namespace UnitTests
{
    [TestClass]
    public class ComputerPlayerTests
    {
        [TestMethod]
        public void TestFirstMove()
        {
            BoardState boardState = new BoardState();
            ComputerPlayer computerPlayer = new ComputerPlayer();
            computerPlayer.Level = LevelEnum.Beginner;
            computerPlayer.BoardState = boardState;
            Coord choice = computerPlayer.ChooseNextMove();
            //Assert.AreEqual(new Coord(6, 3), choice);
        }
    }
}
