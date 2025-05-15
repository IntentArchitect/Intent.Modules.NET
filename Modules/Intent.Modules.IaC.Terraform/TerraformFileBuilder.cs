using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Intent.Modules.IaC.Terraform;

internal class TerraformFileBuilder
{
    private readonly List<TerraformElementBuilder> _elementBuilders = [];

    public IReadOnlyList<TerraformElementBuilder> GetElementBuilders() => new ReadOnlyCollection<TerraformElementBuilder>(_elementBuilders);

    public TerraformFileBuilder AddTerraformConfig(Action<TerraformBlockBuilder> configAction)
    {
        var builder = new TerraformBlockBuilder("terraform");
        configAction.Invoke(builder);
        _elementBuilders.Add(builder);
        return this;
    }

    public TerraformFileBuilder AddProvider(string providerName, Action<TerraformBlockBuilder>? providerAction = null)
    {
        var builder = new TerraformBlockBuilder("provider");
        builder.AddArgument(providerName);
        providerAction?.Invoke(builder);
        _elementBuilders.Add(builder);
        return this;
    }

    public TerraformFileBuilder AddResource(string resourceType, string resourceName, Action<TerraformBlockBuilder>? resourceAction = null)
    {
        var builder = new TerraformBlockBuilder("resource");
        builder.AddArgument(resourceType);
        builder.AddArgument(resourceName);
        resourceAction?.Invoke(builder);
        _elementBuilders.Add(builder);
        return this;
    }

    public TerraformFileBuilder AddVariable(string variableName, Action<TerraformBlockBuilder>? variableAction = null)
    {
        var builder = new TerraformBlockBuilder("variable");
        builder.AddArgument(variableName);
        variableAction?.Invoke(builder);
        _elementBuilders.Add(builder);
        return this;
    }

    public TerraformFileBuilder AddData(string dataType, string dataName, Action<TerraformBlockBuilder>? dataAction = null)
    {
        var builder = new TerraformBlockBuilder("data");
        builder.AddArgument(dataType);
        builder.AddArgument(dataName);
        dataAction?.Invoke(builder);
        _elementBuilders.Add(builder);
        return this;
    }

    public TerraformFileBuilder AddLocals(Action<TerraformBlockBuilder>? localsAction = null)
    {
        var builder = new TerraformBlockBuilder("locals");
        localsAction?.Invoke(builder);
        _elementBuilders.Add(builder);
        return this;
    }

    public TerraformFileBuilder AddOutput(string name, Action<TerraformBlockBuilder>? outputAction = null)
    {
        var builder = new TerraformBlockBuilder("output");
        builder.AddArgument(name);
        outputAction?.Invoke(builder);
        _elementBuilders.Add(builder);
        return this;
    }

    public TerraformFileBuilder AddModule(string name, Action<TerraformBlockBuilder>? moduleAction = null)
    {
        var builder = new TerraformBlockBuilder("module");
        builder.AddArgument(name);
        moduleAction?.Invoke(builder);
        _elementBuilders.Add(builder);
        return this;
    }

    public TerraformFileBuilder AddComment(string comment)
    {
        var builder = new TerraformCommentBuilder();
        builder.AddCommentBlock(comment);
        _elementBuilders.Add(builder);
        return this;
    }

    public string Build()
    {
        return string.Join(Environment.NewLine, _elementBuilders.Select(elementBuilder =>
        {
            var content = elementBuilder.Build();
            if (elementBuilder is TerraformBlockBuilder)
            {
                return content + Environment.NewLine;
            }

            return content;
        }));
    }
}

internal abstract class TerraformElementBuilder
{
    public abstract string Build(int indentLevel = 0, bool startIndented = true);

    protected static string GetIndentation(int indentLevel)
    {
        var indent = new string(' ', indentLevel * 2);
        return indent;
    }
}

internal class TerraformCommentBuilder : TerraformElementBuilder
{
    private readonly List<string> _commentBlocks = [];

