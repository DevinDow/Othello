using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Othello
{
    public class ComputerPlayer_Advanced : ComputerPlayer
    {
        public static bool LogEachAdvancedOption = false;
        public static bool LogEachOpponentOption = false;
        public static bool LogOpponentChoice = false;
        public static bool LogAdvancedChoice = false;
        public static bool LogAdvancedChoiceOptions = false;

        public ComputerPlayer_Advanced(bool amIWhite = true) : base(amIWhite)
        {
            LevelName = "Advanced";
        }

        /// <summary>
        /// Finds Moves that minimize best weighted Score that Opponent can attain.
        /// If multiple Moves tie then picks one that has highest Weighted Score.
        /// </summary>
        /// <returns>Choices that minimizes weighted Score that Opponent can attain</returns>
        protected override List<Coord> findBestChoices(BoardState boardState)
        {
            int maxAdvancedScoreAfterOpponentsBestResponse = -int.MaxValue;
            List<Coord> bestAdvancedChoices = new List<Coord>();

            // loop through all of Computer's Legal Moves
            // collect the ones that don't let the Opponent score well
            List<Coord> legalComputerMoves = boardState.LegalMoves();
            foreach (Coord computerChoice in legalComputerMoves)
            {
                BoardState computerBoardState = boardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogEachAdvancedOption)
                    Debug.Print(" - Advanced option: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}",
                            computerBoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);

                // find opponentsBestResponseScore
                int opponentsBestResponseScore;
                if (computerBoardState.WhitesTurn == boardState.WhitesTurn) // Opponent's Turn skipped
                {
                    if (LogEachOpponentOption)
                        Debug.Print("    - Advanced Opponent response option: SKIPPED resulting Score={0:+#;-#;+0}\nresulting BoardState:{1}",
                                computerChoiceScore, computerBoardState);
                    opponentsBestResponseScore = computerChoiceScore;
                }
                else
                    opponentsBestResponseScore = FindOpponentsBestResponseScore(computerBoardState);

                if (opponentsBestResponseScore > maxAdvancedScoreAfterOpponentsBestResponse) // remember maxComputerScoreAfterOpponentsBestResponse and start a new List of Moves that attain it
                {
                    maxAdvancedScoreAfterOpponentsBestResponse = opponentsBestResponseScore;
                    bestAdvancedChoices = new List<Coord>();
                }

                if (opponentsBestResponseScore >= maxAdvancedScoreAfterOpponentsBestResponse) // add choice to bestComputerChoices
                {
                    if (!bestAdvancedChoices.Contains(computerChoice))
                        bestAdvancedChoices.Add(computerChoice);
                }
            }

            if (LogAdvancedChoice && bestAdvancedChoices.Count > 1)
                Debug.Print("** bestAdvancedChoices count={0}, maxAdvancedScoreAfterOpponentsBestResponse={1:+#;-#;+0}.  Choose the highest scoring Move.",
                        bestAdvancedChoices.Count, maxAdvancedScoreAfterOpponentsBestResponse);

            // find finalComputerChoices from bestAdvancedChoices based on computerChoiceScore
            int maxComputerScore = -int.MaxValue;
            List<Coord> finalComputerChoices = new List<Coord>();
            foreach (Coord computerChoice in bestAdvancedChoices)
            {
                BoardState computerBoardState = boardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogAdvancedChoiceOptions && bestAdvancedChoices.Count > 1)
                    Debug.Print("Top Advanced choice: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}",
                            computerBoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);
                if (computerChoiceScore > maxComputerScore)
                {
                    maxComputerScore = computerChoiceScore;
                    finalComputerChoices = new List<Coord>();
                }
                if (computerChoiceScore >= maxComputerScore)
                {
                    finalComputerChoices.Add(computerChoice);
                }
            }

            if (LogAdvancedChoice && finalComputerChoices.Count > 1)
                Debug.Print("finalComputerChoices count={0}, maxComputerScore={1} maxAdvancedScoreAfterOpponentsBestResponse={2:+#;-#;+0}",
                        finalComputerChoices.Count, maxComputerScore, maxAdvancedScoreAfterOpponentsBestResponse);

            return finalComputerChoices;
        }

        /// <summary>
        /// Score all legalOpponentMoves to find Opponent's best response Score
        /// </summary>
        /// <param name="computerBoardState">BoardState for Opponent's current Turn</param>
        /// <returns>minScoreAfterOpponentTurn of all Opponent's legalMoves</returns>
        private int FindOpponentsBestResponseScore(BoardState computerBoardState)
        {
            int minScoreAfterOpponentTurn = int.MaxValue;
            //List<Coord> bestOpponentResponses = new List<Coord>(); // don't need a list, any of the ties will do
            Coord? bestOpponentResponse = null;
            BoardState bestOpponentResponseBoardState = null;

            List<Coord> legalOpponentMoves = computerBoardState.LegalMoves();
            foreach (Coord opponentResponse in legalOpponentMoves)
            {
                BoardState opponentResponseBoardState = computerBoardState.Clone();
                opponentResponseBoardState.PlacePieceAndFlipPiecesAndChangeTurns(opponentResponse);
                int opponentResponseScore = ScoreBoard(opponentResponseBoardState);
                if (LogEachOpponentOption)
                    Debug.Print("    - Advanced Opponent response option: {0} resulting Score={1:+#;-#;+0}\nresulting BoardState:{2}",
                            opponentResponse, opponentResponseScore, opponentResponseBoardState);

                if (opponentResponseScore < minScoreAfterOpponentTurn) // remember minScoreAfterOpponentTurn and start a new List of Moves that attain it
                {
                    minScoreAfterOpponentTurn = opponentResponseScore;
                    bestOpponentResponse = opponentResponse;
                    bestOpponentResponseBoardState = opponentResponseBoardState;
                    //bestOpponentResponses = new List<Coord>();
                }

                /*if (opponentResponseScore <= minScoreAfterOpponentTurn) // add choice to maxScoringChoices
                {
                    if (!bestOpponentResponses.Contains(opponentResponse))
                        bestOpponentResponses.Add(opponentResponse);
                }*/
            }

            if (LogOpponentChoice)
                Debug.Print("    - best Advanced Opponent response: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}",
                        computerBoardState.WhitesTurn ? 'W' : 'B', bestOpponentResponse, minScoreAfterOpponentTurn, bestOpponentResponseBoardState);

            return minScoreAfterOpponentTurn;
        }
    }
}
