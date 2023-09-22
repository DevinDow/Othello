using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Othello
{
	public class Board
	{
		// Fields
		public BoardState boardState;
		private BoardState previousState;
		private static int leftMarginDimension, topMarginDimension, sideDimension;
		public static int squareDimension;

		private Timer computersTurnTimer = null;
		public const int flipDelay = 100;
		public static bool cancelFlipping = false;

		public ComputerPlayer ComputerPlayer = null;


		// Properties
		public int WhiteCount
		{
			get
			{
				int count = 0;
				for (int row = 0; row < 8; row++)
					for (int column = 0; column < 8; column++)
						if (boardState.squares[row, column].State == StateEnum.White)
							count++;
				return count;
			}
		}

		public int BlackCount
		{
			get
			{
				int count = 0;
				for (int row = 0; row < 8; row++)
					for (int column = 0; column < 8; column++)
						if (boardState.squares[row, column].State == StateEnum.Black)
							count++;
				return count;
			}
		}


		// Constructor
		public Board()
		{
			ClearBoard();
			UpdateStatus();
		}


		// Methods
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
				MakeMove(ComputerPlayer.Choose());
			}
		}

		public void UpdateBoardSize(int clientWidth, int clientHeight)
		{
			const int minMargin = 10;
			if (clientWidth < clientHeight)
				sideDimension = (clientWidth - 2 * minMargin) / 8 * 8;
			else
				sideDimension = (clientHeight - 2 * minMargin) / 8 * 8;

			leftMarginDimension = (clientWidth - sideDimension) / 2;
			topMarginDimension = (clientHeight - sideDimension) / 2;
		}

		public static void SetupGraphics(Graphics g)
		{
			// Translate to middle of Square (1,1)
            g.TranslateTransform(leftMarginDimension + squareDimension / 2, topMarginDimension + squareDimension / 2);
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

			SetupGraphics(g);
			for (int y=1; y<=8; y++) // start at top row
			{
				GraphicsState graphicsState = g.Save();

				for (int x=1; x<=8; x++) // start at left column
				{
					Square square = boardState.GetSquare(new Coord(x, y));
					square.Draw(g);
					g.TranslateTransform(squareDimension, 0, MatrixOrder.Append); // next column
				}

				g.Restore(graphicsState);

				g.TranslateTransform(0, squareDimension, MatrixOrder.Append); // next row
			}
		}

		public void Click(System.Windows.Forms.MouseEventArgs e)
		{
			if (ComputerPlayer != null && (ComputerPlayer.AmIWhite ^ !boardState.WhitesTurn))
				return;

			Point pointOnBoard = new Point(e.X - leftMarginDimension, e.Y - topMarginDimension);

			if (pointOnBoard.X < 0 || pointOnBoard.Y < 0)
				return;

			Coord choice = new Coord(pointOnBoard.X / squareDimension + 1, pointOnBoard.Y / squareDimension + 1);

			if (choice.x > 8 || choice.y > 8)
				return;

			if (!boardState.IsLegalMove(choice))
			{
				System.Windows.Forms.MessageBox.Show("Illegal Move");
				return;
			}

			previousState = boardState;
			boardState = boardState.Clone();

			MakeMove(choice);
		}

		public void MakeMove(Coord coord)
		{
			Square square = boardState.GetSquare(coord);
			if (boardState.WhitesTurn)
                square.State = StateEnum.White;
			else
                square.State = StateEnum.Black;

			Graphics g = MainForm.instance.CreateGraphics();
			SetupGraphics(g);
			g.TranslateTransform((coord.x-1) * squareDimension, (coord.y-1) * squareDimension, MatrixOrder.Append);
            square.Draw(g);

			cancelFlipping = false;
            boardState.FlipPieces(coord);

			// animate flipped pieces
			Animation.coordsToFlip = boardState.coordsToFlip;
			Animation.newState = boardState.WhitesTurn ? StateEnum.White : StateEnum.Black;
            Animation.Animate();

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

			MakeMove(ComputerPlayer.Choose());
		}

		private void UpdateStatus()
		{
			if (boardState.WhitesTurn)
                MainForm.instance.statusBarTurn.Text = "White's Turn";
			else
                MainForm.instance.statusBarTurn.Text = "Black's Turn";

            MainForm.instance.statusBarBlackScore.Text = string.Format("Black={0}", BlackCount);
            MainForm.instance.statusBarWhiteScore.Text = string.Format("White={0}", WhiteCount); 
		}
		
		public void ShowLegalMoves()
		{
			for (int x = 1; x <= 8; x++)
				for (int y = 1; y <= 8; y++)
				{
					Coord choice = new Coord(x, y);
					if (boardState.IsLegalMove(choice))
						boardState.GetSquare(choice).State = StateEnum.LegalMove;
				}
		}

		private void ClearLegalMoves()
		{
			Graphics g = MainForm.instance.CreateGraphics();
			SetupGraphics(g);

			for (int x = 0; x < 8; x++)
				for (int y = 0; y < 8; y++)
                    if (boardState.squares[x, y].State == StateEnum.LegalMove) 
						boardState.squares[x, y].State = StateEnum.Empty;
/*					if (boardState.squares[x,y].State == StateEnum.LegalMove)
					{
						GraphicsState graphicsState = g.Save();

						g.TranslateTransform(x * squareDimension, y * squareDimension, MatrixOrder.Append);

						Square square = boardState.GetSquare(new Coord(x, y));
                        square.State = StateEnum.Empty;
                        square.Draw(g);

						g.Restore(graphicsState);
					}*/
        }
	}
}
