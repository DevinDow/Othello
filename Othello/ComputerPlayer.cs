using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Othello
{
	public enum LevelEnum {
		Beginner,		// maximizes relative Score using number of Square
		Intermediate,	// maximizes relative Score using weighting for more valuable/dangerous Squares
        Advanced,		// minimizes Opponents best response
		Expert			// Minimax algorithm to a Depth
	}

	public class ComputerPlayer
	{
		public bool AmIWhite;
		public BoardState BoardState;
		public LevelEnum Level;
        public static bool LogDecisions = true;
		private Random random = new Random();
        private const int EXPERT_DEPTH = 7;

		public ComputerPlayer(LevelEnum level = LevelEnum.Beginner, bool amIWhite = true)
		{
			AmIWhite = amIWhite;
			Level = level;
		}

		/// <summary>
		/// returns ComputerPlayer's choice for next move
		/// </summary>
		public Coord? ChooseNextMove()
		{
            if (LogDecisions)
            {
                int initialScore = ScoreBoard(BoardState);
                Debug.Print("{0} {1}\ninitial BoardState:{2}\ninitial Score={3:+#;-#;+0}", 
                        Level, AmIWhite ? "W" : "B", BoardState, initialScore);
            }

            List<Coord> choices = new List<Coord>();

            switch (Level)
			{
				case LevelEnum.Beginner:
				case LevelEnum.Intermediate:
				default:
                    choices = chooseHighestScoringMove();
					break;

                case LevelEnum.Advanced:
                    choices = chooseHighestScoringAfterOpponentMove();
					break;

                case LevelEnum.Expert:
                    choices = chooseHighestScoringAfterSeveralTurns();
					break;
            }

			// no legal Moves
            if (choices.Count == 0)
                return null;
            
			// only 1 best Move
			if (choices.Count == 1)
			{
				Coord choice = choices[0];
                if (LogDecisions)
                    Debug.Print("{0} chose {1}->{2}", Level, BoardState.WhitesTurn ? 'W' : 'B', choice);
                return choice;
			}

			// multiple equally best Moves
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Equal Choices: {0}->", BoardState.WhitesTurn ? 'W' : 'B');
			foreach (Coord choice in choices)
				sb.Append(choice + " ");
            if (LogDecisions)
                Debug.Print(sb.ToString());

			// randomly pick one of the choices
            int randomIndex = random.Next(choices.Count);
			Coord randomChoice = choices[randomIndex];
            if (LogDecisions)
                Debug.Print("{0} chose {1}->{2}", Level, BoardState.WhitesTurn ? 'W' : 'B', randomChoice);
            return randomChoice;
        }

        /// <summary>
        /// finds Moves that maximize weighted Score and picks one at random
        /// </summary>
        /// <returns>a Choice that maximizes weighted Score</returns>
        private List<Coord> chooseHighestScoringMove()
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

        /// <summary>
        /// finds Moves that minimize best weighted Score that Human can attain and picks one that has highest weighted Score
        /// </summary>
        /// <returns>a Choice that minimizes weighted Score that Human can attain</returns>
        private List<Coord> chooseHighestScoringAfterOpponentMove()
        {
            int maxComputerScoreAfterHumansBestResponse = -int.MaxValue;
            List<Coord> bestComputerChoices = new List<Coord>();

            // loop through all of Computer's Legal Moves
            // collect the ones that don't let the human score well
            List<Coord> legalComputerMoves = BoardState.LegalMoves();
            foreach (Coord computerChoice in legalComputerMoves)
            {
                BoardState computerBoardState = BoardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogDecisions)
                    Debug.Print(" - Computer choice: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}",
                            computerBoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);

                int humansBestResponseScore;
                if (computerBoardState.WhitesTurn == BoardState.WhitesTurn) // Human Turn skipped
                {
                    if (LogDecisions)
                        Debug.Print("    - Human response: SKIPPED resulting Score={0:+#;-#;+0}\nresulting BoardState:{1}",
                                computerChoiceScore, computerBoardState);
                    humansBestResponseScore = computerChoiceScore;
                }
                else
                    humansBestResponseScore = findHumansBestResponseScore(computerBoardState);

                if (humansBestResponseScore > maxComputerScoreAfterHumansBestResponse) // remember maxComputerScoreAfterHumansBestResponse and start a new List of Moves that attain it
                {
                    maxComputerScoreAfterHumansBestResponse = humansBestResponseScore;
                    bestComputerChoices = new List<Coord>();
                }

                if (humansBestResponseScore >= maxComputerScoreAfterHumansBestResponse) // add choice to bestComputerChoices
                {
                    if (!bestComputerChoices.Contains(computerChoice))
                        bestComputerChoices.Add(computerChoice);
                }
            }

            if (LogDecisions)
                Debug.Print("** bestComputerChoices count={0}, maxScoreAfterHumanTurn={1:+#;-#;+0}.  Choose the highest scoring Move.", bestComputerChoices.Count, maxComputerScoreAfterHumansBestResponse);

            // find finalComputerChoices from bestComputerChoices based on computerChoiceScore
            int maxComputerScore = -int.MaxValue;
            List<Coord> finalComputerChoices = new List<Coord>();
            foreach (Coord computerChoice in bestComputerChoices)
            {
                BoardState computerBoardState = BoardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogDecisions)
                    Debug.Print("Top Computer choice: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}",
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

            return finalComputerChoices;
        }

        private int findHumansBestResponseScore(BoardState computerBoardState)
        {
            int minScoreAfterHumanTurn = int.MaxValue;
            //List<Coord> bestHumanResponses = new List<Coord>();
            Coord? bestHumanResponse = null;
            BoardState bestHumanResponseBoardState = null;

            List<Coord> legalHumanMoves = computerBoardState.LegalMoves();
            foreach (Coord humanResponse in legalHumanMoves)
            {
                BoardState humanResponseBoardState = computerBoardState.Clone();
                humanResponseBoardState.PlacePieceAndFlipPiecesAndChangeTurns(humanResponse);
                int humanResponseScore = ScoreBoard(humanResponseBoardState);
                //if (LogDecisions) Debug.Print("    - Human choice: {0} resulting Score={1:+#;-#;+0}\nresulting BoardState:{2}", humanChoice, humanChoiceScore, humanBoardState);

                if (humanResponseScore < minScoreAfterHumanTurn) // remember minScoreAfterHumanTurn and start a new List of Moves that attain it
                {
                    minScoreAfterHumanTurn = humanResponseScore;
                    bestHumanResponse = humanResponse;
                    bestHumanResponseBoardState = humanResponseBoardState;
                    //bestHumanResponses = new List<Coord>();
                }

                /*if (humanResponseScore <= minScoreAfterHumanTurn) // add choice to maxScoringChoices
                {
                    if (!bestHumanResponses.Contains(humanResponse))
                        bestHumanResponses.Add(humanResponse);
                }*/
            }

            if (LogDecisions)
                Debug.Print("    - Human response: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}",
                        computerBoardState.WhitesTurn ? 'W' : 'B', bestHumanResponse, minScoreAfterHumanTurn, bestHumanResponseBoardState);

            return minScoreAfterHumanTurn;
        }

        private List<Coord> chooseHighestScoringAfterSeveralTurns()
        {
            int maxComputerScoreAfterSeveralTurns = -int.MaxValue;
            List<Coord> bestComputerChoices = new List<Coord>();
            // loop through all of Computer's Legal Moves
            // collect the ones that maximize Score after several Turns
            List<Coord> legalComputerMoves = BoardState.LegalMoves();
            foreach (Coord computerChoice in legalComputerMoves)
            {
                BoardState computerBoardState = BoardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogDecisions)
                    Debug.Print(" - Computer choice: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}", 
                            BoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);

                int minMaxScoreAfterSeveralTurns = findMinMaxScore(computerBoardState, EXPERT_DEPTH);

                if (minMaxScoreAfterSeveralTurns > maxComputerScoreAfterSeveralTurns) // remember maxComputerScoreAfterHumansBestResponse and start a new List of Moves that attain it
                {
                    maxComputerScoreAfterSeveralTurns = minMaxScoreAfterSeveralTurns;
                    bestComputerChoices = new List<Coord>();
                }

                if (minMaxScoreAfterSeveralTurns >= maxComputerScoreAfterSeveralTurns) // add choice to bestComputerChoices
                {
                    if (!bestComputerChoices.Contains(computerChoice))
                        bestComputerChoices.Add(computerChoice);
                }
            }

            if (LogDecisions)
                Debug.Print("** bestComputerChoices count={0}, maxComputerScoreAfterSeveralTurns={1:+#;-#;+0}.  Choose the highest scoring Move.", bestComputerChoices.Count, maxComputerScoreAfterSeveralTurns);

            // find finalComputerChoices from bestComputerChoices based on computerChoiceScore
            int maxComputerScore = -int.MaxValue;
            List<Coord> finalComputerChoices = new List<Coord>();
            foreach (Coord computerChoice in bestComputerChoices)
            {
                BoardState computerBoardState = BoardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogDecisions)
                    Debug.Print("Top Computer choice: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}", 
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

            return finalComputerChoices;
        }

        private int findMinMaxScore(BoardState boardState, int depth)
        {
            bool myTurn = boardState.WhitesTurn ^ !AmIWhite;
            int minMaxScore = myTurn ? -int.MaxValue : int.MaxValue;
            Coord minMaxResponse = new Coord();
            BoardState minMaxResponseBoardState = null;

            List<Coord> legalMoves = boardState.LegalMoves();
            if (legalMoves.Count == 0) // game over
            {
                return ScoreEndOfGame(boardState);
            }

            foreach (Coord response in legalMoves)
            {
                BoardState responseBoardState = boardState.Clone();
                responseBoardState.PlacePieceAndFlipPiecesAndChangeTurns(response);

                int responseScore;
                if (responseBoardState.endOfGame)
                {
                    responseScore = ScoreEndOfGame(responseBoardState);
                }
                else
                {
                    responseScore = ScoreBoard(responseBoardState);
                }
                //if (LogDecisions) Debug.Print("    - Human choice: {0} resulting Score={1:+#;-#;+0}\nresulting BoardState:{2}", humanChoice, humanChoiceScore, humanBoardState);

                if (myTurn)
                {
                    if (responseScore > minMaxScore) // my Turn goes for highest Score for me
                    {
                        minMaxScore = responseScore;
                        minMaxResponse = response;
                        minMaxResponseBoardState = responseBoardState;
                    }
                }
                else
                {
                    if (responseScore < minMaxScore) // opponent's Turn chooses lowest Score for me
                    {
                        minMaxScore = responseScore;
                        minMaxResponse = response;
                        minMaxResponseBoardState = responseBoardState;
                    }
                }
            }

            if (LogDecisions)
                Debug.Print("- response {0}={1}->{2}: resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}", 
                        depth, boardState.WhitesTurn?'W':'B', minMaxResponse, minMaxScore, minMaxResponseBoardState);

            if (depth == 0)
                return minMaxScore;

            int levelsLeft = depth - 1;
            if (minMaxResponseBoardState.WhitesTurn == boardState.WhitesTurn) // turn skipped due to no legal moves
                levelsLeft--; // depth should go down to same Player to compare equally

            // recurse to return resulting minMaxScore after levelsLeft more Turns
            return findMinMaxScore(minMaxResponseBoardState, levelsLeft);
        }

        private int ScoreEndOfGame(BoardState boardState)
        {
            int endOfGameScore;
            const int MULTIPLIER = 10;
            if (AmIWhite)
                endOfGameScore = MULTIPLIER * (boardState.WhiteCount - boardState.BlackCount);
            else
                endOfGameScore = MULTIPLIER * (boardState.BlackCount - boardState.WhiteCount);

            return endOfGameScore;
        }

        /// <summary>
        /// calculates a Score for a BoardState
        /// uses WeightedCoordValue()
        /// uses difference between Computer's Score & Human's Score
        /// </summary>
        /// <param name="boardState">BoardState to caluclate Score for</param>
        /// <returns>weighted Score of boardState</returns>
        private int ScoreBoard(BoardState boardState)
		{
            const int numEmptyToConsiderBoardMostlyFilled = 3;
            bool boardMostlyFilled = BoardState.EmptyCount <= numEmptyToConsiderBoardMostlyFilled;

            int score = 0;
            foreach (Coord coord in boardState)
            { 
				Square square = boardState.GetSquare(coord);
				if (square.State != StateEnum.White && square.State != StateEnum.Black)
					continue;
				int weightedCoordValue;
                if (Level == LevelEnum.Expert && boardMostlyFilled)
                    weightedCoordValue = 1; // after board is mostly full, Expert should just try to win the game
                else
                    weightedCoordValue = WeightedCoordValue(coord);

                if (AmIWhite) // Computer is White
				{
					switch (square.State)
					{
						case StateEnum.White:
							score += weightedCoordValue;
							break;
						case StateEnum.Black:
							score -= weightedCoordValue;
							break;
					}
				}
				else // Computer is Black
				{
                    switch (square.State)
                    {
                        case StateEnum.Black:
                            score += weightedCoordValue;
                            break;
                        case StateEnum.White:
                            score -= weightedCoordValue;
                            break;
                    }
                }
			}
			return score;
		}

        /// <summary>
        /// returns a weighted value for a Coord
		/// Beginner values all Coords equally
		/// higher Levels weight Coord values by location as more valuable or dangerous
        /// </summary>
        /// <param name="coord">Coord to get weighted Score of</param>
        /// <returns>weighted Score of coord</returns>
        private int WeightedCoordValue(Coord coord)
		{
			switch (Level)
			{
                // Beginner Level scores each square as 1
                case LevelEnum.Beginner:
                default:
					return 1;

                // Higher Levels value Corners highest, then Ends.  It devalues Squares before Corners & Ends since they lead to Opponent getting Corners & Ends.
				case LevelEnum.Intermediate:
                case LevelEnum.Advanced:
                case LevelEnum.Expert:
                    switch (coord.x)
					{
						case 1:
						case 8:
							switch (coord.y)
							{
								case 1:
								case 8:
									return 50;
								case 2:
								case 7:
									return -5;
								case 3:
								case 6:
									return 5;
								default:
									return 10;
							}
						case 2:
						case 7:
							switch (coord.y)
							{
								case 1:
								case 8:
									return 10;
								case 2:
								case 7:
									return -10;
								case 3:
								case 6:
									return 2;
								default:
									return -5;
							}
						case 3:
						case 6:
							switch (coord.y)
							{
								case 1:
								case 8:
									return 10;
								case 2:
								case 7:
									return -5;
								case 3:
								case 6:
									return 4;
								default:
									return 2;
							}
						default:
							switch (coord.y)
							{
								case 1:
								case 8:
									return 10;
								case 2:
								case 7:
									return -5;
								case 3:
								case 6:
									return 2;
								default:
									return 1;
							}
					}
			}
		}
    }
}
