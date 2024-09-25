using System.Diagnostics;
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
using Chess;
using ChessBoardSpace;

namespace G12_ChessApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Grid mainBoard = new ChessBoard();
        public MainWindow()
        {
            InitializeComponent();
            

            mainBoard = new ChessBoard(500, 500);
            mainBoard.VerticalAlignment = VerticalAlignment.Top;
            mainBoard.HorizontalAlignment = HorizontalAlignment.Left;
            Game.Children.Add(mainBoard);

            foreach (var item in mainBoard.Children)
            {
                Grid square = (Grid)item;
                square.MouseDown += OnBoardClick;
            }

            //MovePiece(52, 36);
            //MovePiece(12, 28);
            LoginScene();
            MainMenu();
            GameLoop();
        }

        public void LoginScene()
        {

        }

        public void MainMenu()
        {

        }


        public void GameLoop()
        {
            
        }



        public static void MovePiece(int indexFrom, int indexTo)
        {
            Grid squareFrom = (Grid)mainBoard.Children[indexFrom];
            Grid squareTo = (Grid)mainBoard.Children[indexTo];
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
            squareColorFrom.StrokeThickness = (squareColorFrom.StrokeThickness == 0) ? 3 : 0;
            squareColorTo.StrokeThickness = (squareColorTo.StrokeThickness == 0) ? 3 : 0;

            squareTo.Children.Add(pieceToMove);

        }

        public void OnBoardClick(object sender, RoutedEventArgs e)
        {
            Grid square = (Grid)sender;
            int index = mainBoard.Children.IndexOf(square);
            if (index != -1)
            {
                Trace.WriteLine(index);
            }
        }
    }
}