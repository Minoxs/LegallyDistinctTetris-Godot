using System;
using Godot;

namespace Tetris.scripts.objects;

[GlobalClass]
public partial class TileBoard : TileMap {
    private static readonly Vector2I PIECE_WHITE = Vector2I.Zero;
    private static readonly Vector2I PIECE_BLACK = Vector2I.Right;
    private static readonly double TICK_RATE = 0.25f;
    private Piece PIECE_CUR;
    private Vector2I PIECE_POS = Vector2I.Zero;
    private PieceBoard REAL_BOARD;

    private int SCORE;
    private Vector2I SPAWN = Vector2I.Zero;
    private int SQUARE_UNIT;
    private double TICK_STEP;

    private int X_SIZE;
    private int Y_SIZE;


    private bool GenerateNewPiece() {
        var dice = GD.RandRange(0, TileSet.GetPatternsCount() - 1);
        var patte = TileSet.GetPattern(dice);

        PIECE_CUR = new Piece(patte);
        PIECE_POS = SPAWN;
        return REAL_BOARD.CanPlace(PIECE_POS, PIECE_CUR);
    }

    private void RotatePiece() {
        var r = PIECE_CUR.Rotate();
        if (REAL_BOARD.CanPlace(PIECE_POS, r)) PIECE_CUR = r;
    }

    private bool MovePiece(Vector2I mov) {
        if (!REAL_BOARD.CanPlace(PIECE_POS + mov, PIECE_CUR)) return false;
        PIECE_POS += mov;
        return true;
    }

    private void BreakBlocks() {
        var broken = REAL_BOARD.BreakLines();
        SCORE += 25 * broken * broken;
        GD.Print(SCORE);
    }

    private void GameOver() {
        GD.Print("Game Over!");
        SetProcessInput(false);
        SetProcess(false);
    }

    private void CreateBoard(Vector2I size) {
        X_SIZE = size.X;
        Y_SIZE = size.Y;
        SPAWN = new Vector2I(X_SIZE / 2, 0);
        REAL_BOARD = new PieceBoard(size);
        GenerateNewPiece();

        for (var y = 0; y < Y_SIZE; y++) {
            SetCell(2, new Vector2I(-1, y), 0, PIECE_BLACK);
            SetCell(2, new Vector2I(X_SIZE, y), 0, PIECE_BLACK);
        }

        for (var x = -1; x < X_SIZE + 1; x++) SetCell(2, new Vector2I(x, Y_SIZE), 0, PIECE_BLACK);
    }

    private void Tick(double delta) {
        TICK_STEP += delta;
        if (!(TICK_STEP >= TICK_RATE)) return;
        TICK_STEP -= TICK_RATE;

        if (MovePiece(Vector2I.Down)) return;

        REAL_BOARD.Place(PIECE_POS, PIECE_CUR);
        BreakBlocks();
        if (!GenerateNewPiece()) GameOver();
    }

    private void Render() {
        ClearLayer(1);
        foreach (var cell in PIECE_CUR.Cells) SetCell(1, cell + PIECE_POS, 0, PIECE_CUR.Value);

        ClearLayer(0);
        for (var y = 0; y < REAL_BOARD.height; y++)
        for (var x = 0; x < REAL_BOARD.width; x++) {
            var cell = REAL_BOARD.board[y][x];
            if (PieceBoard.IsEmptyCell(cell)) continue;
            SetCell(0, new Vector2I(x, y), 0, cell);
        }
    }

    private void ProcessInput() {
        if (Input.IsActionJustPressed("ui_up")) RotatePiece();
        if (Input.IsActionJustPressed("ui_right")) MovePiece(Vector2I.Right);
        if (Input.IsActionJustPressed("ui_left")) MovePiece(Vector2I.Left);
        if (Input.IsActionPressed("ui_down")) Tick(Math.Max(TICK_RATE - TICK_STEP, 0));
    }

    public override void _Ready() {
        SQUARE_UNIT = CellQuadrantSize;
        CreateBoard(new Vector2I(10, 20));
        Render();
    }

    public override void _Process(double delta) {
        var window = GetViewportRect().Size;
        window.X = (window.X - X_SIZE * SQUARE_UNIT * Scale.X) / 2;
        window.Y -= (Y_SIZE + 1) * SQUARE_UNIT * Scale.Y;
        Position = window;
        ProcessInput();
        Tick(delta);
        Render();
    }
}
