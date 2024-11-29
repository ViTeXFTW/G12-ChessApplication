using G12_ChessApplication.Src.chess_game.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace G12_ChessApplication.Src.chess_game
{
    class AnalysisGame : Game
    {
        public AnalysisGame(MainWindow main) : base(main, ChessColor.WHITE) 
        {
            UserPlayer = new Player("User", ChessColor.WHITE);
        }

        public override void HandleClick(int index)
        {
            if (prevLegalMoves.Any(item => item.toIndex == index))
            {
                Move currentMove = prevLegalMoves.Find(item => item.toIndex == index);

                ApplyMove(currentMove);
                // Switch to the next player
                UserPlayer.ChangePlayer();
                HandleChecks();
            }
        }
    }
}
