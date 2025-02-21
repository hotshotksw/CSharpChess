using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Tests
{
    [TestClass()]
    public class ChessBoardTests
    {
        [TestMethod()]
        public void ShouldCreateRegularBoard()
        {
            List<string> truePieces = new List<string> { "Rook", "Knight", "Bishop", "Queen", "King", "Bishop", "Knight", "Rook",
                "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn" };
            
            ChessBoard board = new ChessBoard(false);
            List<string> pieces = board.CreateBoard();

            bool correct = true;
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i] != truePieces[i]) correct = false;
            }

            Assert.AreEqual(true, correct);
        }

        [TestMethod()]
        public void ShouldCreate960Board()
        {
            List<string> truePieces = new List<string> { "Rook", "Knight", "Bishop", "Queen", "King", "Bishop", "Knight", "Rook",
                "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn" };

            ChessBoard board = new ChessBoard(true);
            List<string> pieces = board.CreateBoard();

            bool correct = true;
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i] != truePieces[i])
                {
                    correct = false;
                    i = pieces.Count;
                }
            }
            
            Assert.AreEqual(false, correct);
        }

        [TestMethod()]
        public void ShouldhaveRooksOnOppositeSidesOfKing()
        {
            ChessBoard board = new ChessBoard(true);
            List<string> pieces = board.CreateBoard();

            int LeftRookIndex = pieces.IndexOf("Rook");
            int RightRookIndex = pieces.LastIndexOf("Rook");
            int KingIndex = pieces.IndexOf("King");

            bool correct = (LeftRookIndex < KingIndex && KingIndex < RightRookIndex) ? true : false;

            Assert.AreEqual(true, correct);
        }

        [TestMethod()]
        public void ShouldOnlyHavePawns()
        {
            ChessBoard board = new ChessBoard(true);
            List<string> pieces = board.CreateBoard();

            bool correct = true;
            for (int i = 8; i < pieces.Count; i++)
            {
                if (pieces[i] != "Pawn")
                {
                    correct = false;
                    i = pieces.Count;
                }
            }

            Assert.AreEqual(true, correct);
        }
    }
}