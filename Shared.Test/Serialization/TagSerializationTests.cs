using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VoxelForge.Shared.Serialization;
using VoxelForge.Shared.Serialization.Tags;
using Xunit;

namespace Shared.Test.Serialization;

public class TagSerializationTests
{
    private static T RoundTrip<T>(T tag) where T : Tag
    {
        using MemoryStream ms = new();
        using (BinaryWriter writer = new(ms, Encoding.UTF8, true))
            tag.Write(writer);

        ms.Position = 0;
        using BinaryReader reader = new(ms);
        T copy = (T)TagFactory.Create(tag.Type);
        copy.Read(reader);
        return copy;
    }

    [Fact]
    public void TagByte_SerializesAndDeserializes()
    {
        var tag = new TagByte(42, "testByte");
        var copy = RoundTrip(tag);

        Assert.Equal(tag.Type, copy.Type);
        Assert.Equal(tag.Value, copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagByteArray_SerializesAndDeserializes()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var tag = new TagByteArray(data, "testArray");
        var copy = RoundTrip(tag);

        Assert.Equal(tag.Type, copy.Type);
        Assert.Equal(data, copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagList_SerializesAndDeserializes()
    {
        var list = new TagList("numbers", TagType.Byte,
            new TagByte(10),
            new TagByte(20),
            new TagByte(30));

        var copy = RoundTrip(list);
        var items = copy.Cast<TagByte>().ToArray();

        Assert.Equal(TagType.List, copy.Type);
        Assert.Equal(3, items.Length);
        Assert.Equal(10, items[0].Value);
        Assert.Equal(30, items[2].Value);
    }

    [Fact]
    public void TagList_RejectsWrongElementType()
    {
        var list = new TagList("bytes", TagType.Byte);
        Assert.Throws<InvalidOperationException>(() => list.Add(new TagByteArray(new byte[0])));
    }

    [Fact]
    public void TagCompound_SerializesAndDeserializes()
    {
        var compound = new TagCompound("root");
        compound.Add("byte", new TagByte(123));
        compound.Add("array", new TagByteArray(new byte[] { 9, 8, 7 }));
        compound.Add("list", new TagList("numbers", TagType.Byte,
            new TagByte(1), new TagByte(2), new TagByte(3)));

        var copy = RoundTrip(compound);

        Assert.Equal(TagType.Compound, copy.Type);
        Assert.Equal(3, copy.Count());

        var byteTag = (TagByte)((TagCompound)copy)["byte"];
        Assert.Equal(123, byteTag.Value);

        var listTag = (TagList)((TagCompound)copy)["list"];
        var listItems = listTag.Cast<TagByte>().Select(t => t.Value).ToArray();
        Assert.Equal(new byte[] { 1, 2, 3 }, listItems);
    }

    [Fact]
    public void TagShort_SerializesAndDeserializes()
    {
        var tag = new TagShort(12345, "testShort");
        var copy = RoundTrip(tag);
        Assert.Equal(tag.Type, copy.Type);
        Assert.Equal(tag.Value, copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagInt_SerializesAndDeserializes()
    {
        var tag = new TagInt(123456789, "testInt");
        var copy = RoundTrip(tag);
        Assert.Equal(tag.Type, copy.Type);
        Assert.Equal(tag.Value, copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagLong_SerializesAndDeserializes()
    {
        var tag = new TagLong(1234567890123456789L, "testLong");
        var copy = RoundTrip(tag);
        Assert.Equal(tag.Type, copy.Type);
        Assert.Equal(tag.Value, copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagFloat_SerializesAndDeserializes()
    {
        var tag = new TagFloat(3.14159f, "testFloat");
        var copy = RoundTrip(tag);
        Assert.Equal(tag.Type, copy.Type);
        Assert.Equal(tag.Value, copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagDouble_SerializesAndDeserializes()
    {
        var tag = new TagDouble(2.718281828459, "testDouble");
        var copy = RoundTrip(tag);
        Assert.Equal(tag.Type, copy.Type);
        Assert.Equal(tag.Value, copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagString_SerializesAndDeserializes()
    {
        var tag = new TagString("Hello, world!", "testString");
        var copy = RoundTrip(tag);
        Assert.Equal(tag.Type, copy.Type);
        Assert.Equal(tag.Value, copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagIntArray_SerializesAndDeserializes()
    {
        var data = new int[] { 10, 20, 30, 40 };
        var tag = new TagIntArray(data, "testIntArray");
        var copy = RoundTrip(tag);
        Assert.Equal(tag.Type, copy.Type);
        Assert.Equal(data, copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagLongArray_SerializesAndDeserializes()
    {
        var data = new long[] { 100L, 200L, 300L };
        var tag = new TagLongArray(data, "testLongArray");
        var copy = RoundTrip(tag);
        Assert.Equal(tag.Type, copy.Type);
        Assert.Equal(data, copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagString_EmptyString_SerializesAndDeserializes()
    {
        var tag = new TagString(string.Empty, "emptyString");
        var copy = RoundTrip(tag);
        Assert.Equal(tag.Value, copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagByteArray_EmptyArray_SerializesAndDeserializes()
    {
        var tag = new TagByteArray(Array.Empty<byte>(), "emptyArray");
        var copy = RoundTrip(tag);
        Assert.Empty(copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagIntArray_EmptyArray_SerializesAndDeserializes()
    {
        var tag = new TagIntArray(Array.Empty<int>(), "emptyIntArray");
        var copy = RoundTrip(tag);
        Assert.Empty(copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagLongArray_EmptyArray_SerializesAndDeserializes()
    {
        var tag = new TagLongArray(Array.Empty<long>(), "emptyLongArray");
        var copy = RoundTrip(tag);
        Assert.Empty(copy.Value);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagList_EmptyList_SerializesAndDeserializes()
    {
        var tag = new TagList("emptyList", TagType.Int);
        var copy = RoundTrip(tag);
        Assert.Empty(copy);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagCompound_EmptyCompound_SerializesAndDeserializes()
    {
        var tag = new TagCompound("emptyCompound");
        var copy = RoundTrip(tag);
        Assert.Empty(copy);
        Assert.Equal(tag.Name, copy.Name);
    }

    [Fact]
    public void TagList_NestedLists_SerializesAndDeserializes()
    {
        var inner = new TagList("inner", TagType.Byte, new TagByte(1), new TagByte(2));
        var outer = new TagList("outer", TagType.List, inner);
        var copy = RoundTrip(outer);
        Assert.Equal(TagType.List, copy.Type);
        Assert.Equal(outer.Name, copy.Name);
        var nested = copy.Cast<TagList>().First();
        Assert.Equal(inner.Name, nested.Name);
        Assert.Equal(2, nested.Count());
    }

    [Fact]
    public void TagCompound_NestedCompounds_SerializesAndDeserializes()
    {
        var root = new TagCompound("root");
        var child = new TagCompound("child");
        child.Add("flag", new TagByte(1));
        root.Add("child", child);
        var copy = RoundTrip(root);
        Assert.Equal(root.Name, copy.Name);
        var restoredChild = (TagCompound)((TagCompound)copy)["child"];
        Assert.Equal(child.Name, restoredChild.Name);
        var flag = (TagByte)restoredChild["flag"];
        Assert.Equal(1, flag.Value);
    }

    [Fact]
    public void TagList_ThrowsOnWrongElementType()
    {
        var list = new TagList("ints", TagType.Int);
        Assert.Throws<InvalidOperationException>(() => list.Add(new TagByte(1)));
    }

    [Fact]
    public void TagCompound_ThrowsOnMissingKey()
    {
        var compound = new TagCompound("root");
        Assert.Throws<KeyNotFoundException>(() => { var _ = compound["missing"]; });
    }
    
    [Fact]
    public void DeeplyNested_Structure_RoundTripsCorrectly()
    {
        var deep = new TagCompound("root");
        var innerList = new TagList("list", TagType.Compound);
        for (int i = 0; i < 3; i++)
        {
            var compound = new TagCompound($"entry{i}");
            compound.Add("id", new TagInt(i));
            compound.Add("name", new TagString($"Item {i}"));
            innerList.Add(compound);
        }
        deep.Add("items", innerList);

        var copy = RoundTrip(deep);
        var copiedList = (TagList)((TagCompound)copy)["items"];
        var entries = copiedList.Cast<TagCompound>().ToArray();

        Assert.Equal(3, entries.Length);
        Assert.Equal("Item 2", ((TagString)entries[2]["name"]).Value);
    }
    
    [Fact]
    public void TagCompound_StopsAtEndTag()
    {
        var compound = new TagCompound("root");
        compound.Add("byte", new TagByte(1));
        var copy = RoundTrip(compound);
        Assert.Single(copy);
    }


}