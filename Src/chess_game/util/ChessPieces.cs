using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace G12_ChessApplication.Src.chess_game.util
{
    class Pawn : ChessPiece
    {
        public Pawn(ChessColor color) : base(color)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            if (ChessColor == ChessColor.WHITE)
            {
                image.UriSource = new Uri("pack://application:,,,/Images/w_pawn_2x_ns.png");
            }
            else
            {
                image.UriSource = new Uri("pack://application:,,,/Images/b_pawn_2x_ns.png");
            }
            image.EndInit();
            Source = image;

            legalMoves = new List<int> { 7, 8, 9 };
        }
    }

    class Bishop : ChessPiece
    {
        public Bishop(ChessColor color) : base(color)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            if (ChessColor == ChessColor.WHITE)
            {
                image.UriSource = new Uri("pack://application:,,,/Images/w_bishop_2x_ns.png");
            }
            else
            {
                image.UriSource = new Uri("pack://application:,,,/Images/b_bishop_2x_ns.png");
            }
            image.EndInit();
            Source = image;

            legalMoves = new List<int> {-7, 7, -9, 9 };
        }
    }

    class Knight : ChessPiece
    {
        public Knight(ChessColor color) : base(color)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            if (ChessColor == ChessColor.WHITE)
            {
                image.UriSource = new Uri("pack://application:,,,/Images/w_knight_2x_ns.png");
            }
            else
            {
                image.UriSource = new Uri("pack://application:,,,/Images/b_knight_2x_ns.png");
            }
            image.EndInit();
            Source = image;

            legalMoves = new List<int> {-6, 6, -10, 10, -15, 15, -17, 17 };
        }
    }

    class Rook : ChessPiece
    {
        public Rook(ChessColor color) : base(color)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            if (ChessColor == ChessColor.WHITE)
            {
                image.UriSource = new Uri("pack://application:,,,/Images/w_rook_2x_ns.png");
            }
            else
            {
                image.UriSource = new Uri("pack://application:,,,/Images/b_rook_2x_ns.png");
            }
            image.EndInit();
            Source = image;

            legalMoves = new List<int> { -1, 1, -8, 8 };
        }
    }

    class Queen : ChessPiece
    {
        public Queen(ChessColor color) : base(color)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            if (ChessColor == ChessColor.WHITE)
            {
                image.UriSource = new Uri("pack://application:,,,/Images/w_queen_2x_ns.png");
            }
            else
            {
                image.UriSource = new Uri("pack://application:,,,/Images/b_queen_2x_ns.png");
            }
            image.EndInit();
            Source = image;

            legalMoves = new List<int> { -7, 7, -9, 9, -1, 1, -8, 8 };
        }
    }

    class King : ChessPiece
    {
        public King(ChessColor color) : base(color)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            if (ChessColor == ChessColor.WHITE)
            {
                image.UriSource = new Uri("pack://application:,,,/Images/w_king_2x_ns.png");
            }
            else
            {
                image.UriSource = new Uri("pack://application:,,,/Images/b_king_2x_ns.png");
            }
            image.EndInit();
            Source = image;

            legalMoves = new List<int> { -7, 7, -9, 9, -1, 1, -8, 8 };
        }
    }
}
