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
        private void TestExpectedChoice(BoardState boardState, ComputerPlayer computerPlayer, Coord expected, string reasoning)
        {
            Debug.Print("\n** {0} chooses {1} {2} **", computerPlayer.LevelName, expected, reasoning);
            Coord? chose = computerPlayer.ChooseNextMove(boardState);
            Assert.AreEqual(expected, chose.Value, string.Format("{0} chose {1} instead of {2} {3}", computerPlayer.LevelName, chose.Value, expected, reasoning));
            Debug.Print("{0} SUCCESSFULLY choose {1} {2}", computerPlayer.LevelName, expected, reasoning);
        }

        /// <summary>
        /// runs a Test for a BoardState & Level where there are multiple Acceptable Choices
        /// </summary>
        /// <param name="boardState"></param>
        /// <param name="level"></param>
        /// <param name="acceptableChoices"></param>
        /// <param name="reasoning">text explaining why it should make this choice</param>
        private void TestAcceptableChoices(BoardState boardState, ComputerPlayer computerPlayer, List<Coord> acceptableChoices, string reasoning)
        {
            StringBuilder acceptableChoicesString = new StringBuilder();
            foreach (Coord coord in acceptableChoices)
                acceptableChoicesString.Append(coord.ToString());

            Debug.Print("\n** {0} chooses from {1} {2} **", computerPlayer.LevelName, acceptableChoicesString, reasoning);
            Coord? chose = computerPlayer.ChooseNextMove(boardState);
            Assert.IsTrue(acceptableChoices.Contains(chose.Value), string.Format("{0} chose {1} instead of {2} {3}", computerPlayer.LevelName, chose.Value, acceptableChoicesString, reasoning));
            Debug.Print("{0} SUCCESSFULLY chose from {1} {2}", computerPlayer.LevelName, acceptableChoicesString, reasoning);
        }


        [TestMethod]
        public void TestFirstMove()
        {
            BoardState boardState = new BoardState();
            List<Coord> acceptableChoices = new List<Coord>
            {
                new Coord(5, 3),
                new Coord(6, 4),
                new Coord(3, 5),
                new Coord(4, 6),
            };

            TestAcceptableChoices(boardState, new ComputerPlayer_Beginner(false), acceptableChoices, string.Empty);
            TestAcceptableChoices(boardState, new ComputerPlayer_Intermediate(false), acceptableChoices, string.Empty);
            TestAcceptableChoices(boardState, new ComputerPlayer_Advanced(false), acceptableChoices, string.Empty);
            //TestAcceptableChoices(boardState, LevelEnum.Expert, acceptableChoices, string.Empty);
            //TestAcceptableChoices(boardState, LevelEnum.Ultimate, acceptableChoices, string.Empty);
        }

        [TestMethod]
        public void TestLevels_Int()
        {
            BoardState boardState = new BoardState(true, false);
            boardState.SetSquare(new Coord(2, 1), new Square(StateEnum.White));
            boardState.SetSquare(new Coord(2, 2), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(2, 3), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(2, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(3, 4), new Square(StateEnum.Black));
            boardState.SetSquare(new Coord(4, 4), new Square(StateEnum.White));

            TestExpectedChoice(boardState, new ComputerPlayer_Beginner(true), new Coord(2, 5), "to flip 3");
            TestExpectedChoice(boardState, new ComputerPlayer_Intermediate(true), new Coord(1, 4), "to flip 2 and get an Edge");
            TestExpectedChoice(boardState, new ComputerPlayer_Advanced(true), new Coord(1, 4), "to flip 2 and get an Edge while preventing Human from getting Edge");
            //TestExpectedChoice(boardState, LevelEnum.Expert, new Coord(1, 4), "to flip 2 and get an Edge while preventing Human from getting Edge");
            //TestExpectedChoice(boardState, LevelEnum.Ultimate, new Coord(1, 4), "to flip 2 and get an Edge while preventing Human from getting Edge");
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

            TestExpectedChoice(boardState, new ComputerPlayer_Beginner(true), new Coord(3, 4), "to flip 2");
            TestExpectedChoice(boardState, new ComputerPlayer_Intermediate(true), new Coord(8, 4), "to get an Edge");
            TestExpectedChoice(boardState, new ComputerPlayer_Advanced(true), new Coord(6, 6), "to avoid Human reflipping Row");
            //TestExpectedChoice(boardState, LevelEnum.Expert, new Coord(8, 4), "to get an Edge");
            //TestExpectedChoice(boardState, LevelEnum.Ultimate, new Coord(8, 4), "to get an Edge");
            /*TestExpectedChoice(boardState, LevelEnum.Ultimate, new Coord(6, 6), "because it can do better by doing (6,6) first and then doing (8,4) later");*/
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

            TestAcceptableChoices(boardState, new ComputerPlayer_Beginner(true), new List<Coord> { new Coord(1, 1), new Coord(3, 2) }, "to flip 5 in 2 ways");
            TestExpectedChoice(boardState, new ComputerPlayer_Intermediate(true), new Coord(1, 1), "to flip left Edge");
            TestExpectedChoice(boardState, new ComputerPlayer_Advanced(true), new Coord(1, 1), "because it's valuable to flip left Edge");
            //TestAcceptableChoices(boardState, LevelEnum.Expert, new List<Coord> { new Coord(1, 1), new Coord(3, 2), new Coord(4, 2), new Coord(5, 2), new Coord(6, 2) }, "it can save TL Corner (1,1) until later since it's not at risk of being taken and can take actions to prevent losing TR Corner");
            //TestAcceptableChoices(boardState, LevelEnum.Ultimate, new List<Coord> { new Coord(1, 1), new Coord(3, 2), new Coord(4, 2), new Coord(5, 2), new Coord(6, 2), new Coord(8, 2) }, "it can save TL Corner (1,1) until later since it's not at risk of being taken and can take actions to prevent losing TR Corner");
        }
    }
}
