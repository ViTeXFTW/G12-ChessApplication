using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G12_ChessApplication.Src.chess_game.util;

namespace G12_ChessApplication.Src.chess_game.util
{
    public class Player
    {
        public string Name { get; set; }
        public ChessColor Color { get; set; }

        public Player(string name, ChessColor color)
        {
            Name = name;
            Color = color;
        }
    }
}
