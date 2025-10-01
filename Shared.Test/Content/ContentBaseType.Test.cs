using VoxelForge.Shared.Content;
using VoxelForge.Shared.Registry;
using Xunit;

namespace Shared.Test.Content;



public class TestItem : Item
{
    public TestItem(string id) : base(id) { }
}

public class TestEntity : Entity
{
    public TestEntity(string id) : base(id) { }
}

public class ContentBaseTypeTest
{

    
    [Fact]
    public void Item_ShouldBeAbstract()
    {
        // Assert
        Assert.True(typeof(Item).IsAbstract);
    }

    [Fact]
    public void Entity_ShouldBeAbstract()
    {
        // Assert
        Assert.True(typeof(Entity).IsAbstract);
    }

    // Test to ensure that subclasses have Id property and it works correctly
    [Fact]
    public void SubClassHasIdProperty()
    {
        // Arrange
        var testItem = new TestItem("test_item");
        var testEntity = new TestEntity("test_entity");

        // Act & Assert
        Assert.Equal("test_item", testItem.Id);
        Assert.Equal("test_entity", testEntity.Id);
    }

    // Test to ensure that subclasses can be registered and retrieved from Registry<T>
    [Fact]
    public void SubClassWorksInRegistry()
    {
        var itemRegistry = new Registry<Item>();
        var entityRegistry = new Registry<Entity>();
        
        var testItem = new TestItem("test_item");
        var testEntity = new TestEntity("test_entity");
        
        itemRegistry.Register(testItem.Id, testItem);
        entityRegistry.Register(testEntity.Id, testEntity);
        
        Assert.Equal(testItem, itemRegistry.Get("test_item"));
        Assert.Equal(testEntity, entityRegistry.Get("test_entity"));
    }
}