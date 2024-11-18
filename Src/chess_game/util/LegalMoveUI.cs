using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace G12_ChessApplication.Src.chess_game.util
{
    public class LegalMoveUI : Border
    {
        public LegalMoveUI(SolidColorBrush color = null)
        {
            if (color == null)
            {
                color = MainWindow.DefaultLegalMoveColor;
            }
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = color;
            Child = ellipse;
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            double size = Math.Min(availableSize.Width, availableSize.Height) / MainWindow.DefaultLegalMoveRatio;
            Size finalSize = new Size(size, size);
            base.MeasureOverride(finalSize);
            return finalSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double size = Math.Min(finalSize.Width, finalSize.Height) / MainWindow.DefaultLegalMoveRatio;
            Size squareSize = new Size(size, size);
            base.ArrangeOverride(squareSize);
            return squareSize;
        }
    }
}
