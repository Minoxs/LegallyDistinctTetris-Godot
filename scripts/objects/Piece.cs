using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Tetris.scripts.objects;

// TODO CONVERT PIECE TO RECORD PROBABLY
// TODO USE C# ARRAY WHEREVER POSSIBLE
public record struct Piece {
    public readonly Vector2I Value;

    public IEnumerable<Vector2I> CellsPosition {
        get {
            var cells = _cells.Duplicate();
            for (var i = 0; i < cells.Count; i++) cells[i] += _position;

            return cells;
        }
    }

    private readonly Array<Vector2I> _cells;
    private Vector2I _position;

    private Piece(Piece piece) {
        Value = piece.Value;
        _cells = piece._cells.Duplicate();
        _position = piece._position;
    }

    public Piece(Vector2I value, Array<Vector2I> cells, Vector2I position) {
        Value = value;
        _cells = cells.Duplicate();
        _position = position;
    }

    public Piece(TileMapPattern pattern, Vector2I position) {
        _cells = pattern.GetUsedCells();
        Value = pattern.GetCellAtlasCoords(_cells[0]);
        _position = position;
    }

    public Piece Duplicate() {
        return new Piece(this);
    }

    public override string ToString() {
        return "Value: " + Value + " | Cells: " + _cells;
    }

    public Piece Rotate() {
        for (var i = 0; i < _cells.Count; i++) {
            var aux = _cells[i];
            aux.X = -_cells[i].Y;
            aux.Y = +_cells[i].X;
            _cells[i] = aux;
        }

        return this;
    }

    public Piece Offset(Vector2I offset) {
        _position += offset;
        return this;
    }

    public static Piece operator +(Piece piece, Vector2I offset) {
        return piece.Duplicate().Offset(offset);
    }
}
