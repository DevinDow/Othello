using System;
using System.Diagnostics;

namespace Othello
{
	public enum LevelEnum {Beginner, Intermediate, Advanced}

	public class ComputerPlayer
	{
		public bool AmIWhite;
		public Board Board;
		public LevelEnum Level;
		private Random random = new Random();

		public ComputerPlayer() { }

		/// <summary>
		/// returns ComputerPlayer's choice for next move
		/// </summary>
		public Coord Choose()
		{
			Coord choice;
			scoreAllSquares(Board.boardState, true, Level == LevelEnum.Advanced ? 2 : 0, out choice);
			return choice;
        }

        /// <summary>
        /// loops through all Legal Moves
		/// https://en.wikipedia.org/wiki/Minimax#Pseudocode
        /// </summary>
        /// <param name="boardState">recursive Board State</param>
        /// <param name="maximizing">whether to Maximize or Minimize Score</param>
        /// <param name="depth">how many times to recurse through minimax</param>
        /// <param name="minimaxChoice">Choice that minimizes/maximizes score</param>
        /// <returns>minimaxScore</returns>
        private int scoreAllSquares(BoardState boardState, bool maximizing, int depth, out Coord minimaxChoice)
        {
			minimaxChoice = new Coord();
            int minimaxScore = maximizing ? -int.MaxValue : int.MaxValue;

			// loop through all Legal Moves
            for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					Coord choice = new Coord(x, y);
					if (Board.boardState.IsLegalMove(choice))
					{
						int squareScore;
						if (depth == 0)
						{
							squareScore = Score(choice.x, choice.y);
						}
						else
						{
							Coord miniMaxChoice;
							squareScore = scoreAllSquares((BoardState)boardState.Clone(), !maximizing, depth - 1, out miniMaxChoice);
						}

                        Debug.Print("legal: {0} score={1}", choice, squareScore);

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

        /// <summary>
        /// returns an integer Score for a Legal Move that sums the Scores for the chosen Square and all flipped Squares
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>score for specified Move</returns>
        private int Score(int x, int y)
		{
			// Score this Square
			int score = ScoreSquare(x, y);

			// Score all Squares flipped in every direction
			score += ScoreInDirection(x, y, 0, -1);
			score += ScoreInDirection(x, y, -1, -1);
			score += ScoreInDirection(x, y, -1, 0);
			score += ScoreInDirection(x, y, -1, 1);
			score += ScoreInDirection(x, y, 0, 1);
			score += ScoreInDirection(x, y, 1, 1);
			score += ScoreInDirection(x, y, 1, 0);
			score += ScoreInDirection(x, y, 1, -1);

			return score;
		}

		/// <summary>
		/// returns an integer Score for all Squares flipped in a direction specified by deltaRow & deltaColumn
		/// </summary>
		/// <param name="originalX"></param>
		/// <param name="originalY"></param>
		/// <param name="dx"></param>
		/// <param name="dy"></param>
		/// <returns></returns>
		private int ScoreInDirection(int originalX, int originalY, int dx, int dy)
		{
			int score = 0;

			int x = originalX + dx;
			int y = originalY + dy;

			while (x >= 0 && x < 8 && y >=0 && y < 8)
			{
				Square square = Board.boardState.GetSquare(new Coord(x, y));

                if (square.State == StateEnum.Empty)
					return score;

				if (AmIWhite && square.State == StateEnum.Black || 
					!AmIWhite && square.State == StateEnum.White)
				{
					x += dx;
					y += dy;
					continue;
				}
				
				x -= dx;
				y -= dy;
				
				while (!(x == originalX && y == originalY))
				{
					score += ScoreSquare(x, y);

					x -= dx;
					y -= dy;
				}

				return score;
			}

			return score;
		}

		/// <summary>
		/// returns a number value for a Square
		/// Beginner scores each square as 1
		/// Intermediate values Corners highest, then Ends.  It devalues Squares before Corners & Ends since they lead to Opponent getting Corners & Ends.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private int ScoreSquare(int x, int y)
		{
			switch (Level)
			{
				case LevelEnum.Beginner:
				default:
					return 1;
				case LevelEnum.Intermediate:
				case LevelEnum.Advanced:
				{
					switch (x)
					{
						case 0:
						case 7:
						switch (y)
						{
							case 0:
							case 7:
								return 50;
							case 1:
							case 6:
								return -5;
							case 2:
							case 4:
								return 5;
							default:
								return 10;
						}
						case 1:
						case 6:
						switch (y)
						{
							case 0:
							case 7:
								return 10;
							case 1:
							case 6:
								return -10;
							case 2:
							case 4:
								return 2;
							default:
								return -5;
						}
						case 2:
						case 4:
						switch (y)
						{
							case 0:
							case 7:
								return 10;
							case 1:
							case 6:
								return -5;
							case 2:
							case 4:
								return 4;
							default:
								return 2;
						}
						default:
						switch (y)
						{
							case 0:
							case 7:
								return 10;
							case 1:
							case 6:
								return -5;
							case 2:
							case 4:
								return 2;
							default:
								return 1;
						}
					}
				}
			}
		}	
	}
}
