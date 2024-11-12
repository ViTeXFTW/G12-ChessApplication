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
    class Pawn : ChessPiece
    {
        public Pawn(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            
            BitmapImage image = new BitmapImage();
            image.BeginInit();

            image.UriSource = new Uri("pack://application:,,,/Images/" + colorLetter + "_pawn_2x_ns.png");

            image.EndInit();
            Source = image;

            directions = new List<Tuple<int, int>> { Tuple.Create(-1, 0), Tuple.Create(-1, -1), Tuple.Create(-1, 1) };
            distance = 2;
        }

        public Pawn(Pawn p) : base(p) { }

        public override object Clone()
        {
            return new Pawn(this);
        }

        public override List<Move> FindLegalMoves(int index, ref ChessPiece[] gameState, Move lastMove)
        {
            List<Move> result = new List<Move>();
            bool check = CheckForBinding(index, ref gameState, out result);

            if (check)
            {
                foreach (var move in result)
                {
                    if (move.isACapture)
                    {
                        if (CanAttackPos(move.toIndex, move.fromIndex))
                        {
                            return new List<Move> { new Move(move) };
                        }
                    }
                }
                result.Clear();
                return result;
            }

            result.Clear();

            int fromRow = index / 8;
            int fromCol = index % 8;
            List<Tuple<int, int>> newDirections = Game.UserPlayer.directionCo == 1 ? InvertDirections() : this.directions;

            foreach (var item in newDirections)
            {
                int row = fromRow + item.Item1;
                int col = fromCol + item.Item2;
                
                // this.distance can be 2 if pawn hasnt moved, so can only be used if pawn is moving forward
                int disCounter = (item.Item2 == 0) ? this.distance : 1;

                while (row >= 0 && row <= 7 && col >= 0 && col <= 7 && disCounter > 0)
                {
                    int newIndex = row * 8 + col;
                    if (gameState[newIndex] != null)
                    {
                        // Can attack if piece is opponents and its diagonal
                        if (this.ChessColor != gameState[newIndex].ChessColor && item.Item2 != 0)
                        {
                            result.Add(new Move(index, newIndex, true));
                        }
                        break;
                    }
                    if (item.Item2 == 0)
                    {
                        result.Add(new Move(index, newIndex));
                    }

                    disCounter--;
                    row += item.Item1;
                    col += item.Item2;
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
                                result.Add(new EnPassantMove(index, enPassentIndex, newIndex));
                            }
                        }
                    }
                    newCol = fromCol + 1;
                }
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

    }


    class Bishop : ChessPiece
    {
        public Bishop(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Images/" + colorLetter + "_bishop_2x_ns.png");
            image.EndInit();
            Source = image;

            directions = new List<Tuple<int, int>> { Tuple.Create(-1, -1), Tuple.Create(-1, 1), Tuple.Create(1, 1), Tuple.Create(1, -1) };
        }
        public Bishop(Bishop b) : base(b) { }

        public override object Clone()
        {
            return new Bishop(this);
        }
    }

    class Knight : ChessPiece
    {
        public Knight(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Images/" + colorLetter + "_knight_2x_ns.png");
            image.EndInit();
            Source = image;

            directions = new List<Tuple<int, int>> { Tuple.Create(1, 2), Tuple.Create(1, -2), Tuple.Create(-1, 2), Tuple.Create(-1, -2),
                                                     Tuple.Create(2, 1), Tuple.Create(2, -1), Tuple.Create(-2, 1), Tuple.Create(-2, -1)
            };
            distance = 1;
        }
        public Knight(Knight k) : base(k) { }
        public override object Clone()
        {
            return new Knight(this);
        }
    }

    class Rook : ChessPiece
    {
        public Rook(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Images/" + colorLetter + "_rook_2x_ns.png");
            image.EndInit();
            Source = image;

            directions = new List<Tuple<int, int>> { Tuple.Create(1, 0), Tuple.Create(-1, 0), Tuple.Create(0, 1), Tuple.Create(0, -1) };
        }
        public Rook(Rook r) : base(r) { }
        public override object Clone()
        {
            return new Rook(this);
        }
    }

    class Queen : ChessPiece
    {
        public Queen(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Images/" + colorLetter + "_queen_2x_ns.png");
            image.EndInit();
            Source = image;
            directions = new List<Tuple<int, int>> { Tuple.Create(1, 0), Tuple.Create(-1, 0), Tuple.Create(0, 1), Tuple.Create(0, -1),
                                                    Tuple.Create(-1, -1), Tuple.Create(-1, 1), Tuple.Create(1, 1), Tuple.Create(1, -1) 
            };
        }
        public Queen(Queen q) : base(q) { }
        public override object Clone()
        {
            return new Queen(this);
        }
    }

    class King : ChessPiece
    {
        public bool hasCastled = false;
        public King(ChessColor color) : base(color)
        {
            string colorLetter = (ChessColor == ChessColor.WHITE) ? "w" : "b";
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/Images/" + colorLetter + "_king_2x_ns.png");
            image.EndInit();
            Source = image;

            directions = new List<Tuple<int, int>> { Tuple.Create(1, 0), Tuple.Create(-1, 0), Tuple.Create(0, 1), Tuple.Create(0, -1),
                                                    Tuple.Create(-1, -1), Tuple.Create(-1, 1), Tuple.Create(1, 1), Tuple.Create(1, -1)
            };
            distance = 1;
        }
        public King(King q) : base(q)
        {
            this.hasCastled = q.hasCastled;
        }
        public override object Clone()
        {
            return new King(this);
        }

        public override List<Move> FindLegalMoves(int index, ref ChessPiece[] gameState, Move lastMove)
        {
            List<Move> result = new List<Move>();


            return GenerelFindMoves(index, gameState, result);
        }
    }
}
