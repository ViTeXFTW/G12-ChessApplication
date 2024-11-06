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
        public event EventHandler goBack;
        public static ChessBoard mainBoard;
        private Game game;
        private string GameCode = string.Empty;
        private ChessColor color = ChessColor.WHITE;
        private int colorFacing = 1;
        public string GameType { get; }


        public MainWindow(string gameType, string code = "")
        {
            InitializeComponent();
            
            if (code != "")
            {
                GameCode = code;
                if (code != "Host")
                {
                    color = ChessColor.BLACK;
                    colorFacing = -1;
                }
            }
            GameType = gameType;

            InitializeBoard();
            SetupGameType();

        }

        private void SetupGameType()
        {
            switch(GameType)
            {
                case "play":
                    game = new PlayerGame(this, GameCode, color);
                    break;
                case "puzzles":
                    game = new PuzzleGame(this, color);
                    break;
                default:
                    break;
            }
        }

        private void InitializeBoard(string board = "")
        {
            mainBoard = new ChessBoard(colorFacing, 500, 500);
            //mainBoard.VerticalAlignment = VerticalAlignment.Top;
            //mainBoard.HorizontalAlignment = HorizontalAlignment.Left;
            foreach (Grid square in mainBoard.Children)
            {
                square.MouseDown += OnBoardClick;
            }
            GameGrid.Children.Add(mainBoard);
        }

        private void OnBoardClick(object sender, MouseButtonEventArgs e)
        {
            Grid square = (Grid)sender;
            int index = mainBoard.Children.IndexOf(square);
            game.SquareClicked(index);
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

        public void UpdateUIAfterMove(Move move)
        {
            Grid squareFrom = (Grid)mainBoard.Children[move.fromIndex];
            Grid squareTo = (Grid)mainBoard.Children[move.toIndex];
            ChessPiece pieceToMove = (ChessPiece)squareFrom.Children[1];
            squareFrom.Children.Remove(pieceToMove);

            for (int i = 0; i < squareTo.Children.Count; i++)
            {
                if (squareTo.Children[i] is ChessPiece)
                {
                    squareTo.Children.RemoveAt(i);
                }
            }

            squareTo.Children.Add(pieceToMove);


            if (move is EnPassantMove enPassantMove)
            {
                Grid enPassantSquare = (Grid)mainBoard.Children[enPassantMove.capturedIndex];
                ChessPiece enPassantPiece = (ChessPiece)enPassantSquare.Children[1];
                enPassantSquare.Children.Remove(enPassantPiece);
            }

        }

        public void ShowLegalMoves(List<Move> legalMoves)
        {

            foreach (var item in legalMoves)
            {
                if (item.isACapture)
                {
                    HighlightSquare(item.toIndex);
                }
                else if (mainBoard.Children[item.toIndex] is Grid square)
                {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = 20;
                    ellipse.Height = 20;
                    ellipse.Fill = new SolidColorBrush(Colors.Cyan);
                    square.Children.Add(ellipse);
                }
            }


        }
        public void RemoveLegalMoves(List<Move> legalMoves)
        {

            foreach (var item in legalMoves)
            {
                if (item.isACapture)
                {
                    ResetSquareColor(item.toIndex);
                }
                else if (mainBoard.Children[item.toIndex] is Grid square)
                {
                    foreach (var child in square.Children)
                    {
                        if (child is Ellipse ellipse)
                        {
                            square.Children.Remove(ellipse);
                            break;
                        }
                    }
                }
            }


        }

        private void RepeatPuzzle_Click(object sender, RoutedEventArgs e)
        {
            mainBoard.ResetBoard();
            PuzzleGame puzzleGame = (PuzzleGame) game;
            puzzleGame.RepeatPuzzle();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(FenParser.GetFenStringFromArray(game.gameState, game.colorFacing));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (game is PlayerGame playerGame)
            {
                playerGame.CleanUpSockets();
            }
            goBack?.Invoke(this, EventArgs.Empty);
        }
    }
}