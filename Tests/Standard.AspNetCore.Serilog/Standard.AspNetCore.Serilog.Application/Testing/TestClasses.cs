using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Dynamic;

namespace Standard.AspNetCore.Serilog.Application.Testing;

public class ComplexTypeTest
{
    // Basic value types
    public int IntValue { get; set; } = 42;
    public long LongValue { get; set; } = 9223372036854775807L;
    public short ShortValue { get; set; } = 32767;
    public byte ByteValue { get; set; } = 255;
    public sbyte SByteValue { get; set; } = -128;
    public uint UIntValue { get; set; } = 4294967295U;
    public ulong ULongValue { get; set; } = 18446744073709551615UL;
    public ushort UShortValue { get; set; } = 65535;

    // Floating point types
    public float FloatValue { get; set; } = 3.14159f;
    public double DoubleValue { get; set; } = 1.7976931348623157E+308d;
    public decimal DecimalValue { get; set; } = 79228162514264337593543950335M;

    // Other value types
    public bool BoolValue { get; set; } = true;
    public char CharValue { get; set; } = 'A';
    public DateTime DateTimeValue { get; set; } = DateTime.Now;
    public DateTimeOffset DateTimeOffsetValue { get; set; } = DateTimeOffset.Now;
    public TimeSpan TimeSpanValue { get; set; } = TimeSpan.FromHours(1);
    public Guid GuidValue { get; set; } = Guid.NewGuid();

    // String and text
    public string StringValue { get; set; } = "Hello, World!";
    public string NullString { get; set; }
    public string EmptyString { get; set; } = string.Empty;

    // Other collections
    public int[] IntArray { get; set; } = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public List<string> StringList { get; set; } = new List<string> { "one", "two", "three" };
    public Dictionary<string, object> Dictionary { get; set; } = new Dictionary<string, object>
    {
        ["key1"] = "value",
        ["key2"] = 123
    };

    // Enumerables and queryable
    public IEnumerable<int> IntEnumerable { get; set; } = Enumerable.Range(1, 10);
    public string[] EmptyCollection { get; set; } = Array.Empty<string>();
    
    // Deeply nested objects (3 levels)
    public Level1Object Level1 { get; set; } = new Level1Object();

    // Deep object with different types at each level
    public DeepMixedObject DeepMixed { get; set; } = new DeepMixedObject();

    // Collections with deep nesting
    public DeepCollectionsObject DeepCollections { get; set; } = new DeepCollectionsObject();

    // Recursive-like structure with similar pattern at each level
    public RecursiveNode RecursivePattern { get; set; } = new RecursiveNode
    {
        Value = "Root",
        Children = new List<RecursiveNode>
        {
            new RecursiveNode
            {
                Value = "Child 1",
                Children = new List<RecursiveNode>
                {
                    new RecursiveNode
                    {
                        Value = "Grandchild 1-1",
                        Children = new List<RecursiveNode>
                        {
                            new RecursiveNode { Value = "Great-grandchild 1-1-1", Children = new List<RecursiveNode>() }
                        }
                    },
                    new RecursiveNode
                    {
                        Value = "Grandchild 1-2",
                        Children = new List<RecursiveNode>
                        {
                            new RecursiveNode { Value = "Great-grandchild 1-2-1", Children = new List<RecursiveNode>() }
                        }
                    }
                }
            },
            new RecursiveNode
            {
                Value = "Child 2",
                Children = new List<RecursiveNode>
                {
                    new RecursiveNode
                    {
                        Value = "Grandchild 2-1",
                        Children = new List<RecursiveNode>
                        {
                            new RecursiveNode { Value = "Great-grandchild 2-1-1", Children = new List<RecursiveNode>() }
                        }
                    }
                }
            }
        }
    };

    // Deep tuple structures
    public Tuple<int, Tuple<string, Tuple<DateTime, string, int>>> DeepTuples { get; set; }

    // Value tuples with deep nesting - use object to store the value tuples
    public object DeepValueTuples { get; set; }

    // Nullable types
    public int? NullableInt { get; set; } = 10;
    public double? NullableWithNull { get; set; } = null;

    // Dynamic and expandable objects
    public ExpandoObject DynamicValue { get; set; } = new ExpandoObject();

    // Special values
    public object NullValue { get; set; } = null;
    public int MinValue { get; set; } = int.MinValue;
    public int MaxValue { get; set; } = int.MaxValue;
    public double PositiveInfinity { get; set; } = double.PositiveInfinity;
    public double NegativeInfinity { get; set; } = double.NegativeInfinity;
    public double NaN { get; set; } = double.NaN;

    // Dates in different formats
    public DateOnly DateOnly { get; set; } = new DateOnly(2023, 5, 15);
    public TimeOnly TimeOnly { get; set; } = new TimeOnly(14, 30, 15);

