using G12_ChessApplication.Src.chess_game.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace G12_ChessApplication.Src.chess_game
{
    class AnalysisGame : Game
    {


        private int? selectedSquareIndex = null;
        public AnalysisGame(MainWindow main) : base(main, ChessColor.WHITE)
        {

        }
        public override void SquareClicked(int index)
        {
            if (IsPieceSelected)
            {
                if (prevLegalMoves.Any(item => item.toIndex == index))
                {


                    Move currentMove = prevLegalMoves.Find(item => item.toIndex == index);

                    ApplyMove(currentMove);
                    mainWindow.UpdateUIAfterMove(currentMove, false);

                    // Reset the color of the previously selected square
                    mainWindow.ResetSquareColor(SelectedPieceIndex);
                    selectedSquareIndex = null;

                    // Switch to the next player
                    UserPlayer.ChangePlayer();

                }
                else
                {

                    // Invalid move, deselect the piece
                    DeselectPiece();

                    // Optionally update UI to reflect deselection

                    mainWindow.ResetSquareColor(SelectedPieceIndex);
                    selectedSquareIndex = null;
                }

                mainWindow.RemoveLegalMoves(prevLegalMoves);
            }
            else
            {
                if (CanSelectPieceAt(index))
                {
                    List<Move> legalMoves;

                    if (OpponentMoves.Count > 0)
                    {
                        legalMoves = gameState[index].FindLegalMoves(index, ref gameState, OpponentMoves.Last());                   
                    }
                    else
                    {
                        legalMoves = gameState[index].FindLegalMoves(index, ref gameState, null);
                    }
                    mainWindow.ShowLegalMoves(legalMoves);
                    prevLegalMoves = legalMoves;
                    // Deselect the previous selection if any
                    if (selectedSquareIndex != null)
                    {
                        mainWindow.ResetSquareColor(selectedSquareIndex.Value);
                    }

                    // Select the piece
                    SelectPieceAt(index);

                    // Highlight the selected square
                    mainWindow.HighlightSquare(index);

                    // Keep track of the selected square index
                    selectedSquareIndex = index;
                }
            }
        }
    }
}
