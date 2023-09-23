using System;
using System.Collections.Generic;
using System.Diagnostics;

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
		private Random random = new Random();

		public ComputerPlayer() { }

		/// <summary>
		/// returns ComputerPlayer's choice for next move
		/// </summary>
		public Coord ChooseNextMove()
		{
            List<Coord> choices = new List<Coord>();

            switch (Level)
			{
				case LevelEnum.Beginner:
				case LevelEnum.Intermediate:
				default:
                    choices = chooseHighestScoringMove();
					break;

                case LevelEnum.Advanced:
                    choices = chooseLowestScoringOpponentMove();
					break;

                /*case LevelEnum.Expert:
                    scoreAllSquares(BoardState, Level == LevelEnum.Advanced ? 2 : 0, true, out choice);
					break;*/
            }

            // randomly pick one of the choices
            int randomIndex = random.Next(choices.Count);
			Coord choice = choices[randomIndex];
            Debug.Print("chose {0}", choice);
            return choice;
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
				computerBoardState.PlacePieceAndFlipPieces(computerChoice);
				int computerChoiceScore = ScoreBoard(computerBoardState);
				Debug.Print("Computer choice: {0} computerScore={1} computerBoardState={2}", computerChoice, computerChoiceScore, computerBoardState);

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
        private List<Coord> chooseLowestScoringOpponentMove()
        {
            int maxScoreAfterHumanTurn = -int.MaxValue;
            List<Coord> bestComputerChoices = new List<Coord>();

            // loop through all of Computer's Legal Moves
            List<Coord> legalComputerMoves = BoardState.LegalMoves();
			foreach (Coord computerChoice in legalComputerMoves)
			{
				BoardState computerBoardState = BoardState.Clone();
				computerBoardState.PlacePieceAndFlipPieces(computerChoice);
				int computerChoiceScore = ScoreBoard(computerBoardState);
				Debug.Print(" - Computer choice: {0} computerScore={1} computerBoardState={2}", computerChoice, computerChoiceScore, computerBoardState);

                List<Coord> legalHumanMoves = BoardState.LegalMoves();
				foreach (Coord humanChoice in legalHumanMoves)
				{
					if (computerBoardState.IsLegalMove(humanChoice))
					{
						BoardState humanBoardState = BoardState.Clone();
						humanBoardState.PlacePieceAndFlipPieces(humanChoice);
						int humanChoiceScore = ScoreBoard(humanBoardState);
						Debug.Print("    - Human choice: {0} humanChoiceScore={1} humanBoardState={2}", humanChoice, humanChoiceScore, humanBoardState);

						if (humanChoiceScore > maxScoreAfterHumanTurn) // remember maxScore and start a new List of Moves that attain it
						{
							maxScoreAfterHumanTurn = humanChoiceScore;
							bestComputerChoices = new List<Coord>();
						}

						if (humanChoiceScore >= maxScoreAfterHumanTurn) // add choice to maxScoringChoices
						{
							bestComputerChoices.Add(computerChoice);
						}
					}
				}

                Debug.Print("    - maxScoreAfterHumanTurn={0}", maxScoreAfterHumanTurn);
            }

            Debug.Print("maxScoreAfterHumanTurn={0}", maxScoreAfterHumanTurn);

            // find finalComputerChoices from bestComputerChoices based on computerChoiceScore
            int maxComputerScore = -int.MaxValue;
            List<Coord> finalComputerChoices = new List<Coord>();
            foreach (Coord computerChoice in bestComputerChoices)
			{
                BoardState computerBoardState = BoardState.Clone();
                computerBoardState.PlacePieceAndFlipPieces(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                Debug.Print("Top Computer choice: {0} computerScore={1} computerBoardState={2}", computerChoice, computerChoiceScore, computerBoardState);
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

/*
		/// <summary>
        /// loops through all Legal Moves
		/// https://en.wikipedia.org/wiki/Minimax#Pseudocode
        /// </summary>
        /// <param name="boardState">recursive Board State</param>
        /// <param name="depth">how many times to recurse through minimax</param>
        /// <param name="maximizing">whether to Maximize or Minimize Score</param>
        /// <param name="minimaxChoice">Choice that minimizes/maximizes score</param>
        /// <returns>minimaxScore</returns>
        private int scoreAllSquares(BoardState boardState, int depth, bool maximizing, out Coord minimaxChoice)
        {
			minimaxChoice = new Coord();
            int minimaxScore = maximizing ? -int.MaxValue : int.MaxValue;

			// loop through all Legal Moves
            for (int x = 1; x <= 8; x++)
			{
				for (int y = 1; y <= 8; y++)
				{
					Coord choice = new Coord(x, y);
					if (BoardState.IsLegalMove(choice))
					{
						BoardState newBoardState = boardState.Clone();
						newBoardState.PlacePieceAndFlipPieces(choice);
						int score = ScoreBoard(newBoardState);
                        Debug.Print("choice: {0} score={1} newBoardState={2}", choice, score, newBoardState);

                        if (score > minimaxScore || (score == minimaxScore && random.NextDouble() > 0.5))
                        {
                            minimaxChoice.x = x;
                            minimaxChoice.y = y;
                            minimaxScore = score;
                        }

                        /*int squareScore;
						if (depth == 0)
						{
							squareScore = Score(choice);
						}
						else
						{
							Coord miniMaxChoice;
							squareScore = scoreAllSquares(, !maximizing, depth - 1, out miniMaxChoice);
						}

                        //Debug.Print("legal: {0} score={1}", choice, squareScore);

                        if (maximizing) // maximizing ComputerPlayer's Score
                        {
							if (squareScore > minimaxScore || (squareScore == minimaxScore && random.NextDouble() > 0.5))
							{
								minimaxChoice.x = x;
								minimaxChoice.y = y;
								minimaxScore = squareScore;
                            }
                        }
                        else // minimizing Human's Score
                        {
							if (squareScore < minimaxScore || (squareScore == minimaxScore && random.NextDouble() > 0.5))
							{
                                minimaxChoice.x = x;
                                minimaxChoice.y = y;
                                minimaxScore = squareScore;
                            }
                        }
                    }
                }
			}

            Debug.Print("choosing: {0} score={1}", minimaxChoice, minimaxScore);
            return minimaxScore;
        }
*/

		/// <summary>
		/// calculates a Score for a BoardState
		/// uses WeightedCoordValue()
		/// uses difference between Computer's Score & Human's Score
		/// </summary>
		/// <param name="boardState">BoardState to caluclate Score for</param>
		/// <returns>weighted Score of boardState</returns>
		private int ScoreBoard(BoardState boardState)
		{
			int score = 0;
			for (int x = 1; x <= 8; x++)
			{
				for (int y = 1; y <= 8; y++)
				{
					Coord coord = new Coord(x, y);
					Square square = boardState.GetSquare(coord);
					int weightedCoordValue = WeightedCoordValue(coord);

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
					return 1;

                // Higher Levels value Corners highest, then Ends.  It devalues Squares before Corners & Ends since they lead to Opponent getting Corners & Ends.
                default:
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
						case 5:
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
