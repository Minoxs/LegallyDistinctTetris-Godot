using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace LegallyDistinctTetris.scripts.board;

[Serializable]
public class Piece {
    public readonly Vector2I Value;

    public IEnumerable<Vector2I> CellsPosition {
        get {
            var cells = _cells.ToArray();
            for (var i = 0; i < cells.Length; i++) cells[i] += _position;
            return cells;
        }
    }

    private readonly Vector2I[] _cells;
    private Vector2I _position;

    private Piece(Piece piece) {
        Value = piece.Value;
        _cells = piece._cells.ToArray();
        _position = piece._position;
    }

    public Piece(TileMapPattern pattern, Vector2I position) {
        _cells = pattern.GetUsedCells().ToArray();
        Value = pattern.GetCellAtlasCoords(_cells[0]);
        _position = position;
    }

    public Piece Clone() {
        return new Piece(this);
    }

    public override string ToString() {
        return "Value: " + Value + " | Cells: " + _cells;
    }

    public Piece Rotate() {
        for (var i = 0; i < _cells.Length; i++) {
            var aux = _cells[i];
            _cells[i].X = -aux.Y;
            _cells[i].Y = aux.X;
        }

        return this;
    }

    public Piece ResetPosition() {
        _position = Vector2I.Zero;
        return this;
    }

    public Piece Offset(Vector2I offset) {
        _position += offset;
        return this;
    }

    public static Piece operator +(Piece piece, Vector2I offset) {
        return piece.Clone().Offset(offset);
    }
}
