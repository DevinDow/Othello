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
        private const int RUNS = 20;

        [TestMethod]
        public void BegVsInt()
        {
            int begWins = 0;
            int intWins = 0;
            for (int i = 0; i < RUNS; i++)
            {
                BoardState boardState = ComputerVsComputer(LevelEnum.Beginner, LevelEnum.Intermediate);
                if (boardState.WhiteCount > boardState.BlackCount)
                    begWins++;
                if (boardState.BlackCount > boardState.WhiteCount)
                    intWins++;
            }
            Debug.Print("Intermediate {0} - Beginner {1}", intWins, intWins);
            Assert.IsTrue(intWins > begWins);
        }

        [TestMethod]
        public void IntVsBeg()
        {
            int intWins = 0;
            int begWins = 0;
            for (int i = 0; i < RUNS; i++)
            {
                BoardState boardState = ComputerVsComputer(LevelEnum.Intermediate, LevelEnum.Beginner);
                if (boardState.WhiteCount > boardState.BlackCount)
                    intWins++;
                if (boardState.BlackCount > boardState.WhiteCount)
                    begWins++;
            }
            Debug.Print("Intermediate {0} - Beginner {1}", intWins, intWins);
            Assert.IsTrue(intWins > begWins);
        }

        [TestMethod]
        public void AdvVsInt()
        {
            int advWins = 0;
            int intWins = 0;
            for (int i = 0; i < RUNS; i++)
            {
                BoardState boardState = ComputerVsComputer(LevelEnum.Advanced, LevelEnum.Intermediate);
                if (boardState.WhiteCount > boardState.BlackCount)
                    advWins++;
                if (boardState.BlackCount > boardState.WhiteCount)
                    intWins++;
            }
            Debug.Print("Advanced {0} - Intermediate {1}", advWins, intWins);
            Assert.IsTrue(advWins > intWins);
        }

        [TestMethod]
        public void IntVsAdv()
        {
            int intWins = 0;
            int advWins = 0;
            for (int i = 0; i < RUNS; i++)
            {
                BoardState boardState = ComputerVsComputer(LevelEnum.Intermediate, LevelEnum.Advanced);
                if (boardState.WhiteCount > boardState.BlackCount)
                    intWins++;
                if (boardState.BlackCount > boardState.WhiteCount)
                    advWins++;
            }
            Debug.Print("Advanced {0} - Intermediate {1}", advWins, intWins);
            Assert.IsTrue(advWins > intWins);
        }

        [TestMethod]
        public void ExpVsAdv()
        {
            int expWins = 0;
            int advWins = 0;
            for (int i = 0; i < RUNS; i++)
            {
                BoardState boardState = ComputerVsComputer(LevelEnum.Expert, LevelEnum.Advanced);
                if (boardState.WhiteCount > boardState.BlackCount)
                    expWins++;
                if (boardState.BlackCount > boardState.WhiteCount)
                    advWins++;
            }
            Debug.Print("Expert {0} - Advanced {1}", expWins, advWins);
            Assert.IsTrue(expWins > advWins);
        }


        public BoardState ComputerVsComputer(LevelEnum whiteLevel, LevelEnum blackLevel)
        { 
            ComputerPlayer.LogDecisions = false;
            BoardState boardState = new BoardState();
            ComputerPlayer white = new ComputerPlayer(whiteLevel, true);
            ComputerPlayer black = new ComputerPlayer(blackLevel, false);
            //Debug.Print("White={0} Black={1}", whiteLevel, blackLevel);

            while (true)
            {
                ComputerPlayer currentPlayer = boardState.WhitesTurn ? white : black;
                currentPlayer.BoardState = boardState;
                Coord? choice = currentPlayer.ChooseNextMove();
                if (choice != null)
                {
                    boardState.PlacePieceAndFlipPiecesAndChangeTurns(choice.Value);
                    //Debug.Print(boardState.ToString());
                    //Debug.Print("White={0} Black={1}", boardState.WhiteCount, boardState.BlackCount);
                }

                if (boardState.endOfGame)
                {
                    if (boardState.WhiteCount > boardState.BlackCount)
                        Debug.Print("{0} wins {1}-{2}", whiteLevel, boardState.WhiteCount, boardState.BlackCount);
                    else if (boardState.BlackCount > boardState.WhiteCount)
                        Debug.Print("{0} wins {1}-{2}", blackLevel, boardState.BlackCount, boardState.WhiteCount);
                    else
                        Debug.Print("TIE {0}-{1}", boardState.WhiteCount, boardState.BlackCount);
                    return boardState;
                }
            }
        }
    }
}
