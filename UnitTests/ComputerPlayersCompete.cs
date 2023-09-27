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
            int intWins = 0;
            for (int i = 0; i < 10; i++)
            {
                BoardState boardState = ComputerVsComputer(LevelEnum.Beginner, LevelEnum.Intermediate);
                if (boardState.BlackCount > boardState.WhiteCount)
                    intWins++;
            }
            Debug.Print("Intermediate won {0}/10", intWins);
            Assert.IsTrue(intWins > 5);
        }

        [TestMethod]
        public void IntVsBeg()
        {
            int intWins = 0;
            for (int i = 0; i < 10; i++)
            {
                BoardState boardState = ComputerVsComputer(LevelEnum.Intermediate, LevelEnum.Beginner);
                if (boardState.WhiteCount > boardState.BlackCount)
                    intWins++;
            }
            Debug.Print("Intermediate won {0}/10", intWins);
            Assert.IsTrue(intWins > 5);
        }

        [TestMethod]
        public void AdvVsInt()
        {
            int advWins = 0;
            for (int i = 0; i < 10; i++)
            {
                BoardState boardState = ComputerVsComputer(LevelEnum.Advanced, LevelEnum.Intermediate);
                if (boardState.WhiteCount > boardState.BlackCount)
                    advWins++;
            }
            Debug.Print("Advanced won {0}/10", advWins);
            Assert.IsTrue(advWins > 5);
        }

        [TestMethod]
        public void IntVsAdv()
        {
            int advWins = 0;
            for (int i = 0; i < 10; i++)
            {
                BoardState boardState = ComputerVsComputer(LevelEnum.Intermediate, LevelEnum.Advanced);
                if (boardState.BlackCount > boardState.WhiteCount)
                    advWins++;
            }
            Debug.Print("Advanced won {0}/10", advWins);
            Assert.IsTrue(advWins > 5);
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
