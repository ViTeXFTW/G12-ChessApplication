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
                int file = 0; // Files range from 0 (a) to 7 (h)
                int actualRank = rank; // Map FEN rank to array index

                foreach (char c in rankString)
                {
                    if (char.IsDigit(c))
                    {
                        // Empty squares
                        int emptySquares = c - '0';
                        file += emptySquares; // Move file pointer
                    }
                    else
                    {
                        // Determine the color of the piece
                        ChessColor color = char.IsUpper(c) ? ChessColor.WHITE : ChessColor.BLACK;
                        ChessPiece piece = CreatePieceFromChar(char.ToLower(c), color);

                        int squareIndex = actualRank * 8 + file;
                        board[squareIndex] = piece;

                        file++; // Move to the next file
                    }

                    if (file > 8)
                    {
                        throw new ArgumentException("Invalid FEN string: Too many squares in rank");
                    }
                }

                if (file != 8)
                {
                    throw new ArgumentException("Invalid FEN string: Not enough squares in rank");
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
    }
}
