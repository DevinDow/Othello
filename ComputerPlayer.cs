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
            for (int x = 1; x <= 8; x++)
			{
				for (int y = 1; y <= 8; y++)
				{
					Coord choice = new Coord(x, y);
					if (Board.boardState.IsLegalMove(choice))
					{
						int squareScore;
						if (depth == 0)
						{
							squareScore = Score(choice);
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
        /// <param name="choice"></param>
        /// <returns>score for specified Move</returns>
        private int Score(Coord choice)
		{
			// Score this Square
			int score = ScoreSquare(choice);

			// Score all Squares flipped in every direction
			score += ScoreInDirection(choice, 0, -1);
			score += ScoreInDirection(choice, -1, -1);
			score += ScoreInDirection(choice, -1, 1);
			score += ScoreInDirection(choice, 0, 1);
			score += ScoreInDirection(choice, -1, 0);
			score += ScoreInDirection(choice, 1, 1);
			score += ScoreInDirection(choice, 1, 0);
			score += ScoreInDirection(choice, 1, -1);

			return score;
		}

        /// <summary>
        /// returns an integer Score for all Squares flipped in a direction specified by deltaRow & deltaColumn
        /// </summary>
        /// <param name="choice"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        private int ScoreInDirection(Coord choice, int dx, int dy)
		{
			int score = 0;

			int x = choice.x + dx;
			int y = choice.y + dy;

			while (x >= 1 && x <= 8 && y >=1 && y <= 8)
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
				
				while (!(x == choice.x && y == choice.y))
				{
					score += ScoreSquare(new Coord(x, y));

					x -= dx;
					y -= dy;
				}

				return score;
			}

			return score;
		}

        /// <summary>
        /// returns a number value for a Square
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        private int ScoreSquare(Coord coord)
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
							case 5:
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
							case 5:
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
							case 5:
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
							case 5:
								return 2;
							default:
								return 1;
						}
				}
			}
		}	
	}
}
