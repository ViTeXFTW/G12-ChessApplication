using System;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using G12_ChessApplication.Src.chess_game;
using G12_ChessApplication.Src.chess_game.util;

namespace G12_ChessApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ChessBoard mainBoard;
        private int? selectedSquareIndex = null;
        private Game game;
        private PuzzleGame puzzleGame;
        private string GameCode = string.Empty;
        public string GameType { get; }


        public MainWindow(string gameType, string code = "")
        {
            InitializeComponent();


            if (code != "")
            {
                GameCode = code;
            }
            GameType = gameType;

            game = new Game();
            puzzleGame = new PuzzleGame(this);
            InitializeBoard();


        }

        private void InitializeBoard()
        {
            mainBoard = new ChessBoard(500, 500);
            //mainBoard = new ChessBoard(500, 500, puzzleGame.Puzzles.First().board);
            mainBoard.VerticalAlignment = VerticalAlignment.Top;
            mainBoard.HorizontalAlignment = HorizontalAlignment.Left;
            GamePlane.Child = mainBoard;
            foreach (Grid square in mainBoard.Children)
            {
                square.MouseDown += OnBoardClick;
            }
        }
        public void PuzzleClick(object sender, MouseButtonEventArgs e)
        {
            Grid square = (Grid)sender;
            int index = mainBoard.Children.IndexOf(square);

            puzzleGame.SquareClicked(index);
        }


        private void OnBoardClick(object sender, MouseButtonEventArgs e)
        {
            Grid square = (Grid)sender;
            int index = mainBoard.Children.IndexOf(square);
            if (index != -1 && game.currentPlayer == game.whitePlayer || game.currentPlayer == game.blackPlayer)
            {
                HandlePlayerMove(index);
            }
        }

        private void HandlePlayerMove(int index)
        {
            if (game.IsPieceSelected)
            {
                if (game.IsValidMove(game.SelectedPieceIndex, index))
                {
                    // Apply the move
                    game.ApplyMove(game.SelectedPieceIndex, index);
                    UpdateUIAfterMove(game.SelectedPieceIndex, index);

                    // Reset the color of the previously selected square
                    ResetSquareColor(game.SelectedPieceIndex);
                    selectedSquareIndex = null;

                    // Switch to the next player
                    game.currentPlayer = game.currentPlayer == game.whitePlayer ? game.blackPlayer : game.whitePlayer;
                    Trace.WriteLine($"Changed player to {game.currentPlayer.Color}");
                }
                else
                {

                    // Invalid move, deselect the piece
                    game.DeselectPiece();

                    // Optionally update UI to reflect deselection

                    ResetSquareColor(game.SelectedPieceIndex);
                    selectedSquareIndex = null;
                }
            }
            else
            {
                if (game.CanSelectPieceAt(index))
                {
                    // Deselect the previous selection if any
                    if (selectedSquareIndex != null)
                    {
                        ResetSquareColor(selectedSquareIndex.Value);
                    }

                    // Select the piece
                    game.SelectPieceAt(index);

                    // Highlight the selected square
                    HighlightSquare(index);

                    // Keep track of the selected square index
                    selectedSquareIndex = index;
                }
            }
        }

        public void HighlightSquare(int index)
        {
            Grid square = (Grid)mainBoard.Children[index];
            Rectangle squareColor = (Rectangle)square.Children[0];

            squareColor.Fill = Brushes.IndianRed;
        }

        public void ResetSquareColor(int index)
        {
            Grid square = (Grid)mainBoard.Children[index];
            Rectangle squareColor = (Rectangle)square.Children[0];

            // Calculate the original color based on the square's position
            int row = index / 8;
            int col = index % 8;
            bool isLightSquare = (row + col) % 2 == 0;

            squareColor.Fill = isLightSquare ? Brushes.Beige : Brushes.DarkGreen;
        }

        public void UpdateUIAfterMove(int fromIndex, int toIndex)
        {
            Grid squareFrom = (Grid)mainBoard.Children[fromIndex];
            Grid squareTo = (Grid)mainBoard.Children[toIndex];
            ChessPiece pieceToMove = (ChessPiece)squareFrom.Children[1];
            squareFrom.Children.Remove(pieceToMove);

            for (int i = 0; i < squareTo.Children.Count; i++)
            {
                if (squareTo.Children[i] is ChessPiece)
                {
                    squareTo.Children.RemoveAt(i);
                }
            }
            Rectangle squareColorFrom = (Rectangle)squareFrom.Children[0];
            Rectangle squareColorTo = (Rectangle)squareTo.Children[0];

            //squareColorFrom.StrokeThickness = (squareColorFrom.StrokeThickness == 0) ? 3 : 0;
            //squareColorTo.StrokeThickness = (squareColorTo.StrokeThickness == 0) ? 3 : 0;

            squareTo.Children.Add(pieceToMove);

        }

        private void RepeatPuzzle_Click(object sender, RoutedEventArgs e)
        {
            mainBoard.ResetBoard();
            puzzleGame.RepeatPuzzle();
        }
    }
}