using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Othello
{
	public enum StateEnum {Empty, Black, White, LegalMove};

	public class Square
	{
		private StateEnum state;
		private Board board;
		private int row, column;
		private Graphics g;
		private Timer flipTimer = null;
		private int flipAngle = 0;

		public StateEnum State { get{return state;} set{state = value;} }

		public Square(Board board, int row, int column)
		{
			this.board = board;
			this.row = row;
			this.column = column;
			state = StateEnum.Empty;
		}

		public void Draw(Graphics g)
		{
			int pieceRadius = (int)Math.Round(0.4 * board.squareDimension);

			switch (state)
			{
				case StateEnum.Empty:
				{
					Brush brush = new SolidBrush(Color.Green);
					g.FillRectangle(brush, -pieceRadius, -pieceRadius, 2*pieceRadius, 2*pieceRadius); 
					break;
				}
				case StateEnum.Black:
				{
					Brush brush = new SolidBrush(Color.Black);
					g.FillEllipse(brush, -pieceRadius, -pieceRadius, 2*pieceRadius, 2*pieceRadius); 
					break;
				}
				case StateEnum.White:
				{
					Brush brush = new SolidBrush(Color.White);
					g.FillEllipse(brush, -pieceRadius, -pieceRadius, 2*pieceRadius, 2*pieceRadius); 
					break;
				}
				case StateEnum.LegalMove:
				{
					Brush brush = new SolidBrush(Color.Red);
					int legalMoveRadius = (int)Math.Round(0.2 * board.squareDimension);
					g.FillEllipse(brush, -legalMoveRadius, -legalMoveRadius, 2*legalMoveRadius, 2*legalMoveRadius); 
					break;
				}
			}
		}

		public void Flip(Graphics g)
		{
			this.g = g;
			flipAngle = 0;

			if (flipTimer != null)
			{
				flipTimer.Stop();
				flipTimer = null;
			}

			flipTimer = new Timer();
			flipTimer.Interval = board.flipDelay;
			flipTimer.Tick += new EventHandler(OnFlipTimer);
			flipTimer.Start();
		}

		private void OnFlipTimer(Object myObject, EventArgs myEventArgs) 
		{
			GraphicsState graphicsState = g.Save();
			g.TranslateTransform(column * board.squareDimension, row * board.squareDimension, MatrixOrder.Append);
			int pieceRadius = (int)Math.Round(0.4 * board.squareDimension);
			Brush brush = new SolidBrush(Color.Green);
			g.FillEllipse(brush, -pieceRadius, -pieceRadius, 2*pieceRadius, 2*pieceRadius); 

			if (state == StateEnum.Black ^ flipAngle < 90)
				brush = new SolidBrush(Color.Black);
			else
				brush = new SolidBrush(Color.White);

			int pieceWidth = (int)Math.Round(0.4 * board.squareDimension * Math.Cos(Math.PI * flipAngle / 180));
			g.FillEllipse(brush, -pieceWidth, -pieceRadius, 2*pieceWidth, 2*pieceRadius); 

			if (flipAngle >= 180)
			{
				flipTimer.Stop();
				flipTimer = null;
			}

			flipAngle += 10;

			g.Restore(graphicsState);
		}
	}
}
