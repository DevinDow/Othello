using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Othello
{
	public enum LevelEnum {
        Beginner,       // maximizes relative Score using number of Square
        Intermediate,   // maximizes relative Score using weighting for more valuable/dangerous Squares
        Advanced,       // minimizes Opponents best response
        Expert,         // Minimax algorithm to a Depth using Highest Scoring Move
        Ultimate,       // Minimax algorithm to a Depth
    }

    public class ComputerPlayer
	{
		public bool AmIWhite;
		public BoardState BoardState;
		public LevelEnum Level;
        public static bool LogDecisions = true;
        public static bool LogEachExpertTurn = true;
        public static bool LogEachExpertLegalMoveResponse = false;
        public static bool LogEachUltimateTurn = true;
        public static bool LogEachUltimateLegalMoveResponse = true;
        private Random random = new Random();
        private const int EXPERT_TURNS_DEPTH = 5;
        private const int ULTIMATE_TURNS_DEPTH = 5;

        public ComputerPlayer(LevelEnum level = LevelEnum.Beginner, bool amIWhite = true)
		{
			AmIWhite = amIWhite;
			Level = level;
		}

		/// <summary>
		/// returns ComputerPlayer's choice for next move
		/// </summary>
		public Coord? ChooseNextMove()
		{
            if (LogDecisions)
            {
                int initialScore = ScoreBoard(BoardState);
                Debug.Print("{0} {1}\ninitial BoardState:{2}\ninitial Score={3:+#;-#;+0}", 
                        Level, AmIWhite ? "W" : "B", BoardState, initialScore);
            }

            List<Coord> choices = new List<Coord>();

            switch (Level)
			{
				case LevelEnum.Beginner:
				case LevelEnum.Intermediate:
				default:
                    choices = basic_ChooseHighestScoringMove();
					break;

                case LevelEnum.Advanced:
                    choices = advanced_ChooseHighestScoringAfterOpponentMove();
					break;

                case LevelEnum.Expert:
                    choices = recursive_ChooseHighestScoringAfterSeveralTurns();
                    break;

                case LevelEnum.Ultimate:
                    choices = recursive_ChooseHighestScoringAfterSeveralTurns();
                    break;
            }

            // no legal Moves
            if (choices.Count == 0)
                return null;
            
			// only 1 best Move
			if (choices.Count == 1)
			{
				Coord choice = choices[0];
                if (LogDecisions)
                    Debug.Print("{0} chose {1}->{2}", Level, BoardState.WhitesTurn ? 'W' : 'B', choice);
                return choice;
			}

			// multiple equally best Moves
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Equal Choices: {0}->", BoardState.WhitesTurn ? 'W' : 'B');
			foreach (Coord choice in choices)
				sb.Append(choice + " ");
            if (LogDecisions)
                Debug.Print(sb.ToString());

			// randomly pick one of the choices
            int randomIndex = random.Next(choices.Count);
			Coord randomChoice = choices[randomIndex];
            if (LogDecisions)
                Debug.Print("{0} chose {1}->{2}", Level, BoardState.WhitesTurn ? 'W' : 'B', randomChoice);
            return randomChoice;
        }

        /// <summary>
        /// finds Moves that maximize weighted Score and picks one at random
        /// </summary>
        /// <returns>a Choice that maximizes weighted Score</returns>
        private List<Coord> basic_ChooseHighestScoringMove()
        {
            int maxScore = -int.MaxValue;
            List<Coord> bestComputerChoices = new List<Coord>();

            // loop through all of Computer's Legal Moves
            List<Coord> legalMoves = BoardState.LegalMoves();
			foreach (Coord computerChoice in legalMoves)
			{
				BoardState computerBoardState = BoardState.Clone();
				computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogDecisions)
				    Debug.Print("Computer choice: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}",
                            BoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);

				if (computerChoiceScore > maxScore) // remember maxScore and start a new List of Moves that attain it
				{
					maxScore = computerChoiceScore;
					bestComputerChoices = new List<Coord>();
				}

				if (computerChoiceScore >= maxScore) // add choice to maxScoringChoices
				{
					bestComputerChoices.Add(computerChoice);
				}
			}

            return bestComputerChoices;
        }

        /// <summary>
        /// finds Moves that minimize best weighted Score that Opponent can attain
        /// if multiple Moves tie then picks one that has highest Weighted Score
        /// </summary>
        /// <returns>a Choice that minimizes weighted Score that Opponent can attain</returns>
        private List<Coord> advanced_ChooseHighestScoringAfterOpponentMove()
        {
            int maxComputerScoreAfterOpponentsBestResponse = -int.MaxValue;
            List<Coord> bestComputerChoices = new List<Coord>();

            // loop through all of Computer's Legal Moves
            // collect the ones that don't let the Opponent score well
            List<Coord> legalComputerMoves = BoardState.LegalMoves();
            foreach (Coord computerChoice in legalComputerMoves)
            {
                BoardState computerBoardState = BoardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogDecisions)
                    Debug.Print(" - Computer choice: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}",
                            computerBoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);

                int opponentsBestResponseScore;
                if (computerBoardState.WhitesTurn == BoardState.WhitesTurn) // Opponent's Turn skipped
                {
                    if (LogDecisions)
                        Debug.Print("    - Opponent response: SKIPPED resulting Score={0:+#;-#;+0}\nresulting BoardState:{1}",
                                computerChoiceScore, computerBoardState);
                    opponentsBestResponseScore = computerChoiceScore;
                }
                else
                    opponentsBestResponseScore = advanced_FindOpponentsBestResponseScore(computerBoardState);

                if (opponentsBestResponseScore > maxComputerScoreAfterOpponentsBestResponse) // remember maxComputerScoreAfterOpponentsBestResponse and start a new List of Moves that attain it
                {
                    maxComputerScoreAfterOpponentsBestResponse = opponentsBestResponseScore;
                    bestComputerChoices = new List<Coord>();
                }

                if (opponentsBestResponseScore >= maxComputerScoreAfterOpponentsBestResponse) // add choice to bestComputerChoices
                {
                    if (!bestComputerChoices.Contains(computerChoice))
                        bestComputerChoices.Add(computerChoice);
                }
            }

            if (LogDecisions)
                Debug.Print("** bestComputerChoices count={0}, maxComputerScoreAfterOpponentsBestResponse={1:+#;-#;+0}.  Choose the highest scoring Move.", 
                        bestComputerChoices.Count, maxComputerScoreAfterOpponentsBestResponse);

            // find finalComputerChoices from bestComputerChoices based on computerChoiceScore
            int maxComputerScore = -int.MaxValue;
            List<Coord> finalComputerChoices = new List<Coord>();
            foreach (Coord computerChoice in bestComputerChoices)
            {
                BoardState computerBoardState = BoardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogDecisions)
                    Debug.Print("Top Computer choice: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}",
                            computerBoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);
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

            return finalComputerChoices;
        }

        /// <summary>
        /// Score all legalOpponentMoves to find Opponent's best response Score
        /// </summary>
        /// <param name="computerBoardState">BoardState for Opponent's current Turn</param>
        /// <returns>minScoreAfterOpponentTurn of all Opponent's legalMoves</returns>
        private int advanced_FindOpponentsBestResponseScore(BoardState computerBoardState)
        {
            int minScoreAfterOpponentTurn = int.MaxValue;
            //List<Coord> bestOpponentResponses = new List<Coord>(); // don't need a list, any of the ties will do
            Coord? bestOpponentResponse = null;
            BoardState bestOpponentResponseBoardState = null;

            List<Coord> legalOpponentMoves = computerBoardState.LegalMoves();
            foreach (Coord opponentResponse in legalOpponentMoves)
            {
                BoardState opponentResponseBoardState = computerBoardState.Clone();
                opponentResponseBoardState.PlacePieceAndFlipPiecesAndChangeTurns(opponentResponse);
                int opponentResponseScore = ScoreBoard(opponentResponseBoardState);
                if (LogEachExpertTurn && LogEachExpertLegalMoveResponse) 
                    Debug.Print("    - Opponent response: {0} resulting Score={1:+#;-#;+0}\nresulting BoardState:{2}", 
                            opponentResponse, opponentResponseScore, opponentResponseBoardState);

                if (opponentResponseScore < minScoreAfterOpponentTurn) // remember minScoreAfterOpponentTurn and start a new List of Moves that attain it
                {
                    minScoreAfterOpponentTurn = opponentResponseScore;
                    bestOpponentResponse = opponentResponse;
                    bestOpponentResponseBoardState = opponentResponseBoardState;
                    //bestOpponentResponses = new List<Coord>();
                }

                /*if (opponentResponseScore <= minScoreAfterOpponentTurn) // add choice to maxScoringChoices
                {
                    if (!bestOpponentResponses.Contains(opponentResponse))
                        bestOpponentResponses.Add(opponentResponse);
                }*/
            }

            if (LogDecisions)
                Debug.Print("    - Opponent response: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}",
                        computerBoardState.WhitesTurn ? 'W' : 'B', bestOpponentResponse, minScoreAfterOpponentTurn, bestOpponentResponseBoardState);

            return minScoreAfterOpponentTurn;
        }

        /// <summary>
        /// loop through all of Computer's Legal Moves
        /// collect the ones that maximize Score after several Turns
        /// if multiple Choices tie after several Turns then choose the ones with the highest first-move Score
        /// </summary>
        /// <returns>list of equal Computer Choices</returns>
        private List<Coord> recursive_ChooseHighestScoringAfterSeveralTurns()
        {
            int maxComputerScoreAfterSeveralTurns = -int.MaxValue;
            List<Coord> bestComputerChoices = new List<Coord>();
            List<Coord> legalComputerMoves = BoardState.LegalMoves();
            bool loggingEachTurn = (Level == LevelEnum.Expert && LogEachExpertTurn) || (Level == LevelEnum.Ultimate && LogEachUltimateTurn);
            foreach (Coord computerChoice in legalComputerMoves)
            {
                if (loggingEachTurn) // re-Log initial BoardState before each legal Move
                    Debug.Print("initial BoardState:{0}", BoardState);

                BoardState computerBoardState = BoardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (loggingEachTurn) // Log computerBoardState & computerChoiceScore for computerChoice
                    Debug.Print(" - Computer choice: 1={0}->{1} resulting Board's Score={2:+#;-#;+0}\nresulting BoardState:{3}", 
                            BoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);

                int nextTurn = 2;
                if (computerBoardState.WhitesTurn == BoardState.WhitesTurn) // turn skipped due to no legal moves
                    nextTurn++; // depth should go down to same Player to compare equally

                int minMaxScoreAfterSeveralTurns;
                if (Level == LevelEnum.Expert) // Expert
                    minMaxScoreAfterSeveralTurns = expert_FindMinMaxScoreForHighestScoringMove(computerBoardState, nextTurn);
                else // Ultimate
                    minMaxScoreAfterSeveralTurns = ultimate_FindMinMaxScoreForAllMyLegalMoves(computerBoardState, nextTurn);
                if (LogDecisions) // Log minMaxScoreAfterSeveralTurns for computerChoice
                    Debug.Print(" - Computer choice: {0}={1}->{2} resulting Board's Score={2:+#;-#;+0} minMaxScoreAfterSeveralTurns={3}\n\n",
                            BoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, minMaxScoreAfterSeveralTurns);

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

            if (LogDecisions)
                Debug.Print("** bestComputerChoices count={0}, maxComputerScoreAfterSeveralTurns={1:+#;-#;+0}.  Choose the highest scoring Move.", 
                        bestComputerChoices.Count, maxComputerScoreAfterSeveralTurns);

            // find finalComputerChoices from equal bestComputerChoices based on the one with best computerChoiceScore
            int maxComputerScore = -int.MaxValue;
            List<Coord> finalComputerChoices = new List<Coord>();
            foreach (Coord computerChoice in bestComputerChoices)
            {
                BoardState computerBoardState = BoardState.Clone();
                computerBoardState.PlacePieceAndFlipPiecesAndChangeTurns(computerChoice);
                int computerChoiceScore = ScoreBoard(computerBoardState);
                if (LogDecisions)
                    Debug.Print("Top Computer choice: {0}->{1} resulting Score={2:+#;-#;+0}\nresulting BoardState:{3}", 
                            computerBoardState.WhitesTurn ? 'W' : 'B', computerChoice, computerChoiceScore, computerBoardState);
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
                    Debug.Print("       - LegalMove Response: {0}={1}->{2} resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
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
                Debug.Print("- response {0}={1}->{2}: resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                        turn, boardState.WhitesTurn ? 'W' : 'B', minMaxResponse, minMaxScore, minMaxResponseBoardState);

            if (turn == EXPERT_TURNS_DEPTH)
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

            if (myTurn) // find recursive Score for each legalMove
            {
                int maxRecursiveScore = -int.MaxValue;
                Coord maxRecursiveResponse = new Coord();
                BoardState maxRecursiveResponseBoardState = null;
                foreach (Coord legalMove in legalMoves)
                {
                    BoardState legalMoveBoardState = boardState.Clone();
                    legalMoveBoardState.PlacePieceAndFlipPiecesAndChangeTurns(legalMove);

                    int recusiveScore;
                    if (legalMoveBoardState.endOfGame) // return ScoreEndOfGame()
                        recusiveScore = ScoreEndOfGame(legalMoveBoardState);
                    else if (turn == ULTIMATE_TURNS_DEPTH) // return ScoreBoard()
                        recusiveScore = ScoreBoard(legalMoveBoardState);
                    else // recurse
                    {
                        if (LogEachUltimateTurn)
                            Debug.Print("       - Computer LegalMove: {0}={1}->{2}\nresulting BoardState:{3}",
                                    turn, boardState.WhitesTurn ? 'W' : 'B', legalMove, legalMoveBoardState);

                        if (legalMoveBoardState.WhitesTurn == boardState.WhitesTurn) // turn skipped due to no legal moves
                            nextTurn++; // depth should go down to same Player to compare equally

                        // recurse to return resulting minMaxScore after nextTurn
                        recusiveScore = ultimate_FindMinMaxScoreForAllMyLegalMoves(legalMoveBoardState, nextTurn);
                    }

                    // Log each legalMove
                    if (LogEachUltimateTurn)
                        Debug.Print("- Computer LegalMove: {0}={1}->{2} recusiveScore={3:+#;-#;+0}\nlegalMoveBoardState:{4}",
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

            // find best Opponent Response
            int minScore = int.MaxValue;
            Coord minResponse = new Coord();
            BoardState minResponseBoardState = null;
            foreach (Coord response in legalMoves)
            {
                BoardState responseBoardState = boardState.Clone();
                responseBoardState.PlacePieceAndFlipPiecesAndChangeTurns(response);

                int responseScore;
                if (responseBoardState.endOfGame)
                    responseScore = ScoreEndOfGame(responseBoardState);
                else
                    responseScore = ScoreBoard(responseBoardState);

                // Log each legalMove response
                if (LogEachUltimateTurn && LogEachUltimateLegalMoveResponse)
                    Debug.Print("       - Opponent LegalMove: {0}={1}->{2} resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                            turn, boardState.WhitesTurn ? 'W' : 'B', response, responseScore, responseBoardState);

                if (responseScore < minScore) // opponent's Turn chooses lowest Score for me
                {
                    minScore = responseScore;
                    minResponse = response;
                    minResponseBoardState = responseBoardState;
                }
            }

            // Log the chosen minMaxResponse
            if (LogEachUltimateTurn)
                Debug.Print("- Opponent's best Response {0}={1}->{2}: resulting Score={3:+#;-#;+0}\nresulting BoardState:{4}",
                        turn, boardState.WhitesTurn ? 'W' : 'B', minResponse, minScore, minResponseBoardState);

            if (turn == ULTIMATE_TURNS_DEPTH)
                return minScore;

            if (minResponseBoardState.WhitesTurn == boardState.WhitesTurn) // turn skipped due to no legal moves
                nextTurn++; // depth should go down to same Player to compare equally

            // recurse to return resulting minMaxScore after nextTurn
            return ultimate_FindMinMaxScoreForAllMyLegalMoves(minResponseBoardState, nextTurn);
        }

        /// <summary>
        /// End-of-Game Score should just be a comparison of Counts
        /// MULTIPLIER helps it fit in with & out-weigh other Scores
        /// </summary>
        /// <param name="boardState">BoardState to Score</param>
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
        private int ScoreBoard(BoardState boardState)
		{
            const int numEmptyToConsiderBoardMostlyFilled = 3;
            bool boardMostlyFilled = BoardState.EmptyCount <= numEmptyToConsiderBoardMostlyFilled;

            int score = 0;
            foreach (Coord coord in boardState)
            { 
				Square square = boardState.GetSquare(coord);
				if (square.State != StateEnum.White && square.State != StateEnum.Black)
					continue;
				int weightedCoordValue;
                if ((Level == LevelEnum.Expert ||Level == LevelEnum.Ultimate) && boardMostlyFilled)
                    weightedCoordValue = 1000; // after board is mostly full, Expert should just try to win the game
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
        private int WeightedCoordValue(Coord coord)
		{
            // Beginner Level scores each square as 1, so it's just trying to fliup the most Pieces.

            // Higher Levels value Coords differently
            // Corners are highest, then Ends.
            // Coords before Corners & Ends are devalued since they lead to Opponent getting Corners & Ends.
            // And Coords before those are valuable since they lead to Opponent getting those devalued Coords.

            // Intermediate performs better with Negatives helping make better decisions.
            // Advanced & Expert perform worse when Negatives throw a wrench in comparing BoardStates, esp near the end of the Game.
            switch (Level)
			{
                case LevelEnum.Beginner:
                default:
					return 1; // All Coords

                // 50 -5 20 20
                // -5 -9 -2 -2
                // 20 -2  4  2
                // 20 -2  2  1
                case LevelEnum.Intermediate:
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

                // 2000   2 200 200
                //    2   1   3   3
                //  200   3  50  30
                //  200   3  30  10
                case LevelEnum.Advanced:
                case LevelEnum.Expert:
                case LevelEnum.Ultimate:
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
}
