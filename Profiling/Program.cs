using OthelloLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Profiling
{
    internal class Program
    {
        static void Main(string[] args)
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

            //TestAcceptableChoices(boardState, new ComputerPlayer_Beginner(true), new List<Coord> { new Coord(1, 1), new Coord(3, 2) }, "to flip 5 in 2 ways");
            //TestExpectedChoice(boardState, new ComputerPlayer_Intermediate(true), new Coord(1, 1), "to flip left Edge");
            //TestExpectedChoice(boardState, new ComputerPlayer_Advanced(true), new Coord(1, 1), "because it's valuable to flip left Edge");
            //TestAcceptableChoices(boardState, new ComputerPlayer_Expert(true), new List<Coord> { new Coord(1, 1), new Coord(3, 2), new Coord(4, 2), new Coord(5, 2), new Coord(6, 2) }, "it can save TL Corner (1,1) until later since it's not at risk of being taken and can take actions to prevent losing TR Corner");
            TestAcceptableChoices(boardState, new ComputerPlayer_Ultimate(true), new List<Coord> { new Coord(1, 1), new Coord(3, 2), new Coord(4, 2), new Coord(5, 2), new Coord(6, 2), new Coord(8, 2) }, "it can save TL Corner (1,1) until later since it's not at risk of being taken and can take actions to prevent losing TR Corner");
        }

        /// <summary>
        /// runs a Test for a BoardState & Level where there is one Expected Choice
        /// </summary>
        /// <param name="boardState"></param>
        /// <param name="level"></param>
        /// <param name="expected"></param>
        /// <param name="reasoning">text explaining why it should make this choice</param>
        private static void TestExpectedChoice(BoardState boardState, ComputerPlayer computerPlayer, Coord expected, string reasoning)
        {
            Debug.Print("\n** {0} chooses {1} {2} **", computerPlayer.LevelName, expected, reasoning);
            Coord? chose = computerPlayer.ChooseNextMove(boardState);
            Debug.Print("{0} choose {1} {2}", computerPlayer.LevelName, expected, reasoning);
        }

        /// <summary>
        /// runs a Test for a BoardState & Level where there are multiple Acceptable Choices
        /// </summary>
        /// <param name="boardState"></param>
        /// <param name="level"></param>
        /// <param name="acceptableChoices"></param>
        /// <param name="reasoning">text explaining why it should make this choice</param>
        private static void TestAcceptableChoices(BoardState boardState, ComputerPlayer computerPlayer, List<Coord> acceptableChoices, string reasoning)
        {
            StringBuilder acceptableChoicesString = new StringBuilder();
            foreach (Coord coord in acceptableChoices)
                acceptableChoicesString.Append(coord.ToString());

            Debug.Print("\n** {0} chooses from {1} {2} **", computerPlayer.LevelName, acceptableChoicesString, reasoning);
            Coord? chose = computerPlayer.ChooseNextMove(boardState);
            Debug.Print("{0} chose from {1} {2}", computerPlayer.LevelName, acceptableChoicesString, reasoning);
        }
    }
}
