﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            ComputerPlayer computerPlayer = new ComputerPlayer(LevelEnum.Beginner);
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
            Coord? choice = computerPlayer.ChooseNextMove();
            Assert.IsTrue(acceptableChoices.Contains(choice.Value));

            Debug.Print("** Intermediate **");
            computerPlayer.Level = LevelEnum.Intermediate;
            choice = computerPlayer.ChooseNextMove();
            Assert.IsTrue(acceptableChoices.Contains(choice.Value));

            Debug.Print("** Advanced **");
            computerPlayer.Level = LevelEnum.Advanced;
            choice = computerPlayer.ChooseNextMove();
            Assert.IsTrue(acceptableChoices.Contains(choice.Value));
        }

        [TestMethod]
        public void TestLevels()
        {
            BoardState boardState = new BoardState(false);
            boardState.SetSquare(new Coord(2, 1), new Square(StateEnum.White));
            boardState.SetSquare(new Coord(2, 2), new Square(StateEnum.Black));

            ComputerPlayer computerPlayer = new ComputerPlayer();
            computerPlayer.BoardState = boardState;

            Debug.Print("** Beginner **");
            computerPlayer.Level = LevelEnum.Beginner;
            Coord? choice = computerPlayer.ChooseNextMove();
            //Assert.IsTrue(acceptableChoices.Contains(choice));

            Debug.Print("** Intermediate **");
            computerPlayer.Level = LevelEnum.Intermediate;
            choice = computerPlayer.ChooseNextMove();
            //Assert.IsTrue(acceptableChoices.Contains(choice));

            Debug.Print("** Advanced **");
            computerPlayer.Level = LevelEnum.Advanced;
            choice = computerPlayer.ChooseNextMove();
            //Assert.IsTrue(acceptableChoices.Contains(choice));
        }
    }
}
