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

            Debug.Print("\n** Beginner **");
            computerPlayer.Level = LevelEnum.Beginner;
            Coord? choice = computerPlayer.ChooseNextMove();
            Assert.IsTrue(acceptableChoices.Contains(choice.Value));

            Debug.Print("\n** Intermediate **");
            computerPlayer.Level = LevelEnum.Intermediate;
            choice = computerPlayer.ChooseNextMove();
            Assert.IsTrue(acceptableChoices.Contains(choice.Value));

            Debug.Print("\n** Advanced **");
            computerPlayer.Level = LevelEnum.Advanced;
            choice = computerPlayer.ChooseNextMove();
            Assert.IsTrue(acceptableChoices.Contains(choice.Value));

            Debug.Print("\n** Expert **");
            computerPlayer.Level = LevelEnum.Expert;
            choice = computerPlayer.ChooseNextMove();
            Assert.IsTrue(acceptableChoices.Contains(choice.Value));
        }

        [TestMethod]
        public void TestLevels_Beg()
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

            Debug.Print("\n** Beginner chooses (2,5) to flip 3 **");
            computerPlayer.Level = LevelEnum.Beginner;
            Coord? choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(2, 5), choice.Value);

            Debug.Print("\n** Intermediate chooses (1,4) to flip 2 and get an edge **");
            computerPlayer.Level = LevelEnum.Intermediate;
            choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(1, 4), choice.Value);

            Debug.Print("\n** Advanced chooses (1,4) to flip 2 and get an edge while preventing Human from getting edge **");
            computerPlayer.Level = LevelEnum.Advanced;
            choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(1, 4), choice.Value);

            Debug.Print("\n** Expert chooses (1,4) to flip 2 and get an edge while preventing Human from getting edge **");
            computerPlayer.Level = LevelEnum.Expert;
            choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(1, 4), choice.Value);
        }

        [TestMethod]
        public void TestLevels_Adv()
        {
            BoardState boardState = new BoardState(true, false);
            boardState.SetSquare(new Coord(2, 4), new Square(StateEnum.White));
            boardState.SetSquare(new Coord(4, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(5, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(6, 4), new Square(StateEnum.White));
            boardState.SetSquare(new Coord(7, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(6, 5), new Square(StateEnum.Black));

            ComputerPlayer computerPlayer = new ComputerPlayer(LevelEnum.Beginner, true);
            computerPlayer.BoardState = boardState;

            Debug.Print("\n** Beginner chooses (3,4) to flip 2 **");
            computerPlayer.Level = LevelEnum.Beginner;
            Coord? choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(3, 4), choice.Value);

            Debug.Print("\n** Intermediate chooses (6,6) to get higher value pieces in col 6 **");
            computerPlayer.Level = LevelEnum.Intermediate;
            choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(6, 6), choice.Value);

            Debug.Print("\n** Advanced chooses (6,6) to avoid Human reflipping row **");
            computerPlayer.Level = LevelEnum.Advanced;
            choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(6, 6), choice.Value);

            Debug.Print("\n** Expert chooses (8,4) to get edge and eventually get row **");
            computerPlayer.Level = LevelEnum.Expert;
            choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(8, 4), choice.Value);
        }

        [TestMethod]
        public void TestLevels_Exp()
        {
            BoardState boardState = new BoardState(true, false);
            // col 1
            boardState.SetSquare(new Coord(1, 2), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(1, 3), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(1, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(1, 5), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(1, 6), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(1, 7), new Square(StateEnum.White));
            boardState.SetSquare(new Coord(1, 8), new Square(StateEnum.Black));
            // col 2
            boardState.SetSquare(new Coord(2, 3), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(2, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(2, 5), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(2, 6), new Square(StateEnum.White));
            boardState.SetSquare(new Coord(2, 7), new Square(StateEnum.White));
            boardState.SetSquare(new Coord(2, 8), new Square(StateEnum.Black));
            // col 3
            boardState.SetSquare(new Coord(3, 3), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(3, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(3, 5), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(3, 6), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(3, 7), new Square(StateEnum.White));
            boardState.SetSquare(new Coord(3, 8), new Square(StateEnum.Black));
            // col 4
            boardState.SetSquare(new Coord(4, 3), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(4, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(4, 5), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(4, 6), new Square(StateEnum.White));
            boardState.SetSquare(new Coord(4, 7), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(4, 8), new Square(StateEnum.Black));
            // col 5
            boardState.SetSquare(new Coord(5, 3), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(5, 4), new Square(StateEnum.White));
            boardState.SetSquare(new Coord(5, 5), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(5, 6), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(5, 7), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(5, 8), new Square(StateEnum.Black));
            // col 6
            boardState.SetSquare(new Coord(6, 3), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(6, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(6, 5), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(6, 6), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(6, 7), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(6, 8), new Square(StateEnum.Black));
            // col 7
            boardState.SetSquare(new Coord(7, 3), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(7, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(7, 5), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(7, 6), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(7, 7), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(7, 8), new Square(StateEnum.Black));
            // col 8
            boardState.SetSquare(new Coord(8, 5), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(8, 6), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(8, 7), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(8, 8), new Square(StateEnum.Black));

            ComputerPlayer computerPlayer = new ComputerPlayer(LevelEnum.Expert, true);
            computerPlayer.BoardState = boardState;

            Debug.Print("\n** Beginner chooses to flip 5 in 2 ways **");
            computerPlayer.Level = LevelEnum.Beginner;
            Coord? choice = computerPlayer.ChooseNextMove();
            List<Coord> begAcceptableChoices = new List<Coord>
            {
                new Coord(1,1),
                new Coord(3,2),
            };
            Assert.IsTrue(begAcceptableChoices.Contains(choice.Value));

            Debug.Print("\n** Intermediate chooses valuable (1,1) to flip left edge **");
            computerPlayer.Level = LevelEnum.Intermediate;
            choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(1, 1), choice.Value);

            Debug.Print("\n** Advanced chooses valuable (1,1) to flip left edge **");
            // TODO: Human Black gets skipped, but algorithm doesn't handle it
            computerPlayer.Level = LevelEnum.Advanced;
            choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(1, 1), choice.Value);

            Debug.Print("\n** Expert chooses (1,1) to flip left edge **");
            computerPlayer.Level = LevelEnum.Expert;
            choice = computerPlayer.ChooseNextMove();
            Assert.AreEqual(new Coord(1, 1), choice.Value);
        }
    }
}
