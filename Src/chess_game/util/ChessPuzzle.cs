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
        public List<Tuple<int, int>> playerMoves { get; set; } = new List<Tuple<int, int>>();
        public List<Tuple<int, int>> enemyMoves { get; set; } = new List<Tuple<int, int>>();

        public ChessPuzzle(ChessPuzzle puzzle)
        {
            board = puzzle.board;
            enemyMoves = new List<Tuple<int, int>>(puzzle.enemyMoves);
            playerMoves = new List<Tuple<int, int>>(puzzle.playerMoves);
        }

        public ChessPuzzle () { }
    }
}
