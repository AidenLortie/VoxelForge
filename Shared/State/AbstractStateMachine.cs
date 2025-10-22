namespace VoxelForge.Shared.State;

public class AbstractStateMachine<T>
{
    private T _state;
    
    public T State => _state;
    
    public void SetState(T state)
    {
        _state = state;
    }
}