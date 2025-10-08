using System.Reflection;

namespace VoxelForge.Shared;

/// <summary>
/// Manages the loading, registration, and lifecycle of mods in VoxelForge.
/// Provides functionality to dynamically load mod DLLs and manage their lifecycle.
/// </summary>
public static class ModLoader
{
    /// <summary>
    /// Gets the dictionary of all registered mods, keyed by mod name.
    /// </summary>
    public static Dictionary<string, IMod> Mods { get; } = new();
    
    /// <summary>
    /// Registers a mod instance with the mod loader.
    /// </summary>
    /// <param name="mod">The mod instance to register.</param>
    public static void RegisterMod(IMod mod)
    {
        Mods[mod.Name] = mod;
    }
    
    /// <summary>
    /// Gets a mod by its name.
    /// </summary>
    /// <param name="name">The name of the mod to retrieve.</param>
    /// <returns>The mod instance if found, otherwise null.</returns>
    public static IMod? GetMod(string name)
    {
        Mods.TryGetValue(name, out var mod);
        return mod;
    }
    
    /// <summary>
    /// Gets all registered mods.
    /// </summary>
    /// <returns>An enumerable collection of all registered mods.</returns>
    public static IEnumerable<IMod> GetAllMods() => Mods.Values;
    
    /// <summary>
    /// Unloads all registered mods by calling their OnUnload method and clearing the registry.
    /// </summary>
    public static void ClearMods() {
        foreach(IMod mod in Mods.Values)
        {
            mod.OnUnload();
        }
    }
    
    /// <summary>
    /// Loads all registered mods by calling their OnLoad method.
    /// This should be called after all mods have been registered.
    /// </summary>
    public static void loadMods()
    {
        foreach(IMod mod in Mods.Values)
        {
            mod.OnLoad();
        }
    }

    /// <summary>
    /// Dynamically loads a mod from a DLL file. The DLL must contain classes implementing IMod.
    /// All IMod implementations found in the assembly will be instantiated and registered.
    /// </summary>
    /// <param name="modDllPath">The file path to the mod DLL.</param>
    /// <exception cref="FileNotFoundException">Thrown when the specified DLL file does not exist.</exception>
    public static void Load(string modDllPath)
    {
        if (!File.Exists(modDllPath))
        {
            throw new FileNotFoundException($"Mod DLL not found: {modDllPath}");
        }

        var assembly = Assembly.LoadFrom(modDllPath);
        var modTypes = assembly.GetTypes().Where(t => typeof(IMod).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var modType in modTypes)
        {
            if (Activator.CreateInstance(modType) is IMod modInstance)
            {
                RegisterMod(modInstance);
                modInstance.OnInitialize();
            }
        }
    }
    
    
    

}