    // URI and networking
    public Uri Uri { get; set; } = new Uri("https://example.com");

    // Custom formatting
    public CultureInfo CustomFormattable { get; set; } = new CultureInfo("fr-FR");

    // Binary data
    public byte[] ByteArray { get; set; } = new byte[] { 0x01, 0x02, 0x03, 0x04 };
    public string Base64String { get; set; } = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("test"));

    // JSON serializable content
    public JsonElement JsonElement { get; set; }

    public ComplexTypeTest()
    {
        // Initialize the deep tuples
        DeepTuples = Tuple.Create(
            1,
            Tuple.Create(
                "Second level",
                Tuple.Create(
                    DateTime.Now,
                    "Deepest level",
                    42)
            )
        );

        // Initialize DeepValueTuples using an anonymous type to mimic the structure
        DeepValueTuples = new
        {
            First = "First level",
            Second = new
            {
                Level = 2,
                Nested = new
                {
                    Level = 3,
                    FinalValue = "Three levels deep",
                    LastNumber = 333
                }
            }
        };

        // Initialize JsonElement
        JsonElement = JsonDocument.Parse(@"{
            ""level1"": {
                ""level2"": {
                    ""level3"": {
                        ""value"": ""JSON three levels deep""
                    }
                }
            }
        }").RootElement;
    }
}

public class Level1Object
{
    public string Name { get; set; } = "Level 1";
    public int Number { get; set; } = 1;
    public Level2Object Level2 { get; set; } = new Level2Object();
}

public class Level2Object
{
    public string Name { get; set; } = "Level 2";
    public int Number { get; set; } = 2;
    public Level3Object Level3 { get; set; } = new Level3Object();
}

public class Level3Object
{
    public string Name { get; set; } = "Level 3";
    public int Number { get; set; } = 3;
    public string FinalValue { get; set; } = "Deepest level";
}

public class DeepMixedObject
{
    public string StringProp { get; set; } = "First level";
    public int IntProp { get; set; } = 100;
    public byte[] ByteArray { get; set; } = new byte[] { 0x01, 0x02, 0x03, 0x04 };
    public NestedMixedObject NestedMixed { get; set; } = new NestedMixedObject();
}

public class NestedMixedObject
{
    public DateTime DateProp { get; set; } = DateTime.Today;
    public bool BoolProp { get; set; } = false;
    public byte[] ByteArray { get; set; } = new byte[] { 0x01, 0x02, 0x03, 0x04 };
    public NestedDeeperObject NestedDeeper { get; set; } = new NestedDeeperObject();
}

public class NestedDeeperObject
{
    public Guid GuidProp { get; set; } = Guid.NewGuid();
    public float FloatProp { get; set; } = 3.14159f;
    public int[] ArrayProp { get; set; } = new[] { 1, 2, 3 };
    public byte[] ByteArray { get; set; } = new byte[] { 0x01, 0x02, 0x03, 0x04 };
}

public class DeepCollectionsObject
{
    // Array of objects with nested objects
    public List<ObjectArrayItem> ObjectArray { get; set; } = new List<ObjectArrayItem>
    {
        new ObjectArrayItem { Id = 1, Name = "Item 1", Details = new ItemDetails { Color = "Red", Size = "Large" } },
        new ObjectArrayItem { Id = 2, Name = "Item 2", Details = new ItemDetails { Color = "Blue", Size = "Medium" } },
        new ObjectArrayItem { Id = 3, Name = "Item 3", Details = new ItemDetails { Color = "Green", Size = "Small" } }
    };

    // Dictionary with nested dictionaries
    public Dictionary<string, object> NestedDictionary { get; set; } = new Dictionary<string, object>();

    // List of lists of lists
    public List<List<List<int>>> TripleNestedList { get; set; } = new List<List<List<int>>>
    {
        new List<List<int>>
        {
            new List<int> { 1, 2, 3 },
            new List<int> { 4, 5, 6 }
        },
        new List<List<int>>
        {
            new List<int> { 7, 8, 9 },
            new List<int> { 10, 11, 12 }
        }
    };

    public DeepCollectionsObject()
    {
        var level3Dict = new Dictionary<string, object>
        {
            ["ThirdLevel"] = "Deep value",
            ["AnotherThird"] = 333
        };

        var level2Dict = new Dictionary<string, object>
        {
            ["SecondLevel"] = level3Dict
        };

        NestedDictionary["FirstLevel"] = level2Dict;
    }
}

public class ObjectArrayItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ItemDetails Details { get; set; }
}

public class ItemDetails
{
    public string Color { get; set; }
    public string Size { get; set; }
}

public class RecursiveNode
{
    public string Value { get; set; }
    public List<RecursiveNode> Children { get; set; } = new List<RecursiveNode>();
}
