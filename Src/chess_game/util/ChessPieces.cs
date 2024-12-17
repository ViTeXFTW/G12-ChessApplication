using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;

namespace G12_ChessApplication.Src.chess_game.util
{
    public class Pawn : ChessPiece
    {
        public Pawn(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            uri = "pack://application:,,,/Images/" + colorLetter + "_pawn_2x_ns.png";

            directions = new List<Direction> { new Direction(-1, 0), new Direction(-1, -1), new Direction(-1, 1) };
            distance = 2;
        }

        public Pawn(Pawn p) : base(p) { }
        public Pawn() { }

        public override object Clone()
        {
            return new Pawn(this);
        }

        public override List<Move> FindLegalMoves(int index, ref ChessPiece[] gameState, Move lastMove)
        {
            List<Move> tempResult = new List<Move>();
            List<Move> result = new List<Move>();

            if (Game.twoCheck)
            {
                return tempResult;
            }

            bool check = CheckForBinding(index, ref gameState, out tempResult);

            int promotionRow = Game.PlayerColor == Game.UserPlayer.Color ? 0 : 7;

            if (check)
            {
                foreach (var move in tempResult)
                {
                    if (move.isACapture)
                    {
                        if (CanAttackPos(move.toIndex, move.fromIndex))
                        {
                            int curRow = move.toIndex / 8;
                            if (curRow == promotionRow)
                            {
                                result.Add(new PromotionMove(move));
                            }
                            else
                            {
                                result.Add(move);
                            }
                            break;
                        }
                    }
                }
                return result;
            }

            tempResult.Clear();

            int fromRow = index / 8;
            int fromCol = index % 8;
            List<Direction> newDirections = Game.UserPlayer.directionCo == 1 ? InvertDirections() : this.directions;

            foreach (var item in newDirections)
            {
                int row = fromRow + item.row; 
                int col = fromCol + item.col;

                int startingRow = Game.UserPlayer.directionCo == 1 ? 1 : 6; 

                // this.distance can be 2 if pawn hasnt moved, so can only be used if pawn is moving forward
                int disCounter = (item.col == 0 && startingRow == fromRow) ? this.distance : 1;

                while (row >= 0 && row <= 7 && col >= 0 && col <= 7 && disCounter > 0)
                {
                    int newIndex = row * 8 + col;
                    if (gameState[newIndex] != null)
                    {
                        // Can attack if piece is opponents and its diagonal
                        if (this.ChessColor != gameState[newIndex].ChessColor && item.col != 0)
                        {
                            if (row == promotionRow)
                            {
                                tempResult.Add(new PromotionMove(index, newIndex, true));
                            }
                            else
                            {
                                tempResult.Add(new Move(index, newIndex, true));
                            }
                        }
                        break;
                    }
                    if (item.col == 0)
                    {
                        if (row == promotionRow)
                        {
                            tempResult.Add(new PromotionMove(index, newIndex));
                        }
                        else
                        {
                            tempResult.Add(new Move(index, newIndex));
                        }
                    }

                    disCounter--;
                    row += item.row;
                    col += item.col;
                }
            }

            if (fromRow == 3 && lastMove != null)
            {
                int newCol = fromCol - 1;
                for (int i = 0; i < 2; i++)
                {
                    int newIndex = fromRow * 8 + newCol;
                    if (gameState[newIndex] is Pawn && gameState[newIndex].ChessColor != ChessColor)
                    {
                        if (lastMove.toIndex == newIndex)
                        {
                            int newFromRow = lastMove.fromIndex / 8;
                            int newToRow = lastMove.toIndex / 8;
                            if (Math.Abs(newFromRow - newToRow) == 2)
                            {
                                int enPassentIndex = newIndex - 8;
                                tempResult.Add(new EnPassantMove(index, enPassentIndex, newIndex));
                            }
                        }
                    }
                    newCol = fromCol + 1;
                }
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

        public bool CanAttackPos(int toIndex, int fromIndex)
        {

            int fromRow = fromIndex / 8;
            int fromCol = fromIndex % 8;
            int toRow = toIndex / 8;
            int toCol = toIndex % 8;

            if (Math.Abs(fromRow - toRow) == 1 && Math.Abs(fromCol - toCol) == 1)
            {
                return true;
            }

            return false;
        }

        public override bool CanTakePieceAt(int ownIndex, int attackIndex, ref ChessPiece[] gameState, out List<Move> moves)
        {
            moves = new List<Move>();
            int ownRow = ownIndex / 8;
            int ownCol = ownIndex % 8;

            int attackRow = attackIndex / 8;
            int attackCol = attackIndex % 8;

            int rowDiff = ownRow - attackRow;
            int colDiff = ownCol - attackCol;

            if (Math.Abs(rowDiff) == Math.Abs(colDiff) && Math.Abs(colDiff) == 1 && rowDiff == Game.UserPlayer.directionCo)
            {
                return true;
            }

            return false;
        }
    }


    public class Bishop : ChessPiece
    {
        public Bishop(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            uri = "pack://application:,,,/Images/" + colorLetter + "_bishop_2x_ns.png";

            pieceCharacter = "B";
            directions = new List<Direction> { new Direction(-1, -1), new Direction(-1, 1), new Direction(1, 1), new Direction(1, -1) };
        }
        public Bishop(Bishop b) : base(b) { }
        public Bishop() { }

        public override bool CanTakePieceAt(int ownIndex, int attackIndex, ref ChessPiece[] gameState, out List<Move> moves)
        {
            return CanTakeDiagonal(ownIndex, attackIndex, ref gameState, out moves);
        }

        public override object Clone()
        {
            return new Bishop(this);
        }
    }

    public class Knight : ChessPiece
    {
        public Knight(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            uri = "pack://application:,,,/Images/" + colorLetter + "_knight_2x_ns.png";

            pieceCharacter = "N";
            directions = new List<Direction> { new Direction(1, 2), new Direction(1, -2), new Direction(-1, 2), new Direction(-1, -2),
                                                     new Direction(2, 1), new Direction(2, -1), new Direction(-2, 1), new Direction(-2, -1)
            };
            distance = 1;
        }
        public Knight(Knight k) : base(k) { }
        public Knight() { }

        public override bool CanTakePieceAt(int ownIndex, int attackIndex, ref ChessPiece[] gameState, out List<Move> moves)
        {
            moves = new List<Move>();
            int ownRow = ownIndex / 8;
            int ownCol = ownIndex % 8;

            int attackRow = attackIndex / 8;
            int attackCol = attackIndex % 8;

            int rowDiff = Math.Abs(ownRow - attackRow);
            int colDiff = Math.Abs(ownCol - attackCol);
            
            if ( (rowDiff == 1 && colDiff == 2) || (rowDiff == 2 && colDiff == 1) )
            {
                return true;
            }

            return false;
        }

        public override object Clone()
        {
            return new Knight(this);
        }
    }

    public class Rook : ChessPiece
    {
        public Rook(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            uri = "pack://application:,,,/Images/" + colorLetter + "_rook_2x_ns.png";

            pieceCharacter = "R";
            directions = new List<Direction> { new Direction(1, 0), new Direction(-1, 0), new Direction(0, 1), new Direction(0, -1) };
        }
        public Rook(Rook r) : base(r) { }
        
        public Rook() { }

        public override bool CanTakePieceAt(int ownIndex, int attackIndex, ref ChessPiece[] gameState, out List<Move> moves)
        {
            return CanTakeHorizontalOrVertical(ownIndex, attackIndex, ref gameState, out moves);
        }

        public override object Clone()
        {
            return new Rook(this);
        }
    }

    public class Queen : ChessPiece
    {
        public Queen(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            uri = "pack://application:,,,/Images/" + colorLetter + "_queen_2x_ns.png";

            pieceCharacter = "Q";
            directions = new List<Direction> { new Direction(1, 0), new Direction(-1, 0), new Direction(0, 1), new Direction(0, -1),
                                                    new Direction(-1, -1), new Direction(-1, 1), new Direction(1, 1), new Direction(1, -1) 
            };
        }
        public Queen(Queen q) : base(q) { }
        public Queen() { }

        public override bool CanTakePieceAt(int ownIndex, int attackIndex, ref ChessPiece[] gameState, out List<Move> moves)
        {
            return CanTakeDiagonal(ownIndex, attackIndex, ref gameState, out moves) || CanTakeHorizontalOrVertical(ownIndex, attackIndex, ref gameState, out moves); 
        }

        public override object Clone()
        {
            return new Queen(this);
        }
    }

    public class King : ChessPiece
    {
        public King(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            uri = "pack://application:,,,/Images/" + colorLetter + "_king_2x_ns.png";

            pieceCharacter = "K";
            directions = new List<Direction> { new Direction(1, 0), new Direction(-1, 0), new Direction(0, 1), new Direction(0, -1),
                                                    new Direction(-1, -1), new Direction(-1, 1), new Direction(1, 1), new Direction(1, -1)
            };
            distance = 1;
        }
        public King(King q) : base(q) { }
        public King() { }

        public override bool CanTakePieceAt(int ownIndex, int attackIndex, ref ChessPiece[] gameState, out List<Move> moves)
        {
            moves = new List<Move>();
            int ownRow = ownIndex / 8;
            int ownCol = ownIndex % 8;

            int attackRow = attackIndex / 8;
            int attackCol = attackIndex % 8;

            int rowDiff = Math.Abs(ownRow - attackRow);
            int colDiff = Math.Abs(ownCol - attackCol);

            if ( (rowDiff == 1 && colDiff == 0) || (rowDiff == 1 && colDiff == 1) || (rowDiff == 0 && colDiff == 1) )
            {
                return true;
            }

            return false;
        }

        public override object Clone()
        {
            return new King(this);
        }

        public override List<Move> FindLegalMoves(int index, ref ChessPiece[] gameState, Move lastMove)
        {
            List<Move> result = new List<Move>();

            List<Move> possibleMoves = GenerelFindMoves(index, gameState);

            if (!hasMoved)
            {
                for (int i = 0; i < gameState.Length; i++)
                {
                    bool canCastle = false;
                    ChessPiece piece = gameState[i];
                    if (piece != null && piece is Rook && piece.ChessColor == Game.UserPlayer.Color && !piece.hasMoved)
                    {
                        List<Move> castleMoves;
                        bool check = piece.CanTakePieceAt(i, index, ref gameState, out castleMoves);
                        if (castleMoves.Count >= 2)
                        {
                            castleMoves.RemoveRange(0, castleMoves.Count - 2);
                            foreach (Move move in castleMoves)
                            {
                                List<List<Move>> checks = Game.IsKingInCheck(ref gameState, Game.UserPlayer.Color, move.toIndex);
                                if (checks.Count != 0)
                                {
                                    canCastle = false;
                                    break;
                                }
                                else
                                {
                                    canCastle = true;
                                }
                            }
                            if (canCastle)
                            {
                                Move kingMove = castleMoves.First();
                                Move rookMove = castleMoves.Last();
                                possibleMoves.Add(new CastlingMove(index, kingMove.toIndex, i, rookMove.toIndex));
                            }
                        }

                    }
                }
            }

            foreach (Move move in possibleMoves)
            {
                List<List<Move>> checks = Game.IsKingInCheck(ref gameState, Game.UserPlayer.Color, move.toIndex);
                if (checks.Count == 0)
                {
                    result.Add(move);
                }
            }

            return result;
        }
    }
}
