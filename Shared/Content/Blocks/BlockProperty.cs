namespace VoxelForge.Shared.Content.Blocks;

public class BlockProperty<T>
{
    public string Name { get; }
    public IReadOnlyCollection<T> AllowedValues { get; }

    public BlockProperty(string name, IEnumerable<T> allowedValues)
    {
        Name = name;
        AllowedValues = allowedValues.ToList().AsReadOnly();
    }

    public bool IsValid(T value) => AllowedValues.Contains(value);

    public override string ToString() => Name;
}