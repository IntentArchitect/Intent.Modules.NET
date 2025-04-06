using System;
using System.Collections.Generic;
using System.Text;

internal class BicepResource : BicepObject
{
    private readonly string _name;
    private readonly string _type;

    public BicepResource(string name, string type)
    {
        _name = name;
        _type = type;
    }

    public override string Build(int indentLevel = 0)
    {
        var sb = new StringBuilder(128);
        var indent = new string(' ', indentLevel * 2);
        
        // Fix any double quotes issue
        var fixedType = _type.Replace("''", "'");
        sb.AppendLine($"resource {_name} {fixedType} = {{");
        
        FormatProperties(sb, indentLevel + 1);
        
        sb.AppendLine(indent + "}");
        return sb.ToString();
    }
}

internal class BicepObject
{
    protected readonly Dictionary<string, object> _properties = new();

    public BicepObject Set(string key, string value)
    {
        _properties[key] = value;
        return this;
    }

    public BicepObject Block(string key, Action<BicepObject> block)
    {
        var nestedObject = new BicepObject();
        block(nestedObject);
        _properties[key] = nestedObject;
        return this;
    }

    public BicepObject Array(string key, Action<BicepArray> arrayBuilder)
    {
        var array = new BicepArray();
        arrayBuilder(array);
        _properties[key] = array;
        return this;
    }

    public virtual string Build(int indentLevel = 0)
    {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        FormatProperties(sb, indentLevel + 1);
        sb.Append(new string(' ', indentLevel * 2) + "}");
        return sb.ToString();
    }

    protected void FormatProperties(StringBuilder sb, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 2);
        var count = 0;
        foreach (var property in _properties)
        {
            count++;
            
            sb.Append(indent + property.Key + ": ");
            
            switch (property.Value)
            {
                case BicepObject obj:
                {
                    var objContent = obj.Build(indentLevel);
                    sb.AppendLine(objContent);
                    break;
                }
                case BicepArray array:
                {
                    var arrayContent = array.Build(indentLevel);
                    sb.AppendLine(arrayContent);
                    break;
                }
                default:
                    sb.AppendLine(property.Value.ToString());
                    break;
            }
        }
    }
}

internal class BicepArray
{
    private readonly List<object> _items = [];

    public BicepArray Object(Action<BicepObject> objectBuilder)
    {
        var obj = new BicepObject();
        objectBuilder(obj);
        _items.Add(obj);
        return this;
    }
    
    public string Build(int indentLevel)
    {
        var sb = new StringBuilder(64);
        var baseIndent = new string(' ', indentLevel * 2);
        var itemIndent = new string(' ', (indentLevel + 1) * 2);
        
        sb.AppendLine("[");
        
        for (var i = 0; i < _items.Count; i++)
        {
            if (_items[i] is not BicepObject obj)
            {
                continue;
            }
            
            var objStr = obj.Build(indentLevel + 1);
            sb.Append(itemIndent + objStr);
            if (i < _items.Count - 1)
            {
                sb.AppendLine();
            }
        }
        
        sb.AppendLine();
        sb.Append(baseIndent + "]");
        return sb.ToString();
    }
}
