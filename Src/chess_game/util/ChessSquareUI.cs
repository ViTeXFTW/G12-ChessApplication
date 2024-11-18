using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace G12_ChessApplication.Src.chess_game.util
{
    public class ChessSquareUI : Grid
    {
        private List<SolidColorBrush> Colors = new List<SolidColorBrush>();
        private SolidColorBrush DefaultColor { get; set; }
        public ChessSquareUI(SolidColorBrush color)
        {
            DefaultColor = color;
            Background = color;
        }

        public void SetNewColor(SolidColorBrush color)
        {
            if (color != MainWindow.DefaultLegalMoveColor)
            {
                Colors.Add(color);
            }
            Background = color;
        }

        public void SetPrevColor()
        {
            if (Colors.Count > 0)
            {
                Colors.Remove(Colors.Last());
                SetColor();
            }
        }

        public void RemoveLegalMoveColor()
        {
            if (Background == MainWindow.DefaultLegalMoveColor)
            {
                SetColor();
            }
        }

        public void RemoveCheckColor()
        {
            if (Background == MainWindow.DefaultCheckColor)
            {
                Colors.Remove(Colors.Last());
                SetColor();
            }
        }

        private void SetColor()
        {
            if (Colors.Count > 0)
            {
                Background = Colors.Last();
            }
            else
            {
                SetDefaultColor();
            }
        }

        public void SetDefaultColor()
        {
            Background = DefaultColor;
        }
    }
}
