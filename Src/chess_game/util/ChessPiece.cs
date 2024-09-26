using System;
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
    }
}
