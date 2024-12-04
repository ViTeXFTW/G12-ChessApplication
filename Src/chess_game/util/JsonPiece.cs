using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace G12_ChessApplication.Src.chess_game.util
{
    // This class is used to send through the tcp connection instead of using an instance of ChessPiece to keep ChessPiece abstract.
    public class JsonPiece
    {
        // piece is the fen character for the piece, so p would be black pawn, and P would be white pawn etc.
        public char piece { get; set; }
        // pieceType is the character that gets used for chess notaion, for example if the piece is a knight: Nc3.
        // A pawn does not use a P, but rather just the coloum it came from, so fx. e4, or exf5 for a capture
        public char pieceType { get; set; }
        public bool hasMoved { get; set; }
        public ChessColor chessColor { get; set; }

        [JsonConstructor]
        public JsonPiece(char piece, bool hasMoved, ChessColor chessColor)
        {
            this.piece = piece;
            this.hasMoved = hasMoved;
            this.chessColor = chessColor;
            if (char.ToUpper(piece) != 'P') // A pawn does not have its own character
            {
                this.pieceType = char.ToUpper(piece);
            }
        }

        public ChessPiece GetChessPiece()
        {
            ChessPiece chessPiece = FenParser.CreatePieceFromChar(piece);
            chessPiece.hasMoved = hasMoved;
            
            return chessPiece;
        }
    }
}
