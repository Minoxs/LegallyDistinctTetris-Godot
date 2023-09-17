using Godot;

namespace Tetris.scripts.interfaces;

public interface ICenterable {
    public Vector2I CanvasSize { get; }
}
