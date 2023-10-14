using System;
using System.Collections.Generic;
using System.Text;

namespace Othello
{
    /// <summary>
    /// Coord of Squares ( 1..8 , 1..8 )
    /// </summary>
    public struct Coord : IEquatable<Coord>
    {
        public int x, y;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Coord other)
        {
            return other.x == x && other.y == y;
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }
    }
}
