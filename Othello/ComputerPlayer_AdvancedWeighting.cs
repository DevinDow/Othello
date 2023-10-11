using System;
using System.Collections.Generic;
using System.Text;

namespace Othello
{
    public abstract class ComputerPlayer_AdvancedWeighting : ComputerPlayer
    {
        protected ComputerPlayer_AdvancedWeighting(bool amIWhite) : base(amIWhite) { }

        /// <summary>
        /// returns a weighted value for a Coord
        /// Beginner values all Coords equally
        /// higher Levels weight Coord values by location as more valuable or dangerous
        /// </summary>
        /// <param name="coord">Coord to get weighted Score of</param>
        /// <returns>weighted Score of coord</returns>
        protected override int WeightedCoordValue(Coord coord)
        {
            // Higher Levels value Coords differently
            // Corners are highest, then Ends.
            // Coords before Corners & Ends are devalued since they lead to Opponent getting Corners & Ends.
            // And Coords before those are valuable since they lead to Opponent getting those devalued Coords.

            // Advanced & Expert perform worse when Negatives throw a wrench in comparing BoardStates, esp near the end of the Game.
            // 2000   2 200 200
            //    2   1   3   3
            //  200   3  50  30
            //  200   3  30  10
            switch (coord.x)
            {
                // Edge COLs
                case 1:
                case 8:
                    switch (coord.y)
                    {
                        case 1:
                        case 8:
                            return 2000; // Corner
                        case 2:
                        case 7:
                            return 2; // leads to Corner
                        default:
                            return 200; // Edge
                    }
                // COL before Edge
                case 2:
                case 7:
                    switch (coord.y)
                    {
                        case 1:
                        case 8:
                            return 2; // leads to Corner
                        case 2:
                        case 7:
                            return 1; // leads to Corner
                        default:
                            return 3; // leads to Edge
                    }
                // COL before COL before Edge
                case 3:
                case 6:
                    switch (coord.y)
                    {
                        case 1:
                        case 8:
                            return 200; // Edge
                        case 2:
                        case 7:
                            return 3; // leads to Edge
                        case 3:
                        case 6:
                            return 50; // leads to Opponent getting devalued Coord near Corner
                        default:
                            return 30; // leads to Opponent getting devalued Coord near Edge
                    }
                // middle COLs
                default:
                    switch (coord.y)
                    {
                        case 1:
                        case 8:
                            return 200; // Edge
                        case 2:
                        case 7:
                            return 3; // leads to Edge
                        case 3:
                        case 6:
                            return 30; // leads to Opponent getting devalued Coord near Edge
                        default:
                            return 10; // Middles
                    }
            }
        }
    }
}
