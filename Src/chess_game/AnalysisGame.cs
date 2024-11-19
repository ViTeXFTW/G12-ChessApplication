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
        public AnalysisGame(MainWindow main) : base(main, ChessColor.WHITE) { }

        public override void HandleClick(int index)
        {
            if (prevLegalMoves.Any(item => item.toIndex == index))
            {
                Move currentMove = prevLegalMoves.Find(item => item.toIndex == index);

                ApplyMove(currentMove);
                AddMoveToHistory(currentMove);
                // Switch to the next player
                UserPlayer.ChangePlayer();
                HandleChecks();
            }
        }

        public void AddMoveToHistory(Move move)
        {
            MoveRecord m = new MoveRecord();
            if (move.movingPiece.ChessColor == ChessColor.BLACK)
            {
                m = (MoveRecord)mainWindow.GameHistory.Items.GetItemAt(mainWindow.GameHistory.Items.Count - 1);
                mainWindow.GameHistory.Items.Remove(m);
                m.BlackMove = move.GetMoveString();
                mainWindow.GameHistory.Items.Add(m);

            }
            else
            {
                m.MoveNumber = mainWindow.GameHistory.Items.Count + 1;
                m.WhiteMove = move.GetMoveString();
                mainWindow.GameHistory.Items.Add(m);
            }
        }
    }
}
