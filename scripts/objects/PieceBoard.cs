using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Tetris.scripts.objects;

[GlobalClass]
public partial class PieceBoard : RefCounted {
    public Array<Array<Vector2I>> board;
    public int height;
    public int width;

    public PieceBoard Init(Vector2I size) {
        width = size.X;
        height = size.Y;
        board = new Array<Array<Vector2I>>();
        for (var y = 0; y < height; y++) {
            var line = new Array<Vector2I>();
            for (var x = 0; x < width; x++) line.Add(Vector2I.Zero);
            board.Add(line);
        }

        return this;
    }

    public override string ToString() {
        return board.Aggregate(
            "",
            (res, line) => line.Aggregate(
                res,
                (cur, cell) => cur + (cell + "\n")
            )
        );
    }

    private static bool IsEmptyCell(Vector2I cell) {
        return cell.LengthSquared() == 0;
    }

    private static bool IsLineEmpty(IEnumerable<Vector2I> line) {
        return line.All(IsEmptyCell);
    }

    private static bool IsLineBreakable(IEnumerable<Vector2I> line) {
        return !line.Any(IsEmptyCell);
    }

    private bool CanPlaceCell(Vector2I pos) {
        try {
            return IsEmptyCell(board[pos.Y][pos.X]);
        }
        catch (Exception) {
            return false;
        }
    }

    public bool CanPlace(Vector2I pos, Piece piece) {
        return piece.Cells.All(cell => CanPlaceCell(cell + pos));
    }

    public bool Place(Vector2I pos, Piece piece) {
        if (!CanPlace(pos, piece)) return false;
        foreach (var cell in piece.Cells) {
            var p = cell + pos;
            board[p.Y][p.X] = piece.Value;
        }

        return true;
    }

    private void MoveLineDown(int lineIndex, int amount) {
        for (var x = 0; x < width; x++) {
            board[lineIndex + amount][x] = board[lineIndex][x];
            board[lineIndex][x] = Vector2I.Zero;
        }
    }

    private void MoveAllLinesDown(int startIndex, int amount) {
        for (var y = startIndex; y >= 0; y--) MoveLineDown(y, amount);
    }

    private void ShiftDown() {
        var emptyLines = 0;
        for (var y = height - 1; y >= 0; y--) {
            if (IsLineEmpty(board[y])) {
                emptyLines++;
                continue;
            }

            if (emptyLines == 0) continue;

            MoveAllLinesDown(y, emptyLines);
            return;
        }
    }

    private bool BreakLine(Array<Vector2I> line) {
        if (!IsLineBreakable(line)) return false;
        for (var i = 0; i < width; i++) line[i] = Vector2I.Zero;
        return true;
    }

    public int BreakLines() {
        var res = board.Count(BreakLine);
        if (res > 0) ShiftDown();
        return res;
    }
}
