namespace VoxelForge.Client.Rendering;

/// <summary>
/// Base interface for all rendering components.
/// </summary>
public interface IRenderer
{
    // /// Initialize the renderer. Called once when the renderer is created.
    void Initialize();
    
    // /// Render a frame. Called every frame.
    void Render(double deltaTime);
    
    // /// Clean up resources. Called when the renderer is disposed.
    void Dispose();
}
