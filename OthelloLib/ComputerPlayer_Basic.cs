using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OthelloLib
{
    public abstract class ComputerPlayer_Basic : ComputerPlayer
    {
        public static bool LogBasicOptions = false;

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
            List<Coord> legalMoves = boardState.LegalMoves();
            foreach (Coord computerChoice in legalMoves)
            {
                BoardState computerBoardState = boardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogBasicOptions)
                    Debug.Print("{0} choice: {1}->{2} resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                            LevelName, boardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);

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
