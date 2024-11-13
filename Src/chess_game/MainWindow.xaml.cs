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
        public static ChessColor PlayerColor = ChessColor.WHITE;
        public string GameType { get; }
        public static int boardHeight { get; set; } = 500;
        public static int boardWidth { get; set; } = 500;


        public MainWindow(string gameType, string code = "")
        {
            InitializeComponent();
            
            if (code != "")
            {
                GameCode = code;
                if (code != "Host")
                {
                    PlayerColor = ChessColor.BLACK;
                }
                else
                {
                    PlayerColor = ChessColor.WHITE;
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
                    game = new PlayerGame(this, GameCode, PlayerColor);
                    break;
                case "puzzles":
                    game = new PuzzleGame(this, PlayerColor);
                    break;
                case "Analysis":
                    game = new AnalysisGame(this);
                    break;
                default:
                    break;
            }
        }

        private void InitializeBoard(string board = "")
        {
            
            mainBoard = new ChessBoard(boardHeight, boardWidth);
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

        public void HighlightSquare(int index, SolidColorBrush color = null)
        {
            if (color == null)
            {
                color = Brushes.IndianRed;
            }
            Grid square = (Grid)mainBoard.Children[index];
            Rectangle squareColor = (Rectangle)square.Children[0];

            squareColor.Fill = color;
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

        public void UpdateUIAfterMove()
        {
            for (int i = 0; i < mainBoard.Children.Count; i++)
            {
                bool pieceFound = false;
                Grid square = (Grid)mainBoard.Children[i];
                for (int j = 0; j < square.Children.Count; j++)
                {
                    if (square.Children[j] is ChessPieceUI)
                    {
                        square.Children.RemoveAt(j);
                        if (game.gameState[i] != null)
                        {
                            square.Children.Add(new ChessPieceUI(game.gameState[i].uri));
                        }
                        pieceFound = true;
                        break;
                    }
                }
                if ( !pieceFound && game.gameState[i] != null)
                {
                    square.Children.Add(new ChessPieceUI(game.gameState[i].uri));
                }
            }
        }

        public void ShowLegalMoves(List<Move> legalMoves)
        {
            foreach (var item in legalMoves)
            {
                if (item.isACapture)
                {
                    HighlightSquare(item.toIndex, Brushes.Gray);
                }
                else if (mainBoard.Children[item.toIndex] is Grid square)
                {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = 20;
                    ellipse.Height = 20;
                    ellipse.Fill = Brushes.Gray;
                    square.Children.Add(ellipse);
                }
            }
        }

        public void RemoveLegalMoves(List<Move> legalMoves)
        {
            foreach (var item in legalMoves)
            {
                if (mainBoard.Children[item.toIndex] is Grid square)
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
            Trace.WriteLine(FenParser.GetFenStringFromArray(game.gameState));
            game.UndoMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (game is PlayerGame playerGame)
            {
                playerGame.CleanUpSockets();
            }
            goBack?.Invoke(this, EventArgs.Empty);
        }

        public void HighLightMove(Move move)
        {
            HighlightSquare(move.fromIndex);
            HighlightSquare(move.toIndex);
        }

        public void UnHighLightMove(Move move)
        {
            ResetSquareColor(move.fromIndex);
            ResetSquareColor(move.toIndex);
        }
    }
}