    public TerraformCommentBuilder AddCommentBlock(string commentBlock)
    {
        ArgumentNullException.ThrowIfNull(commentBlock);
        _commentBlocks.Add(commentBlock);
        return this;
    }

    public override string Build(int indentLevel = 0, bool startIndented = true)
    {
        var sb = new StringBuilder(32);
        var indent = GetIndentation(indentLevel);
        foreach (var commentBlock in _commentBlocks)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            var lines = commentBlock.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
            var isBeginning = true;
            foreach (var line in lines)
            {
                if (!isBeginning)
                {
                    sb.AppendLine();
                }

                sb.Append($"{(startIndented ? indent : string.Empty)}# {line}");
                isBeginning = false;
            }
        }

        return sb.ToString();
    }
}

internal class TerraformBlockBuilder : TerraformElementBuilder
{
    private readonly List<string> _arguments = [];
    private readonly List<TerraformElementBuilder> _elements = [];
    private readonly WidthMonitor _widthMonitor = new();

    public TerraformBlockBuilder(string blockName)
    {
        BlockName = blockName;
    }

    public TerraformBlockBuilder()
    {
    }

    public string? BlockName { get; }
    public IReadOnlyList<string> Arguments => new ReadOnlyCollection<string>(_arguments);
    public IReadOnlyList<TerraformElementBuilder> GetElementBuilders() => new ReadOnlyCollection<TerraformElementBuilder>(_elements);
    
    public TerraformBlockBuilder AddComment(string commentBlock)
    {
        if (string.IsNullOrWhiteSpace(commentBlock))
        {
            throw new ArgumentNullException(nameof(commentBlock));
        }

        var builder = new TerraformCommentBuilder();
        builder.AddCommentBlock(commentBlock);
        _elements.Add(builder);
        return this;
    }

    public TerraformBlockBuilder AddArgument(string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentNullException(nameof(argument));
        }

        _arguments.Add(argument);
        return this;
    }

    public TerraformBlockBuilder AddBlock(string blockName, Action<TerraformBlockBuilder>? blockAction = null)
    {
        if (string.IsNullOrWhiteSpace(blockName))
        {
            throw new ArgumentNullException(nameof(blockName));
        }

        var builder = new TerraformBlockBuilder(blockName);
        blockAction?.Invoke(builder);
        _elements.Add(builder);
        return this;
    }

    public TerraformBlockBuilder AddSetting(string key, object? value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        _elements.Add(new KeyValueBuilder(key, value, _widthMonitor));
        return this;
    }

    public TerraformBlockBuilder AddRawSetting(string key, string rawValue)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        ArgumentNullException.ThrowIfNull(rawValue);
        _elements.Add(new KeyValueBuilder(key, new RawValue(rawValue), _widthMonitor));
        return this;
    }

    public TerraformBlockBuilder AddObject(string key, Action<TerraformBlockBuilder> objectAction)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        ArgumentNullException.ThrowIfNull(objectAction);

        var builder = new TerraformBlockBuilder();
        objectAction.Invoke(builder);
        _elements.Add(new KeyValueBuilder(key, builder, null));
        return this;
    }

    public override string Build(int indentLevel = 0, bool startIndented = true)
    {
        var sb = new StringBuilder(32);
        var indent = GetIndentation(indentLevel);

        if (startIndented)
        {
            sb.Append(indent);
        }

        if (!string.IsNullOrWhiteSpace(BlockName))
        {
            sb.Append(BlockName);
        }

        foreach (var argument in _arguments)
        {
            sb.Append(' ');
            sb.Append($@"""{argument}""");
        }

        if (!string.IsNullOrWhiteSpace(BlockName))
        {
            sb.Append(' ');
        }

        if (_elements.Count == 0)
        {
            sb.Append("{}");
            return sb.ToString();
        }

        sb.AppendLine("{");

        var isBeginning = true;
        TerraformElementBuilder? prevElement = null;
        foreach (var element in _elements)
        {
            var shouldAppendLine = !isBeginning && ((element is TerraformBlockBuilder && prevElement is not TerraformBlockBuilder) ||
                                                    (element is not TerraformBlockBuilder && prevElement is TerraformBlockBuilder));
            if (shouldAppendLine)
            {
                sb.AppendLine();
            }

            sb.AppendLine(element.Build(indentLevel + 1));
            isBeginning = false;
            prevElement = element;
        }

        sb.Append($"{indent}}}");

        return sb.ToString();
    }
}

