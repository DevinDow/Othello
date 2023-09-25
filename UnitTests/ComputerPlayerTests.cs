using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTests
{
    [TestClass]
    public class ComputerPlayerTests
    {
        [TestMethod]
        public void TestFirstMove()
        {
            BoardState boardState = new BoardState();
            ComputerPlayer computerPlayer = new ComputerPlayer(LevelEnum.Beginner, false);
            computerPlayer.BoardState = boardState;
            List<Coord> acceptableChoices = new List<Coord>
            {
                new Coord(3,5),
                new Coord(4,6),
                new Coord(5,3),
                new Coord(6,4),
            };

            Debug.Print("** Beginner **");
            computerPlayer.Level = LevelEnum.Beginner;
            Debug.Print(computerPlayer.ToString());
            Coord? choice = computerPlayer.ChooseNextMove();
            Assert.IsTrue(acceptableChoices.Contains(choice.Value));

            Debug.Print("** Intermediate **");
            computerPlayer.Level = LevelEnum.Intermediate;
            Debug.Print(computerPlayer.ToString());
            choice = computerPlayer.ChooseNextMove();
            Assert.IsTrue(acceptableChoices.Contains(choice.Value));

            Debug.Print("** Advanced **");
            computerPlayer.Level = LevelEnum.Advanced;
            Debug.Print(computerPlayer.ToString());
            choice = computerPlayer.ChooseNextMove();
            Assert.IsTrue(acceptableChoices.Contains(choice.Value));
        }

        [TestMethod]
        public void TestLevels()
        {
            BoardState boardState = new BoardState(true, false);
            boardState.SetSquare(new Coord(2, 1), new Square(StateEnum.White));
            boardState.SetSquare(new Coord(2, 2), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(2, 3), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(2, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(3, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(4, 4), new Square(StateEnum.White));

            ComputerPlayer computerPlayer = new ComputerPlayer(LevelEnum.Beginner, true);
            computerPlayer.BoardState = boardState;

            Debug.Print("** Beginner chooses to flip 3 **");
            computerPlayer.Level = LevelEnum.Beginner;
            Debug.Print(computerPlayer.ToString());
            Coord? choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(2, 5), choice.Value);

            Debug.Print("** Intermediate chooses to flip 2 and get an edge **");
            computerPlayer.Level = LevelEnum.Intermediate;
            Debug.Print(computerPlayer.ToString());
            choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(1, 4), choice.Value);

            Debug.Print("** Advanced chooses to flip 2 and get an edge while preventing Human from getting edge **");
            computerPlayer.Level = LevelEnum.Advanced;
            Debug.Print(computerPlayer.ToString());
            choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(1, 4), choice.Value);
        }
    }
}
