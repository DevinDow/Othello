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

		private static Graphics gForFlipping;
        private static Timer flipTimer = null;
        private static int flipAngle = 0;

        public static void Animate()
        {
            gForFlipping = MainForm.instance.CreateGraphics();
			Board.SetupGraphics(gForFlipping);

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

		private static void OnFlipTimer(Object myObject, EventArgs myEventArgs)
		{
			if (Board.cancelFlipping)
			{
				flipTimer.Stop();
				flipTimer = null;
				return;
			}

			Bitmap flippingPeieceBitmap = DrawFlippingPieceToBitmap();

            // draw flippingPeieceBitmap at each coordToFlip
            foreach (Coord coordToFlip in coordsToFlip)
			{
				gForFlipping.DrawImage(flippingPeieceBitmap, (coordToFlip.x - 1) * Board.squareDimension - Board.squareDimension / 2, (coordToFlip.y - 1) * Board.squareDimension - Board.squareDimension / 2); // draw bitmap from upper-left of square
			}
        }

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
			flipAngle += 10;

			return bitmap;
		}
    }
}
