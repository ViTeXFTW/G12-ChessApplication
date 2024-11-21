using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public int directionCo { get; set; } = -1;

        public Player(string name, ChessColor color)
        {
            Name = name;
            Color = color;
        }

        public void ChangePlayer()
        {
            directionCo *= -1;
            Color = Color == ChessColor.WHITE ? ChessColor.BLACK : ChessColor.WHITE;
            Trace.WriteLine($"Changed player to {Color}");
        }

        public void SetColor(ChessColor color)
        {
            this.Color = color;
        }
    }
}
