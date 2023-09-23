using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Othello
{
    class Animation
    {
		public static List<Coord> coordsToFlip;
		public static StateEnum newState;

		private static Graphics boardGraphics;
        private static Timer flipTimer = null;
        public const int flipDelay = 100;
        public const int flipDegrees = 10;
        public static bool cancelFlipping = false;
        private static int flipAngle = 0;

        /// <summary>
        /// setup the boardGraphics and start the Timer 
        /// </summary>
        public static void Animate()
        {
            boardGraphics = MainForm.instance.CreateGraphics();
			Board.SetupGraphics(boardGraphics);

			flipAngle = 0;

			if (flipTimer != null)
			{
				flipTimer.Stop();
				flipTimer = null;
			}

			flipTimer = new Timer();
			flipTimer.Interval = flipDelay;
			flipTimer.Tick += new EventHandler(OnFlipTimer);
			flipTimer.Start();
		}

        /// <summary>
        /// draw flippingPieceBitmap at each coordToFlip
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private static void OnFlipTimer(Object myObject, EventArgs myEventArgs)
		{
			if (cancelFlipping)
			{
				flipTimer.Stop();
				flipTimer = null;
				return;
			}

			Bitmap flippingPieceBitmap = DrawFlippingPieceToBitmap();

            // draw flippingPieceBitmap at each coordToFlip
            foreach (Coord coordToFlip in coordsToFlip)
			{
				boardGraphics.DrawImage(flippingPieceBitmap, (coordToFlip.x - 1) * Board.squareDimension - Board.squareDimension / 2, (coordToFlip.y - 1) * Board.squareDimension - Board.squareDimension / 2); // draw bitmap at upper-left of square
			}
        }

		/// <summary>
		/// draw a flipping Piece to a Bitmap
		/// </summary>
		/// <returns>Bitmap of current rotation of a Piece</returns>
        private static Bitmap DrawFlippingPieceToBitmap()
		{ 
			// draw to a bitmap
			Bitmap bitmap = new Bitmap(Board.squareDimension, Board.squareDimension);
			Graphics bitmapGraphics = Graphics.FromImage(bitmap);
			bitmapGraphics.TranslateTransform(Board.squareDimension/2, Board.squareDimension/2);
			int pieceRadius = (int)Math.Round(0.4 * Board.squareDimension);

			// erase by filling green circle
			Brush brush = new SolidBrush(Color.Green);
			bitmapGraphics.FillEllipse(brush, -pieceRadius, -pieceRadius, 2 * pieceRadius, 2 * pieceRadius);

			// color switches at 90 degrees
			if (newState == StateEnum.Black ^ flipAngle < 90)
				brush = new SolidBrush(Color.Black);
			else
				brush = new SolidBrush(Color.White);

			// width of oval determined by flipAngle
			int pieceWidth = (int)Math.Round(0.4 * Board.squareDimension * Math.Cos(Math.PI * flipAngle / 180));
			bitmapGraphics.FillEllipse(brush, -pieceWidth, -pieceRadius, 2 * pieceWidth, 2 * pieceRadius);

			// stop at 180 degrees
			if (flipAngle >= 180)
			{
				flipTimer.Stop();
				flipTimer = null;
			}

			// increment by 10 degrees
			flipAngle += flipDegrees;

			return bitmap;
		}
    }
}
