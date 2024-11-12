


using System.Text.Json.Serialization;

namespace G12_ChessApplication.Src.chess_game.util
{
    [JsonDerivedType(typeof(Move), "Move")]
    [JsonDerivedType(typeof(CastlingMove), "CastlingMove")]
    [JsonDerivedType(typeof(EnPassantMove), "EnPassantMove")]
    public class Move
    {
        public int fromIndex { get; set; }
        public int toIndex { get; set; }
        public bool isACapture { get; }
        public ChessPiece movingPiece { get; set; } = null;
        public ChessPiece capturedPiece { get; set; } = null; 

        [JsonConstructor]
        public Move(int fromIndex, int toIndex, bool isACapture = false)
        {
            this.fromIndex = fromIndex;
            this.toIndex = toIndex;
            this.isACapture = isACapture;
        }

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

        public override void InvertMove()
        {
            base.InvertMove();
            this.capturedIndex = Math.Abs(this.capturedIndex - 63);
        }
    }

    public class CastlingMove : Move
    {
        public Move kingMove;
        public Move rookMove;
        [JsonConstructor]
        public CastlingMove(int fromIndex, int toIndex, int rookFromIndex, int rookToIndex) : base(fromIndex, toIndex)
        {
            kingMove = new Move(fromIndex, toIndex);
            rookMove = new Move(rookFromIndex, rookToIndex);
        }
        public CastlingMove(CastlingMove castlingMove) : base(castlingMove)
        {
            kingMove = new Move(castlingMove.kingMove.fromIndex, castlingMove.kingMove.toIndex);
            rookMove = new Move(castlingMove.rookMove.fromIndex, castlingMove.rookMove.toIndex);
        }

        public override void InvertMove()
        {
            kingMove.InvertMove();
            rookMove.InvertMove();
        }

        public override void ReverseMove()
        {
            kingMove.ReverseMove();
            rookMove.ReverseMove();
        }

    }
}