internal record RawValue(string Value);

internal class WidthMonitor
{
    public int Width { get; private set; }

    public void RecordWidth(int input)
    {
        if (input > Width)
        {
            Width = input;
        }
    }
}

internal class KeyValueBuilder : TerraformElementBuilder
{
    private readonly WidthMonitor? _widthMonitor;

    public KeyValueBuilder(string key, object? value, WidthMonitor? widthMonitor)
    {
        Key = key;
        Value = value;
        _widthMonitor = widthMonitor;
        _widthMonitor?.RecordWidth(Key.Length);
    }

    public string Key { get; }

    public object? Value { get; }

    public override string Build(int indentLevel = 0, bool startIndented = true)
    {
        var indent = GetIndentation(indentLevel);
        var padding = CalculateKeyAlignmentPadding();

        return $"{(startIndented ? indent : string.Empty)}{Key}{padding} = {FormatValue(Value, indentLevel)}";
    }

    private string CalculateKeyAlignmentPadding()
    {
        if (_widthMonitor is null)
        {
            return string.Empty;
        }

        var paddingLength = _widthMonitor.Width - Key.Length;
        var padding = new string(' ', paddingLength);
        return padding;
    }

    private static string? FormatValue(object? value, int indentLevel)
    {
        switch (value)
        {
            case null:
                return string.Empty;
            case RawValue rawValue:
                return rawValue.Value;
            case string str:
                return $"\"{str}\"";
            case bool boolValue:
                return boolValue.ToString().ToLower();
            case TerraformBlockBuilder builder:
                return builder.Build(indentLevel, false);
            case IDictionary<string, object> dict:
            {
                var sb = new StringBuilder(32);
                sb.AppendLine("{");

                var indent = GetIndentation(indentLevel + 1);

                foreach (var item in dict)
                {
                    sb.AppendLine($"{indent}{item.Key} = {FormatValue(item.Value, indentLevel + 1)}");
                }

                sb.Append($"{indent}}}");
                return sb.ToString();
            }
            case IReadOnlyList<object> list:
            {
                return RenderListFormatValue(list, indentLevel);
            }
            case IEnumerable<object> enumerable:
            {
                return RenderListFormatValue(enumerable.ToList(), indentLevel);
            }
            default:
                return value.ToString();
        }
    }

    private static string? RenderListFormatValue(IReadOnlyList<object> list, int indentLevel)
    {
        var virtualLength = list.Sum(x => FormatValue(x, indentLevel)?.Length ?? 0);
        var shouldMultiline = virtualLength > 80;
        var indent = GetIndentation(indentLevel + 1);
                
        var sb = new StringBuilder(32);
        sb.Append('[');

        if (list.Count > 0)
        {
            if (shouldMultiline)
            {
                sb.AppendLine().Append(indent);
            }
            else
            {
                sb.Append(' ');
            }

            for (var i = 0; i < list.Count; i++)
            {
                sb.Append(FormatValue(list[i], indentLevel));
                if (i >= list.Count - 1)
                {
                    continue;
                }
                if (shouldMultiline)
                {
                    sb.AppendLine(",").Append(indent);
                }
                else
                {
                    sb.Append(", ");
                }
            }

            if (shouldMultiline)
            {
                sb.AppendLine().Append(GetIndentation(indentLevel));
            }
            else
            {
                sb.Append(' ');
            }
        }

        sb.Append(']');
        return sb.ToString();
    }
}