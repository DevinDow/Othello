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
        public static bool LogEachAdvancedTurn = false;
        public static bool LogEachExpertTurn = false;
        public static bool LogEachExpertLegalMoveResponse = false;
        public static bool LogEachUltimateTurn = false;
        public static bool LogEachUltimateLegalMoveResponse = false;
        private Random random = new Random();
        private const int EXPERT_TURNS_DEPTH = 11;
        //private const int ULTIMATE_TURNS_DEPTH = 11;
        private const int ULTIMATE_TURNS_DEPTH_TO_START_USING_EXPERT = 6; // Ultimate recurses for every Legal Move, but that is excessively slow and less critical at deeper Depths

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

            /*
                case LevelEnum.Advanced:
                    choices = advanced_ChooseHighestScoringAfterOpponentMove();
					break;

                case LevelEnum.Expert:
                    choices = recurseToChooseHighestScoringAfterSeveralTurns();
                    break;

                case LevelEnum.Ultimate:
                    choices = recurseToChooseHighestScoringAfterSeveralTurns();
                    break;
            }*/

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


        /*
        /*
        /// <summary>
        /// loop through all of Computer's Legal Moves
        /// collect the ones that maximize Score after several Turns
        /// if multiple Choices tie after several Turns then choose the ones with the highest first-move Score
        /// </summary>
        /// <returns>list of equal Computer Choices</returns>
        private List<Coord> recurseToChooseHighestScoringAfterSeveralTurns()
        {
            bool loggingEachTurn = (Level == LevelEnum.Expert && LogEachExpertTurn) || (Level == LevelEnum.Ultimate && LogEachUltimateTurn);
            int maxComputerScoreAfterSeveralTurns = -int.MaxValue;
            List<Coord> bestComputerChoices = new List<Coord>();

            // try every Legal Move
            List<Coord> legalComputerMoves = BoardState.LegalMoves();
            foreach (Coord computerChoice in legalComputerMoves)
            {
                if (loggingEachTurn) // re-Log initial BoardState before each legal Move
                    Debug.Print("initial BoardState:{0}", BoardState);

                // Turn Depth = 1
                BoardState computerBoardState = BoardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (loggingEachTurn) // Log computerBoardState & computerChoiceScore for computerChoice
                    Debug.Print(" - {0} choice: 1={1}->{2} resulting Board's Score={3:+#;-#;+0}\nresulting BoardState:{4}", 
                            Level, BoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);

                // next Turn Depth = 2 (unless Turn Skipped due to no leagl Moves)
                int nextTurn = 2;
                if (computerBoardState.WhitesTurn == BoardState.WhitesTurn) // Turn Skipped due to no legal moves
                    nextTurn++; // depth should go down to same Player to compare equally

                // find minMaxScoreAfterSeveralTurns by starting recursion
                int minMaxScoreAfterSeveralTurns;
                if (Level == LevelEnum.Expert) // Expert
                    minMaxScoreAfterSeveralTurns = expert_FindMinMaxScoreForHighestScoringMove(computerBoardState, nextTurn);
                else // Ultimate
                    minMaxScoreAfterSeveralTurns = ultimate_FindMinMaxScoreForAllMyLegalMoves(computerBoardState, nextTurn);
                if (loggingEachTurn) // Log minMaxScoreAfterSeveralTurns for computerChoice
                    Debug.Print(" - {0} choice: 1={1}->{2} resulting Board's Score={3:+#;-#;+0} minMaxScoreAfterSeveralTurns={4}\n\n",
                            Level, BoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, minMaxScoreAfterSeveralTurns);

                // remember the list of bestComputerChoices that attain maxComputerScoreAfterSeveralTurns
                if (minMaxScoreAfterSeveralTurns > maxComputerScoreAfterSeveralTurns) // remember maxComputerScoreAfterSeveralTurns and start a new List of Moves that attain it
                {
                    maxComputerScoreAfterSeveralTurns = minMaxScoreAfterSeveralTurns;
                    bestComputerChoices = new List<Coord>();
                }

                if (minMaxScoreAfterSeveralTurns >= maxComputerScoreAfterSeveralTurns) // add choice to bestComputerChoices
                {
                    if (!bestComputerChoices.Contains(computerChoice))
                        bestComputerChoices.Add(computerChoice);
                }
            }

            if (loggingEachTurn)
                Debug.Print("** {0} bestComputerChoices count={1}, maxComputerScoreAfterSeveralTurns={2:+#;-#;+0}.  Choose the highest scoring Move.", 
                        Level, bestComputerChoices.Count, maxComputerScoreAfterSeveralTurns);

            // find finalComputerChoices from equal bestComputerChoices based on the one with best computerChoiceScore
            int maxComputerScore = -int.MaxValue;
            List<Coord> finalComputerChoices = new List<Coord>();
            foreach (Coord computerChoice in bestComputerChoices)
            {
                BoardState computerBoardState = BoardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (loggingEachTurn)
                    Debug.Print("{0} Top Computer choice: {1}->{2} resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                            Level, computerBoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);
                if (computerChoiceScore > maxComputerScore)
                {
                    maxComputerScore = computerChoiceScore;
                    finalComputerChoices = new List<Coord>();
                }
                if (computerChoiceScore >= maxComputerScore)
                {
                    finalComputerChoices.Add(computerChoice);
                }
            }

            if (LogDecisions)
                Debug.Print("finalComputerChoices count={0}, maxComputerScore={1} maxComputerScoreAfterSeveralTurns={2}",
                        finalComputerChoices.Count, maxComputerScore, maxComputerScoreAfterSeveralTurns);

            return finalComputerChoices;
        }

        /// <summary>
        /// Recusively find optimal choice by finding highest/lowest Score possible on each Turn
        /// on My/Computer's Turn: maximize my Score
        /// on Opponent's Turn: assume Opponent will choose lowest Score for Me/Computer
        /// return the minMaxScore after several Turns/recusions
        /// </summary>
        /// <param name="boardState">BoardState for current Turn</param>
        /// <param name="turns">how many Turns to recursively try</param>
        /// <returns>minMaxScore after several Turns/recusions</returns>
        private int expert_FindMinMaxScoreForHighestScoringMove(BoardState boardState, int turn)
        {
            bool myTurn = boardState.WhitesTurn ^ !AmIWhite;
            int minMaxScore = myTurn ? -int.MaxValue : int.MaxValue;
            Coord minMaxResponse = new Coord();
            BoardState minMaxResponseBoardState = null;

            List<Coord> legalMoves = boardState.LegalMoves();
            if (legalMoves.Count == 0) // game over
            {
                return ScoreEndOfGame(boardState);
            }

            // find best/worst Score for every leagalMove response
            foreach (Coord response in legalMoves)
            {
                BoardState responseBoardState = boardState.Clone();
                responseBoardState.PlacePieceAndFlipPiecesAndChangeTurns(response);

                int responseScore;
                if (responseBoardState.endOfGame)
                {
                    responseScore = ScoreEndOfGame(responseBoardState);
                }
                else
                {
                    responseScore = ScoreBoard(responseBoardState);
                }
                // Log each legalMove response
                if (LogEachExpertTurn && LogEachExpertLegalMoveResponse)
                    Debug.Print("       - Expert LegalMove Response: {0}={1}->{2} resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                            turn, boardState.WhitesTurn ? 'W' : 'B', response, responseScore, responseBoardState);

                if (myTurn)
                {
                    if (responseScore > minMaxScore) // my Turn goes for highest Score for me
                    {
                        minMaxScore = responseScore;
                        minMaxResponse = response;
                        minMaxResponseBoardState = responseBoardState;
                    }
                }
                else
                {
                    if (responseScore < minMaxScore) // opponent's Turn chooses lowest Score for me
                    {
                        minMaxScore = responseScore;
                        minMaxResponse = response;
                        minMaxResponseBoardState = responseBoardState;
                    }
                }
            }

            // Log the chosen minMaxResponse
            if (LogEachExpertTurn)
                Debug.Print("- Expert response {0}={1}->{2}: resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                        turn, boardState.WhitesTurn ? 'W' : 'B', minMaxResponse, minMaxScore, minMaxResponseBoardState);

            if (turn >= EXPERT_TURNS_DEPTH)
                return minMaxScore;

            int nextTurn = turn + 1;
            if (minMaxResponseBoardState.WhitesTurn == boardState.WhitesTurn) // turn skipped due to no legal moves
                nextTurn++; // depth should go down to same Player to compare equally

            // recurse to return resulting minMaxScore after levelsLeft more Turns
            return expert_FindMinMaxScoreForHighestScoringMove(minMaxResponseBoardState, nextTurn);
        }

        /// <summary>
        /// Recusively find optimal choice by trying every LegalMove for Computer Turn and lowest Score possible for each Opponent Turn
        /// on My/Computer's Turn: recurse for every LegalMove
        /// on Opponent's Turn: assume Opponent will choose lowest Score for Me/Computer
        /// return the minMaxScore after several Turns/recusions
        /// </summary>
        /// <param name="boardState">BoardState for current Turn</param>
        /// <param name="turns">how many Turns to recursively try</param>
        /// <returns>minMaxScore after several Turns/recusions</returns>
        private int ultimate_FindMinMaxScoreForAllMyLegalMoves(BoardState boardState, int turn)
        {
            bool myTurn = boardState.WhitesTurn ^ !AmIWhite;
            int nextTurn = turn + 1;
            List<Coord> legalMoves = boardState.LegalMoves();
            if (legalMoves.Count == 0)
                return ScoreEndOfGame(boardState);

            if (myTurn) // find recursive Score for each legalMove
            {
                int maxRecursiveScore = -int.MaxValue;
                Coord maxRecursiveResponse;
                BoardState maxRecursiveResponseBoardState;
                foreach (Coord legalMove in legalMoves)
                {
                    BoardState legalMoveBoardState = boardState.Clone();
                    legalMoveBoardState.PlacePieceAndFlipPiecesAndChangeTurns(legalMove);

                    int recusiveScore;
                    if (legalMoveBoardState.endOfGame) // return ScoreEndOfGame()
                        recusiveScore = ScoreEndOfGame(legalMoveBoardState);
                    /*else if (turn >= ULTIMATE_TURNS_DEPTH) // return ScoreBoard()
                        recusiveScore = ScoreBoard(legalMoveBoardState);*
                    else // recurse
                    {
                        if (LogEachUltimateTurn)
                            Debug.Print("       - Ultimate LegalMove: {0}={1}->{2}\nresulting BoardState:{3}",
                                    turn, boardState.WhitesTurn ? 'W' : 'B', legalMove, legalMoveBoardState);

                        if (legalMoveBoardState.WhitesTurn == boardState.WhitesTurn) // turn skipped due to no legal moves
                            nextTurn++; // depth should go down to same Player to compare equally

                        /*if (nextTurn > ULTIMATE_TURNS_DEPTH)
                            recusiveScore = ScoreBoard(legalMoveBoardState);
                        else // recurse to return resulting minMaxScore after nextTurn*
                        {
                            if (nextTurn < ULTIMATE_TURNS_DEPTH_TO_START_USING_EXPERT)
                                recusiveScore = ultimate_FindMinMaxScoreForAllMyLegalMoves(legalMoveBoardState, nextTurn);
                            else
                                recusiveScore = expert_FindMinMaxScoreForHighestScoringMove(legalMoveBoardState, nextTurn);
                        }
                    }

                    // Log each legalMove
                    if (LogEachUltimateTurn)
                        Debug.Print("- Ultimate LegalMove: {0}={1}->{2} recusiveScore={3:+#;-#;+0}\nlegalMoveBoardState:{4}",
                                turn, boardState.WhitesTurn ? 'W' : 'B', legalMove, recusiveScore, legalMoveBoardState);

                    if (recusiveScore > maxRecursiveScore) // is this ultimately the best resulting recusiveScore to bubble-up?
                    {
                        maxRecursiveScore = recusiveScore;
                        maxRecursiveResponse = legalMove;
                        maxRecursiveResponseBoardState = legalMoveBoardState;
                    }
                }
                return maxRecursiveScore; // ultimately bubble-up the maxRecursiveScore until we figure out which first Move results in the best eventual Board Score
            }

            else // Opponent's Turn
            {
                // find best Opponent Response using min Board Score
                int minRecursiveScore = int.MaxValue;
                Coord minRecursiveResponse = new Coord();
                BoardState minRecursiveResponseBoardState = null;
                foreach (Coord legalMove in legalMoves)
                {
                    BoardState legalMoveBoardState = boardState.Clone();
                    legalMoveBoardState.PlacePieceAndFlipPiecesAndChangeTurns(legalMove);

                    int responseScore;
                    if (legalMoveBoardState.endOfGame)
                        responseScore = ScoreEndOfGame(legalMoveBoardState);
                    else
                        responseScore = ScoreBoard(legalMoveBoardState);

                    // Log each legalMove response
                    if (LogEachUltimateTurn && LogEachUltimateLegalMoveResponse)
                        Debug.Print("       - Ultimate Opponent LegalMove: {0}={1}->{2} resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                                turn, boardState.WhitesTurn ? 'W' : 'B', legalMove, responseScore, legalMoveBoardState);

                    if (responseScore < minRecursiveScore) // opponent's Turn chooses lowest Score for me
                    {
                        minRecursiveScore = responseScore;
                        minRecursiveResponse = legalMove;
                        minRecursiveResponseBoardState = legalMoveBoardState;
                    }
                }

                // Log the chosen minMaxResponse
                if (LogEachUltimateTurn)
                    Debug.Print("- Ultimate Opponent's best Response {0}={1}->{2}: resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                            turn, boardState.WhitesTurn ? 'W' : 'B', minRecursiveResponse, minRecursiveScore, minRecursiveResponseBoardState);

                if (minRecursiveResponseBoardState.endOfGame)
                    return minRecursiveScore;

                /*if (turn >= ULTIMATE_TURNS_DEPTH)
                    return minScore;*

                if (minRecursiveResponseBoardState.WhitesTurn == boardState.WhitesTurn) // turn skipped due to no legal moves
                    nextTurn++; // depth should go down to same Player to compare equally

                // recurse to return resulting minMaxScore after nextTurn
                if (nextTurn < ULTIMATE_TURNS_DEPTH_TO_START_USING_EXPERT)
                    return ultimate_FindMinMaxScoreForAllMyLegalMoves(minRecursiveResponseBoardState, nextTurn);
                else
                    return expert_FindMinMaxScoreForHighestScoringMove(minRecursiveResponseBoardState, nextTurn);
            }
        }
*/
        /// <summary>
        /// End-of-Game Score should just be a comparison of Counts
        /// MULTIPLIER helps it fit in with & out-weigh other Scores
        /// </summary>
        /// <param name="boardState">the BoardState to calculate Score for</param>
        /// <returns>a high Score if won, a low negative Score if lost</returns>
        private int ScoreEndOfGame(BoardState boardState)
        {
            int endOfGameScore;
            const int MULTIPLIER = 10000;
            if (AmIWhite)
                endOfGameScore = MULTIPLIER * (boardState.WhiteCount - boardState.BlackCount);
            else
                endOfGameScore = MULTIPLIER * (boardState.BlackCount - boardState.WhiteCount);

            return endOfGameScore;
        }

        /// <summary>
        /// calculates a Score for a BoardState
        /// uses WeightedCoordValue()
        /// uses difference between Computer's Score for his Piecec & Opponent's Score for his Pieces
        /// </summary>
        /// <param name="boardState">BoardState to caluclate Score for</param>
        /// <returns>weighted Score of boardState</returns>
        protected int ScoreBoard(BoardState boardState)
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
		/// Beginner values all Coords equally
		/// higher Levels weight Coord values by location as more valuable or dangerous
        /// </summary>
        /// <param name="coord">Coord to get weighted Score of</param>
        /// <returns>weighted Score of coord</returns>
        protected virtual int WeightedCoordValue(Coord coord)
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
