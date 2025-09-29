namespace VoxelForge.Shared.Lifecycle;

public interface IGameHandler
{
    void OnInitialize();
    void OnLoad();
    void OnUnload();
}