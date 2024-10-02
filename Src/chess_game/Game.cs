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
    public class Game
    {
        public Player whitePlayer {  get; set; }
        public Player blackPlayer { get; set; }
        public Player currentPlayer { get; set; }
        public static Player userPlayer { get; set; }
        public bool IsPieceSelected { get; set; }
        public int SelectedPieceIndex { get; set; }

        private ChessPiece[] gameState = new ChessPiece[64];

        public Game(string whitePlayerName, string blackPlayerName)
        {
            whitePlayer = new Player(whitePlayerName, ChessColor.WHITE);
            blackPlayer = new Player(blackPlayerName, ChessColor.BLACK);
            currentPlayer = whitePlayer;
            userPlayer = whitePlayer;
            gameState = FenParser.CreatePieceArray();
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

            if (pieceColor == ChessColor.BLACK && currentPlayer == blackPlayer)
            {
                return true;
            }
            if (pieceColor == ChessColor.WHITE && currentPlayer == whitePlayer)
            {
                return true;
            }
            return false;
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
