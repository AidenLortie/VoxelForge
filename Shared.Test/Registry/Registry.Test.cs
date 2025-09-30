using System.Linq;
using VoxelForge.Shared.Registry;
using Xunit;

namespace Shared.Test.Registry;

class TestObject
{
    public string Name { get; set; }
    public int Value { get; set; }

    public TestObject(string name, int value)
    {
        Name = name;
        Value = value;
    }
}

public class RegistryTest
{
    
    
    // Test registering and retrieving objects
    [Fact]
    public void RegisterAndRetrieve()
    {
        var objRegistry = new Registry<TestObject>();
        
        var obj1 = new TestObject("Object1", 10);
        var obj2 = new TestObject("Object2", 20);
        
        objRegistry.Register("obj1", obj1);
        objRegistry.Register("obj2", obj2);
        
        var retrievedObj1 = objRegistry.Get("obj1");
        var retrievedObj2 = objRegistry.Get("obj2");
        
        Assert.NotNull(retrievedObj1);
        Assert.NotNull(retrievedObj2);
        
        Assert.Equal(obj1, retrievedObj1);
        Assert.Equal(obj2, retrievedObj2);
    }

    // Test retrieving a non-existent object
    [Fact]
    public void RetrieveNonExistent()
    {
        var objRegistry = new Registry<TestObject>();
        var retrievedObj = objRegistry.Get("nonexistent");
        Assert.Null(retrievedObj);
    }

    // Test duplicate registration
    [Fact]
    public void DuplicateRegistration()
    {
        var objRegistry = new Registry<TestObject>();
        
        var obj1 = new TestObject("Object1", 10);
        var obj2 = new TestObject("Object2", 20);
        
        objRegistry.Register("obj", obj1);
        objRegistry.Register("obj", obj2); // Duplicate registration
        
        var retrievedObj = objRegistry.Get("obj");
        
        Assert.NotNull(retrievedObj);
        Assert.NotEqual(obj1, retrievedObj); // Should not retrieve the first object
        Assert.Equal(obj2, retrievedObj); // Should retrieve the second object
    }

    // Test retrieving all entries
    [Fact]
    public void GetAllEntries()
    {
        var objRegistry = new Registry<TestObject>();
        var obj1 = new TestObject("Object1", 10);
        var obj2 = new TestObject("Object2", 20);
        objRegistry.Register("obj1", obj1);
        objRegistry.Register("obj2", obj2);
        var allEntries = objRegistry.GetAll().ToArray();
        Assert.Contains(obj1, allEntries);
        Assert.Contains(obj2, allEntries);
    }

}