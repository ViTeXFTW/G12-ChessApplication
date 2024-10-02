using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace G12_ChessApplication.Src.chess_game.util
{
    public abstract class ChessPiece : Image
    {
        public readonly ChessColor ChessColor;

        public ChessPiece(ChessColor color)
        {
            ChessColor = color;
        }

        public abstract bool MoveValid(int fromIndex, int toIndex, ref ChessPiece[] gameState);

        public bool DiaganolValid(int fromIndex, int toIndex, ref ChessPiece[] gameState)
        {
            int fromRow = fromIndex / 8;
            int fromCol = fromIndex % 8;
            int toRow = toIndex / 8;
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
                    int intermediateIndex = intermediateRow * 8 + intermediateCol;
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
        public bool HorizontalOrVerticalValid(int fromIndex, int toIndex, ref ChessPiece[] gameState)
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
                    int intermediateIndex = fromRow * 8 + intermediateCol;
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
                    int intermediateIndex = intermediateRow * 8 + fromCol;
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
    }
}
