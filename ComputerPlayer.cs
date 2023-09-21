using System;

namespace Othello
{
	public enum LevelEnum {Beginner, Intermediate, Advanced}

	public class ComputerPlayer
	{
		public bool AmIWhite;
		public Board Board;
		public LevelEnum Level;
		private Random random = new Random();

		public ComputerPlayer() {}

		public void Choose(out int row, out int column)
		{
			int maxScore = -100;
			row = column = -1;
			
			for (int i=0; i<8; i++)
			{
				for (int j=0; j<8; j++)
				{
					int squareScore = Score(i, j); 
					if (Board.IsLegalMove(i, j) && (squareScore > maxScore || (squareScore == maxScore && random.NextDouble() > 0.5)))
					{
						row = i;
						column = j;
						maxScore = squareScore;
					}
				}
			}
		}

		private int Score(int row, int column)
		{
			int score = ScoreSquare(row, column);

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

		private int ScoreInDirection(int originalRow, int originalColumn, int deltaRow, int deltaColumn)
		{
			int score = 0;

			int row = originalRow + deltaRow;
			int column = originalColumn + deltaColumn;

			while (row >= 0 && row < 8 && column >=0 && column < 8)
			{
				if (Board.squares[row,column].State == StateEnum.Empty)
					return score;

				if (AmIWhite && Board.squares[row,column].State == StateEnum.Black || 
					!AmIWhite && Board.squares[row,column].State == StateEnum.White)
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

		private int ScoreSquare(int row, int column)
		{
			switch (Level)
			{
				case LevelEnum.Beginner:
				default:
					return 1;
				case LevelEnum.Intermediate:
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
