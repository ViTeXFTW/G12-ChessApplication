using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace G12_ChessApplication.Src.chess_game.util
{
    public class ChessBoard : Grid
    {
        Dictionary<char, Func<ChessPiece>> charToPieceConverter = new Dictionary<char, Func<ChessPiece>>
        {
            {'r', () => new Rook(ChessColor.BLACK)},   // Black rook
            {'R', () => new Rook(ChessColor.WHITE)},   // White rook
            {'n', () => new Knight(ChessColor.BLACK)}, // Black knight
            {'N', () => new Knight(ChessColor.WHITE)}, // White knight
            {'b', () => new Bishop(ChessColor.BLACK)}, // Black bishop
            {'B', () => new Bishop(ChessColor.WHITE)}, // White bishop
            {'q', () => new Queen(ChessColor.BLACK)},  // Black queen
            {'Q', () => new Queen(ChessColor.WHITE)},  // White queen
            {'k', () => new King(ChessColor.BLACK)},   // Black king
            {'K', () => new King(ChessColor.WHITE)},   // White king
            {'p', () => new Pawn(ChessColor.BLACK)},   // Black pawn
            {'P', () => new Pawn(ChessColor.WHITE)}    // White pawn
        };

        string BoardSetup = "r1bk3r/p2pBpNp/n4n2/1p1NP2P/6P1/3P4/P1P1K3/q5b1";

        public ChessBoard(string pieceLayout = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR")
        {
            DefualtBoard();
        }

        private void DefualtBoard()
        {
            this.Children.Clear();
            this.RowDefinitions.Clear();
            this.ColumnDefinitions.Clear();

            for (int i = 0; i < 8; i++)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            int switchColor = 0;
            SolidColorBrush green = new SolidColorBrush(Colors.DarkGreen);
            SolidColorBrush beige = new SolidColorBrush(Colors.Beige);

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    ChessSquareUI square = new ChessSquareUI((switchColor % 2 == 0) ? beige : green);
                    switchColor++;
                    ChessSquareUI.SetRow(square, row);
                    ChessSquareUI.SetColumn(square, col);
                    this.Children.Add(square);
                }
                switchColor++;
            }
        }

        public void SetBoardSetup(string layout = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR")
        {
            RemovePieces();
            BoardSetup = layout;
            int color = Game.PlayerColor == ChessColor.WHITE ? 1 : -1;

            int squareIndex = (color == 1) ? 0 : 63;
            foreach (char item in layout)
            {
                if (char.IsDigit(item))
                {
                    int emptySquares = Convert.ToInt32(item.ToString());
                    squareIndex += emptySquares * color;
                }
                else if (charToPieceConverter.TryGetValue(item, out Func<ChessPiece> createPiece))
                {
                    ChessPiece newPiece = createPiece();
                    ChessSquareUI square = Children[squareIndex] as ChessSquareUI;

                    ChessPieceUI pieceImage = new ChessPieceUI(newPiece.uri);
                    //pieceImage.HorizontalAlignment = HorizontalAlignment.Center;
                    //pieceImage.VerticalAlignment = VerticalAlignment.Center;
                    square.Children.Add(pieceImage);
                    squareIndex += 1 * color;
                }
            }
        }

        public void ResetBoard()
        {
            RemovePieces();
            SetBoardSetup(BoardSetup);
        }

        private void RemovePieces()
        {
            foreach (ChessSquareUI square in Children)
            {
                square.Children.Clear();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double size = Math.Min(availableSize.Width, availableSize.Height);
            Size finalSize = new Size(size, size);
            base.MeasureOverride(finalSize);
            return finalSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double size = Math.Min(finalSize.Width, finalSize.Height);
            Size squareSize = new Size(size, size);
            base.ArrangeOverride(squareSize);
            return squareSize;
        }
    }
}