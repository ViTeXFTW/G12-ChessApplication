using G12_ChessApplication.Src.chess_game.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Printing;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


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

        public int? selectedSquareIndex = null;
        public static bool oneCheck { get; set; } = false;
        public static bool twoCheck { get; set; } = false;
        public bool checkMate { get; set; } = false;
        public bool staleMate { get; set; } = false;
        public static List<Move> checkIndexes { get; set; } = new List<Move>();

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

        public virtual void SquareClicked(int index)
        {
            if (IsPieceSelected)
            {
                HandleClick(index);

                DeselectPiece();
                selectedSquareIndex = null;
                mainWindow.ResetLegalMoveColor(SelectedPieceIndex);
                mainWindow.RemoveLegalMoves(prevLegalMoves);
            }
            else
            {
                if (CanSelectPieceAt(index) && !staleMate && !checkMate)
                {
                    List<Move> legalMoves;
                    Move lastMove = OpponentMoves.Count > 0 ? OpponentMoves.Last() : null;
                    legalMoves = gameState[index].FindLegalMoves(index, ref gameState, lastMove);


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
                    mainWindow.HighlightSquare(index, Brushes.Gray);

                    // Keep track of the selected square index
                    selectedSquareIndex = index;
                }
            }
        }

        public abstract void HandleClick(int index);

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

            if (move is CastlingMove castlingMove)
            {
                castlingMove.rookMove.movingPiece = (ChessPiece)gameState[castlingMove.rookMove.fromIndex].Clone();
                gameState[castlingMove.rookMove.fromIndex].hasMoved = true;
                gameState[castlingMove.rookMove.toIndex] = gameState[castlingMove.rookMove.fromIndex];
                gameState[castlingMove.rookMove.fromIndex] = null;
            }

            if (gameState[move.fromIndex] is Rook or King)
            {
                gameState[move.fromIndex].hasMoved = true;
            }

            if (move is EnPassantMove enPassantMove)
            {
                enPassantMove.enPassantPiece = (ChessPiece)gameState[enPassantMove.capturedIndex].Clone();
                gameState[enPassantMove.capturedIndex] = null;

            }

            if (move.isACapture || gameState[move.toIndex] != null)
            {
                move.capturedPiece = (ChessPiece)gameState[move.toIndex].Clone();
            }

            if (PlayerColor == gameState[move.fromIndex].ChessColor)
            {
                if (OpponentMoves.Count > 0)
                {
                    mainWindow.UnHighLightMove(OpponentMoves.Last());
                }
                AddPlayerMove(move);
            }
            else
            {
                if (PlayerMoves.Count > 0)
                {
                    mainWindow.UnHighLightMove(PlayerMoves.Last());
                }
                AddOpponentMove(move);
            }
            gameState[move.toIndex] = gameState[move.fromIndex];
            gameState[move.fromIndex] = null;
            HandleChecks();
            mainWindow.UpdateUIAfterMove();
            mainWindow.HighLightMove(move);

        }
        public void ApplyReverseMove(Move move)
        {
            mainWindow.UnHighLightMove(move);
            if (PlayerColor == move.movingPiece.ChessColor)
            {
                PlayerMoves.Remove(move);
                if (OpponentMoves.Count > 0)
                {
                    mainWindow.HighLightMove(OpponentMoves.Last());
                }
            }
            else
            {
                OpponentMoves.Remove(move);
                if (PlayerMoves.Count > 0)
                {
                    mainWindow.HighLightMove(PlayerMoves.Last());
                }
            }

            if (move is CastlingMove castlingMove)
            {
                gameState[castlingMove.rookMove.toIndex] = castlingMove.rookMove.movingPiece;
                gameState[castlingMove.rookMove.fromIndex] = null;
            }

            if (move is EnPassantMove enPassantMove)
            {
                gameState[enPassantMove.capturedIndex] = enPassantMove.enPassantPiece;
            }
            gameState[move.fromIndex] = move.capturedPiece;
            gameState[move.toIndex] = move.movingPiece;
            HandleChecks();
            mainWindow.UpdateUIAfterMove();
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
                mainWindow.UpdateUIAfterMove();
                UserPlayer.ChangePlayer();
                HandleChecks();
            }
        }

        public void CancelMove()
        {

        }

        public static List<List<Move>> IsKingInCheck(ref ChessPiece[] gameState, int indexToCheck = -1)
        {
            List<List<Move>> checkIndexes = new List<List<Move>>();
            int kingIndex = GetOwnKingIndex(ref gameState);
            ChessPiece kingPiece = gameState[kingIndex];
            
            kingIndex = indexToCheck == -1 ? kingIndex : indexToCheck;

            int kingRow = kingIndex / 8;
            int kingCol = kingIndex % 8;
            for (int i = 0; i < gameState.Length; i++)
            {
                if (gameState[i] == null || gameState[i].ChessColor == kingPiece.ChessColor)
                {
                    continue;
                }

                List<Move> moves;
                if (gameState[i].CanTakePieceAt(i, kingIndex, ref gameState, out moves))
                {
                    moves.Add(new Move(i, i, true));
                    checkIndexes.Add(moves);
                }
            }


            return checkIndexes;

        }

        public static int GetOwnKingIndex(ref ChessPiece[] gameState)
        {
            int kingIndex = -1;
            for (int i = 0; i < gameState.Length; i++)
            {
                if (gameState[i] is King king)
                {
                    if (king.ChessColor == Game.UserPlayer.Color)
                    {
                        kingIndex = i;
                        break;
                    }
                }
            }

            return kingIndex;
        }

        public void HandleChecks()
        {
            List<List<Move>> checks = IsKingInCheck(ref gameState);
            Trace.WriteLine("Amount of checks: " + checks.Count);
            checkIndexes.Clear();
            oneCheck = checks.Count == 1;
            twoCheck = checks.Count == 2;
            checkMate = false;
            staleMate = false;

            if (oneCheck)
            {
                checkIndexes = checks.First();
            }
            bool cantMove = true;
            for (int i = 0; i < gameState.Length; i++)
            {
                if (gameState[i] != null && gameState[i].ChessColor == Game.UserPlayer.Color)
                {
                    List<Move> legalMoves;
                    if (OpponentMoves.Count > 0)
                    {
                        legalMoves = gameState[i].FindLegalMoves(i, ref gameState, OpponentMoves.Last());
                    }
                    else
                    {
                        legalMoves = gameState[i].FindLegalMoves(i, ref gameState, null);
                    }
                    if (legalMoves.Count > 0)
                    {
                        cantMove = false;
                    }
                }
            }

            if (oneCheck || twoCheck)
            {
                mainWindow.HighlightSquare(GetOwnKingIndex(ref gameState), MainWindow.DefaultCheckColor);
                if (cantMove)
                {
                    // CHECKMATE
                    checkMate = true;
                    ChessColor chessColor = Game.UserPlayer.Color == ChessColor.WHITE ? ChessColor.BLACK : ChessColor.WHITE;
                    Trace.WriteLine("Player " + chessColor + " has won!!!!");
                    MessageBox.Show("Player " + chessColor + " has won!!!!");
                }
                else
                {
                    staleMate = true;
                    Trace.WriteLine("ITS A STALEMATE DUMBASS");
                    MessageBox.Show("ITS A STALEMATE DUMBASS");
                }
            }
        }
    }
}
