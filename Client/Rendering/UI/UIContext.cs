using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace VoxelForge.Client.UI;

public abstract class UiContext : IDisposable, IRenderable
{
    private UiStateMachine _parentStateMachine;

    protected UiContext(UiStateMachine parentStateMachine)
    {
        _parentStateMachine = parentStateMachine;
    }
    public virtual void HandleEvents(PalHandle? handle, PlatformEventType type, EventArgs args) { } // Each UI Context will implement its own event handler if needed.
    public abstract void Update(double deltaTime);
    public abstract void Render();
    public virtual void Dispose() { }
}