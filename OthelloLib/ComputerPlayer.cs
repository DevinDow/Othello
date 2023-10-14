using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OthelloLib
{
    public abstract class ComputerPlayer
	{
		public bool AmIWhite;
        public string LevelName;
        public static bool LogDecisions = true;
        private Random random = new Random();

        public ComputerPlayer(bool amIWhite)
		{
			AmIWhite = amIWhite;
		}

		/// <summary>
		/// returns ComputerPlayer's choice for next move
		/// </summary>
		public Coord? ChooseNextMove(BoardState boardState)
		{
            if (LogDecisions)
            {
                int initialScore = ScoreBoard(boardState);
                Debug.Print("\n{0} {1}\ninitial BoardState:{2}\ninitial Score={3:+#;-#;+0}", 
                        LevelName, AmIWhite ? "W" : "B", boardState, initialScore);
            }

            List<Coord> choices = findBestChoices(boardState);

            // no legal Moves
            if (choices.Count == 0)
                return null;
            
			// only 1 best Move
			if (choices.Count == 1)
			{
				Coord choice = choices[0];
                if (LogDecisions)
                    Debug.Print("{0} chose {1}->{2}", LevelName, boardState.WhitesTurn ? 'W' : 'B', choice);
                return choice;
			}

			// multiple equally best Moves
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Equal Choices: {0}->", boardState.WhitesTurn ? 'W' : 'B');
			foreach (Coord choice in choices)
				sb.Append(choice + " ");
            if (LogDecisions)
                Debug.Print(sb.ToString());

			// randomly pick one of the choices
            int randomIndex = random.Next(choices.Count);
			Coord randomChoice = choices[randomIndex];
            if (LogDecisions)
                Debug.Print("{0} chose {1}->{2}", LevelName, boardState.WhitesTurn ? 'W' : 'B', randomChoice);
            return randomChoice;
        }

        protected abstract List<Coord> findBestChoices(BoardState boardState);

        /// <summary>
        /// calculates a Score for a BoardState
        /// uses WeightedCoordValue()
        /// uses difference between Computer's Score for his Piecec & Opponent's Score for his Pieces
        /// </summary>
        /// <param name="boardState">BoardState to caluclate Score for</param>
        /// <returns>weighted Score of boardState</returns>
        internal int ScoreBoard(BoardState boardState)
		{
            int emptyCount = boardState.EmptyCount; // cache this Property instead of repeatedly recalculating

            int score = 0;
            foreach (Coord coord in boardState)
            { 
				Square square = boardState.GetSquare(coord);
				if (square.State != StateEnum.White && square.State != StateEnum.Black)
					continue;
				int weightedCoordValue = WeightedCoordValue(coord, emptyCount);

                if (AmIWhite) // Computer is White
				{
					switch (square.State)
					{
						case StateEnum.White:
							score += weightedCoordValue;
							break;
						case StateEnum.Black:
							score -= weightedCoordValue;
							break;
					}
				}
				else // Computer is Black
				{
                    switch (square.State)
                    {
                        case StateEnum.Black:
                            score += weightedCoordValue;
                            break;
                        case StateEnum.White:
                            score -= weightedCoordValue;
                            break;
                    }
                }
			}
			return score;
		}

        /// <summary>
        /// returns a weighted value for a Coord
        /// Beginner values all Coords equally, while higher Levels weight Coord values by location as more valuable or dangerous
        /// Intermediate/Advanced use negatives to make better decisions
        /// Recursive Levels avoid negatives
        /// </summary>
        /// <param name="coord">Coord to get weighted Score of</param>
        /// <returns>weighted Score of coord</returns>
        protected virtual int WeightedCoordValue(Coord coord, int emptyCount)
        {
            // Intermediate/Advanced perform better with Negatives helping make better decisions.
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

        public override string ToString()
        {
            return string.Format("{0} = {1}", LevelName, AmIWhite ? "White" : "Black");
        }
    }
}
