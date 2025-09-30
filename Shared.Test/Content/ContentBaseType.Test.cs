using VoxelForge.Shared.Content;
using VoxelForge.Shared.Registry;
using Xunit;

namespace Shared.Test.Content;

// Test classes to instantiate abstract classes
public class TestBlock : Block
{
    public TestBlock(string id) : base(id) { }
}

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
    // Test to ensure that Block, Item, and Entity are abstract classes
    [Fact]
    public void Block_ShouldBeAbstract()
    {
        // Assert
        Assert.True(typeof(Block).IsAbstract);
    }
    
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
        var testBlock = new TestBlock("test_block");
        var testItem = new TestItem("test_item");
        var testEntity = new TestEntity("test_entity");

        // Act & Assert
        Assert.Equal("test_block", testBlock.Id);
        Assert.Equal("test_item", testItem.Id);
        Assert.Equal("test_entity", testEntity.Id);
    }

    // Test to ensure that subclasses can be registered and retrieved from Registry<T>
    [Fact]
    public void SubClassWorksInRegistry()
    {
        var blockRegistry = new Registry<Block>();
        var itemRegistry = new Registry<Item>();
        var entityRegistry = new Registry<Entity>();
        
        var testBlock = new TestBlock("test_block");
        var testItem = new TestItem("test_item");
        var testEntity = new TestEntity("test_entity");
        
        blockRegistry.Register(testBlock.Id, testBlock);
        itemRegistry.Register(testItem.Id, testItem);
        entityRegistry.Register(testEntity.Id, testEntity);
        
        Assert.Equal(testBlock, blockRegistry.Get("test_block"));
        Assert.Equal(testItem, itemRegistry.Get("test_item"));
        Assert.Equal(testEntity, entityRegistry.Get("test_entity"));
    }
}