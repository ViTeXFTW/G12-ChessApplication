using G12_ChessApplication.Src.chess_game.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace G12_ChessApplication.Src.chess_game
{
    public abstract class Game
    {
        public static Player userPlayer { get; set; }
        public bool IsPieceSelected { get; set; }
        public int SelectedPieceIndex { get; set; }

        public ChessPiece[] gameState = new ChessPiece[64];
        public MainWindow mainWindow { get; set; }
        public int colorFacing { get; set; }

        public bool turnToMove { get; set; } = true;
        public Game(MainWindow main, ChessColor chessColor)
        {
            mainWindow = main;
            userPlayer = new Player("User", chessColor);
            colorFacing = (chessColor == ChessColor.WHITE) ? 1 : -1;
            gameState = FenParser.CreatePieceArray(colorFacing);
        }

        public abstract void SquareClicked(int index);

        public void SetGameState(string board)
        {
            gameState = FenParser.CreatePieceArray(colorFacing, board);
        }

        public bool CanSelectPieceAt(int index)
        {
            var piece = gameState[index];

            if (piece == null)
            {
                Trace.WriteLine($"Selected empty square");
                return false;
            }

            var pieceColor = piece.ChessColor;

            Trace.WriteLine($"Selected Piece: {piece}, {pieceColor}");

            if (!turnToMove || pieceColor != userPlayer.Color)
            {
                return false;
            }

            return true;
        }

        public void SelectPieceAt(int index)
        {
            IsPieceSelected = true;
            SelectedPieceIndex = index;
        }

        public void DeselectPiece()
        {
            IsPieceSelected = false;
        }


        public bool IsValidMove(int fromIndex, int toIndex)
        {
            ChessPiece fromPiece = gameState[fromIndex];
            ChessPiece toPiece = gameState[toIndex];

            if (fromPiece == null)
            {
                return false;
            }

            if (toPiece != null && fromPiece.ChessColor == toPiece.ChessColor)
            {
                Trace.WriteLine($"Cannot capture own piece at {toIndex}");
                return false;
            }

            return fromPiece.MoveValid(fromIndex, toIndex, ref gameState);

        }

        public void ApplyMove(int fromIndex, int toIndex)
        {
            gameState[toIndex] = gameState[fromIndex];
            gameState[fromIndex] = null;
            DeselectPiece();
        }

    }
}
