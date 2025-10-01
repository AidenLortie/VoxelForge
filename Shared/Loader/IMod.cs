namespace VoxelForge.Shared;

public interface IMod
{
    string ModId { get; }
    string Name { get; }
    string Version { get; }
    string Author { get; }
    string Description { get; }
    
    void OnInitialize(); // Called when the mod is first loaded
    void OnLoad(); // Called when the game is loading
    void OnUnload(); // Called when the game is unloading
}