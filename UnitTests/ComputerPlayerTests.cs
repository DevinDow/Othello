using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class ComputerPlayerTests
    {
        [TestMethod]
        public void TestBeginnerFirstMove()
        {
            BoardState boardState = new BoardState();
            ComputerPlayer computerPlayer = new ComputerPlayer();
            computerPlayer.Level = LevelEnum.Beginner;
            computerPlayer.BoardState = boardState;
            Coord choice = computerPlayer.ChooseNextMove();
            List<Coord> acceptableChoices = new List<Coord>
            {
                new Coord(3,5),
                new Coord(4,6),
                new Coord(5,3),
                new Coord(6,4),
            };
            Assert.IsTrue(acceptableChoices.Contains(choice));
        }

        [TestMethod]
        public void TestAdvancedFirstMove()
        {
            BoardState boardState = new BoardState();
            ComputerPlayer computerPlayer = new ComputerPlayer();
            computerPlayer.Level = LevelEnum.Advanced;
            computerPlayer.BoardState = boardState;
            Coord choice = computerPlayer.ChooseNextMove();
            List<Coord> acceptableChoices = new List<Coord>
            {
                new Coord(3,5),
                new Coord(4,6),
                new Coord(5,3),
                new Coord(6,4),
            };
            Assert.IsTrue(acceptableChoices.Contains(choice));
        }
    }
}
