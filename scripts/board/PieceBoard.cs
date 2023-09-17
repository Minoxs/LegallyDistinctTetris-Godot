using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Tetris.scripts.board;

// TODO CREATE SOME FORM OF FOREACH CELL
public class PieceBoard {
    // TODO CREATE/USE AN ACTUAL MATRIX TYPE
    public Array<Array<Vector2I>> Board;

    // TODO USE SIZE VECTOR
    public int Height;
    public int Width;

    public PieceBoard(Vector2I size) {
        Width = size.X;
        Height = size.Y;
        Board = new Array<Array<Vector2I>>();
        for (var y = 0; y < Height; y++) {
            var line = new Array<Vector2I>();
            for (var x = 0; x < Width; x++) line.Add(Vector2I.Zero);
            Board.Add(line);
        }
    }

    public override string ToString() {
        return Board.Aggregate(
            "",
            (res, line) => line.Aggregate(
                res,
                (cur, cell) => cur + (cell + "\n")
            )
        );
    }

    public static bool IsEmptyCell(Vector2I cell) {
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
            return IsEmptyCell(Board[pos.Y][pos.X]);
        }
        catch (Exception) {
            return false;
        }
    }

    public bool CanPlace(Piece piece) {
        return piece.CellsPosition.All(CanPlaceCell);
    }

    public bool Place(Piece piece) {
        if (!CanPlace(piece)) return false;
        foreach (var cell in piece.CellsPosition) Board[cell.Y][cell.X] = piece.Value;
        return true;
    }

    private void MoveLineDown(int lineIndex, int amount) {
        for (var x = 0; x < Width; x++) {
            Board[lineIndex + amount][x] = Board[lineIndex][x];
            Board[lineIndex][x] = Vector2I.Zero;
        }
    }

    private void MoveAllLinesDown(int startIndex, int amount) {
        for (var y = startIndex; y >= 0; y--) MoveLineDown(y, amount);
    }

    private void ShiftDown() {
        var emptyLines = 0;
        for (var y = Height - 1; y >= 0; y--) {
            if (IsLineEmpty(Board[y])) {
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
        for (var i = 0; i < Width; i++) line[i] = Vector2I.Zero;
        return true;
    }

    public int BreakLines() {
        var res = Board.Count(BreakLine);
        if (res > 0) ShiftDown();
        return res;
    }
}
