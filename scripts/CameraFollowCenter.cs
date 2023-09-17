using Godot;
using LegallyDistinctTetris.scripts.interfaces;

namespace LegallyDistinctTetris.scripts;

public partial class CameraFollowCenter : Camera2D {
    [Export]
    public Node2D NodeToCenter;

    private ICenterable _nodeToCenter;

    public override void _Ready() {
        if (NodeToCenter is not ICenterable) GD.PrintErr("Node isn't ICenterable");
        _nodeToCenter = NodeToCenter as ICenterable;
    }

    public override void _Process(double delta) {
        if (_nodeToCenter == null) return;
        Offset = _nodeToCenter.CanvasSize / 2;
    }
}