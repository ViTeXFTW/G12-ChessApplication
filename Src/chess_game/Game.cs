using G12_ChessApplication.Src.chess_game.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace G12_ChessApplication.Src.chess_game
{
    public class Game
    {
        public Player whitePlayer {  get; set; }
        public Player blackPlayer { get; set; }
        public Player currentPlayer { get; set; }
        public bool IsPieceSelected { get; set; }
        public int SelectedPieceIndex { get; set; }

        private ChessPiece[] gameState = new ChessPiece[64];

        public Game(string whitePlayerName, string blackPlayerName)
        {
            whitePlayer = new Player(whitePlayerName, ChessColor.WHITE);
            blackPlayer = new Player(blackPlayerName, ChessColor.BLACK);
            currentPlayer = whitePlayer;
            gameState = FenParser.CreatePieceArray();
        }

        public bool CanSelectPieceAt(int index)
        {
            var piece = gameState[index];

            if (piece == null)
            {
                Trace.WriteLine($"Selected empty square");
                return false;
            }

            var pieceColor = piece.ChessColor;

            Trace.WriteLine($"Selected Piece: {piece}, {pieceColor}");

            if (pieceColor == ChessColor.BLACK && currentPlayer == blackPlayer)
            {
                return true;
            }
            if (pieceColor == ChessColor.WHITE && currentPlayer == whitePlayer)
            {
                return true;
            }
            return false;
        }

        public void SelectPieceAt(int index)
        {
            IsPieceSelected = true;
            SelectedPieceIndex = index;
        }

        public void DeselectPiece()
        {
            IsPieceSelected = false;
        }


        public bool IsValidMove(int fromIndex, int toIndex)
        {
            ChessPiece fromPiece = gameState[fromIndex];
            ChessPiece toPiece = gameState[toIndex];

            if (fromPiece == null)
            {
                return false;
            }

            if (toPiece != null && fromPiece.ChessColor == toPiece.ChessColor)
            {
                Trace.WriteLine($"Cannot capture own piece at {toIndex}");
                return false;
            }

            foreach (var validMove in fromPiece.legalMoves)
            {

            }


        }

        //public bool IsValidMove(int fromIndex, int toIndex)
        //{
        //    ChessPiece fromPiece = gameState[fromIndex];
        //    ChessPiece toPiece = gameState[toIndex];

        //    if (fromPiece == null)
        //    {
        //        Trace.WriteLine($"fromPiece was null");
        //        return false;
        //    }

        //    // Cannot capture your own piece
        //    if (toPiece != null && fromPiece.ChessColor == toPiece.ChessColor)
        //    {
        //        Trace.WriteLine($"Cannot capture own piece at {toIndex}");
        //        return false;
        //    }

        //    // Determine the type of the piece and validate the move
        //    if (fromPiece is Pawn)
        //    {
        //        return IsValidPawnMove(fromIndex, toIndex);
        //    }
        //    else if (fromPiece is Knight)
        //    {
        //        return IsValidKnightMove(fromIndex, toIndex);
        //    }
        //    else if (fromPiece is Bishop)
        //    {
        //        return IsValidBishopMove(fromIndex, toIndex);
        //    }
        //    else if (fromPiece is Rook)
        //    {
        //        return IsValidRookMove(fromIndex, toIndex);
        //    }
        //    else if (fromPiece is Queen)
        //    {
        //        return IsValidQueenMove(fromIndex, toIndex);
        //    }
        //    else if (fromPiece is King)
        //    {
        //        return IsValidKingMove(fromIndex, toIndex);
        //    }
        //    else
        //    {
        //        Trace.WriteLine($"Unknown piece type at {fromIndex}");
        //        return false;
        //    }
        //}

        public void ApplyMove(int fromIndex, int toIndex)
        {
            gameState[toIndex] = gameState[fromIndex];
            gameState[fromIndex] = null;
            DeselectPiece();
        }

        private bool IsValidPawnMove(int fromIndex, int toIndex)
        {
            ChessPiece fromPiece = gameState[fromIndex];
            ChessPiece toPiece = gameState[toIndex];
            int fromRow = fromIndex / 8;
            int fromCol = fromIndex % 8;
            int toRow = toIndex / 8;
            int toCol = toIndex % 8;

            int rowDiff = toRow - fromRow;
            int colDiff = toCol - fromCol;

            if (fromPiece.ChessColor == ChessColor.WHITE)
            {
                // Forward movement
                if (colDiff == 0)
                {
                    Trace.WriteLine("Forward pawn movement");
                    // Move forward one square
                    if (rowDiff == 1 && toPiece == null)
                    {
                        Trace.WriteLine("Forward 1 square");
                        return true;
                    }
                    // Move forward two squares from starting position
                    if (rowDiff == 2 && fromRow == 1 && toPiece == null && gameState[(fromRow + 1) * 8 + fromCol] == null)
                    {
                        Trace.WriteLine("Forward 2 square");
                        return true;
                    }
                }
                // Capture diagonally
                else if (Math.Abs(colDiff) == 1 && rowDiff == 1 && toPiece != null && toPiece.ChessColor == ChessColor.BLACK)
                {
                    Trace.WriteLine("Diagonal attack");
                    return true;
                }
            }
            else // Black pawn
            {
                // Forward movement
                if (colDiff == 0)
                {
                    // Move forward one square
                    if (rowDiff == -1 && toPiece == null)
                    {
                        return true;
                    }
                    // Move forward two squares from starting position
                    if (rowDiff == -2 && fromRow == 6 && toPiece == null && gameState[(fromRow - 1) * 8 + fromCol] == null)
                    {
                        return true;
                    }
                }
                // Capture diagonally
                else if (Math.Abs(colDiff) == 1 && rowDiff == -1 && toPiece != null && toPiece.ChessColor == ChessColor.WHITE)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsValidKnightMove(int fromIndex, int toIndex)
        {
            int fromRow = fromIndex / 8;
            int fromCol = fromIndex % 8;
            int toRow = toIndex / 8;
            int toCol = toIndex % 8;

            int rowDiff = Math.Abs(toRow - fromRow);
            int colDiff = Math.Abs(toCol - fromCol);

            // Knight moves in L-shape: 2 by 1 or 1 by 2
            if ((rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2))
            {
                return true;
            }

            return false;
        }

        private bool IsValidBishopMove(int fromIndex, int toIndex)
        {
            int fromRow = 7 - (fromIndex / 8);
            int fromCol = fromIndex % 8;
            int toRow = 7 - (toIndex / 8);
            int toCol = toIndex % 8;

            int rowDiff = toRow - fromRow;
            int colDiff = toCol - fromCol;

            // Move must be diagonal
            if (Math.Abs(rowDiff) == Math.Abs(colDiff))
            {
                // Check if path is clear
                int rowStep = rowDiff > 0 ? 1 : -1;
                int colStep = colDiff > 0 ? 1 : -1;
                int steps = Math.Abs(rowDiff);

                // Exclude the target square from path checking
                for (int i = 1; i < steps; i++)
                {
                    int intermediateRow = fromRow + i * rowStep;
                    int intermediateCol = fromCol + i * colStep;
                    int intermediateIndex = (7 - intermediateRow) * 8 + intermediateCol;
                    if (gameState[intermediateIndex] != null)
                    {
                        // Path is blocked
                        return false;
                    }
                }

                // At this point, path is clear
                // We have already checked in IsValidMove that we are not capturing our own piece
                return true;
            }

            return false;
        }

        private bool IsValidRookMove(int fromIndex, int toIndex)
        {
            int fromRow = (fromIndex / 8);
            int fromCol = fromIndex % 8;
            int toRow = (toIndex / 8);
            int toCol = toIndex % 8;

            if (fromRow == toRow)
            {
                // Horizontal movement
                int colStep = toCol > fromCol ? 1 : -1;
                int steps = Math.Abs(toCol - fromCol);

                for (int i = 1; i < steps; i++)
                {
                    int intermediateCol = fromCol + i * colStep;
                    int intermediateIndex = (7 - fromRow) * 8 + intermediateCol;
                    if (gameState[intermediateIndex] != null)
                    {
                        // Path is blocked
                        return false;
                    }
                }

                return true;
            }
            else if (fromCol == toCol)
            {
                // Vertical movement
                int rowStep = toRow > fromRow ? 1 : -1;
                int steps = Math.Abs(toRow - fromRow);

                for (int i = 1; i < steps; i++)
                {
                    int intermediateRow = fromRow + i * rowStep;
                    int intermediateIndex = (7 - intermediateRow) * 8 + fromCol;
                    if (gameState[intermediateIndex] != null)
                    {
                        // Path is blocked
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private bool IsValidQueenMove(int fromIndex, int toIndex)
        {
            // Queen can move like a rook or a bishop
            return IsValidRookMove(fromIndex, toIndex) || IsValidBishopMove(fromIndex, toIndex);
        }

        private bool IsValidKingMove(int fromIndex, int toIndex)
        {
            int fromRow = fromIndex / 8;
            int fromCol = fromIndex % 8;
            int toRow = toIndex / 8;
            int toCol = toIndex % 8;

            int rowDiff = Math.Abs(toRow - fromRow);
            int colDiff = Math.Abs(toCol - fromCol);

            // King moves one square in any direction
            if (rowDiff <= 1 && colDiff <= 1)
            {
                return true;
            }

            // TODO: Implement castling (not covered here)

            return false;
        }
    }
}
