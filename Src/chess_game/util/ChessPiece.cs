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

        private List<int> _legalMoves = new List<int>();

        public List<int> legalMoves
        {
            get
            { return _legalMoves; }
            protected set
            {
                _legalMoves.Add(Convert.ToInt32(value));
            }
        }


        public ChessPiece(ChessColor color)
        {
            ChessColor = color;
        }
    }
}
