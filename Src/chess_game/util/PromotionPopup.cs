using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace G12_ChessApplication.Src.chess_game.util
{
    public class PromotionPopup : Grid
    {
        private readonly TaskCompletionSource<ChessPiece> _taskCompletionSource;
        public PromotionPopup(ChessColor chessColor, int index)
        {
            int row = index / 8;

            _taskCompletionSource = new TaskCompletionSource<ChessPiece>();

            // Create promotion pieces
            AddPromotionPiece(new Queen(chessColor));
            AddPromotionPiece(new Rook(chessColor));
            AddPromotionPiece(new Bishop(chessColor));
            AddPromotionPiece(new Knight(chessColor));

            Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
        }

        private void AddPromotionPiece(ChessPiece piece)
        {
            ChessSquareUI square = new ChessSquareUI(Brushes.Transparent);
            ChessPieceUI pieceUI = new ChessPieceUI(piece.uri);

            square.Children.Add(pieceUI);
            square.MouseLeftButtonUp += (sender, args) =>
            {
                _taskCompletionSource.TrySetResult(piece); // Complete the task with the selected piece
            };

            int row = Children.Count; // Add to the next row
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            Grid.SetRow(square, row);
            Children.Add(square);
        }
        public Task<ChessPiece> GetSelectedPieceAsync()
        {
            return _taskCompletionSource.Task;
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            Size finalSize = new Size(availableSize.Width / MainWindow.DefaultPopupWidthRatio, availableSize.Height / MainWindow.DefaultPopupHeightRatio);
            base.MeasureOverride(finalSize);
            return finalSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size squareSize = new Size(finalSize.Width / MainWindow.DefaultPopupWidthRatio, finalSize.Height / MainWindow.DefaultPopupHeightRatio);
            base.ArrangeOverride(squareSize);
            return squareSize;
        }
    }
}
