using G12_ChessApplication.Src.chess_game.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace G12_ChessApplication.Src.chess_game
{
    internal class PuzzleGame : Game
    {
        public List<ChessPuzzle> Puzzles { get; set; } = new List<ChessPuzzle>();

        public ChessPuzzle CurrentPuzzle { get; set; }
        public bool puzzleIsActive { get; set; } = true;
        public PuzzleGame(MainWindow main) : base(main, ChessColor.WHITE)
        {
            ChessPuzzle chessPuzzle = new ChessPuzzle();
            chessPuzzle.board = "4r1rk/5K1b/7R/R7/8/8/8/8";
            chessPuzzle.color = ChessColor.WHITE;
            chessPuzzle.enemyMoves.Add(new Move(7, 15));
            chessPuzzle.playerMoves.Add(new Move(23, 15));
            chessPuzzle.playerMoves.Add(new Move(24, 31));

            Puzzles.Add(chessPuzzle);

            CurrentPuzzle = new ChessPuzzle(chessPuzzle);

            Game.PlayerColor = CurrentPuzzle.color;
            Game.UserPlayer = new Player("User", CurrentPuzzle.color);

            gameState = FenParser.CreatePieceArray(CurrentPuzzle.board);
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

        public override void HandleClick(int index)
        {
            if (puzzleIsActive == false)
            { return; }

            if (CurrentPuzzle.playerMoves.First().fromIndex == selectedSquareIndex && CurrentPuzzle.playerMoves.First().toIndex == index)
            {
                ApplyMove(CurrentPuzzle.playerMoves.First());
                CurrentPuzzle.playerMoves.RemoveAt(0);

                if (CurrentPuzzle.enemyMoves.Count > 0)
                {
                    UserPlayer.ChangePlayer();
                    ApplyMove(CurrentPuzzle.enemyMoves.First());
                    CurrentPuzzle.enemyMoves.RemoveAt(0);
                }
                UserPlayer.ChangePlayer();
            }

            IsPuzzleDone();
        }

        public override void Setup()
        {
            MainWindow.mainBoard.SetBoardSetup(CurrentPuzzle.board);
        }
    }
}
