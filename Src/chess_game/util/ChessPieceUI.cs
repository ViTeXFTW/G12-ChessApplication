using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Diagnostics;

namespace G12_ChessApplication.Src.chess_game.util
{
    public class ChessPieceUI : Border
    {
        public ChessPieceUI(string uri)
        {
            Image piece = new Image();
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(uri, UriKind.RelativeOrAbsolute);
            image.EndInit();
            piece.Source = image;
            piece.HorizontalAlignment = HorizontalAlignment.Center;
            piece.VerticalAlignment = VerticalAlignment.Center;
            Child = piece;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double size = Math.Min(availableSize.Width, availableSize.Height) * 0.9;
            Size finalSize = new Size(size, size);
            base.MeasureOverride(finalSize);
            return finalSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double size = Math.Min(finalSize.Width, finalSize.Height) * 0.9;
            Size squareSize = new Size(size, size);
            base.ArrangeOverride(squareSize);
            return squareSize;
        }
    }
}
