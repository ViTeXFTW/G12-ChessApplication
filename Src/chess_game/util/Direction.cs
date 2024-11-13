using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G12_ChessApplication.Src.chess_game.util
{
    public class Direction
    {
        public int row;
        public int col;
        public Direction() { }
        public Direction(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
        public Direction(Direction other)
        {
            this.row = other.row;
            this.col = other.col;
        }

    }
}
