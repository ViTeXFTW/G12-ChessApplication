using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G12_ChessApplication.Src.chess_game.util
{
    internal class ChessPuzzle
    {
        public string board { get; set; }
        public ChessColor color { get; set; }
        public List<Move> playerMoves { get; set; } = new List<Move>();
        public List<Move> enemyMoves { get; set; } = new List<Move>();


        public ChessPuzzle() { }
        public ChessPuzzle(ChessPuzzle puzzle)
        {
            board = puzzle.board;
            color = puzzle.color;
            enemyMoves = new List<Move>(puzzle.enemyMoves);
            playerMoves = new List<Move>(puzzle.playerMoves);
        }
    }
}
