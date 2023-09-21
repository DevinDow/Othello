using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Othello
{
	public class BoardState
	{
		// Fields
		public Square[,] squares;
		public bool WhitesTurn;

        // Constructor
        public BoardState()
        {
            squares = new Square[8, 8];
            WhitesTurn = false;

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    squares[i, j] = new Square(i, j);
                }

            squares[3, 3].State = StateEnum.Black;
            squares[4, 4].State = StateEnum.Black;
            squares[3, 4].State = StateEnum.White;
            squares[4, 3].State = StateEnum.White;
        }

        // Methods
		public BoardState copy()
		{
			BoardState newBoardState = new BoardState();
            newBoardState.squares = new Square[8, 8];
            newBoardState.WhitesTurn = WhitesTurn;

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    newBoardState.squares[i, j] = new Square(i, j, squares[i, j].State);
                }

			return newBoardState;
        }

        public bool IsLegalMoveAvailable()
		{
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
					if (IsLegalMove(i, j))
						return true;

			return false;
		}

		public bool IsLegalMove(int row, int column)
		{
			if (squares[row, column].State == StateEnum.Black || squares[row, column].State == StateEnum.White)
				return false;

			if (IsSuccessfulDirection(row, column, -1, 0))
				return true;

			if (IsSuccessfulDirection(row, column, -1, 1))
				return true;

			if (IsSuccessfulDirection(row, column, 0, 1))
				return true;

			if (IsSuccessfulDirection(row, column, 1, 1))
				return true;

			if (IsSuccessfulDirection(row, column, 1, 0))
				return true;

			if (IsSuccessfulDirection(row, column, 1, -1))
				return true;

			if (IsSuccessfulDirection(row, column, 0, -1))
				return true;

			if (IsSuccessfulDirection(row, column, -1, -1))
				return true;

			return false;
		}

		private bool IsSuccessfulDirection(int row, int column, int dx, int dy)
		{
			bool foundOpposite = false;

			row += dx;
			column += dy;

			while (row >= 0 && row < 8 && column >= 0 && column < 8)
			{
				if (squares[row, column].State != StateEnum.Black && squares[row, column].State != StateEnum.White)
					return false;

				if (foundOpposite)
				{
					if (squares[row, column].State == StateEnum.White && WhitesTurn ||
						squares[row, column].State == StateEnum.Black && !WhitesTurn)
						return true;
				}
				else
				{
					if (squares[row, column].State == StateEnum.White && !WhitesTurn ||
						squares[row, column].State == StateEnum.Black && WhitesTurn)
						foundOpposite = true;
					else
						return false;
				}

				row += dx;
				column += dy;
			}

			return false;
		}

		// Overrides
		public override string ToString()
		{
			string s = string.Empty;
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
				{
					if (squares[i, j].State != StateEnum.Empty)
					{
						s += string.Format("{0},{1}={2}, ", i, j, squares[i, j].State == StateEnum.Black ? "B" : "W");
					}
				}
			return s;
		}
	}

    public class Board
	{
		public BoardState boardState;
		private BoardState previousState;
		private int leftMarginDimension, topMarginDimension, sideDimension;
		public static int squareDimension;

		public MainForm mainForm;
		private Timer computersTurnTimer = null;
		public const int flipDelay = 100;
		public static bool cancelFlipping = false;
		
		public ComputerPlayer ComputerPlayer = null;

		public int WhiteCount 
		{
			get
			{
				int count = 0;
				for (int row=0; row<8; row++)
					for (int column=0; column<8; column++)
						if (boardState.squares[row,column].State == StateEnum.White)
							count++;
				return count;
			}
		}

		public int BlackCount 
		{
			get
			{
				int count = 0;
				for (int row=0; row<8; row++)
					for (int column=0; column<8; column++)
						if (boardState.squares[row,column].State == StateEnum.Black)
							count++;
				return count;
			}
		}

		public Board(MainForm mainForm)
		{
			this.mainForm = mainForm;
			ClearBoard();
			UpdateStatus();
		}

		public void ClearBoard()
		{
			boardState = new BoardState();
			previousState = null;
		}

		public void Undo()
		{
			if (previousState == null)
				return;

			if (computersTurnTimer != null)
				computersTurnTimer.Stop();
			cancelFlipping = true;

			boardState = previousState;
			previousState = null;

			UpdateStatus();
		}

		public void NewGame()
		{
			ClearBoard();
			UpdateStatus();

			if (ComputerPlayer != null && (ComputerPlayer.AmIWhite ^ !boardState.WhitesTurn))
			{
				int row, column;
				ComputerPlayer.Choose(out row, out column);
				MakeMove(row, column);
			}
		}

		public void UpdateBoardSize(int clientWidth, int clientHeight)
		{
			const int minMargin = 10;
			if (clientWidth < clientHeight)
				sideDimension = (clientWidth - 2*minMargin) / 8 * 8;
			else
				sideDimension = (clientHeight - 2*minMargin) / 8 * 8;

			leftMarginDimension = (clientWidth - sideDimension) / 2;
			topMarginDimension = (clientHeight - sideDimension) / 2;
		}

		public void Draw(Graphics g)
		{
			Brush boardBrush = new SolidBrush(Color.Green);
			g.FillRectangle(boardBrush, leftMarginDimension, topMarginDimension, sideDimension, sideDimension);

			Pen pen = new Pen(Color.Black);
			g.DrawRectangle(pen, leftMarginDimension, topMarginDimension, sideDimension, sideDimension);

			squareDimension = sideDimension / 8;
			for (int i=1; i<8; i++)
			{
				int x = leftMarginDimension + i*squareDimension;
				g.DrawLine(pen, x, topMarginDimension, x, topMarginDimension + sideDimension); 
			}
			for (int i=1; i<8; i++)
			{
				int y = topMarginDimension + i*squareDimension;
				g.DrawLine(pen, leftMarginDimension, y, leftMarginDimension + sideDimension, y); 
			}

			g.TranslateTransform(leftMarginDimension + squareDimension/2, topMarginDimension + squareDimension/2);
			for (int row=0; row<8; row++)
			{
				GraphicsState graphicsState = g.Save();

				for (int column=0; column<8; column++)
				{
                    boardState.squares[row,column].Draw(g);
					g.TranslateTransform(squareDimension, 0, MatrixOrder.Append);
				}

				g.Restore(graphicsState);

				g.TranslateTransform(0, squareDimension, MatrixOrder.Append);
			}
		}

		public void Click(System.Windows.Forms.MouseEventArgs e)
		{
			if (ComputerPlayer != null && (ComputerPlayer.AmIWhite ^ !boardState.WhitesTurn))
				return;

			Point pointOnBoard = new Point(e.X - leftMarginDimension, e.Y - topMarginDimension);

			if (pointOnBoard.X < 0 || pointOnBoard.Y < 0)
				return;

			int column = pointOnBoard.X / squareDimension;
			int row = pointOnBoard.Y / squareDimension;

			if (row > 7 || column > 7)
				return;

			if (!boardState.IsLegalMove(row, column))
			{
				System.Windows.Forms.MessageBox.Show("Illegal Move");
				return;
			}

			previousState = boardState;
			boardState = boardState.copy();

			MakeMove(row, column);
		}

		public void MakeMove(int row, int column)
		{
			if (boardState.WhitesTurn)
                boardState.squares[row,column].State = StateEnum.White;
			else
                boardState.squares[row,column].State = StateEnum.Black;

			Graphics g = mainForm.CreateGraphics();
			g.TranslateTransform(leftMarginDimension + squareDimension/2, topMarginDimension + squareDimension/2);
			g.TranslateTransform(column * squareDimension, row * squareDimension, MatrixOrder.Append);
            boardState.squares[row,column].Draw(g);

			cancelFlipping = false;
			FlipPieces(row, column);

			ChangeTurns();
		}

		public void ChangeTurns()
		{
			ClearLegalMoves();

            boardState.WhitesTurn = !boardState.WhitesTurn;

			if (!boardState.IsLegalMoveAvailable())
			{
                boardState.WhitesTurn = !boardState.WhitesTurn;

				if (boardState.IsLegalMoveAvailable())
				{
					System.Windows.Forms.MessageBox.Show("No legal moves available...  Skipping turn");
				}
				else
				{
					if (BlackCount > WhiteCount)
						System.Windows.Forms.MessageBox.Show(string.Format("Black Wins {0}-{1}", BlackCount, WhiteCount));
					else if (WhiteCount > BlackCount)
						System.Windows.Forms.MessageBox.Show(string.Format("White Wins {0}-{1}", WhiteCount, BlackCount));
					else
						System.Windows.Forms.MessageBox.Show("Tie");

					return;
				}
			}

			UpdateStatus();

			if (ComputerPlayer != null && (ComputerPlayer.AmIWhite ^ !boardState.WhitesTurn))
			{
				computersTurnTimer = new Timer();
				computersTurnTimer.Interval = flipDelay * 20;
				computersTurnTimer.Tick += new EventHandler(OnComputersTurn);
				computersTurnTimer.Start();
			}
		}

		public void OnComputersTurn(Object sender, EventArgs e)
		{
			computersTurnTimer.Stop();
			computersTurnTimer = null;

			int row, column;
			ComputerPlayer.Choose(out row, out column);
			MakeMove(row, column);
		}

		private void UpdateStatus()
		{
			if (boardState.WhitesTurn)
				mainForm.statusBarTurn.Text = "White's Turn";
			else
				mainForm.statusBarTurn.Text = "Black's Turn";

			mainForm.statusBarBlackScore.Text = string.Format("Black={0}", BlackCount); 
			mainForm.statusBarWhiteScore.Text = string.Format("White={0}", WhiteCount); 
		}

		private void FlipPieces(int row, int column)
		{
			FlipInDirection(row, column, 0, -1);
			FlipInDirection(row, column, -1, -1);
			FlipInDirection(row, column, -1, 0);
			FlipInDirection(row, column, -1, 1);
			FlipInDirection(row, column, 0, 1);
			FlipInDirection(row, column, 1, 1);
			FlipInDirection(row, column, 1, 0);
			FlipInDirection(row, column, 1, -1);
		}

		private void FlipInDirection(int originalRow, int originalColumn, int deltaRow, int deltaColumn)
		{
			int row = originalRow + deltaRow;
			int column = originalColumn + deltaColumn;

			// find partner piece
			while (row >= 0 && row < 8 && column >=0 && column < 8)
			{
				if (boardState.squares[row,column].State != StateEnum.Black && boardState.squares[row,column].State != StateEnum.White)
					return;

				if (boardState.WhitesTurn && boardState.squares[row,column].State == StateEnum.Black || 
					!boardState.WhitesTurn && boardState.squares[row,column].State == StateEnum.White) // not a partner piece
				{
					row += deltaRow;
					column += deltaColumn;
					continue;
				}

				// partner piece found
				
				row -= deltaRow;
				column -= deltaColumn;
				
				Graphics g = mainForm.CreateGraphics();
				g.TranslateTransform(leftMarginDimension + squareDimension/2, topMarginDimension + squareDimension/2);

				// work back to placed piece flipping
				while (!(row == originalRow && column == originalColumn))
				{
					if (boardState.WhitesTurn)
                        boardState.squares[row,column].State = StateEnum.White;
					else
                        boardState.squares[row,column].State = StateEnum.Black;

                    boardState.squares[row,column].Flip(g);

					row -= deltaRow;
					column -= deltaColumn;
				}

				return;
			}
		}
			
		public void ShowLegalMoves()
		{
			for (int row=0; row<8; row++)
				for (int column=0; column<8; column++)
					if (boardState.IsLegalMove(row, column))
                        boardState.squares[row,column].State = StateEnum.LegalMove;
		}

		private void ClearLegalMoves()
		{
			Graphics g = mainForm.CreateGraphics();
			g.TranslateTransform(leftMarginDimension + squareDimension/2, topMarginDimension + squareDimension/2);

			for (int row=0; row<8; row++)
				for (int column=0; column<8; column++)
					if (boardState.squares[row,column].State == StateEnum.LegalMove)
					{
						GraphicsState graphicsState = g.Save();

						g.TranslateTransform(column * squareDimension, row * squareDimension, MatrixOrder.Append);

                        boardState.squares[row,column].State = StateEnum.Empty;
                        boardState.squares[row,column].Draw(g);

						g.Restore(graphicsState);
					}
		}
	}
}
