using Othello;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OthelloLib
{
    public static class ComputerPlayers
    {
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
                // which Player's turn is it (boardState.WhitesTurn knows if a Player is skipped due to no Legal Moves)
                ComputerPlayer currentPlayer = boardState.WhitesTurn ? white : black;
                Coord? choice = currentPlayer.ChooseNextMove(boardState);
                if (choice != null)
                {
                    boardState.PlacePieceAndFlipPiecesAndChangeTurns(choice.Value);
                    //Debug.Print(boardState.ToString()); // log BoardState after every Move
                    //Debug.Print("Black={0} White={1}", boardState.BlackCount, boardState.WhiteCount);
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
