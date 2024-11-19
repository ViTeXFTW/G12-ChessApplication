


using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace G12_ChessApplication.Src.chess_game.util
{
    [JsonDerivedType(typeof(Move), "Move")]
    [JsonDerivedType(typeof(CastlingMove), "CastlingMove")]
    [JsonDerivedType(typeof(EnPassantMove), "EnPassantMove")]
    [XmlInclude(typeof(EnPassantMove))]
    [XmlInclude(typeof(CastlingMove))]
    public class Move
    {
        public int fromIndex { get; set; }
        public int toIndex { get; set; }
        public bool isACapture { get; set; }
        public ChessPiece movingPiece { get; set; } = null;
        public ChessPiece capturedPiece { get; set; } = null; 

        [JsonConstructor]
        public Move(int fromIndex, int toIndex, bool isACapture = false)
        {
            this.fromIndex = fromIndex;
            this.toIndex = toIndex;
            this.isACapture = isACapture;
        }

        public Move() { }

        public Move(Move move)
        {
            this.fromIndex = move.fromIndex;
            this.toIndex = move.toIndex;
            this.isACapture = move.isACapture;
            this.capturedPiece = move.capturedPiece;
            this.movingPiece = move.movingPiece;
        }

        public virtual void InvertMove()
        {
            this.fromIndex = Math.Abs(this.fromIndex - 63);
            this.toIndex = Math.Abs(this.toIndex - 63);
        }

        public virtual void ReverseMove()
        {
            int temp = fromIndex;
            this.fromIndex = toIndex;
            this.toIndex = temp;
        }

        public virtual string GetMoveString()
        {
            string move = this.movingPiece.pieceCharacter;
            char col = (char)(this.toIndex % 8 + 'a');
            int row = Math.Abs(this.toIndex / 8 - 7) + 1;

            if (this.isACapture || this is EnPassantMove)
            {
                if (this.movingPiece is Pawn)
                {
                    move += (char)(this.fromIndex % 8 + 'a');
                }
                move += "x";
            }
            move += col;
            move += row;
            Trace.WriteLine(move);
            return move;
        }
    }

    public class EnPassantMove : Move
    {
        public int capturedIndex { get; set; }
        public ChessPiece enPassantPiece { get; set; } = null;


        [JsonConstructor]
        public EnPassantMove(int fromIndex, int toIndex, int capturedIndex) : base(fromIndex, toIndex)
        {
            this.capturedIndex = capturedIndex;
        }
        public EnPassantMove(EnPassantMove enPassantMove) : base(enPassantMove)
        {
            this.capturedIndex = enPassantMove.capturedIndex;
            this.enPassantPiece = enPassantMove.enPassantPiece;
        }
        public EnPassantMove () { }

        public override void InvertMove()
        {
            base.InvertMove();
            this.capturedIndex = Math.Abs(this.capturedIndex - 63);
        }
    }

    public class CastlingMove : Move
    {
        public Move rookMove { get; set; }
        [JsonConstructor]
        public CastlingMove(int fromIndex, int toIndex, int rookFromIndex, int rookToIndex) : base(fromIndex, toIndex)
        {
            rookMove = new Move(rookFromIndex, rookToIndex);
        }
        public CastlingMove(CastlingMove castlingMove) : base(castlingMove)
        {
            rookMove = new Move(castlingMove.rookMove.fromIndex, castlingMove.rookMove.toIndex);
        }
        public CastlingMove() { }

        public override void InvertMove()
        {
            base.InvertMove();
            rookMove.InvertMove();
        }

        public override void ReverseMove()
        {
            base.ReverseMove();
            rookMove.ReverseMove();
        }

        public override string GetMoveString()
        {
            string move = "";
            if (Math.Abs(rookMove.toIndex - rookMove.fromIndex) > 2) // Long Castling
            {
                move = "O-O-O";
            }
            else
            {
                move = "O-O";
            }

            Trace.WriteLine(move);
            return move;
        }

    }
}
