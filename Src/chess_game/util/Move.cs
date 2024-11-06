


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

        [JsonConstructor]
        public Move(int fromIndex, int toIndex, bool isACapture)
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
        }

        public virtual void InvertMove()
        {
            this.fromIndex = Math.Abs(this.fromIndex - 63);
            this.toIndex = Math.Abs(this.toIndex - 63);
        }
    }

    public class EnPassantMove : Move
    {
        public int capturedIndex { get; set; }
        [JsonConstructor]
        public EnPassantMove(int fromIndex, int toIndex, bool isACapture, int capturedIndex) : base(fromIndex, toIndex, isACapture)
        {
            this.capturedIndex = capturedIndex;
        }
        public EnPassantMove(EnPassantMove enPassantMove) : base(enPassantMove.fromIndex, enPassantMove.toIndex, enPassantMove.isACapture)
        {
            this.capturedIndex = enPassantMove.capturedIndex;
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
        public CastlingMove(int fromIndex, int toIndex, bool isACapture, int rookFromIndex, int rookToIndex) : base(fromIndex, toIndex, isACapture)
        {
            kingMove = new Move(fromIndex, toIndex, isACapture);
            rookMove = new Move(rookFromIndex, rookToIndex, isACapture);
        }
        public CastlingMove(CastlingMove castlingMove) : base(castlingMove.fromIndex, castlingMove.toIndex, castlingMove.isACapture)
        {
            kingMove = new Move(castlingMove.kingMove.fromIndex, castlingMove.kingMove.toIndex, castlingMove.kingMove.isACapture);
            rookMove = new Move(castlingMove.rookMove.fromIndex, castlingMove.rookMove.toIndex, castlingMove.rookMove.isACapture);
        }
        public override void InvertMove()
        {
            kingMove.InvertMove();
            rookMove.InvertMove();
        }

    }
}
