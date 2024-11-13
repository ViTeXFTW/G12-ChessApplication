using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace G12_ChessApplication.Src.chess_game.util
{
    public class ChessPieceUI : Image
    {
        public ChessPieceUI(string uri)
        {

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(uri);
            image.EndInit();
            Source = image;
            Height = MainWindow.boardHeight / 8 * 0.9;
            Width = MainWindow.boardWidth / 8 * 0.9;
        }
    }
}
