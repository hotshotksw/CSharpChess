using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class ChessBoard
    {
        private ChessPiece[,] boardArray;
        private const int COLUMNS = 8;
        private int ROWS = 8;
        private bool is960;
        
        public ChessBoard(bool is960)
        {
            this.is960 = is960;
            SetupBoard();
        }

        public int GetLength(int l)
        {
            return boardArray.GetLength(l);
        }

        public ChessPiece this[int x, int y]
        {
            get { return boardArray[x, y]; }
        }

        public ChessBoard SetupBoard()
        {
            boardArray = new ChessPiece[COLUMNS, ROWS];
            List<string> playerPieces = new List<string>();

            playerPieces = CreateBoard();

            for (int i = 0; i < COLUMNS; i++)
            {
                // Player 0 pieces
                boardArray[i, 0] =          (ChessPiece)Activator.CreateInstance(
                                                Type.GetType("Chess." + playerPieces[i]));
                boardArray[i, 1] =          (ChessPiece)Activator.CreateInstance(
                                                Type.GetType("Chess." + playerPieces[i + COLUMNS]));
                // Player 1 pieces
                boardArray[i, ROWS - 1] =   (ChessPiece)Activator.CreateInstance(
                                                Type.GetType("Chess." + playerPieces[i]), new object[] { 1 });
                boardArray[i, ROWS - 2] =   (ChessPiece)Activator.CreateInstance(
                                                Type.GetType("Chess." + playerPieces[i + COLUMNS]), new object[] { 1 });
            }
            return this;
        }


        /// <summary>
        /// This method creates a list for the rows of the chess board and uses the setup of either regular chess, or goes
        /// through the process or randomizing the positions of the first row of pieces using the rules established by
        /// Chess960 (Bishops on opposite colors, King in random position with both rooks on opposite sides of it, and
        /// the Queen and Knights filling in the rest of the positions). Both versions have all pawns in the front.
        /// </summary>
        /// <returns>
        /// If the Board object wants to use Chess960, gives a string list that holds the positions of pieces using its rules.
        /// Otherwise, returns a string list that uses the positions of regular Chess.
        /// </returns>
        public List<string> CreateBoard()
        {
            // Initializes List for pieces
            List<string> pieces = new List<string>();


            // If 'ChessBoard' is not using 960 rules, give piece positions of regular chess.
            if (!is960)
            {
                pieces = new List<string> { "Rook", "Knight", "Bishop", "Queen", "King", "Bishop", "Knight", "Rook",
                "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn" };
            } 
            else
            {
                // 'Pieces' now holds the row of pawns and an empty row for the incoming pieces.
                pieces = new List<string> { "", "", "", "", "", "", "", "",
                "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn" };
                // List of indices that will be removed when used.
                List<int> indices = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };

                // For random selection of indices.
                Random r = new Random();

                // Selects numbers between 0 (inclusive) and 4 (exclusive). 'Even' multiplies by two to receive value of
                // 0, 2, 4, or 6. 'Odd' multiplies by 2 to get the same values as the even, but then adds 1 so that it
                // receives a value of 1, 3, 5, or 7. These values determine the array positions of the Bishop pieces.
                int randomNumberEven = r.Next(0, 4) * 2;
                int randomNumberOdd = r.Next(0, 4) * 2 + 1;
                pieces[randomNumberEven] = "Bishop";
                pieces[randomNumberOdd] = "Bishop";

                // Remove the values from 'indices' based on previous random numbers.
                indices.RemoveAt(indices.IndexOf(randomNumberEven));
                indices.RemoveAt(indices.IndexOf(randomNumberOdd));

                // As there are no specific parameters for where the Queen and Knights go in Chess960, simply run through
                // a for loop and pick a random index from indices to determine where the pieces are put in the array.
                // Once selected, remove the index value.
                for (int i = 0; i < 3; i++)
                {
                    int randomNumber = r.Next(0, indices.Count);
                    if (i == 2) pieces[indices[randomNumber]] = "Queen";
                    else pieces[indices[randomNumber]] = "Knight";
                    indices.RemoveAt(randomNumber);
                }

                // As there are only 3 index values remaining, and the Rooks must be on opposite sides of the King, place
                // them in order of Left Rook, King, Right Rook. No need for deleting the indeces now.
                pieces[indices[0]] = "Rook";
                pieces[indices[1]] = "King";
                pieces[indices[2]] = "Rook";
            }

            // Return the list of finalized rows for either regular chess or Chess960.
            return pieces;
        }

        /// <summary>
        /// Calculate the actual actions available for a Chess Piece at a set of coordinates.
        /// </summary>
        /// <param name="x">The number of squares right of the bottom left square</param>
        /// <param name="y">The number of squares above the bottom left square</param>
        /// <param name="ignoreCheck">Do not check for threats to the king</param>
        /// <param name="attackActions">Calculate attacks</param>
        /// <param name="moveActions">Calculate movement</param>
        /// <param name="boardArray">An optional substitute board</param>
        /// <returns>A list of points that can be moved to</returns>
        public IEnumerable<Point> PieceActions(int x, int y, bool ignoreCheck = false, bool attackActions = true, bool moveActions = true, ChessPiece[,] boardArray = null)
        {
            if (boardArray == null)
            {
                boardArray = this.boardArray;
            }

            bool[,] legalActions = new bool[boardArray.GetLength(0), boardArray.GetLength(1)];
            List<Point> availableActions = new List<Point>();
            ChessPiece movingPeice = boardArray[x, y];
            
            if (attackActions)
            {
                foreach (Point[] direction in movingPeice.AvailableAttacks)
                {
                    foreach (Point attackPoint in direction)
                    {
                        Point adjustedPoint = new Point(attackPoint.x + x, attackPoint.y + y);
                        if (ValidatePoint(adjustedPoint))
                        {
                            if (boardArray[adjustedPoint.x, adjustedPoint.y] != null
                                && boardArray[adjustedPoint.x, adjustedPoint.y].Player ==
                                movingPeice.Player) break;
                            if (boardArray[adjustedPoint.x, adjustedPoint.y] != null)
                            {
                                AddMove(availableActions, new Point(x, y), adjustedPoint, ignoreCheck);
                                break;
                            }
                        }
                    }
                }
            }

            if (moveActions)
            {
                foreach (Point[] direction in movingPeice.AvailableMoves)
                {
                    foreach (Point movePoint in direction)
                    {
                        Point adjustedPoint = new Point(movePoint.x + x, movePoint.y + y);
                        if (ValidatePoint(adjustedPoint))
                        {
                            if (boardArray[adjustedPoint.x, adjustedPoint.y] != null) break;
                            AddMove(availableActions, new Point(x, y), adjustedPoint, ignoreCheck);
                        }
                    }
                }
            }

            if (movingPeice is King && ((King)movingPeice).CanCastle)
            {
                int rookX = 0;
                if (boardArray[rookX, y] is Rook && ((Rook)boardArray[rookX, y]).CanCastle)
                {
                    bool missedCondition = false;
                    foreach (int rangeX in Enumerable.Range(rookX + 1, Math.Abs(rookX - x) - 1))
                    {
                        if (boardArray[rangeX, y] != null) missedCondition = true;
                        // TODO: Validate that the king won't move through check
                    }
                    // TODO: Validate that king isn't currently in check
                    missedCondition = missedCondition || KingInCheck(movingPeice.Player);
                    if (!missedCondition) 
                        AddMove(availableActions, new Point(x, y), new Point(x - 2, y), ignoreCheck);
                }
                rookX = COLUMNS - 1;
                if (boardArray[rookX, y] is Rook && ((Rook)boardArray[rookX, y]).CanCastle)
                {
                    bool missedCondition = false;
                    foreach (int rangeX in Enumerable.Range(x + 1, Math.Abs(rookX - x) - 1))
                    {
                        if (boardArray[rangeX, y] != null) missedCondition = true;
                        // TODO: Validate that the king won't move through check
                    }
                    // TODO: Validate that king isn't currently in check
                    missedCondition = missedCondition || KingInCheck(movingPeice.Player);
                    if (!missedCondition) 
                        AddMove(availableActions, new Point(x, y), new Point(x + 2, y), ignoreCheck);
                }
            }

            if (movingPeice is Pawn)
            {
                Pawn pawn = (Pawn)movingPeice;
                int flipDirection = 1;

                if (pawn.Player == 1) flipDirection = -1;
                if (pawn.CanEnPassantLeft)
                {
                    Point attackPoint;
                    attackPoint = ChessPiece.GetDiagnalMovementArray(1, DiagnalDirection.FORWARD_LEFT)[0];
                    attackPoint.y *= flipDirection;
                    attackPoint.y += y;
                    attackPoint.x += x;
                    if (ValidatePoint(attackPoint))
                    {
                        AddMove(availableActions, new Point(x,y), attackPoint, ignoreCheck);
                    }
                }

                if (pawn.CanEnPassantRight)
                {
                    Point attackPoint;
                    attackPoint = ChessPiece.GetDiagnalMovementArray(1, DiagnalDirection.FORWARD_RIGHT)[0];
                    attackPoint.y *= flipDirection;
                    attackPoint.y += y;
                    attackPoint.x += x;
                    if (ValidatePoint(attackPoint))
                    {
                        AddMove(availableActions, new Point(x, y), attackPoint, ignoreCheck);
                    }
                }
            }

            return availableActions;
        }

        private void AddMove(List<Point> availableActions, Point fromPoint, Point toPoint, bool ignoreCheck = false)
        {
            bool kingInCheck = false;

            if (!ignoreCheck)
            {
                ChessPiece movingPiece = boardArray[fromPoint.x, fromPoint.y];
                ChessPiece[,] boardArrayBackup = (ChessPiece[,])boardArray.Clone();
                ActionPiece(fromPoint, toPoint, true);
                kingInCheck = KingInCheck(movingPiece.Player);
                boardArray = boardArrayBackup;
            }

            if (ignoreCheck || !kingInCheck) availableActions.Add(toPoint);
        }

        public bool KingInCheck(int player)
        {
            for (int x = 0; x < COLUMNS; x++)
            {
                for (int y = 0; y < ROWS; y++)
                {
                    ChessPiece chessPiece = boardArray[x, y];
                    if (chessPiece != null
                        && chessPiece.Player == player
                        && chessPiece is King)
                    {
                        if (CheckSquareVulnerable(x, y, player))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            throw new Exception("King wasn't found!");
        }

        public IEnumerable<Point> PieceActions(Point position, bool ignoreCheck = false, bool attackActions = true, bool moveActions = true, ChessPiece[,] boardArray = null)
        {
            return PieceActions(position.x, position.y, ignoreCheck, attackActions, moveActions, boardArray);
        }

        /// <summary>
        /// Move a peice from one location on the board to another.
        /// </summary>
        /// <param name="fromX">The x coordinate of the piece that is moving.</param>
        /// <param name="fromY">The y coordinate of the piece that is moving.</param>
        /// <param name="toX">The x coordinate of the destination.</param>
        /// <param name="toY">The y coordinate of the destination.</param>
        /// <returns>Returns true on success or false on failure.</returns>
        public bool ActionPiece(int fromX, int fromY, int toX, int toY)
        {
            return ActionPiece(new Point(fromX, fromY), new Point(toX, toY));
        }

        /// <summary>
        /// Move a peice from one location on the board to another.
        /// </summary>
        /// <param name="from">The location of the piece that is moving.</param>
        /// <param name="to">The location to move to.</param>
        /// <returns>Returns true on success or false on failure.</returns>
        public bool ActionPiece(Point from, Point to, bool bypassValidaiton = false)
        {
            if (bypassValidaiton || PieceActions(from).Contains(to))
            {
                ChessPiece movingPeice = boardArray[from.x, from.y];
                if (movingPeice is Pawn)
                {
                    Pawn pawn = (Pawn)movingPeice;
                    // If this was a double jump, check enpassant
                    if (Math.Abs(from.y - to.y) == 2)
                    {
                        int adjasentX = to.x - 1;
                        if (adjasentX > -1
                            && boardArray[adjasentX, to.y] != null
                            && boardArray[adjasentX, to.y].Player != movingPeice.Player
                            && boardArray[adjasentX, to.y] is Pawn)
                        {
                            if (!bypassValidaiton) 
                                ((Pawn)boardArray[adjasentX, to.y]).CanEnPassantRight = true;
                        }
                        adjasentX += 2;
                        if (adjasentX < COLUMNS
                            && boardArray[adjasentX, to.y] != null
                            && boardArray[adjasentX, to.y].Player != movingPeice.Player
                            && boardArray[adjasentX, to.y] is Pawn)
                        {
                            if (!bypassValidaiton) 
                                ((Pawn)boardArray[adjasentX, to.y]).CanEnPassantLeft = true;
                        }
                    }
                    // If this was a sideways jump to null, it was enpassant!
                    if (from.x != to.x && boardArray[to.x, to.y] == null)
                    {
                        boardArray[to.x, from.y] = null;
                    }

                    if (!bypassValidaiton) // Pawns can't double jump after they move.
                        pawn.CanDoubleJump = false; 
                }
                if (movingPeice is CastlePiece)
                {
                    CastlePiece rookOrKing = (CastlePiece)movingPeice;
                    if (!bypassValidaiton) // Castling can't be done after moving
                        rookOrKing.CanCastle = false; 
                }
                if (movingPeice is King)
                {
                    King king = (King)movingPeice;
                    if (from.x - to.x == 2)
                    {   // Move rook for Queenside castle
                        boardArray[to.x + 1, from.y] = boardArray[0, from.y];
                        boardArray[0, from.y] = null;
                    }
                    if (from.x - to.x == -2)
                    {   // Move rook for Kingside castle
                        boardArray[to.x - 1, from.y] = boardArray[COLUMNS - 1, from.y];
                        boardArray[COLUMNS - 1, from.y] = null;
                    }
                }
                movingPeice.CalculateMoves();
                boardArray[from.x, from.y] = null;
                boardArray[to.x, to.y] = movingPeice;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Find out if a square is vulnerable to attacks by another player.
        /// </summary>
        /// <param name="player">The vulnerable player</param>
        /// <param name="boardArray">An optional substitute board for validating moves</param>
        /// <returns>True if square can be attacked</returns>
        public bool CheckSquareVulnerable(int squareX, int squareY, int player, ChessPiece[,] boardArray = null)
        {
            if (boardArray == null)
            {
                boardArray = this.boardArray;
            }

            for (int x = 0; x < boardArray.GetLength(0); x++)
            {
                for (int y = 0; y < boardArray.GetLength(1); y++)
                {
                    if (boardArray[x, y] != null && boardArray[x, y].Player != player)
                    {
                        foreach (Point point in PieceActions(x, y, true, true, false, boardArray))
                        {
                            if (point.x == squareX && point.y == squareY)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool ValidateRange(int value, int high, int low = -1)
        {
            return value > low && value < high;
        }

        public bool ValidateX(int value)
        {
            return ValidateRange(value, boardArray.GetLength(0));
        }

        public bool ValidateY(int value)
        {
            return ValidateRange(value, boardArray.GetLength(1));
        }

        public bool ValidatePoint(Point point)
        {
            return ValidateX(point.x) && ValidateY(point.y);
        }
    }
}
