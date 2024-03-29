using OthelloLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Othello
{
	public class Board
	{
		// Fields
		public BoardState boardState = new BoardState();
		private Stack<BoardState> previousStates = new Stack<BoardState>();
		private static int leftMarginDimension, topMarginDimension, sideDimension;
		public static int squareDimension;

		public ComputerPlayer ComputerPlayer = null;
        private Timer computerTurnDelayTimer = null;


		// Constructor
		public Board()
		{
			UpdateStatus();
		}


		// Methods
		public void Undo()
		{
			if (previousStates.Count == 0)
				return;

			if (computerTurnDelayTimer != null)
				computerTurnDelayTimer.Stop();
			Animation.cancelFlipping = true;

			boardState = previousStates.Pop();

			UpdateStatus();
		}

		public void NewGame()
		{
            boardState = new BoardState();
            previousStates = new Stack<BoardState>();
            UpdateStatus();

			if (ComputerPlayer != null && (ComputerPlayer.AmIWhite ^ !boardState.WhitesTurn))
			{
				Coord? choice = ComputerPlayer.ChooseNextMove(boardState);
                if (choice != null)
	                MakeMove(choice.Value);
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

			RedrawSquares();
		}

		public void RedrawSquares()
		{
            Graphics g = MainForm.instance.CreateGraphics();
            SetupGraphics(g);
            for (int y = 1; y <= 8; y++) // start at top row
            {
                GraphicsState graphicsState = g.Save();

                for (int x = 1; x <= 8; x++) // start at left column
                {
                    Square square = boardState.GetSquare(new Coord(x, y));
                    square.Draw(g, squareDimension);
                    g.TranslateTransform(squareDimension, 0, MatrixOrder.Append); // next column
                }

                g.Restore(graphicsState);

                g.TranslateTransform(0, squareDimension, MatrixOrder.Append); // next row
            }
        }

        public void Click(MouseEventArgs e)
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
				MessageBox.Show("Illegal Move");
				return;
			}

			previousStates.Push(boardState);
			boardState = boardState.Clone();

			MakeMove(choice);
		}

		/// <summary>
		/// - places a Piece and draws it
		/// - flips all affected Pieces and animates them flipping
		/// - changes Turn
		/// </summary>
		/// <param name="coord">where current Player is placing a Piece</param>
		public void MakeMove(Coord coord)
		{
			// redraw Board to remove any Legal Moves & partially flipped Pieces
            Animation.cancelFlipping = true;
			RedrawSquares();

            // update boardState with Move and flipped Pieces
            StateEnum colorToFlipTo = boardState.WhitesTurn ? StateEnum.White : StateEnum.Black;
			boardState.PlacePieceAndFlipPiecesAndChangeTurns(coord);
			UpdateStatus();

            // draw the placed Piece
			Graphics g = MainForm.instance.CreateGraphics();
			SetupGraphics(g);
            g.TranslateTransform((coord.x - 1) * squareDimension, (coord.y - 1) * squareDimension, MatrixOrder.Append);
			Square square = boardState.GetSquare(coord);
			square.Draw(g, squareDimension);

			// animate flipping affected Pieces
			Animation.cancelFlipping = false;
			Animation.coordsToFlip = boardState.coordsFlipped;
			Animation.newState = colorToFlipTo;
			Animation.Animate();

			// handle End of Game
			if (boardState.endOfGame)
			{
				if (boardState.BlackCount > boardState.WhiteCount)
					MessageBox.Show(string.Format("Black Wins {0}-{1}", boardState.BlackCount, boardState.WhiteCount));
				else if (boardState.WhiteCount > boardState.BlackCount)
					MessageBox.Show(string.Format("White Wins {0}-{1}", boardState.WhiteCount, boardState.BlackCount));
				else
					MessageBox.Show("Tie");
				return;
			}

			// report skipped Turn
			if (boardState.skippedTurn)
			{
				MessageBox.Show(string.Format("No legal moves available...  Skipping {0}'s turn", boardState.WhitesTurn ? "Black" : "White"), "Skipping Turn");

				// if ComputerPlayer's Turn
				if (ComputerPlayer != null && (ComputerPlayer.AmIWhite ^ !boardState.WhitesTurn))
					ExecuteComputerPlayerTurn(); // do it now after modal MessageBox has been OKed
			}
            else // no modal MessageBox, so delay ComputerPlayer's turn until flipping animation finishes 
            {
				// if ComputerPlayer's Turn
				if (ComputerPlayer != null && (ComputerPlayer.AmIWhite ^ !boardState.WhitesTurn))
				{
					// delay ComputerPlayer's turn until flipping animation finishes
					computerTurnDelayTimer = new Timer();
					computerTurnDelayTimer.Interval = Animation.flipDelay * (180 / Animation.flipDegrees + 4); // Delay a little longer than Flipping Animation takes
					computerTurnDelayTimer.Tick += new EventHandler(OnComputersTurnTimer);
					computerTurnDelayTimer.Start();
				}
			}
		}

		/// <summary>
		/// when computersTurnDelayTimer ends, ComputerPlay chooses and makes a Move
		/// </summary>
		public void OnComputersTurnTimer(Object sender, EventArgs e)
		{
			computerTurnDelayTimer.Stop();
			computerTurnDelayTimer = null;
			ExecuteComputerPlayerTurn();
        }

		public void ExecuteComputerPlayerTurn()
		{
			// if ComputerPlayer's Turn
			if (ComputerPlayer != null && (ComputerPlayer.AmIWhite ^ !boardState.WhitesTurn))
			{
				Coord? choice = ComputerPlayer.ChooseNextMove(boardState);
				if (choice != null)
					MakeMove(choice.Value);
			}
        }

        /// <summary>
        /// Status Bar shows Turn & Scores
        /// </summary>
        private void UpdateStatus()
		{
			MainForm.instance.statusBarTurn.Text = string.Format("{0}'s Turn", boardState.WhitesTurn ? "White" : "Black");
            MainForm.instance.statusBarBlackScore.Text = string.Format("Black={0}", boardState.BlackCount);
            MainForm.instance.statusBarWhiteScore.Text = string.Format("White={0}", boardState.WhiteCount); 
		}

        /// <summary>
        /// loops all LegalMoves to draw a symbol
        /// </summary>
        public void ShowLegalMoves()
		{
            Graphics g = MainForm.instance.CreateGraphics();
            SetupGraphics(g);
            Brush brush = new SolidBrush(Color.Red);
            int legalMoveRadius = (int)Math.Round(0.2 * squareDimension);

            List<Coord> legalMoves = boardState.LegalMoves();
			foreach (Coord coord in legalMoves)
			{
				GraphicsState graphicsState = g.Save();
				g.TranslateTransform((coord.x - 1) * squareDimension, (coord.y - 1) * squareDimension, MatrixOrder.Append); // -1 for 0-based
                g.FillEllipse(brush, -legalMoveRadius, -legalMoveRadius, 2 * legalMoveRadius, 2 * legalMoveRadius);
                g.Restore(graphicsState);
			}
        }
	}
}
