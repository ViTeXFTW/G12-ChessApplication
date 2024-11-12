using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace G12_ChessApplication.Src.chess_game.util
{
    public abstract class ChessPiece : Image, ICloneable
    {
        public readonly ChessColor ChessColor;
        public bool hasMoved = false;
        public List<Tuple<int, int>> directions;
        public int distance = 100;

        public ChessPiece(ChessColor color)
        {
            ChessColor = color;
            Height = MainWindow.boardHeight / 8 * 0.9;
            Width = MainWindow.boardWidth / 8 * 0.9;
        }

        public ChessPiece(ChessPiece chessPiece)
        {
            this.ChessColor = chessPiece.ChessColor;
            this.Height = chessPiece.Height;
            this.Width = chessPiece.Width;
            this.directions = chessPiece.directions;
            this.distance = chessPiece.distance;
            this.hasMoved = chessPiece.hasMoved;
            this.Source = chessPiece.Source;
        }


        public abstract object Clone();

        public List<Tuple<int, int>> InvertDirections()
        {
            List<Tuple<int, int>> newDirections = new List<Tuple<int, int>>();
            this.directions.ForEach(x => newDirections.Add(Tuple.Create(x.Item1 * -1, x.Item2 * -1)));
            return newDirections;
        }

        public virtual List<Move> FindLegalMoves(int index, ref ChessPiece[] gameState, Move lastMove)
        {
            List<Move> result = new List<Move>();

            bool binding = CheckForBinding(index, ref gameState, out result);
            if (binding && this is Knight)
            {
                result.Clear();
            }
            else
            {
                result.Clear();
                result = GenerelFindMoves(index, gameState, result);
            }

            return result;
        }

        public List<Move> GenerelFindMoves(int index, ChessPiece[] gameState, List<Move> result)
        {
            int fromRow = index / 8;
            int fromCol = index % 8;

            foreach (var item in this.directions)
            {
                int row = fromRow + item.Item1;
                int col = fromCol + item.Item2;
                int disCounter = this.distance;

                while (row >= 0 && row <= 7 && col >= 0 && col <= 7 && disCounter > 0)
                {
                    int newIndex = row * 8 + col;
                    if (gameState[newIndex] != null)
                    {
                        if (this.ChessColor != gameState[newIndex].ChessColor)
                        {
                            result.Add(new Move(index, newIndex, true));
                        }
                        break;
                    }
                    result.Add(new Move(index, newIndex));

                    disCounter--;
                    row += item.Item1;
                    col += item.Item2;
                }
            }

            return result;
        }

        public bool CheckForBinding(int index, ref ChessPiece[] gameState, out List<Move> moves)
        {
            moves = new List<Move>();
            // Search for own king
            int kingIndex = GetOwnKingIndex(ref gameState);

            // Check for diagonal or horizontal or vetical connection to the king
            int kingRow = (kingIndex / 8);
            int kingCol = kingIndex % 8;
            int pieceRow = (index / 8);
            int pieceCol = index % 8;

            int row = pieceRow;
            int col = pieceCol;

            int rowDiff = kingRow - pieceRow;
            int colDiff = kingCol - pieceCol;

            if (Math.Abs(rowDiff) == Math.Abs(colDiff))
            {
                int rowStep = rowDiff > 0 ? 1 : -1;
                int colStep = colDiff > 0 ? 1 : -1;
                int steps = Math.Abs(rowDiff);
                bool check = IsPathToKingBlocked(index, gameState, pieceRow, pieceCol, rowStep, colStep, steps, ref moves);
                if (check)
                {
                    return false;
                }

                rowStep = rowDiff > 0 ? -1 : 1;
                colStep = colDiff > 0 ? -1 : 1;

                return LoopSteps(index, gameState, ref row, ref col, colStep, rowStep, true, ref moves);
            }
            if (pieceRow == kingRow || pieceCol == kingCol)
            {
                int colStep = 0;
                int rowStep = 0;
                int steps = 0;
                if (pieceRow == kingRow)
                {
                    colStep = kingCol > pieceCol ? 1 : -1;
                    steps = Math.Abs(colDiff);
                }
                else if (pieceCol == kingCol)
                {
                    rowStep = kingRow > pieceRow ? 1 : -1;
                    steps = Math.Abs(rowDiff);
                }

                bool check = IsPathToKingBlocked(index, gameState, pieceRow, pieceCol, rowStep, colStep, steps, ref moves);
                if (check)
                {
                    return false;
                }

                rowStep = rowDiff > 0 ? -1 : 1;
                colStep = colDiff > 0 ? -1 : 1;

                return LoopSteps(index, gameState, ref row, ref col, colStep, rowStep, false, ref moves);
            }
            return false;

            bool IsPathToKingBlocked(int startIndex, ChessPiece[] gameState, int pieceRow, int pieceCol, int rowStep, int colStep, int steps, ref List<Move> moves)
            {
                // Start at i = 1, so the piece doesnt check itself
                for (int i = 1; i < steps; i++)
                {
                    int intermediateRow = pieceRow + i * rowStep;
                    int intermediateCol = pieceCol + i * colStep;
                    int intermediateIndex = intermediateRow * 8 + intermediateCol;
                    if (gameState[intermediateIndex] != null)
                    {
                        return true;
                    }
                    else
                    {
                        moves.Add(new Move(startIndex, intermediateIndex));
                    }
                }

                return false;
            }

            bool LoopSteps(int startIndex, ChessPiece[] gameState, ref int row, ref int col, int colStep, int rowStep, bool dia, ref List<Move> moves)
            {
                col += colStep;
                row += rowStep;
                while (row >= 0 && row <= 7 && col >= 0 && col <= 7)
                {
                    int newIndex = row * 8 + col;
                    ChessPiece interceptingPiece = gameState[newIndex];

                    if (interceptingPiece != null)
                    {
                        if (interceptingPiece.ChessColor == ChessColor)
                        {
                            return false;
                        }

                        bool retVal = false;
                        // When checking for a diagonal binding, only the Queen or bishop can bind.
                        // For a horizontal or vertical binding, only a queen or a rook can bind.
                        // Dia stands for diagonal binding.
                        if (interceptingPiece is Queen || (dia && interceptingPiece is Bishop) || (!dia && interceptingPiece is Rook))
                        {
                            retVal = true;
                        }

                        moves.Add(new Move(startIndex, newIndex, true));
                        return retVal;
                    }
                    else
                    {
                        moves.Add(new Move(startIndex, newIndex));
                    }

                    col += colStep;
                    row += rowStep;
                }
                return false;
            }
        }

        private int GetOwnKingIndex(ref ChessPiece[] gameState)
        {
            int kingIndex = -1;
            for (int i = 0; i < gameState.Length; i++)
            {
                if (gameState[i] is King king)
                {
                    if (king.ChessColor == ChessColor)
                    {
                        kingIndex = i;
                        break;
                    }
                }
            }

            return kingIndex;
        }
    }
}
