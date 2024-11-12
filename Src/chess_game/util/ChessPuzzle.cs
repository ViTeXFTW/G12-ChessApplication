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
        public List<Move> playerMoves { get; set; } = new List<Move>();
        public List<Move> enemyMoves { get; set; } = new List<Move>();

        public ChessPuzzle(ChessPuzzle puzzle)
        {
            board = puzzle.board;
            enemyMoves = new List<Move>(puzzle.enemyMoves);
            playerMoves = new List<Move>(puzzle.playerMoves);
        }

        public ChessPuzzle () { }
    }
}
