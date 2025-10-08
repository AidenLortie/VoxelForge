namespace VoxelForge.Client.Rendering;

/// <summary>
/// Base interface for all rendering components.
/// </summary>
public interface IRenderer
{
    /// <summary>
    /// Initialize the renderer. Called once when the renderer is created.
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// Render a frame. Called every frame.
    /// </summary>
    /// <param name="deltaTime">Time since last frame in seconds.</param>
    void Render(double deltaTime);
    
    /// <summary>
    /// Clean up resources. Called when the renderer is disposed.
    /// </summary>
    void Dispose();
}
