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
        public static List<string> CreatePieceArray(out ChessPiece[] board, string fenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w")
        {   
            int colorFacing = Game.PlayerColor == ChessColor.WHITE ? 1 : -1;
            board = new ChessPiece[64];
            List<string> parameters = new List<string>();

            // Extract the piece placement field from the FEN string
            string[] fields = fenString.Split(' ');
            if (fields.Length < 2)
            {
                throw new ArgumentException("Invalid FEN string");
            }

            string piecePlacement = fields[0];

            for (int i = 1; i < fields.Length; i++)
            {
                parameters.Add(fields[i]);
            }

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
                        ChessPiece piece = CreatePieceFromChar(c);

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

            return parameters;
        }

        public static ChessPiece CreatePieceFromChar(char pieceChar)
        {
            ChessColor color = char.IsUpper(pieceChar) ? ChessColor.WHITE : ChessColor.BLACK;
            pieceChar = char.ToLower(pieceChar);
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

        public static string GetFenStringFromArray(ChessPiece[] gameState, ChessColor currentTurn, bool reverse = false)
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
                    if (counter != 63)
                    {
                        fenString += "/";
                    }
                }
                counter++;
            }

            if (reverse)
            {
                fenString = Game.Reverse(fenString);
            }

            fenString += " ";
            fenString += (currentTurn == ChessColor.WHITE) ? "w" : "b";

            return fenString;
        }
        public static char GetCharFromPiece(ChessPiece piece)
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
            if (piece.chessColor == ChessColor.WHITE)
            {
                ch = char.ToUpper(ch);
            }
            
            return ch;

        }
    }
}
