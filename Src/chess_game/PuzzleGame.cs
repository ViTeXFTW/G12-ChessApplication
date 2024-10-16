using G12_ChessApplication.Src.chess_game.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G12_ChessApplication.Src.chess_game
{
    internal class PuzzleGame : Game
    {
        public List<ChessPuzzle> Puzzles { get; set; } = new List<ChessPuzzle>();

        public ChessPuzzle CurrentPuzzle { get; set; }
        private int FromSquareIndex { get; set; } = -1;
        private int ToSquareIndex { get; set; } = -1;
        public bool puzzleIsActive { get; set; } = true;
        public PuzzleGame(MainWindow main, ChessColor chessColor) : base(main, chessColor)
        {
            ChessPuzzle chessPuzzle = new ChessPuzzle();
            chessPuzzle.board = "4r1rk/5K1b/7R/R7/8/8/8/8";
            chessPuzzle.enemyMoves.Add(Tuple.Create(7, 15));
            chessPuzzle.playerMoves.Add(Tuple.Create(23, 15));
            chessPuzzle.playerMoves.Add(Tuple.Create(24, 31));

            Puzzles.Add(chessPuzzle);

            CurrentPuzzle = new ChessPuzzle(chessPuzzle);

            gameState = FenParser.CreatePieceArray(colorFacing,CurrentPuzzle.board);

            MainWindow.mainBoard.SetBoardSetup(CurrentPuzzle.board);
        }

        public override void SquareClicked(int index)
        {
            if (puzzleIsActive == false)
                { return; }

            if (FromSquareIndex == -1)
            {
                FromSquareIndex = index;
                mainWindow.HighlightSquare(index);
                return;
            }

            if (CurrentPuzzle.playerMoves.First().Item1 == FromSquareIndex && CurrentPuzzle.playerMoves.First().Item2 == index)
            {
                mainWindow.UpdateUIAfterMove(FromSquareIndex, index);
                CurrentPuzzle.playerMoves.RemoveAt(0);

                if (CurrentPuzzle.enemyMoves.Count > 0)
                {
                    mainWindow.UpdateUIAfterMove(CurrentPuzzle.enemyMoves.First().Item1, CurrentPuzzle.enemyMoves.First().Item2);
                    CurrentPuzzle.enemyMoves.RemoveAt(0);
                }
            }

            mainWindow.ResetSquareColor(FromSquareIndex);
            FromSquareIndex = -1;

            IsPuzzleDone();
        }

        private void IsPuzzleDone()
        {
            if (CurrentPuzzle.playerMoves.Count == 0)
            {
                puzzleIsActive = false;
            }
        }

        public void StartNextPuzzle()
        {
            puzzleIsActive = true;
        }

        public void RepeatPuzzle()
        {
            CurrentPuzzle = new ChessPuzzle(Puzzles.First());
            puzzleIsActive = true;

        }
    }
}
