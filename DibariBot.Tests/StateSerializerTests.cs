using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Diagnostics;
using Xunit.Abstractions;

namespace DibariBot.Tests;

public class TestObject
{
    public int id;
    public InnerTestObject? inner;
    public string name = "";

    public TestObject()
    { }

    public TestObject(int id, string name, InnerTestObject inner)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        this.id = id;
        this.name = name;
        this.inner = inner;
    }

    public static TestObject CreateValid()
    {
        return new(1, "Test Object", InnerTestObject.CreateValid());
    }
}

public struct InnerTestObject
{
    public TestEnum testEnum = TestEnum.InvalidValue;
    public string str = "invalid";
    public int number = -1;

    public InnerTestObject()
    {
    }

    public InnerTestObject(TestEnum testEnum, string str, int number)
    {
        this.testEnum = testEnum;
        this.str = str ?? throw new ArgumentNullException(nameof(str));
        this.number = number;
    }

    public static InnerTestObject CreateValid()
    {
        return new InnerTestObject(TestEnum.ValidValue, "valid", 69);
    }
}

public enum TestEnum
{
    InvalidValue,
    ValidValue
}

public class StateSerializerTests
{
    private readonly ITestOutputHelper output;

    public StateSerializerTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void SerializeAndDeserialize_BasicTypes()
    {
        int originalInt = 123;
        string serializedInt = StateSerializer.SerializeObject(originalInt);
        int deserializedInt = StateSerializer.DeserializeObject<int>(serializedInt);

        Assert.Equal(originalInt, deserializedInt);

        string originalString = "Hello, World!";
        string serializedString = StateSerializer.SerializeObject(originalString);

        output.WriteLine($"serialized: {serializedString}");

        string? deserializedString = StateSerializer.DeserializeObject<string>(serializedString);

        output.WriteLine($"deserialized: {deserializedString}");

        Assert.Equal(originalString, deserializedString);
    }

    [Fact]
    public void SerializeAndDeserialize_ComplexType()
    {
        TestObject originalObj = TestObject.CreateValid();

        string serializedObj = StateSerializer.SerializeObject(originalObj);

        output.WriteLine($"serialized: {serializedObj}");

        TestObject? deserializedObj = StateSerializer.DeserializeObject<TestObject>(serializedObj);

        Assert.NotNull(deserializedObj);

        Assert.Equal(originalObj.id, deserializedObj.id);
        Assert.Equal(originalObj.name, deserializedObj.name);
        Assert.Equal(originalObj.inner, deserializedObj.inner);
    }

    [Fact]
    public void SerializeAndDeserialize_NullHandling()
    {
        TestObject originalObj = TestObject.CreateValid();

        originalObj.inner = null;

        string serializedObj = StateSerializer.SerializeObject(originalObj);
        TestObject? deserializedObj = StateSerializer.DeserializeObject<TestObject>(serializedObj);

        Assert.NotNull(deserializedObj);

        Assert.Equal(originalObj.id, deserializedObj.id);
        Assert.Equal(originalObj.name, deserializedObj.name);
        Assert.Null(deserializedObj.inner);
    }

    [Fact]
    public void Deserialize_InvalidData_ThrowsException()
    {
        string invalidData = "5|invalid";

        Assert.Throws<InvalidOperationException>(() => 
        {
            var deserializedObject = StateSerializer.DeserializeObject<TestObject>(invalidData);
        });
    }
}
