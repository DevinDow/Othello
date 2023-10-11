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
        /*
        public static bool LogEachUltimateTurn = false;
        public static bool LogEachUltimateLegalMoveResponse = false;*/
        private Random random = new Random();
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
    }
}
