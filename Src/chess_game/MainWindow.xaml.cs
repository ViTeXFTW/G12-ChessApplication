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
        private static Grid mainBoard = new ChessBoard();
        private Game game;
        public MainWindow()
        {
            InitializeComponent();

            InitializeBoard();

            game = new Game("Mark", "Kevin");
        }

        private void InitializeBoard()
        {
            mainBoard = new ChessBoard(500, 500);
            mainBoard.VerticalAlignment = VerticalAlignment.Top;
            mainBoard.HorizontalAlignment = HorizontalAlignment.Left;
            Game.Children.Add(mainBoard);

            foreach (Grid square in mainBoard.Children)
            {
                square.MouseDown += OnBoardClick;
            }
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
                    game.ApplyMove(game.SelectedPieceIndex, index);
                    UpdateUIAfterMove(game.SelectedPieceIndex, index);

                    // Switch to the next player
                    game.currentPlayer = game.currentPlayer == game.whitePlayer ? game.blackPlayer : game.whitePlayer;
                }
                else
                {
                    game.DeselectPiece();
                    // Optionally update UI to reflect deselection
                }
            }
            else
            {
                if (game.CanSelectPieceAt(index))
                {
                    game.SelectPieceAt(index);
                    // Optionally update UI to highlight the selected piece
                }
            }
        }

        private void UpdateUIAfterMove(int fromIndex, int toIndex)
        {

        }
        
    }
}