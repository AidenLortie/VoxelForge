using System;
using System.Collections.Generic;
using VoxelForge.Shared.Content.Blocks;
using Xunit;

namespace Shared.Test.Content.Blocks;

public class TestBlock : Block
{
    public TestBlock(string id) : base(id)
    {
    }
}

public class TestBlockWithProperties : Block
{
    // Define block properties
    public static readonly BlockProperty<bool> TestBoolVal =
        new BlockProperty<bool>("TestBoolVal", [true, false]);

    public static readonly BlockProperty<string> TestStringVal =
        new BlockProperty<string>("TestStringVal", ["red", "green", "blue"]);

    public static readonly BlockProperty<int> TestIntVal =
        new BlockProperty<int>("TestIntVal", [0, 1, 2]);

    public TestBlockWithProperties(string id) : base(id) { }

    public override BlockState DefaultState()
    {
        return new BlockState(this, new Dictionary<string, object>
        {
            { TestBoolVal.Name, false },
            { TestStringVal.Name, "red" },
            { TestIntVal.Name, 4 }
        });
    }
}


public class BlockStateTest
{
    [Fact]
    public void DefaultState_ShouldReturnBlockStateWithCorrectBlock()
    {
        // Arrange
        var testBlock = new TestBlock("test_block");

        // Act
        var defaultState = testBlock.DefaultState();

        // Assert
        Assert.NotNull(defaultState);
        Assert.Equal(testBlock, defaultState.Block);
    }

    [Fact]
    public void BlockState_ShouldStorePropertiesCorrectly()
    {
        // Arrange
        var testBlock = new TestBlockWithProperties("test_block_with_props");
        var defaultState = testBlock.DefaultState();

        // Act & Assert
        Assert.True(defaultState.HasProperty("TestBoolVal"));
        Assert.True(defaultState.HasProperty("TestStringVal"));
        Assert.True(defaultState.HasProperty("TestIntVal"));

        Assert.False(defaultState.Get<bool>("TestBoolVal"));
        Assert.Equal("red", defaultState.Get<string>("TestStringVal"));
        Assert.Equal(4, defaultState.Get<int>("TestIntVal"));
    }

    [Fact]
    public void BlockState_Get_ShouldThrowForInvalidProperty()
    {
        // Arrange
        var testBlock = new TestBlockWithProperties("test_block_with_props");
        var defaultState = testBlock.DefaultState();

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => defaultState.Get<int>("NonExistentProp"));
        Assert.Throws<InvalidOperationException>(() => defaultState.Get<int>("TestStringVal"));
    }

    [Fact]
    public void BlockState_With_ShouldReturnNewStateWithUpdatedProperty()
    {
        // Arrange
        var testBlock = new TestBlockWithProperties("test_block_with_props");
        var defaultState = testBlock.DefaultState();
        var newState = defaultState.With("TestBoolVal", true);
        var anotherState = newState.With("TestStringVal", "blue");
        var yetAnotherState = anotherState.With("TestIntVal", 2);

        // Act & Assert
        Assert.True(newState.Get<bool>("TestBoolVal"));
        Assert.Equal("red", newState.Get<string>("TestStringVal")); // unchanged
        Assert.Equal(4, newState.Get<int>("TestIntVal")); // unchanged  
        Assert.Equal("blue", anotherState.Get<string>("TestStringVal"));
        Assert.Equal(2, yetAnotherState.Get<int>("TestIntVal"));
    }

    [Fact]
    public void BlockState_ToString_ShouldReturnCorrectFormat()
    {
        // Arrange
        var testBlock = new TestBlockWithProperties("test_block_with_props");
        var defaultState = testBlock.DefaultState();
        var modifiedState = defaultState.With("TestBoolVal", true).With("TestStringVal", "green");
        // Act
        var defaultStateStr = defaultState.ToString();
        var modifiedStateStr = modifiedState.ToString();
        // Assert
        Assert.Equal("test_block_with_props[TestBoolVal=False, TestStringVal=red, TestIntVal=4]", defaultStateStr);
        Assert.Equal("test_block_with_props[TestBoolVal=True, TestStringVal=green, TestIntVal=4]", modifiedStateStr);
    }

}