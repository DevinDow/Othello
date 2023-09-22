using System;
using System.Collections.Generic;
using System.Text;

namespace Othello
{
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
}
