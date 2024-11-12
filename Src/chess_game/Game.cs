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
        public static Player UserPlayer { get; set; }
        public static ChessColor PlayerColor { get; set; }
        public bool IsPieceSelected { get; set; }
        public int SelectedPieceIndex { get; set; }

        public ChessPiece[] gameState = new ChessPiece[64];
        public MainWindow mainWindow { get; set; }

        public bool turnToMove { get; set; } = true;
        public Game(MainWindow main, ChessColor chessColor)
        {
            mainWindow = main;
            PlayerColor = chessColor;
            UserPlayer = new Player("User", chessColor);
            gameState = FenParser.CreatePieceArray();
        }

        public abstract void SquareClicked(int index);

        public void SetGameState(string board)
        {
            gameState = FenParser.CreatePieceArray(board);
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

            if (!turnToMove || pieceColor != UserPlayer.Color)
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

        public void ApplyMove(Move move)
        {
            move.movingPiece = (ChessPiece)gameState[move.fromIndex].Clone();
            if (gameState[move.fromIndex] is Pawn p)
            {
                if (p.distance == 2)
                {
                    p.distance = 1;
                }
            }

            if (move is EnPassantMove enPassantMove)
            {
                enPassantMove.enPassantPiece = (ChessPiece)gameState[enPassantMove.capturedIndex].Clone();
                gameState[enPassantMove.capturedIndex] = null;

            }

            if (move.isACapture)
            {
                move.capturedPiece = (ChessPiece)gameState[move.toIndex].Clone();
            }

            if (PlayerColor == gameState[move.fromIndex].ChessColor)
            {
                AddPlayerMove(move);
            }
            else
            {
                AddOpponentMove(move);
            }
            gameState[move.toIndex] = gameState[move.fromIndex];
            gameState[move.fromIndex] = null;
            DeselectPiece();
        }
        public void ApplyReverseMove(Move move)
        {
            if (PlayerColor == move.movingPiece.ChessColor)
            {
                PlayerMoves.Remove(move);
            }
            else
            {
                OpponentMoves.Remove(move);
            }

            if (move is EnPassantMove enPassantMove)
            {
                gameState[enPassantMove.capturedIndex] = enPassantMove.enPassantPiece;
            }
            gameState[move.fromIndex] = move.capturedPiece;
            gameState[move.toIndex] = move.movingPiece;
        }

        public void AddOpponentMove(Move move)
        {
            OpponentMoves.Add(move);
        }

        public void AddPlayerMove(Move move)
        {
            PlayerMoves.Add(move);
        }

        public void UndoMove()
        {
            if (PlayerMoves.Count <= 0)
            {
                Trace.WriteLine("Nothing to undo");
                return;
            }
            Move moveToUndo = null;
            if (UserPlayer.Color != PlayerColor)
            {
                moveToUndo = PlayerMoves.Last();
            }
            else
            {
                moveToUndo = OpponentMoves.Last();
            }

            if (moveToUndo != null)
            {
                moveToUndo.ReverseMove();
                ApplyReverseMove(moveToUndo);
                mainWindow.UpdateUIAfterMove(moveToUndo, true);
                UserPlayer.ChangePlayer();
            }
        }

        public void CancelMove()
        {

        }
    }
}
