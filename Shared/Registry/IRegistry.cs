namespace VoxelForge.Shared.Registry;


/// <summary>
///     Interface for a registry that can store and retrieve entries by a string identifier.
/// </summary>
/// <typeparam name="T">Type of data being held in the registry</typeparam>
public interface IRegistry<T>
{
    /// <summary>
    ///     Register an entry with a specific string identifier.
    /// </summary>
    /// <param name="id">identifier for entry</param>
    /// <param name="entry">entry</param>
    void Register(string id, T entry);
    
    /// <summary>
    ///     Return an entry by it's identifier.
    /// </summary>
    /// <param name="id">identifier for entry</param>
    /// <returns>entry or null if not found</returns>
    T? Get(string id);
    
    /// <summary>
    ///     Return all entries in the registry.
    /// </summary>
    /// <returns>all entries</returns>
    IEnumerable<T> GetAll();
}