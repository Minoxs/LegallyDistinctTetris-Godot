using Godot;

namespace LegallyDistinctTetris.scripts.board;

[GlobalClass]
public partial class PieceBox : TileMap {
    private Piece _heldPiece;

    private void Render() {
        Clear();
        foreach (var cell in _heldPiece.CellsPosition) SetCell(0, cell, 0, _heldPiece.Value);
    }

    public Piece Swap(Piece piece) {
        var aux = _heldPiece;
        _heldPiece = piece.Clone().ResetPosition();
        Render();
        return aux;
    }

    public override void _Ready() {
        for (var i = 0; i < 1; i++) AddLayer(0);
        var parent = GetNode<TileMap>("..");
        if (parent == null) return;
        TileSet = parent.TileSet;
        CellQuadrantSize = parent.CellQuadrantSize;
    }
}
