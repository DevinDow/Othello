using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Othello
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
            const int numEmptyToConsiderBoardMostlyFilled = 8;
            bool boardMostlyFilled = boardState.EmptyCount <= numEmptyToConsiderBoardMostlyFilled;

            int score = 0;
            foreach (Coord coord in boardState)
            { 
				Square square = boardState.GetSquare(coord);
				if (square.State != StateEnum.White && square.State != StateEnum.Black)
					continue;
				int weightedCoordValue;
                if (boardMostlyFilled)
                    weightedCoordValue = 100; // after board is mostly full, Expert should just try to win the game
                else
                    weightedCoordValue = WeightedCoordValue(coord);

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
        /// Intermediate uses negatives to make better decisions, Advanced Levels avoid negatives
        /// </summary>
        /// <param name="coord">Coord to get weighted Score of</param>
        /// <returns>weighted Score of coord</returns>
        protected abstract int WeightedCoordValue(Coord coord);

        public override string ToString()
        {
            return string.Format("{0} = {1}", LevelName, AmIWhite ? "White" : "Black");
        }
    }
}
