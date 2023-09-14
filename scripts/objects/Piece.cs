using Godot;
using Godot.Collections;

namespace Tetris.scripts.objects;

[GlobalClass]
public partial class Piece : RefCounted {
    public Vector2I Value;
    public Array<Vector2I> Cells;

    public Piece Init(Vector2I value, Array<Vector2I> cells) {
        Value = value;
        Cells = cells.Duplicate();
        return this;
    }

    public Piece Duplicate() {
        return new Piece().Init(Value, Cells);
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
