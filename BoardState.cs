using System;
using System.Collections.Generic;
using System.Text;

namespace Othello
{
    public class BoardState
    {
        // Fields
        public Square[,] squares; // 8x8 Array ( 0..7 , 0..7 )
        public bool WhitesTurn;

        // Constructor
        public BoardState()
        {
            squares = new Square[8, 8];
            WhitesTurn = false;

            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    squares[x, y] = new Square(x, y);
                }

            squares[3, 3].State = StateEnum.Black;
            squares[4, 4].State = StateEnum.Black;
            squares[3, 4].State = StateEnum.White;
            squares[4, 3].State = StateEnum.White;
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

        public bool IsLegalMoveAvailable()
        {
            for (int x = 1; x <= 8; x++)
                for (int y = 1; y <= 8; y++)
                    if (IsLegalMove(new Coord(x, y)))
                        return true;

            return false;
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
                if (square.State != StateEnum.Black && square.State != StateEnum.White)
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

        public void FlipPieces(Coord coord)
        {
            FlipInDirection(coord, 0, -1);
            FlipInDirection(coord, -1, -1);
            FlipInDirection(coord, -1, 0);
            FlipInDirection(coord, -1, 1);
            FlipInDirection(coord, 0, 1);
            FlipInDirection(coord, 1, 1);
            FlipInDirection(coord, 1, 0);
            FlipInDirection(coord, 1, -1);
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
                    continue;
                }

                // partner square found
                x -= dx;
                y -= dy;

                // work back to placed piece flipping
                while (!(x == choice.x && y == choice.y))
                {
                    Square flippedSquare = GetSquare(new Coord(x, y));
                    if (WhitesTurn)
                        flippedSquare.State = StateEnum.White;
                    else
                        flippedSquare.State = StateEnum.Black;

                    flippedSquare.Flip();

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
            BoardState newBoardState = new BoardState();
            newBoardState.squares = new Square[8, 8];
            newBoardState.WhitesTurn = WhitesTurn;

            for (int x = 1; x <= 8; x++)
                for (int y = 1; y <= 8; y++)
                {
                    Coord coord = new Coord(x, y);
                    Square square = GetSquare(coord);
                    newBoardState.SetSquare(coord, new Square(x, y, square.State));
                }

            return newBoardState;
        }

        // Overrides
        public override string ToString()
        {
            string s = string.Empty;
            for (int x = 1; x <= 8; x++)
                for (int y = 1; y <= 8; y++)
                {
                    Square square = GetSquare(new Coord(x, y));
                    if (square.State != StateEnum.Empty)
                    {
                        s += string.Format("({0},{1})={2}, ", x, y, square.State == StateEnum.Black ? "B" : "W");
                    }
                }
            return s;
        }
    }
}
