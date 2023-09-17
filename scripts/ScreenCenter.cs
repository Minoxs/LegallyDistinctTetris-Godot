using Godot;
using Tetris.scripts.interfaces;

namespace Tetris.scripts;

public partial class ScreenCenter : Node2D {
    [Export]
    public Node2D NodeToCenter;

    private ICenterable _nodeToCenter;

    public override void _Ready() {
        if (NodeToCenter is not ICenterable) GD.PrintErr("Node isn't ICenterable");
        _nodeToCenter = NodeToCenter as ICenterable;
    }

    public override void _Process(double delta) {
        if (_nodeToCenter == null) return;

        var window = GetViewportRect().Size;
        NodeToCenter.Position = new Vector2(
            (window.X - _nodeToCenter.CanvasSize.X) / 2,
            window.Y - _nodeToCenter.CanvasSize.Y
        );
    }
}
