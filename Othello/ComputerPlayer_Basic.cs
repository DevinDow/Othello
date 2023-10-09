using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Othello
{
    internal abstract class ComputerPlayer_Basic : ComputerPlayer
    {
        protected ComputerPlayer_Basic(bool amIWhite) : base(amIWhite) { }

        /// <summary>
        /// finds Moves that maximize weighted Score and picks one at random
        /// </summary>
        /// <returns>a Choice that maximizes weighted Score</returns>
        protected override List<Coord> findBestChoices(BoardState boardState)
        {
            int maxScore = -int.MaxValue;
            List<Coord> bestComputerChoices = new List<Coord>();

            // loop through all of Computer's Legal Moves
            List<Coord> legalMoves = BoardState.LegalMoves();
            foreach (Coord computerChoice in legalMoves)
            {
                BoardState computerBoardState = BoardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogDecisions)
                    Debug.Print("Computer choice: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}",
                            BoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);

                if (computerChoiceScore > maxScore) // remember maxScore and start a new List of Moves that attain it
                {
                    maxScore = computerChoiceScore;
                    bestComputerChoices = new List<Coord>();
                }

                if (computerChoiceScore >= maxScore) // add choice to maxScoringChoices
                {
                    bestComputerChoices.Add(computerChoice);
                }
            }

            return bestComputerChoices;
        }
    }
}
