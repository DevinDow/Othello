using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UnitTests
{
    [TestClass]
    public class ComputerPlayerTests
    {
        /// <summary>
        /// runs a Test for a BoardState & Level where there is one Expected Choice
        /// </summary>
        /// <param name="boardState"></param>
        /// <param name="level"></param>
        /// <param name="expected"></param>
        /// <param name="reasoning">text explaining why it should make this choice</param>
        private void TestExpectedChoice(BoardState boardState, LevelEnum level, Coord expected, string reasoning)
        {
            ComputerPlayer computerPlayer = new ComputerPlayer(level, true);
            computerPlayer.BoardState = boardState;
            Coord? chose;

            Debug.Print("\n** {0} chooses {1} {2} **", computerPlayer.Level, expected, reasoning);
            chose = computerPlayer.ChooseNextMove();
            Assert.AreEqual(expected, chose.Value, string.Format("{0} chose {1} instead of {2} {3}", computerPlayer.Level, chose.Value, expected, reasoning));
            Debug.Print("{0} SUCCESSFULLY choose {1} {2}", computerPlayer.Level, expected, reasoning);
        }

        /// <summary>
        /// runs a Test for a BoardState & Level where there are multiple Acceptable Choices
        /// </summary>
        /// <param name="boardState"></param>
        /// <param name="level"></param>
        /// <param name="acceptableChoices"></param>
        /// <param name="reasoning">text explaining why it should make this choice</param>
        private void TestAcceptableChoices(BoardState boardState, LevelEnum level, List<Coord> acceptableChoices, string reasoning)
        {
            ComputerPlayer computerPlayer = new ComputerPlayer(level, true);
            computerPlayer.BoardState = boardState;
            Coord? chose;

            StringBuilder acceptableChoicesString = new StringBuilder();
            foreach (Coord coord in acceptableChoices)
                acceptableChoicesString.Append(coord.ToString());

            Debug.Print("\n** {0} chooses from {1} {2} **", computerPlayer.Level, acceptableChoicesString, reasoning);
            chose = computerPlayer.ChooseNextMove();
            Assert.IsTrue(acceptableChoices.Contains(chose.Value), string.Format("{0} chose {1} instead of {2} {3}", computerPlayer.Level, chose.Value, acceptableChoicesString, reasoning));
            Debug.Print("{0} SUCCESSFULLY chose from {1} {2}", computerPlayer.Level, acceptableChoicesString, reasoning);
        }


        [TestMethod]
        public void TestFirstMove()
        {
            BoardState boardState = new BoardState();
            ComputerPlayer computerPlayer = new ComputerPlayer(LevelEnum.Beginner, false);
            computerPlayer.BoardState = boardState;
            List<Coord> acceptableChoices = new List<Coord>
            {
                new Coord(3, 5),
                new Coord(4, 6),
                new Coord(5, 3),
                new Coord(6, 4),
            };

            TestAcceptableChoices(boardState, LevelEnum.Beginner, acceptableChoices, string.Empty);
            TestAcceptableChoices(boardState, LevelEnum.Intermediate, acceptableChoices, string.Empty);
            TestAcceptableChoices(boardState, LevelEnum.Advanced, acceptableChoices, string.Empty);
            TestAcceptableChoices(boardState, LevelEnum.Expert, acceptableChoices, string.Empty);
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

            TestExpectedChoice(boardState, LevelEnum.Beginner, new Coord(2, 5), "to flip 3");
            TestExpectedChoice(boardState, LevelEnum.Intermediate, new Coord(1, 4), "to flip 2 and get an edge");
            TestExpectedChoice(boardState, LevelEnum.Advanced, new Coord(1, 4), "to flip 2 and get an edge while preventing Human from getting edge");
            TestExpectedChoice(boardState, LevelEnum.Expert, new Coord(1, 4), "to flip 2 and get an edge while preventing Human from getting edge");
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

            TestExpectedChoice(boardState, LevelEnum.Beginner, new Coord(3, 4), "to flip 2");
            TestExpectedChoice(boardState, LevelEnum.Intermediate, new Coord(6, 6), "to get higher value pieces in col 6");
            TestExpectedChoice(boardState, LevelEnum.Advanced, new Coord(6, 6), "to avoid Human reflipping row");
            TestExpectedChoice(boardState, LevelEnum.Expert, new Coord(6, 6), "to avoid Human reflipping row\"");
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

            TestAcceptableChoices(boardState, LevelEnum.Beginner, new List<Coord> { new Coord(1, 1), new Coord(3, 2) }, "to flip 5 in 2 ways");
            TestExpectedChoice(boardState, LevelEnum.Intermediate, new Coord(1, 1), "to flip left edge");
            TestExpectedChoice(boardState, LevelEnum.Advanced, new Coord(1, 1), "because it's valuable to flip left edge");
            TestExpectedChoice(boardState, LevelEnum.Expert, new Coord(1, 1), "because it's valuable to flip left edge but nearly chooses others first because it can save (1,1) until later since it's not at risk of being taken");
        }
    }
}
