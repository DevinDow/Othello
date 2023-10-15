#define FAST

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OthelloLib
{
    public class BoardState : IEnumerable<Coord>
    {
        // Fields
        public Square[,] squares; // 8x8 Array ( 0..7 , 0..7 )
        public bool WhitesTurn;
        public bool skippedTurn;
        public bool endOfGame;
        public List<Coord> coordsFlipped;


        // Properties
        public int WhiteCount
        {
            get
            {
                int count = 0;
#if FAST
                for (int y = 0; y < 8; y++) // loop rows
                {
                    for (int x = 0; x < 8; x++) // loop columns
                    {
                        if (squares[x, y].State == StateEnum.White)
                            count++;
                    }
                }
#else
                foreach (Coord coord in this)
                    if (GetSquare(coord).State == StateEnum.White)
                        count++;
#endif
                return count;
            }
        }

        public int BlackCount
        {
            get
            {
                int count = 0;
#if FAST
                for (int y = 0; y < 8; y++) // loop rows
                {
                    for (int x = 0; x < 8; x++) // loop columns
                    {
                        if (squares[x, y].State == StateEnum.Black)
                            count++;
                    }
                }
#else
                foreach (Coord coord in this)
                    if (GetSquare(coord).State == StateEnum.Black)
                        count++;
#endif
                return count;
            }
        }

        public int EmptyCount
        {
            get
            {
                int count = 0;
#if FAST
                for (int y = 0; y < 8; y++) // loop rows
                {
                    for (int x = 0; x < 8; x++) // loop columns
                    {
                        if (squares[x, y].State == StateEnum.Empty)
                            count++;
                    }
                }
#else
                foreach (Coord coord in this)
                    if (GetSquare(coord).State == StateEnum.Empty)
                        count++;
#endif
                return count;
            }
        }


        // Constructor
        /// <summary>
        /// Constructs a BoardState
        /// </summary>
        /// <param name="whitesTurn">in Othello, Black moves first</param>
        /// <param name="setInitialPieces">should the initial four Pieces be added?</param>
        public BoardState(bool whitesTurn = false, bool setInitialPieces = true)
        {
            skippedTurn = endOfGame = false;
            squares = new Square[8, 8];
            WhitesTurn = whitesTurn;

#if FAST
            for (int y = 0; y < 8; y++) // loop rows
                for (int x = 0; x < 8; x++) // loop columns
                    squares[x, y] = new Square();
#else
                foreach (Coord coord in this)
                    SetSquare(coord, new Square());
#endif

            if (setInitialPieces)
            {
                GetSquare(new Coord(4, 4)).State = StateEnum.Black;
                GetSquare(new Coord(5, 5)).State = StateEnum.Black;
                GetSquare(new Coord(4, 5)).State = StateEnum.White;
                GetSquare(new Coord(5, 4)).State = StateEnum.White;
            }
        }


        // Methods
        public Square GetSquare(Coord coord)
        {
            return squares[coord.x - 1, coord.y - 1];
        }

        public void SetSquare(Coord coord, Square square)
        {
            squares[coord.x - 1, coord.y - 1] = square;
        }

        /// <summary>
        /// finds all legal Moves
        /// </summary>
        /// <returns>list of Coord</returns>
        public List<Coord> LegalMoves()
        {
            List<Coord> legalMoves = new List<Coord>();

#if FAST
            for (int y = 1; y <= 8; y++) // loop rows
            {
                for (int x = 1; x <= 8; x++) // loop columns
                {
                    Coord coord = new Coord(x, y);
                    if (IsLegalMove(coord))
                        legalMoves.Add(coord);
                }
            }
#else
            foreach (Coord coord in this)
                if (IsLegalMove(coord))
                    legalMoves.Add(coord);
#endif

            return legalMoves;
        }

        public bool IsLegalMoveAvailable()
        {
#if FAST
            for (int y = 1; y <= 8; y++) // loop rows
            {
                for (int x = 1; x <= 8; x++) // loop columns
                {
                    Coord coord = new Coord(x, y);
                    if (IsLegalMove(coord))
                        return true;
                }
            }
            return false;
#else
            return LegalMoves().Count > 0;
#endif
        }

        public bool IsLegalMove(Coord coord)
        {
            Square square = GetSquare(coord);

            if (square.State == StateEnum.Black || square.State == StateEnum.White)
                return false;

            if (IsSuccessfulDirection(coord, -1, 0))
                return true;

            if (IsSuccessfulDirection(coord, -1, 1))
                return true;

            if (IsSuccessfulDirection(coord, 0, 1))
                return true;

            if (IsSuccessfulDirection(coord, 1, 1))
                return true;

            if (IsSuccessfulDirection(coord, 1, 0))
                return true;

            if (IsSuccessfulDirection(coord, 1, -1))
                return true;

            if (IsSuccessfulDirection(coord, 0, -1))
                return true;

            if (IsSuccessfulDirection(coord, -1, -1))
                return true;

            return false;
        }

        private bool IsSuccessfulDirection(Coord coord, int dx, int dy)
        {
            bool foundOpposite = false;

            int x = coord.x + dx;
            int y = coord.y + dy;

            while (x > 0 && x <= 8 && y > 0 && y <= 8)
            {
                Square square = GetSquare(new Coord(x, y));
     
                if (square.State == StateEnum.Empty)
                    return false;

                if (foundOpposite)
                {
                    if (square.State == StateEnum.White && WhitesTurn ||
                        square.State == StateEnum.Black && !WhitesTurn)
                        return true;
                }
                else
                {
                    if (square.State == StateEnum.White && !WhitesTurn ||
                        square.State == StateEnum.Black && WhitesTurn)
                        foundOpposite = true;
                    else
                        return false;
                }

                x += dx;
                y += dy;
            }

            return false;
        }

        public void PlacePieceAndFlipPiecesAndChangeTurns(Coord coord)
        {
            // place Piece at coord
            Square square = GetSquare(coord);
            if (WhitesTurn)
                square.State = StateEnum.White;
            else
                square.State = StateEnum.Black;

            // flip all affected Pieces
            coordsFlipped = new List<Coord>();
            FlipInDirection(coord, 0, -1);
            FlipInDirection(coord, -1, -1);
            FlipInDirection(coord, -1, 0);
            FlipInDirection(coord, -1, 1);
            FlipInDirection(coord, 0, 1);
            FlipInDirection(coord, 1, 1);
            FlipInDirection(coord, 1, 0);
            FlipInDirection(coord, 1, -1);

            // change Turns
            skippedTurn = false;
            WhitesTurn = !WhitesTurn;
            if (!IsLegalMoveAvailable())
            {
                skippedTurn = true;
                WhitesTurn = !WhitesTurn;
                if (!IsLegalMoveAvailable())
                    endOfGame = true;
            }
        }

        private void FlipInDirection(Coord choice, int dx, int dy)
        {
            int x = choice.x + dx;
            int y = choice.y + dy;

            // find partner square
            while (x >= 1 && x <= 8 && y >= 1 && y <= 8)
            {
                Square partnerSquare = GetSquare(new Coord(x, y));
                if (partnerSquare.State != StateEnum.Black && partnerSquare.State != StateEnum.White)
                    return;

                if (WhitesTurn && partnerSquare.State == StateEnum.Black ||
                    !WhitesTurn && partnerSquare.State == StateEnum.White) // not a partner piece
                {
                    x += dx;
                    y += dy;
                    continue; // keep looking for partner piece
                }

                // partner piece found
                x -= dx;
                y -= dy;

                // work back to placed piece flipping
                while (!(x == choice.x && y == choice.y))
                {
                    Coord coordToFlip = new Coord(x, y);
                    coordsFlipped.Add(coordToFlip);
     
                    Square flippedSquare = GetSquare(coordToFlip);
                    flippedSquare.State = WhitesTurn ? StateEnum.White : flippedSquare.State = StateEnum.Black;

                    x -= dx;
                    y -= dy;
                }

                return;
            }
        }

        /// <summary>
        /// returns a Deep Copy
        /// </summary>
        /// <returns>a Deep Copy of this</returns>
        public BoardState Clone()
        {
            BoardState newBoardState = new BoardState(false);
            newBoardState.WhitesTurn = WhitesTurn;

#if FAST
            for (int y = 0; y < 8; y++) // loop rows
                for (int x = 0; x < 8; x++) // loop columns
                    newBoardState.squares[x, y].State = squares[x, y].State;
#else
            foreach (Coord coord in this)
            {
                Square square = GetSquare(coord);
                newBoardState.SetSquare(coord, new Square(square.State));
            }
#endif

            return newBoardState;
        }

        // Overrides
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (endOfGame)
                sb.Append(" *** END OF GAME *** \n");
            else
                sb.AppendFormat("Turn={0}\n", WhitesTurn ? "W" : "B");
            foreach (Coord coord in this)
            {
                Square square = GetSquare(coord);
                switch (square.State)
                {
                    case StateEnum.Black:
                        sb.Append(" B");
                        break;
                    case StateEnum.White:
                        sb.Append(" W");
                        break;
                    case StateEnum.Empty:
                        sb.Append(" .");
                        break;
                }
                if (coord.x == 8 && coord.y < 8)
                    sb.Append("\n");
            }
            return sb.ToString();
        }

        // implement IEnureable<Coord>
        // could have returned Squares instead, but sometimes also need Coord, so just get Square from Coord
        public IEnumerator<Coord> GetEnumerator()
        {
            for (int y = 1; y <= 8; y++) // loop rows
            {
                for (int x = 1; x <= 8; x++) // loop columns
                {
                    Coord coord = new Coord(x, y);
                    yield return coord;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int y = 1; y <= 8; y++) // loop rows
            {
                for (int x = 1; x <= 8; x++) // loop columns
                {
                    Coord coord = new Coord(x, y);
                    yield return coord;
                }
            }
        }
    }
}
