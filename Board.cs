using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Othello
{
	public class BoardState
	{
		// Fields
		public Square[,] squares; // 8x8 Array ( 0..7 , 0..7 )
		public bool WhitesTurn;

        // Constructor
        public BoardState()
        {
            squares = new Square[8, 8];
            WhitesTurn = false;

            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    squares[x, y] = new Square(x, y);
                }

            squares[3, 3].State = StateEnum.Black;
            squares[4, 4].State = StateEnum.Black;
            squares[3, 4].State = StateEnum.White;
            squares[4, 3].State = StateEnum.White;
        }

        // Methods
        public Square GetSquare(Coord coord)
        {
            return squares[coord.x - 1, coord.y - 1];
        }

		public void SetSquare(Coord coord, Square square)
		{
			squares[coord.x - 1, coord.y - 1] = square;
        }

        public bool IsLegalMoveAvailable()
		{
			for (int x = 1; x <= 8; x++)
				for (int y = 1; y <= 8; y++)
					if (IsLegalMove(new Coord(x, y)))
						return true;

			return false;
		}

		public bool IsLegalMove(Coord coord)
		{
			Square square = GetSquare(coord);

            if (square.State == StateEnum.Black || square.State == StateEnum.White)
				return false;

			if (IsSuccessfulDirection(coord, -1, 0))
				return true;

			if (IsSuccessfulDirection(coord, -1, 1))
				return true;

			if (IsSuccessfulDirection(coord, 0, 1))
				return true;

			if (IsSuccessfulDirection(coord, 1, 1))
				return true;

			if (IsSuccessfulDirection(coord, 1, 0))
				return true;

			if (IsSuccessfulDirection(coord, 1, -1))
				return true;

			if (IsSuccessfulDirection(coord, 0, -1))
				return true;

			if (IsSuccessfulDirection(coord, -1, -1))
				return true;

			return false;
		}

		private bool IsSuccessfulDirection(Coord coord, int dx, int dy)
		{
			bool foundOpposite = false;

			int x = coord.x + dx;
            int y = coord.y + dy;

			while (x > 0 && x <= 8 && y > 0 && y <= 8)
			{
				Square square = GetSquare(new Coord(x, y));
				if (square.State != StateEnum.Black && square.State != StateEnum.White)
					return false;

				if (foundOpposite)
				{
					if (square.State == StateEnum.White && WhitesTurn ||
						square.State == StateEnum.Black && !WhitesTurn)
						return true;
				}
				else
				{
					if (square.State == StateEnum.White && !WhitesTurn ||
						square.State == StateEnum.Black && WhitesTurn)
						foundOpposite = true;
					else
						return false;
				}

				x += dx;
                y += dy;
			}

			return false;
		}

        /// <summary>
        /// returns a Deep Copy
        /// </summary>
        /// <returns>a Deep Copy of this</returns>
        public BoardState Clone()
        {
            BoardState newBoardState = new BoardState();
            newBoardState.squares = new Square[8, 8];
            newBoardState.WhitesTurn = WhitesTurn;

            for (int x = 1; x <= 8; x++)
                for (int y = 1; y <= 8; y++)
                {
                    Coord coord = new Coord(x, y);
                    Square square = GetSquare(coord);
                    newBoardState.SetSquare(coord, new Square(x, y, square.State));
                }

            return newBoardState;
        }

        // Overrides
        public override string ToString()
		{
			string s = string.Empty;
			for (int x = 1; x <= 8; x++)
				for (int y = 1; y <= 8; y++)
				{
					Square square = GetSquare(new Coord(x, y));
					if (square.State != StateEnum.Empty)
					{
						s += string.Format("({0},{1})={2}, ", x, y, square.State == StateEnum.Black ? "B" : "W");
					}
				}
			return s;
		}
    }

	/// <summary>
	/// Coord of Squares ( 1..8 , 1..8 )
	/// </summary>
    public class Coord
    {
        public int x, y;

        public Coord()
        {
            x = -1;
            y = -1;
        }

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }
    }

    public class Board
	{
		// Fields
		public BoardState boardState;
		private BoardState previousState;
		private int leftMarginDimension, topMarginDimension, sideDimension;
		public static int squareDimension;

		public MainForm mainForm;
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


		// Constructor
		public Board(MainForm mainForm)
		{
			this.mainForm = mainForm;
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
			for (int x=1; x<=8; x++)
			{
				GraphicsState graphicsState = g.Save();

				for (int y=1; y<=8; y++)
				{
					Square square = boardState.GetSquare(new Coord(x, y));
					square.Draw(g);
					g.TranslateTransform(0, squareDimension, MatrixOrder.Append);
				}

				g.Restore(graphicsState);

				g.TranslateTransform(squareDimension, 0, MatrixOrder.Append);
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

			Graphics g = mainForm.CreateGraphics();
			g.TranslateTransform(leftMarginDimension + squareDimension/2, topMarginDimension + squareDimension/2);
			g.TranslateTransform((coord.x-1) * squareDimension, (coord.y-1) * squareDimension, MatrixOrder.Append);
            square.Draw(g);

			cancelFlipping = false;
			FlipPieces(coord);

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
				mainForm.statusBarTurn.Text = "White's Turn";
			else
				mainForm.statusBarTurn.Text = "Black's Turn";

			mainForm.statusBarBlackScore.Text = string.Format("Black={0}", BlackCount); 
			mainForm.statusBarWhiteScore.Text = string.Format("White={0}", WhiteCount); 
		}

		private void FlipPieces(Coord coord)
		{
			FlipInDirection(coord, 0, -1);
			FlipInDirection(coord, -1, -1);
			FlipInDirection(coord, -1, 0);
			FlipInDirection(coord, -1, 1);
			FlipInDirection(coord, 0, 1);
			FlipInDirection(coord, 1, 1);
			FlipInDirection(coord, 1, 0);
			FlipInDirection(coord, 1, -1);
		}

		private void FlipInDirection(Coord choice, int dx, int dy)
		{
            int x = choice.x + dx;
            int y = choice.y + dy;

			// find partner square
			while (x >= 1 && x <= 8 && y >= 1 && y <= 8)
			{
				Square partnerSquare = boardState.GetSquare(new Coord(x, y));
				if (partnerSquare.State != StateEnum.Black && partnerSquare.State != StateEnum.White)
					return;

				if (boardState.WhitesTurn && partnerSquare.State == StateEnum.Black || 
					!boardState.WhitesTurn && partnerSquare.State == StateEnum.White) // not a partner piece
				{
					x += dx;
					y += dy;
					continue;
				}

				// partner square found
				x -= dx;
				y -= dy;
				
				Graphics g = mainForm.CreateGraphics();
				g.TranslateTransform(leftMarginDimension + squareDimension/2, topMarginDimension + squareDimension/2);

				// work back to placed piece flipping
				while (!(x == choice.x && y == choice.y))
				{
                    Square flippedSquare = boardState.GetSquare(new Coord(x, y));
                    if (boardState.WhitesTurn)
                        flippedSquare.State = StateEnum.White;
					else
                        flippedSquare.State = StateEnum.Black;

                    flippedSquare.Flip(g);

					x -= dx;
					y -= dy;
				}

				return;
			}
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
			Graphics g = mainForm.CreateGraphics();
			g.TranslateTransform(leftMarginDimension + squareDimension/2, topMarginDimension + squareDimension/2);

			for (int x=0; x<8; x++)
				for (int y=0; y<8; y++)
					if (boardState.squares[x,y].State == StateEnum.LegalMove)
					{
						GraphicsState graphicsState = g.Save();

						g.TranslateTransform(x * squareDimension, y * squareDimension, MatrixOrder.Append);

						Square square = boardState.GetSquare(new Coord(x, y));
                        square.State = StateEnum.Empty;
                        square.Draw(g);

						g.Restore(graphicsState);
					}
		}
	}
}
