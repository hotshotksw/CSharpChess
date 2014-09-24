﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    abstract class ChessPiece
    {
        // Pawn fields
        protected bool canEnPassantLeft;
        protected bool canEnPassantRight;
        protected bool canDoubleJump;

        // Other fields
        protected bool canCastle; // For rooks and kings
        protected Point[][] availableMoves; // Jagged array for moves ([direction][distance])
        protected Point[][] availableAttacks; // Same as Moves unless your a pawn.

        public Point[][] AvailableMoves
        {
            get { return availableMoves; }
        }

        public Point[][] AvailableAttacks
        {
            get { return availableAttacks; }
        }
        
        public ChessPiece()
        {
            CalculateMoves();
        }

        public abstract ChessPiece CalculateMoves();

        /// <summary>
        /// Get relative horizontal or virtical movement coordinates
        /// Used by: King, Queen, Pawn, Rook
        /// </summary>
        /// <param name="distance">How far in the given direction.</param>
        /// <param name="direction">Direction relative to player</param>
        /// <returns>Return an array for horizontal or virtical movment</returns>
        protected Point[] GetMovementArray(int distance, Direction direction)
        {
            return new Point[] { new Point(1, 1) }; // TODO: Make this return good data.
        }

        /// <summary>
        /// Get relative diagnal movement coordinates
        /// Used by: King, Queen, Pawn, Bishop
        /// </summary>
        /// <param name="distance">How far in the given direction</param>
        /// <param name="direction">Direction relative to player</param>
        /// <returns>Return an array for diagnal movement</returns>
        protected Point[] GetDiagnalMovementArray(int distance, DiagnalDirection direction)
        {
            return new Point[]{new Point(1,1)}; // TODO: Make this return good data.
        }
    }
}
