using Godot;
using LegallyDistinctTetris.scripts.board;

namespace LegallyDistinctTetris.scripts.editor;

[Tool]
[GlobalClass]
public partial class TileBoardBuilder : TileBoard {
    // Update board in the editor
    [Export]
    public bool UpdateBoardInEditor;

    // Black piece used in the background layer
    [Export]
    public Vector2I BgBlockAtlasCoord = Vector2I.Right;

    // Size required to render board
    // 2 for walls + Board width
    // 1 for floor + Board heigth
    public Vector2I CanvasSize => new Vector2I(2 + BoardSize.X, 1 + BoardSize.Y) * CellQuadrantSize * (Vector2I)Scale;

    private void UpdateBoardVisuals() {
        Position = new Vector2(CellQuadrantSize * Scale.X, 0) - CanvasSize / 2;

        // Make sure there are 3 layers in the thing
        // 0 - Background layer
        // 1 - Fixed blocks layer
        // 2 - Moving piece layer
        for (var i = GetLayersCount(); i < 3; i++) AddLayer(i);

        Clear();
        // Create walls
        for (var y = 0; y < BoardSize.Y; y++) {
            SetCell(0, new Vector2I(-1, y), 0, BgBlockAtlasCoord);
            SetCell(0, new Vector2I(BoardSize.X, y), 0, BgBlockAtlasCoord);
        }

        // Create floor
        for (var x = -1; x < BoardSize.X + 1; x++) SetCell(0, new Vector2I(x, BoardSize.Y), 0, BgBlockAtlasCoord);

        // Offset by one cell to start board in (0, 0), then move center point to (0, 0)
        Position = new Vector2I(CellQuadrantSize, 0) - CanvasSize / 2;
    }

    public override void _Ready() {
        if (!Engine.IsEditorHint()) base._Ready();
    }

    public override void _Process(double delta) {
        if (UpdateBoardInEditor) UpdateBoardVisuals();
        if (!Engine.IsEditorHint()) base._Process(delta);
    }
}
