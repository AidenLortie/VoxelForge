namespace VoxelForge.Shared;

/// <summary>
/// Interface for creating mods that can be dynamically loaded into VoxelForge.
/// Implement this interface to create custom mods with game logic, content, and behaviors.
/// </summary>
public interface IMod
{
    /// <summary>
    /// Gets the unique identifier for this mod. Should be lowercase with underscores (e.g., "my_mod").
    /// </summary>
    string ModId { get; }
    
    /// <summary>
    /// Gets the human-readable name of this mod.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the version string of this mod (e.g., "1.0.0").
    /// </summary>
    string Version { get; }
    
    /// <summary>
    /// Gets the author name or names for this mod.
    /// </summary>
    string Author { get; }
    
    /// <summary>
    /// Gets a brief description of what this mod does.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Called when the mod is first loaded. Use this to register content like blocks, items, or event handlers.
    /// </summary>
    void OnInitialize();
    
    /// <summary>
    /// Called when the game is loading. Use this for initialization that requires other mods to be loaded.
    /// </summary>
    void OnLoad();
    
    /// <summary>
    /// Called when the game is unloading. Use this to clean up resources and save state.
    /// </summary>
    void OnUnload();
}