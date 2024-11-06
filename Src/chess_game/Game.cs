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
        public List<Move> prevLegalMoves = new List<Move>();
        public List<Move> OpponentMoves = new List<Move>();
        public List<Move> PlayerMoves = new List<Move>();
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

        public void ApplyMove(Move move, bool player)
        {
            if (gameState[move.fromIndex] is Pawn p)
            {
                if (p.distance == 2)
                {
                    p.distance = 1;
                }
            }
            if (player)
            {
                AddPlayerMove(move);
            }
            else
            {
                AddOpponentMove(move);
            }
            if (move is EnPassantMove enPassantMove)
            {
                gameState[enPassantMove.capturedIndex] = null;
            }
            gameState[move.toIndex] = gameState[move.fromIndex];
            gameState[move.fromIndex] = null;
            DeselectPiece();
        }
        
        public void AddOpponentMove(Move move)
        {
            OpponentMoves.Add(move);
        }

        public void AddPlayerMove(Move move)
        {
            PlayerMoves.Add(move);
        }
    }
}
