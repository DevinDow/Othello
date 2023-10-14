using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloLib
{
    public class ComputerPlayer_Beginner : ComputerPlayer_Basic
    {
        public ComputerPlayer_Beginner(bool amIWhite = true) : base(amIWhite) 
        {
            LevelName = "Beginner";
        }

        protected override int WeightedCoordValue(Coord coord, int emptyCount)
        {
            // Beginner Level scores each square as 1, so it's just trying to fliup the most Pieces.
            return 1; // All Coords
        }
    }
}
