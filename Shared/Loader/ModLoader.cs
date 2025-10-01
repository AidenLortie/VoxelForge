using System.Reflection;

namespace VoxelForge.Shared;

public static class ModLoader
{
    public static Dictionary<string, IMod> Mods { get; } = new();
    
    public static void RegisterMod(IMod mod)
    {
        Mods[mod.Name] = mod;
    }
    
    public static IMod? GetMod(string name)
    {
        Mods.TryGetValue(name, out var mod);
        return mod;
    }
    
    public static IEnumerable<IMod> GetAllMods() => Mods.Values;
    
    public static void ClearMods() {
        foreach(IMod mod in Mods.Values)
        {
            mod.OnUnload();
        }
    }
    
    public static void loadMods()
    {
        foreach(IMod mod in Mods.Values)
        {
            mod.OnLoad();
        }
    }

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