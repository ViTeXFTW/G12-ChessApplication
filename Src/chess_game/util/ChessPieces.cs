using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace G12_ChessApplication.Src.chess_game.util
{
    class Pawn : ChessPiece
    {
        private bool firstMove = true;
        public Pawn(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";

            BitmapImage image = new BitmapImage();
            image.BeginInit();

            image.UriSource = new Uri("pack://application:,,,/Images/" + colorLetter + "_pawn_2x_ns.png");

            image.EndInit();
            Source = image;
        }

        public override bool MoveValid(int fromIndex, int toIndex, ref ChessPiece[] gameState)
        {
            ChessPiece fromPiece = gameState[fromIndex];
            ChessPiece toPiece = gameState[toIndex];
            int fromRow = fromIndex / 8;
            int fromCol = fromIndex % 8;
            int toRow = toIndex / 8;
            int toCol = toIndex % 8;

            int rowDiff = toRow - fromRow;
            int colDiff = toCol - fromCol;

            int playerSign = -1;

            if (fromPiece.ChessColor != Game.userPlayer.Color)
            {
                playerSign = 1;
            }

            // Forward movement
            if (colDiff == 0)
            {
                Trace.WriteLine("Forward pawn movement");
                // Move forward one square
                if (rowDiff == (1 * playerSign) && toPiece == null)
                {
                    Trace.WriteLine("Forward 1 square");
                    firstMove = false;
                    return true;
                }
                // Move forward two squares from starting position
                if (rowDiff == (2 * playerSign) && firstMove && toPiece == null && gameState[(fromRow + (1 * playerSign)) * 8 + fromCol] == null)
                {
                    Trace.WriteLine("Forward 2 square");
                    firstMove = false;
                    return true;
                }
            }
            // Capture diagonally
            else if (Math.Abs(colDiff) == 1 && rowDiff == (1 * playerSign) && toPiece != null)
            {
                Trace.WriteLine("Diagonal attack");
                firstMove = false;
                return true;
            }

            return false;
        }
    }

    class Bishop : ChessPiece
    {
        public Bishop(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Images/" + colorLetter + "_bishop_2x_ns.png");
            image.EndInit();
            Source = image;
        }

        public override bool MoveValid(int fromIndex, int toIndex, ref ChessPiece[] gameState)
        {
            return DiaganolValid(fromIndex, toIndex, ref gameState);
        }
    }

    class Knight : ChessPiece
    {
        public Knight(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Images/" + colorLetter + "_knight_2x_ns.png");
            image.EndInit();
            Source = image;
        }

        public override bool MoveValid(int fromIndex, int toIndex, ref ChessPiece[] gameState)
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
    }

    class Rook : ChessPiece
    {
        public Rook(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Images/" + colorLetter + "_rook_2x_ns.png");
            image.EndInit();
            Source = image;
        }

        public override bool MoveValid(int fromIndex, int toIndex, ref ChessPiece[] gameState)
        {
            return HorizontalOrVerticalValid(fromIndex, toIndex, ref gameState);
        }
    }

    class Queen : ChessPiece
    {
        public Queen(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Images/" + colorLetter + "_queen_2x_ns.png");
            image.EndInit();
            Source = image;
        }

        public override bool MoveValid(int fromIndex, int toIndex, ref ChessPiece[] gameState)
        {
            // Queen can move like a rook or a bishop
            return HorizontalOrVerticalValid(fromIndex, toIndex, ref gameState) || DiaganolValid(fromIndex, toIndex, ref gameState);
        }
    }

    class King : ChessPiece
    {
        public King(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Images/" + colorLetter + "_king_2x_ns.png");
            image.EndInit();
            Source = image;
        }

        public override bool MoveValid(int fromIndex, int toIndex, ref ChessPiece[] gameState)
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
