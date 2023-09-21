using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Othello
{
	public enum StateEnum {Empty, Black, White, LegalMove};

	/// <summary>
	/// represents a Square that has a State and can Draw itself & animate Flipping itself
	/// </summary>
	public class Square
	{
		private StateEnum state;
		private int x, y;
		private Graphics gForFlipping;
		private Timer flipTimer = null;
		private int flipAngle = 0;

		public StateEnum State { get{return state;} set{state = value;} }

        public Square(int x, int y)
        {
            this.x = x;
            this.y = y;
            state = StateEnum.Empty;
        }

        public Square(int x, int y, StateEnum state)
        {
            this.x = x;
            this.y = y;
            this.state = state;
        }

        public void Draw(Graphics g)
		{
			int pieceRadius = (int)Math.Round(0.4 * Board.squareDimension);

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
					int legalMoveRadius = (int)Math.Round(0.2 * Board.squareDimension);
					g.FillEllipse(brush, -legalMoveRadius, -legalMoveRadius, 2*legalMoveRadius, 2*legalMoveRadius); 
					break;
				}
			}
		}

		public void Flip(Graphics g)
		{
			this.gForFlipping = g;
			flipAngle = 0;

			if (flipTimer != null)
			{
				flipTimer.Stop();
				flipTimer = null;
			}

			flipTimer = new Timer();
			flipTimer.Interval = Board.flipDelay;
			flipTimer.Tick += new EventHandler(OnFlipTimer);
			flipTimer.Start();
		}

		private void OnFlipTimer(Object myObject, EventArgs myEventArgs) 
		{
			if (Board.cancelFlipping)
			{
				flipTimer.Stop();
				flipTimer = null;
				return;
			}

			Bitmap bitmap = new Bitmap(Board.squareDimension, Board.squareDimension);
			Graphics bitmapGraphics = Graphics.FromImage(bitmap);
			bitmapGraphics.TranslateTransform(Board.squareDimension/2, Board.squareDimension/2);
			int pieceRadius = (int)Math.Round(0.4 * Board.squareDimension);
			Brush brush = new SolidBrush(Color.Green);
			bitmapGraphics.FillEllipse(brush, -pieceRadius, -pieceRadius, 2 * pieceRadius, 2 * pieceRadius);

			if (state == StateEnum.Black ^ flipAngle < 90)
				brush = new SolidBrush(Color.Black);
			else
				brush = new SolidBrush(Color.White);

			int pieceWidth = (int)Math.Round(0.4 * Board.squareDimension * Math.Cos(Math.PI * flipAngle / 180));
			bitmapGraphics.FillEllipse(brush, -pieceWidth, -pieceRadius, 2 * pieceWidth, 2 * pieceRadius);

			if (flipAngle >= 180)
			{
				flipTimer.Stop();
				flipTimer = null;
			}

			flipAngle += 10;

			gForFlipping.DrawImage(bitmap, (x-1) * Board.squareDimension - Board.squareDimension / 2, (y-1) * Board.squareDimension - Board.squareDimension / 2); // draw bitmap from upper-left of square
		}
	}
}
