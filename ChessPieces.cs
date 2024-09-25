using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Chess
{
    public enum ChessColor
    {
        WHITE = 0,
        BLACK = 1,
    }
    abstract class ChessPiece : Image
    {
        public readonly ChessColor ChessColor;
        public ChessPiece(ChessColor color) 
        {
            ChessColor = color;
        }
    }

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
        }
    }
}
