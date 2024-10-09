using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace G12_ChessApplication.Src.chess_game.util
{
    internal class ChessBoard : Grid
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

        public ChessBoard( double height = 640, double width = 640, string pieceLayout = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR")
        {
            Height = height;
            Width = width;

            DefualtBoard();
            SetBoardSetup(pieceLayout);
        }


        private void DefualtBoard()
        {
            double squareHeight = Height / 8;
            double squareWidth = Width / 8;

            int col = 0;
            int row = 0;
            int switchColor = 0;

            SolidColorBrush green = new SolidColorBrush(Colors.DarkGreen);
            SolidColorBrush beige = new SolidColorBrush(Colors.Beige);

            for (int i = 0; i < 64; i++)
            {
                Grid square = new Grid();
                square.Height = squareHeight;
                square.Width = squareWidth;
                square.VerticalAlignment = VerticalAlignment.Top;
                square.HorizontalAlignment = HorizontalAlignment.Left;
                square.Margin = new Thickness(col * squareWidth, row * squareHeight, 0, 0);

                Rectangle squareColor = new Rectangle();
                squareColor.Fill = (switchColor + i) % 2 == 0 ? beige : green;
                squareColor.Stroke = new SolidColorBrush(Colors.Cyan);
                squareColor.StrokeThickness = 0;
                square.Children.Add(squareColor);

                col++;
                if (i % 8 == 7)
                {
                    col = 0;
                    row++;
                    switchColor = switchColor == 0 ? 1 : 0;
                }

                Children.Add(square);
            }
        }

        private void SetBoardSetup(string layout)
        {
            BoardSetup = layout;
            double chessPieceHeight = Height / 8 * 0.9;
            double chessPieceWidth = Width / 8 * 0.9;

            int squareIndex = 0;
            foreach (char item in layout)
            {
                if (char.IsDigit(item))
                {
                    int emptySquares = Convert.ToInt32(item.ToString());
                    squareIndex += emptySquares;
                }
                else if (charToPieceConverter.TryGetValue(item, out Func<ChessPiece> createPiece))
                {
                    ChessPiece newPiece = createPiece();
                    newPiece.Height = chessPieceHeight;
                    newPiece.Width = chessPieceWidth;
                    Grid square = Children[squareIndex] as Grid;
                    square.Children.Add(newPiece);

                    squareIndex++;
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
            foreach (Grid square in Children)
            {
                for (int i = 0; i < square.Children.Count; i++)
                {
                    if (square.Children[i] is ChessPiece)
                    {
                        square.Children.Remove(square.Children[i]);
                    }
                }
            }
        }
    }
}
