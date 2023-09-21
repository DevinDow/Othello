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
		public Choice Choose()
		{
			Choice choice;
			int depth = 0;
			if (Level == LevelEnum.Advanced)
				depth = 2;
			scoreAllSquares(Board.boardState, true, depth, out choice);
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
        private int scoreAllSquares(BoardState boardState, bool maximizing, int depth, out Choice minimaxChoice)
        {
			minimaxChoice = new Choice();
            int minimaxScore = maximizing ? -int.MaxValue : int.MaxValue;

			// loop through all Legal Moves
            for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Choice choice = new Choice(i, j);
					if (Board.boardState.IsLegalMove(choice.row, choice.column))
					{
						int squareScore;
						if (depth == 0)
						{
							squareScore = Score(choice.row, choice.column);
						}
						else
						{
							Choice miniMaxChoice;
							squareScore = scoreAllSquares((BoardState)boardState.Clone(), !maximizing, depth - 1, out miniMaxChoice);
						}

                        Debug.Print("legal: {0} score={1}", choice, squareScore);

                        if (maximizing) // maximizing ComputerPlayer's Score
                        {
							if (squareScore > minimaxScore || (squareScore == minimaxScore && random.NextDouble() > 0.5))
							{
								minimaxChoice.row = i;
								minimaxChoice.column = j;
								minimaxScore = squareScore;
                            }
                        }
                        else // minimizing Human's Score
                        {
							if (squareScore < minimaxScore || (squareScore == minimaxScore && random.NextDouble() > 0.5))
							{
                                minimaxChoice.row = i;
                                minimaxChoice.column = j;
                                minimaxScore = squareScore;
                            }
                        }
                    }
                }
			}

            Debug.Print("choosing {0} score={1}", minimaxChoice, minimaxScore);
            return minimaxScore;
        }

        /// <summary>
        /// returns an integer Score for a Legal Move that sums the Scores for the chosen Square and all flipped Squares
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns>score for specified Move</returns>
        private int Score(int row, int column)
		{
			// Score this Square
			int score = ScoreSquare(row, column);

			// Score all Squares flipped in every direction
			score += ScoreInDirection(row, column, 0, -1);
			score += ScoreInDirection(row, column, -1, -1);
			score += ScoreInDirection(row, column, -1, 0);
			score += ScoreInDirection(row, column, -1, 1);
			score += ScoreInDirection(row, column, 0, 1);
			score += ScoreInDirection(row, column, 1, 1);
			score += ScoreInDirection(row, column, 1, 0);
			score += ScoreInDirection(row, column, 1, -1);

			return score;
		}

		/// <summary>
		/// returns an integer Score for all Squares flipped in a direction specified by deltaRow & deltaColumn
		/// </summary>
		/// <param name="originalRow"></param>
		/// <param name="originalColumn"></param>
		/// <param name="deltaRow"></param>
		/// <param name="deltaColumn"></param>
		/// <returns></returns>
		private int ScoreInDirection(int originalRow, int originalColumn, int deltaRow, int deltaColumn)
		{
			int score = 0;

			int row = originalRow + deltaRow;
			int column = originalColumn + deltaColumn;

			while (row >= 0 && row < 8 && column >=0 && column < 8)
			{
				if (Board.boardState.squares[row,column].State == StateEnum.Empty)
					return score;

				if (AmIWhite && Board.boardState.squares[row,column].State == StateEnum.Black || 
					!AmIWhite && Board.boardState.squares[row,column].State == StateEnum.White)
				{
					row += deltaRow;
					column += deltaColumn;
					continue;
				}
				
				row -= deltaRow;
				column -= deltaColumn;
				
				while (!(row == originalRow && column == originalColumn))
				{
					score += ScoreSquare(row, column);

					row -= deltaRow;
					column -= deltaColumn;
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
		/// <param name="row"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		private int ScoreSquare(int row, int column)
		{
			switch (Level)
			{
				case LevelEnum.Beginner:
				default:
					return 1;
				case LevelEnum.Intermediate:
				case LevelEnum.Advanced:
				{
					switch (row)
					{
						case 0:
						case 7:
						switch (column)
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
						switch (column)
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
						switch (column)
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
						switch (column)
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
