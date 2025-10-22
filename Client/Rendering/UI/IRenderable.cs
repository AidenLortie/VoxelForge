namespace VoxelForge.Client.UI;

public interface IRenderable : IDisposable
{
    public void Render();
    public void Update(double deltaTime);
}