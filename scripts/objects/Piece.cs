using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Tetris.scripts.objects;

// TODO CONVERT PIECE TO RECORD PROBABLY
// TODO USE C# ARRAY WHEREVER POSSIBLE
public partial class Piece : RefCounted {
    private Array<Vector2I> Cells;
    private Vector2I Position;

    public Piece(Vector2I value, Array<Vector2I> cells, Vector2I position) {
        Value = value;
        Cells = cells.Duplicate();
        Position = position;
    }

    public Piece(TileMapPattern pattern, Vector2I position) {
        Cells = pattern.GetUsedCells();
        Value = pattern.GetCellAtlasCoords(Cells[0]);
        Position = position;
    }

    public Vector2I Value { get; }

    public IEnumerable<Vector2I> CellsPosition {
        get {
            var cells = Cells.Duplicate();
            for (var i = 0; i < cells.Count; i++) cells[i] += Position;

            return cells;
        }
    }

    private Piece Duplicate() {
        return new Piece(Value, Cells, Position);
    }

    public override string ToString() {
        return "Value: " + Value + " | Cells: " + Cells;
    }

    public Piece Rotate() {
        var p = Duplicate();
        for (var i = 0; i < Cells.Count; i++) {
            var aux = Cells[i];
            aux.X = -Cells[i].Y;
            aux.Y = +Cells[i].X;
            p.Cells[i] = aux + Vector2I.Down;
        }

        return p;
    }

    public Piece Offset(Vector2I offset) {
        Position += offset;
        return this;
    }

    public static Piece operator +(Piece piece, Vector2I offset) {
        return piece.Duplicate().Offset(offset);
    }
}
