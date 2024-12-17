using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace G12_ChessApplication.Src.chess_game.util
{
    [JsonDerivedType(typeof(ChessPiece), "ChessPiece")]
    [JsonDerivedType(typeof(Pawn), "Pawn")]
    [JsonDerivedType(typeof(Bishop), "Bishop")]
    [JsonDerivedType(typeof(Knight), "Knight")]
    [JsonDerivedType(typeof(Rook), "Rook")]
    [JsonDerivedType(typeof(Queen), "Queen")]
    [JsonDerivedType(typeof(King), "King")]
    public abstract class ChessPiece
    {
        public readonly ChessColor chessColor;
        public bool hasMoved { get; set; } = false;
        protected List<Direction> directions { get; set; }
        protected int distance { get; set; } = 100;
        public string uri { get; protected set; } = string.Empty;

        [JsonConstructor]
        public ChessPiece(ChessColor color)
        {
            chessColor = color;
        }

        public ChessPiece(ChessPiece chessPiece)
        {
            this.chessColor = chessPiece.chessColor;
            this.directions = chessPiece.directions;
            this.distance = chessPiece.distance;
            this.hasMoved = chessPiece.hasMoved;
            this.uri = chessPiece.uri;
        }

        public abstract bool CanTakePieceAt(int ownIndex, int attackIndex, ref ChessPiece[] gameState, out List<Move> moves);


        protected bool CanTakeHorizontalOrVertical(int ownIndex, int attackIndex, ref ChessPiece[] gameState, out List<Move> moves)
        {
            moves = new List<Move>();
            int ownRow = ownIndex / 8;
            int ownCol = ownIndex % 8;

            int attackRow = attackIndex / 8;
            int attackCol = attackIndex % 8;

            int rowDiff = Math.Abs(ownRow - attackRow);
            int colDiff = Math.Abs(ownCol - attackCol);

            int colStep = 0;
            int rowStep = 0;
            int steps = 0;

            if (ownRow == attackRow)
            {
                colStep = attackCol > ownCol ? 1 : -1;
                steps = colDiff;
            }
            else if (ownCol == attackCol)
            {
                rowStep = attackRow > ownRow ? 1 : -1;
                steps = rowDiff;
            }
            else
            {
                return false;
            }
            bool check = IsPathToKingBlocked(ownIndex, gameState, ownRow, ownCol, rowStep, colStep, steps, ref moves);

            if (check || steps == 0)
            {
                return false;
            }

            return true;
            
        }

        protected bool CanTakeDiagonal(int ownIndex, int attackIndex, ref ChessPiece[] gameState, out List<Move> moves)
        {
            moves = new List<Move>();
            int ownRow = ownIndex / 8;
            int ownCol = ownIndex % 8;

            int attackRow = attackIndex / 8;
            int attackCol = attackIndex % 8;

            int rowDiff = attackRow - ownRow;
            int colDiff = attackCol - ownCol;

            if (Math.Abs(rowDiff) == Math.Abs(colDiff))
            {
                int rowStep = rowDiff > 0 ? 1 : -1;
                int colStep = colDiff > 0 ? 1 : -1;
                int steps = Math.Abs(rowDiff);

                bool check = IsPathToKingBlocked(ownIndex, gameState, ownRow, ownCol, rowStep, colStep, steps, ref moves);

                if (check || steps == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }

            return false;
        }

        public List<Direction> InvertDirections()
        {
            List<Direction> newDirections = new List<Direction>();
            this.directions.ForEach(x => newDirections.Add(new Direction(x.row * -1, x.col * -1)));
            return newDirections;
        }

        public virtual List<Move> FindLegalMoves(int index, ref ChessPiece[] gameState, Move lastMove)
        {
            List<Move> result = new List<Move>();
            List<Move> bindingMoves = new List<Move>();
            List<Move> generalMoves = new List<Move>();
            List<Move> tempResult = new List<Move>();

            // If there is a double check, the only piece that can move is the king
            if (Game.twoCheck)
            {
                return tempResult;
            }

            generalMoves = GenerelFindMoves(index, gameState);

            bool binding = CheckForBinding(index, ref gameState, out bindingMoves);
            if (binding)
            {
                if (this is Knight)
                {   
                    // If the knight is bound then there are no possible moves
                    bindingMoves.Clear();
                    return bindingMoves;
                }
                else
                {
                    // Cross reference general moves with the binding, and keep only the ones that match.
                    // The reason we cant just take the binding moves is because they dont take into account what the piece can do.
                    // It just gives a list of moves that would avoid putting the king in check
                    foreach (var res in generalMoves)
                    {
                        if (bindingMoves.Any(x => x.toIndex == res.toIndex))
                        {
                            tempResult.Add(res);
                        }
                    }
                }
            }
            else
            {
                tempResult = generalMoves;
            }

            foreach (Move move in tempResult)
            {
                List<Move> moves;
                int oppKingIndex = Game.GetKingIndex(ref gameState, Game.UserPlayer.Color == ChessColor.WHITE ? ChessColor.BLACK : ChessColor.WHITE);
                if (CanTakePieceAt(move.toIndex, oppKingIndex, ref gameState, out moves))
                {
                    move.isACheck = true;
                }
            }

            // If there is a check the piece must stop the check if it wants to move
            // So we cross reference with the checkIndexes, which are the moves that could stop or block a check
            if (Game.oneCheck)
            {
                foreach (Move move in tempResult)
                {
                    if (Game.checkIndexes.Any(m => m.toIndex == move.toIndex))
                    {
                        result.Add(move);
                    }
                }
            }
            else
            {
                result = tempResult;
            }

            return result;
        }

        public List<Move> GenerelFindMoves(int index, ChessPiece[] gameState)
        {
            List <Move> result = new List<Move>();

            int fromRow = index / 8;
            int fromCol = index % 8;

            foreach (var item in this.directions)
            {
                int row = fromRow + item.row;
                int col = fromCol + item.col;
                int disCounter = this.distance;

                while (row >= 0 && row <= 7 && col >= 0 && col <= 7 && disCounter > 0)
                {
                    int newIndex = row * 8 + col;
                    if (gameState[newIndex] != null)
                    {
                        if (this.chessColor != gameState[newIndex].chessColor)
                        {
                            result.Add(new Move(index, newIndex, true));
                        }
                        break;
                    }
                    result.Add(new Move(index, newIndex));

                    disCounter--;
                    row += item.row;
                    col += item.col;
                }
            }

            return result;
        }

        public bool CheckForBinding(int index, ref ChessPiece[] gameState, out List<Move> moves)
        {
            moves = new List<Move>();
            // Search for own king
            int kingIndex = Game.GetKingIndex(ref gameState, Game.UserPlayer.Color);

            // Check for diagonal or horizontal or vetical connection to the king
            int kingRow = (kingIndex / 8);
            int kingCol = kingIndex % 8;
            int pieceRow = (index / 8);
            int pieceCol = index % 8;

            int row = pieceRow;
            int col = pieceCol;

            int rowDiff = kingRow - pieceRow;
            int colDiff = kingCol - pieceCol;

            if (Math.Abs(rowDiff) == Math.Abs(colDiff))
            {
                int rowStep = rowDiff > 0 ? 1 : -1;
                int colStep = colDiff > 0 ? 1 : -1;
                int steps = Math.Abs(rowDiff);
                bool check = IsPathToKingBlocked(index, gameState, pieceRow, pieceCol, rowStep, colStep, steps, ref moves);
                if (check)
                {
                    return false;
                }

                rowStep = rowDiff > 0 ? -1 : 1;
                colStep = colDiff > 0 ? -1 : 1;

                return LoopSteps(index, gameState, ref row, ref col, colStep, rowStep, true, ref moves);
            }
            if (pieceRow == kingRow || pieceCol == kingCol)
            {
                int colStep = 0;
                int rowStep = 0;
                int steps = 0;
                if (pieceRow == kingRow)
                {
                    colStep = kingCol > pieceCol ? 1 : -1;
                    steps = Math.Abs(colDiff);
                }
                else if (pieceCol == kingCol)
                {
                    rowStep = kingRow > pieceRow ? 1 : -1;
                    steps = Math.Abs(rowDiff);
                }

                bool check = IsPathToKingBlocked(index, gameState, pieceRow, pieceCol, rowStep, colStep, steps, ref moves);
                if (check)
                {
                    return false;
                }

                rowStep *= -1;
                colStep *= -1;

                return LoopSteps(index, gameState, ref row, ref col, colStep, rowStep, false, ref moves);
            }
            return false;
        }
        public bool IsPathToKingBlocked(int startIndex, ChessPiece[] gameState, int pieceRow, int pieceCol, int rowStep, int colStep, int steps, ref List<Move> moves)
        {
            // Start at i = 1, so the piece doesnt check itself
            for (int i = 1; i < steps; i++)
            {
                int intermediateRow = pieceRow + i * rowStep;
                int intermediateCol = pieceCol + i * colStep;
                int intermediateIndex = intermediateRow * 8 + intermediateCol;
                if (gameState[intermediateIndex] != null)
                {
                    if (gameState[intermediateIndex] is King && gameState[intermediateIndex].chessColor == Game.UserPlayer.Color)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    moves.Add(new Move(startIndex, intermediateIndex));
                }
            }

            return false;
        }

        public bool LoopSteps(int startIndex, ChessPiece[] gameState, ref int row, ref int col, int colStep, int rowStep, bool dia, ref List<Move> moves)
        {
            col += colStep;
            row += rowStep;
            while (row >= 0 && row <= 7 && col >= 0 && col <= 7)
            {
                int newIndex = row * 8 + col;
                ChessPiece interceptingPiece = gameState[newIndex];

                if (interceptingPiece != null)
                {
                    if (interceptingPiece.chessColor == chessColor)
                    {
                        return false;
                    }

                    bool retVal = false;
                    // When checking for a diagonal binding, only the Queen or bishop can bind.
                    // For a horizontal or vertical binding, only a queen or a rook can bind.
                    // Dia stands for diagonal binding.
                    if (interceptingPiece is Queen || (dia && interceptingPiece is Bishop) || (!dia && interceptingPiece is Rook))
                    {
                        retVal = true;
                    }

                    moves.Add(new Move(startIndex, newIndex, true));
                    return retVal;
                }
                else
                {
                    moves.Add(new Move(startIndex, newIndex));
                }

                col += colStep;
                row += rowStep;
            }
            return false;
        }

    }
}
