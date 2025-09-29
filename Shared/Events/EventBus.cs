namespace VoxelForge.Shared.Events;

/// <summary>
///     A simple event bus for publishing and subscribing to game events.
/// </summary>
public class EventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    /// <summary>
    ///     Subscribe handler to event of type T
    /// </summary>
    /// <param name="handler">Event Handler to be registered</param>
    /// <typeparam name="T">Event Type</typeparam>
    public void Subscribe<T>(Action<T> handler) where T : IGameEvent
    {
        if (!_handlers.TryGetValue(typeof(T), out var list))
        {
            list = new List<Delegate>();
            _handlers[typeof(T)] = list;
        }
        list.Add(handler);
    }

    /// <summary>
    ///     Publish event to all subscribed handlers
    /// </summary>
    /// <param name="ev">Event</param>
    /// <typeparam name="T">Event Type</typeparam>
    public void Publish<T>(T ev) where T : IGameEvent
    {
        if (_handlers.TryGetValue(typeof(T), out var list))
        {
            foreach (var handler in list)
            {
                ((Action<T>)handler)(ev);
            }
        }
    }
}