using Godot;

namespace LegallyDistinctTetris.scripts.board;

[GlobalClass]
public partial class PieceBox : Node2D {
    private Piece _heldPiece;

    public Piece Swap(Piece piece) {
        var aux = _heldPiece;
        _heldPiece = piece.Clone().ResetPosition();
        return aux;
    }
}
