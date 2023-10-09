using System;
using System.Collections.Generic;
using System.Text;

namespace Othello
{
    internal class ComputerPlayer_Intermediate : ComputerPlayer_Basic
    {
        public ComputerPlayer_Intermediate(bool amIWhite = true) : base(amIWhite)
        {
            LevelName = "Intermediate";
        }

        protected override int WeightedCoordValue(Coord coord)
        {
            // Intermediate performs better with Negatives helping make better decisions.
            // 50 -5 20 20
            // -5 -9 -2 -2
            // 20 -2  4  2
            // 20 -2  2  1
            switch (coord.x)
            {
                // Edge COLs
                case 1:
                case 8:
                    switch (coord.y)
                    {
                        case 1:
                        case 8:
                            return 50; // Corner
                        case 2:
                        case 7:
                            return -5; // leads to Corner
                        default:
                            return 20; // Edge
                    }
                // COL before Edge
                case 2:
                case 7:
                    switch (coord.y)
                    {
                        case 1:
                        case 8:
                            return -5; // leads to Corner
                        case 2:
                        case 7:
                            return -9; // leads to Corner
                        default:
                            return -2; // leads to Edge
                    }
                // COL before COL before Edge
                case 3:
                case 6:
                    switch (coord.y)
                    {
                        case 1:
                        case 8:
                            return 20; // Edge
                        case 2:
                        case 7:
                            return -2; // leads to Edge
                        case 3:
                        case 6:
                            return 4; // leads to Opponent getting devalued Coord near Corner
                        default:
                            return 2; // leads to Opponent getting devalued Coord near Edge
                    }
                // middle COLs
                default:
                    switch (coord.y)
                    {
                        case 1:
                        case 8:
                            return 10; // Edge
                        case 2:
                        case 7:
                            return -2; // leads to Edge
                        case 3:
                        case 6:
                            return 2; // leads to Opponent getting devalued Coord near Edge
                        default:
                            return 1; // Middles
                    }
            }
        }
    }
}
