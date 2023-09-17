using Godot;

namespace LegallyDistinctTetris.scripts.interfaces;

public interface ICenterable {
    public Vector2I CanvasSize { get; }
}