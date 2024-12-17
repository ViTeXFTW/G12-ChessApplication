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
using System.Text.Json.Serialization;
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
        public ChessColor CurrentPlayerColor { get; set; } = ChessColor.WHITE;

        public bool IsPieceSelected { get; set; }
        public int SelectedPieceIndex { get; set; }

        public int? selectedSquareIndex = null;
        public static bool oneCheck { get; set; } = false;
        public static bool twoCheck { get; set; } = false;
        public bool isCheckIndex { get; set; } = false;
        public int oldKingIndex { get; set; } = 0;
        public bool Online { get; set; } = false;
        public static List<Move> checkIndexes { get; set; } = new List<Move>();

        public ChessPiece[] gameState = new ChessPiece[64];
        public bool gameRunning { get; set; } = true;

        public MainWindow mainWindow { get; set; }

        public bool turnToMove { get; set; } = true;
        public Game(MainWindow main, ChessColor chessColor)
        {
            mainWindow = main;
            PlayerColor = chessColor;
            FenParser.CreatePieceArray(out gameState);
            MainWindow.mainBoard.SetBoardSetup();
            oneCheck = false;
            twoCheck = false;
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
                if (CanSelectPieceAt(index) && gameRunning)
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

            List<string> strings = new List<string>();
            try
            {
                strings = FenParser.CreatePieceArray(out gameState, board);
            }
            catch (ArgumentException a)
            {
                Trace.WriteLine(a.Message + " board: " + board);
                return;
            }

            MainWindow.mainBoard.SetBoardSetup(board);
            mainWindow.ClearHighLights();
            prevLegalMoves = new List<Move>();
            OpponentMoves = new List<Move>();
            PlayerMoves = new List<Move>();
            oneCheck = false;
            twoCheck = false;
            isCheckIndex = false;
            gameRunning = true;

            CurrentPlayerColor = strings.First() == "w" ? ChessColor.WHITE : ChessColor.BLACK;
            if (UserPlayer.Color != CurrentPlayerColor && !Online)
            {
                UserPlayer.ChangePlayer();
            }

        }

        public bool CanSelectPieceAt(int index)
        {
            var piece = gameState[index];

            if (piece == null)
            {
                Trace.WriteLine($"Selected empty square");
                return false;
            }

            var pieceColor = piece.chessColor;

            Trace.WriteLine($"Selected Piece: {piece}, {pieceColor}");

            if (pieceColor != CurrentPlayerColor || pieceColor != UserPlayer.Color)
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

        public async Task ApplyMove(Move move)
        {
            move.movingPiece = new JsonPiece(FenParser.GetCharFromPiece(gameState[move.fromIndex]), gameState[move.fromIndex].hasMoved, gameState[move.fromIndex].chessColor);
            if (gameState[move.fromIndex] is Pawn p)
            {
                if (move is PromotionMove pm)
                {
                    if (UserPlayer.Color == CurrentPlayerColor)
                    {
                        ChessPiece promotePiece = await mainWindow.PromotionPopupFunc(move);
                        gameState[move.fromIndex] = promotePiece;
                        pm.piecePromotedTo = new JsonPiece(FenParser.GetCharFromPiece(promotePiece), promotePiece.hasMoved, promotePiece.chessColor);
                    }
                    else
                    {
                        gameState[move.fromIndex] = pm.piecePromotedTo.GetChessPiece();
                    }
                }
            }

            if (move is CastlingMove castlingMove)
            {
                ChessPiece rook = gameState[castlingMove.rookMove.fromIndex];
                castlingMove.rookMove.movingPiece = new JsonPiece(FenParser.GetCharFromPiece(rook), rook.hasMoved, rook.chessColor);
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
                ChessPiece enPassantPiece = gameState[enPassantMove.capturedIndex];
                enPassantMove.enPassantPiece = new JsonPiece(FenParser.GetCharFromPiece(enPassantPiece), enPassantPiece.hasMoved, enPassantPiece.chessColor);
                gameState[enPassantMove.capturedIndex] = null;
            }

            if (move.isACapture || gameState[move.toIndex] != null)
            {
                ChessPiece capturedPiece = gameState[move.toIndex];
                move.capturedPiece = new JsonPiece(FenParser.GetCharFromPiece(capturedPiece), capturedPiece.hasMoved, capturedPiece.chessColor);
            }

            AddMoveToHistory(move);

            if (Game.PlayerColor == gameState[move.fromIndex].chessColor)
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
            mainWindow.UpdateUIAfterMove();
            mainWindow.HighLightMove(move);
            HandleChecks();

        }
        public void ApplyReverseMove(Move move)
        {
            mainWindow.UnHighLightMove(move);
            RemoveMoveFromHistory(move);
            if (PlayerColor == move.movingPiece.chessColor)
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
                gameState[castlingMove.rookMove.toIndex] = castlingMove.rookMove.movingPiece.GetChessPiece();
                gameState[castlingMove.rookMove.fromIndex] = null;
            }

            if (move is EnPassantMove enPassantMove)
            {
                gameState[enPassantMove.capturedIndex] = enPassantMove.enPassantPiece.GetChessPiece();
            }
            gameState[move.fromIndex] = move.capturedPiece != null ? move.capturedPiece.GetChessPiece() : null;
            gameState[move.toIndex] = move.movingPiece.GetChessPiece();
            mainWindow.UpdateUIAfterMove();
            HandleChecks();
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
                CurrentPlayerColor = UserPlayer.Color;
                HandleChecks();
            }
        }

        public void CancelMove()
        {

        }

        public static List<List<Move>> IsKingInCheck(ref ChessPiece[] gameState, ChessColor colorToCheck, int indexToCheck = -1)
        {
            List<List<Move>> checkIndexes = new List<List<Move>>();
            int kingIndex = GetKingIndex(ref gameState, colorToCheck);
            ChessPiece kingPiece = gameState[kingIndex];

            kingIndex = indexToCheck == -1 ? kingIndex : indexToCheck;

            int kingRow = kingIndex / 8;
            int kingCol = kingIndex % 8;
            for (int i = 0; i < gameState.Length; i++)
            {
                if (gameState[i] == null || gameState[i].chessColor == kingPiece.chessColor)
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

        public static int GetKingIndex(ref ChessPiece[] gameState, ChessColor kingColor)
        {
            int kingIndex = -1;
            for (int i = 0; i < gameState.Length; i++)
            {
                if (gameState[i] is King king)
                {
                    if (king.chessColor == kingColor)
                    {
                        kingIndex = i;
                        break;
                    }
                }
            }

            return kingIndex;
        }

        public async void HandleChecks()
        {
            List<List<Move>> checks = IsKingInCheck(ref gameState, UserPlayer.Color);
            Trace.WriteLine("Amount of checks: " + checks.Count);
            checkIndexes.Clear();
            oneCheck = checks.Count == 1;
            twoCheck = checks.Count == 2;

            if (oneCheck)
            {
                checkIndexes = checks.First();
            }
            bool cantMove = true;
            for (int i = 0; i < gameState.Length; i++)
            {
                if (gameState[i] != null && gameState[i].chessColor == Game.UserPlayer.Color)
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
                oldKingIndex = GetKingIndex(ref gameState, Game.UserPlayer.Color);
                if (Online)
                {
                    isCheckIndex = true;
                    SendMsg("Check:" + oldKingIndex);
                }
                mainWindow.HighlightSquare(oldKingIndex, MainWindow.DefaultCheckColor);
                if (cantMove)
                {
                    // CHECKMATE
                    gameRunning = false;
                    ChessColor chessColor = Game.UserPlayer.Color == ChessColor.WHITE ? ChessColor.BLACK : ChessColor.WHITE;
                    if (Online)
                    {
                        SendMsg("CheckMate:" + chessColor);
                        await HandleGameEnd(2);
                    }
                    Trace.WriteLine("Player " + chessColor + " has won!!!!");
                    MessageBox.Show("Player " + chessColor + " has won you lose !!!!");
                }
            }
            else if (cantMove)
            {
                gameRunning = false;
                if (Online)
                {
                    SendMsg("StaleMate");
                    await HandleGameEnd(0); // Color doesn't matter for stalemate
                }
                MessageBox.Show("Stalemate!");
            }
            else
            {
                if (Online && isCheckIndex)
                {
                    isCheckIndex = false;
                    SendMsg("UnCheck:" + oldKingIndex);
                }
                mainWindow.ResetCheckColor(oldKingIndex);
            }
        }

        public abstract bool SendMsg(string msg);


        public void AddMoveToHistory(Move move)
        {
            GameRecord m = new GameRecord();
            if (move.movingPiece.chessColor == ChessColor.BLACK && mainWindow.GameHistory.Items.Count > 0)
            {
                m = (GameRecord)mainWindow.GameHistory.Items.GetItemAt(mainWindow.GameHistory.Items.Count - 1);
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

        private void RemoveMoveFromHistory(Move move)
        {
            GameRecord m = new GameRecord();
            if (move.movingPiece.chessColor == ChessColor.BLACK)
            {
                m = (GameRecord)mainWindow.GameHistory.Items.GetItemAt(mainWindow.GameHistory.Items.Count - 1);
                mainWindow.GameHistory.Items.Remove(m);
                m.BlackMove = string.Empty;
                mainWindow.GameHistory.Items.Add(m);

            }
            else
            {
                mainWindow.GameHistory.Items.RemoveAt(mainWindow.GameHistory.Items.Count - 1);
            }
        }
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public virtual async Task HandleGameEnd(int result) { }
    }

    public class GameRecord
    {
        public int MoveNumber { get; set; } // E.g., 1, 2, 3
        public string WhiteMove { get; set; } // E.g., "e4"
        public string BlackMove { get; set; } // E.g., "e5"

        [JsonConstructor]
        public GameRecord() { }

        public GameRecord(GameRecord moveRecord)
        {
            this.MoveNumber = moveRecord.MoveNumber;
            this.WhiteMove = moveRecord.WhiteMove;
            this.BlackMove = moveRecord.BlackMove;
        }
    }
}
