using OthelloLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OthelloLib
{
    public static class ComputerPlayers
    {
        public static bool OutputToConsole = false;

        /// <summary>
        /// run a Game of black vs white
        /// </summary>
        /// <param name="black">Level for Black</param>
        /// <param name="white">Level for White</param>
        /// <returns>BoardState after end of Game</returns>
        public static BoardState ComputerVsComputer(ComputerPlayer black, ComputerPlayer white)
        {
            BoardState boardState = new BoardState();

            while (true) // repeat currentPlayer making a Move until endOfGame
            {
                DateTime start = DateTime.Now;

                // which Player's turn is it (boardState.WhitesTurn knows if a Player is skipped due to no Legal Moves)
                ComputerPlayer currentPlayer = boardState.WhitesTurn ? white : black;
                Coord? choice = currentPlayer.ChooseNextMove(boardState);
                if (choice != null)
                {
                    boardState.PlacePieceAndFlipPiecesAndChangeTurns(choice.Value);
                    //Debug.Print(boardState.ToString()); // log BoardState after every Move
                    //Debug.Print("Black={0} White={1}", boardState.BlackCount, boardState.WhiteCount);
                }

                if (OutputToConsole)
                {
                    string lead = string.Format("{0:+#;-#;+0}", boardState.BlackCount - boardState.WhiteCount).PadLeft(3);
                    Console.WriteLine("{0} : {1} remaining, {2:N1}\" by {3}", lead, boardState.EmptyCount, (DateTime.Now - start).TotalSeconds, currentPlayer);
                }

                // break when End Of Game
                if (boardState.endOfGame)
                {
                    // log BoardState after endOfGame
                    if (ComputerPlayer.LogDecisions)
                    {
                        Debug.Print("\n");
                        Debug.Print(boardState.ToString());
                        Debug.Print("Black={0} White={1}", boardState.BlackCount, boardState.WhiteCount);
                    }

                    return boardState;
                }
            }
        }
    }
}
