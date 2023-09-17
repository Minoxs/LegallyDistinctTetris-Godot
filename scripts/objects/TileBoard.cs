using System;
using Godot;

namespace Tetris.scripts.objects;

public partial class TileBoard : TileMap {
    // Black piece used in the background layer
    private static readonly Vector2I PieceBlack = Vector2I.Right;

    // Current piece being moved
    private Piece _pieceCur;

    // Underlying game board
    private PieceBoard _realBoard;

    // Current score
    private int _score;

    // Piece spawn locatiom
    private Vector2I _spawn = Vector2I.Zero;

    // Cell size
    private int _squareUnit;

    // How far along a tick game is currently
    private double _tickStep;

    // How many ticks touching the floor before the piece is placed
    [Export(PropertyHint.Range, "0,10,1,or_greater")]
    public int FloorTicks = 3;

    // Points given for a single line cleared
    [Export(PropertyHint.Range, "5,100,5,or_greater")]
    public int ScoreMultiplier = 25;

    // Size of the board
    [Export]
    public Vector2I Size = new(10, 20);

    // Amount of seconds for a single game tick
    [Export(PropertyHint.Range, "0,1,0.05,or_greater")]
    public double TickRate = 0.25f;

    private bool GenerateNewPiece() {
        var dice = GD.RandRange(0, TileSet.GetPatternsCount() - 1);
        var patte = TileSet.GetPattern(dice);

        _pieceCur = new Piece(patte, _spawn);
        return _realBoard.CanPlace(_pieceCur);
    }

    private void RotatePiece() {
        var r = _pieceCur.Rotate();
        if (_realBoard.CanPlace(r)) _pieceCur = r;
    }

    private bool MovePiece(Vector2I mov) {
        if (!_realBoard.CanPlace(_pieceCur + mov)) return false;
        _pieceCur.Offset(mov);
        return true;
    }

    private void BreakBlocks() {
        var broken = _realBoard.BreakLines();
        _score += ScoreMultiplier * broken * broken;
        GD.Print(_score);
    }

    private void GameOver() {
        GD.Print("Game Over!");
        SetProcessInput(false);
        SetProcess(false);
    }

    private void CreateBoard() {
        _spawn = new Vector2I(Size.X / 2, 0);
        _realBoard = new PieceBoard(Size);
        GenerateNewPiece();

        for (var y = 0; y < Size.Y; y++) {
            SetCell(2, new Vector2I(-1, y), 0, PieceBlack);
            SetCell(2, new Vector2I(Size.X, y), 0, PieceBlack);
        }

        for (var x = -1; x < Size.X + 1; x++) SetCell(2, new Vector2I(x, Size.Y), 0, PieceBlack);
    }

    private void Tick(double delta) {
        _tickStep += delta;
        if (_tickStep < TickRate) return;
        _tickStep -= TickRate;

        if (MovePiece(Vector2I.Down) || FloorTicks++ < 3) return;
        FloorTicks = 0;

        _realBoard.Place(_pieceCur);
        BreakBlocks();
        if (!GenerateNewPiece()) GameOver();
    }

    private void Render() {
        ClearLayer(1);
        foreach (var cell in _pieceCur.CellsPosition) SetCell(1, cell, 0, _pieceCur.Value);

        ClearLayer(0);
        for (var y = 0; y < _realBoard.Height; y++)
        for (var x = 0; x < _realBoard.Width; x++) {
            var cell = _realBoard.Board[y][x];
            if (PieceBoard.IsEmptyCell(cell)) continue;
            SetCell(0, new Vector2I(x, y), 0, cell);
        }
    }

    private void ProcessInput() {
        if (Input.IsActionJustPressed("ui_up")) RotatePiece();
        if (Input.IsActionJustPressed("ui_right")) MovePiece(Vector2I.Right);
        if (Input.IsActionJustPressed("ui_left")) MovePiece(Vector2I.Left);
        if (Input.IsActionPressed("ui_down")) Tick(Math.Max(TickRate - _tickStep, 0));
    }

    public override void _Ready() {
        _squareUnit = CellQuadrantSize;
        CreateBoard();
        Render();
    }

    public override void _Process(double delta) {
        var window = GetViewportRect().Size;
        window.X = (window.X - Size.X * _squareUnit * Scale.X) / 2;
        window.Y -= (Size.Y + 1) * _squareUnit * Scale.Y;
        Position = window;
        ProcessInput();
        Tick(delta);
        Render();
    }
}
