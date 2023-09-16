using Godot;
using Godot.Collections;

namespace Tetris.scripts.objects;

// TODO CONVERT PIECE TO RECORD PROBABLY
// TODO USE C# ARRAY WHEREVER POSSIBLE
public partial class Piece : RefCounted {
    public Array<Vector2I> Cells;
    public Vector2I Value;

    public Piece(Vector2I value, Array<Vector2I> cells) {
        Value = value;
        Cells = cells.Duplicate();
    }

    public Piece(TileMapPattern pattern) {
        Cells = pattern.GetUsedCells();
        Value = pattern.GetCellAtlasCoords(Cells[0]);
    }

    public Piece Duplicate() {
        return new Piece(Value, Cells);
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
            p.Cells[i] = aux;
        }

        return p;
    }
}
