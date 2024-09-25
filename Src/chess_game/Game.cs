using G12_ChessApplication.Src.chess_game.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace G12_ChessApplication.Src.chess_game
{
    public class Game
    {
        public Player whitePlayer {  get; set; }
        public Player blackPlayer { get; set; }
        public Player currentPlayer { get; set; }
        public bool IsPieceSelected { get; set; }
        public int SelectedPieceIndex { get; set; }
        // Other game state variables

        public Game(string whitePlayerName, string blackPlayerName)
        {
            whitePlayer = new Player(whitePlayerName, ChessColor.WHITE);
            blackPlayer = new Player(blackPlayerName, ChessColor.BLACK);
            currentPlayer = whitePlayer;
        }

        public bool CanSelectPieceAt(int index)
        {
            // Logic to determine if a piece can be selected
            return true;
        }

        public void SelectPieceAt(int index)
        {
            IsPieceSelected = true;
            SelectedPieceIndex = index;
        }

        public void DeselectPiece()
        {
            IsPieceSelected = false;
        }

        public bool IsValidMove(int fromIndex, int toIndex)
        {
            // Validate the move according to chess rules
            return true;
        }

        public void ApplyMove(int fromIndex, int toIndex)
        {
            // Update the game state with the move
        }
    }
}
