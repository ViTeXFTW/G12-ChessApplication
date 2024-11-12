using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G12_ChessApplication.Src.chess_game.util;


namespace G12_ChessApplication.Src.chess_game.util
{
    public static class FenParser
    {
        public static ChessPiece[] CreatePieceArray(string fenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR")
        {
            int colorFacing = Game.PlayerColor == ChessColor.WHITE ? 1 : -1;
            ChessPiece[] board = new ChessPiece[64];

            // Extract the piece placement field from the FEN string
            string[] fields = fenString.Split(' ');
            if (fields.Length < 1)
            {
                throw new ArgumentException("Invalid FEN string");
            }

            string piecePlacement = fields[0];

            // Split the ranks
            string[] ranks = piecePlacement.Split('/');
            if (ranks.Length != 8)
            {
                throw new ArgumentException("Invalid FEN string: Incorrect number of ranks");
            }

            // Parse each rank starting from rank 8 to rank 1
            for (int rank = 0; rank < 8; rank++)
            {
                string rankString = ranks[rank];
                int file = (colorFacing == 1) ? 0 : 7; // Files range from 0 (a) to 7 (h)
                int actualRank = (colorFacing == 1) ? rank : Math.Abs(rank - 7); // Map FEN rank to array index

                foreach (char c in rankString)
                {
                    if (char.IsDigit(c))
                    {
                        // Empty squares
                        int emptySquares = c - '0';
                        file += emptySquares * colorFacing; // Move file pointer
                    }
                    else
                    {
                        // Determine the color of the piece
                        ChessColor color = char.IsUpper(c) ? ChessColor.WHITE : ChessColor.BLACK;
                        ChessPiece piece = CreatePieceFromChar(char.ToLower(c), color);

                        int squareIndex = actualRank * 8 + file;
                        board[squareIndex] = piece;

                        file += 1 * colorFacing; // Move to the next file
                    }

                    if (file > 8)
                    {
                        throw new ArgumentException("Invalid FEN string: Too many squares in rank");
                    }
                }
            }

            return board;
        }

        private static ChessPiece CreatePieceFromChar(char pieceChar, ChessColor color)
        {
            switch (pieceChar)
            {
                case 'p':
                    return new Pawn(color);
                case 'n':
                    return new Knight(color);
                case 'b':
                    return new Bishop(color);
                case 'r':
                    return new Rook(color);
                case 'q':
                    return new Queen(color);
                case 'k':
                    return new King(color);
                default:
                    throw new ArgumentException($"Invalid piece type '{pieceChar}' in FEN string");
            }
        }

        public static string GetFenStringFromArray(ChessPiece[] gameState)
        {
            string fenString = "";
            int emptySpaces = 0;
            int counter = 0;
            foreach (ChessPiece piece in gameState)
            {
                if (piece == null)
                {
                    emptySpaces++;
                }
                else
                {
                    if (emptySpaces > 0)
                    {
                        fenString += emptySpaces.ToString();
                        emptySpaces = 0;
                    }
                    string newPieceStr = GetCharFromPiece(piece).ToString();
                    fenString += newPieceStr;
                }
                if (counter % 8 == 7)
                {
                    if (emptySpaces > 0)
                    {
                        fenString += emptySpaces.ToString();
                        emptySpaces = 0;
                    }
                    fenString += "/";
                }
                counter++;
            }
            return fenString;
        }
        private static char GetCharFromPiece(ChessPiece piece)
        {
            char ch; 
            switch (piece)
            {
                case Pawn:
                    ch = 'p';
                    break;
                case Knight:
                    ch = 'n';
                    break;
                case Bishop:
                    ch = 'b';
                    break;
                case Rook:
                    ch = 'r';
                    break;
                case Queen:
                    ch = 'q';
                    break;
                case King:
                    ch = 'k';
                    break;
                default:
                    ch = ' ';
                    break;

            }
            if (piece.ChessColor == ChessColor.WHITE)
            {
                ch = char.ToUpper(ch);
            }
            
            return ch;

        }
    }
}
