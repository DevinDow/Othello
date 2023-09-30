using System;
using System.Drawing;

namespace Othello
{
	public enum StateEnum {Empty, Black, White, LegalMove};

	/// <summary>
	/// represents a Square that has a State and can Draw itself & animate Flipping itself
	/// </summary>
	public class Square
	{
		private StateEnum state;

		public StateEnum State { get{return state;} set{state = value;} }

        public Square() : this(StateEnum.Empty) { }

        public Square(StateEnum state)
        {
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
	}
}
