using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.Core.Templates
{
    public class BlazorFile : RazorFile
    {
        private Action<BlazorFile> _configure;
        public ICSharpTemplate Template { get; }
        public IList<RazorCodeBlock> CodeBlocks => Nodes.OfType<RazorCodeBlock>().Where(x => x.Expression == null).ToList();

        public BlazorFile(ICSharpTemplate template)
        {
            Template = template;
        }

        public RazorFile AddPageDirective(string route)
        {
            Directives.Insert(0, new RazorDirective("page", new CSharpStatement($"\"{route}\"")));
            return this;
        }

        public RazorFile AddInjectDirective(string fullyQualifiedTypeName, string propertyName = null)
        {
            var serviceDeclaration = $"{Template.UseType(fullyQualifiedTypeName)} {propertyName ?? Template.UseType(fullyQualifiedTypeName)}";
            if (!Directives.Any(x => x.Keyword == "inject" && x.Expression.ToString() == serviceDeclaration))
            {
                Directives.Add(new RazorDirective("inject", new CSharpStatement(serviceDeclaration)));
            }
            return this;
        }

        public BlazorFile AddCodeBlock(Action<RazorCodeBlock> configure = null)
        {
            var razorCodeBlock = new RazorCodeBlock(null, File);
            Nodes.Add(razorCodeBlock);
            configure?.Invoke(razorCodeBlock);
            return this;
        }

        public BlazorFile Configure(Action<BlazorFile> configure)
        {
            _configure = configure;
            return this;
        }

        public new RazorFile Build()
        {
            _configure(this);
            return this;
        }
    }

    public class RazorFile : RazorFileNodeBase<RazorFile>
    {
        private Action<RazorFile> _configure;

        public RazorFile() : base(null)
        {
        }

        public IList<RazorDirective> Directives { get; } = new List<RazorDirective>();

        public RazorFile AddUsing(string @namespace)
        {
            Directives.Add(new RazorDirective("using", new CSharpStatement(@namespace.Replace("using ", ""))));
            return this;
        }

        public RazorFile Configure(Action<RazorFile> configure)
        {
            _configure = configure;
            return this;
        }

        public override string ToString()
        {
            return GetText("");
        }

        public RazorFile Build()
        {
            _configure(this);
            return this;
        }

        public override string GetText(string indentation)
        {
            var sb = new StringBuilder();

            var orderedDirectives = Directives.OrderBy(x => x.Order).ToList();
            for (var index = 0; index < orderedDirectives.Count; index++)
            {
                var directive = orderedDirectives[index];
                sb.AppendLine(directive.ToString());

                if (index == orderedDirectives.Count - 1)
                {
                    sb.AppendLine();
                }
            }

            foreach (var node in Nodes)
            {
                sb.Append(node.GetText(""));
            }

            return sb.ToString();
        }
    }

    public interface IRazorFileNode
    {
        string GetText(string indentation);
        void AddNode(IRazorFileNode node);
    }

    public abstract class RazorFileNodeBase<T> : CSharpMetadataBase<T>, IRazorFileNode where T : RazorFileNodeBase<T>
    {
        public RazorFileNodeBase(RazorFile file)
        {
            File = file;
        }

        public RazorFile File { get; }

        public IList<IRazorFileNode> Nodes { get; } = new List<IRazorFileNode>();

        public T AddHtmlElement(string name, Action<HtmlElement> configure = null)
        {
            var htmlElement = new HtmlElement(name, File);
            Nodes.Add(htmlElement);
            configure?.Invoke(htmlElement);
            return (T)this;
        }

        public T AddHtmlElement(HtmlElement htmlElement)
        {
            Nodes.Add(htmlElement);
            return (T)this;
        }

        public T AddCodeBlock(CSharpStatement expression, Action<RazorCodeBlock> configure = null)
        {
            var razorCodeBlock = new RazorCodeBlock(expression, File);
            Nodes.Add(razorCodeBlock);
            configure?.Invoke(razorCodeBlock);
            return (T)this;
        }

        public abstract string GetText(string indentation);

        public void AddNode(IRazorFileNode node)
        {
            Nodes.Add(node);
        }
    }

    public class RazorDirective
    {
        public string Keyword { get; }
        public ICSharpExpression Expression { get; }

        public int Order { get; set; }

        public RazorDirective(string keyword, ICSharpExpression expression = null)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(keyword));
            }

            Keyword = keyword;
            Expression = expression;
            switch (keyword)
            {
                case "page":
                    Order = 0;
                    break;
                case "using":
                    Order = 1;
                    break;
                case "inject":
                    Order = 2;
                    break;
                default:
                    Order = 0;
                    break;
            }
        }

        public override string ToString()
        {
            return $"@{Keyword}{(Expression != null ? $" {Expression}" : "")}";
        }
    }

    public class RazorCodeBlock : RazorFileNodeBase<RazorCodeBlock>, IRazorFileNode
    {
        public ICSharpExpression Expression { get; set; }
        public IList<ICodeBlock> Declarations { get; } = new List<ICodeBlock>();

        public RazorCodeBlock(RazorFile file) : base(file)
        {
        }

        public RazorCodeBlock(ICSharpExpression expression, RazorFile file) : this(file)
        {
            Expression = expression;
        }

        public RazorCodeBlock AddProperty(string type, string name, Action<CSharpProperty> configure = null)
        {
            var property = new CSharpProperty(type, name, null)
            {
                BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
                AfterSeparator = CSharpCodeSeparatorType.EmptyLines
            };
            Declarations.Add(property);
            configure?.Invoke(property);
            return this;
        }

        public RazorCodeBlock AddMethod(string type, string name, Action<CSharpClassMethod> configure = null)
        {
            var method = new CSharpClassMethod(type, name, null)
            {
                BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
                AfterSeparator = CSharpCodeSeparatorType.EmptyLines
            };
            Declarations.Add(method);
            configure?.Invoke(method);
            return this;
        }

        public override string GetText(string indentation)
        {
            return $@"{indentation}@{Expression?.GetText(indentation)?.TrimStart() ?? "code"} {{
{string.Join("", Nodes.Select(x => x.GetText($"{indentation}    ")))}{string.Join(@"
", Declarations.ConcatCode(indentation + "    "))}
{indentation}}}
";
        }
    }

    public class HtmlElement : RazorFileNodeBase<HtmlElement>, IRazorFileNode
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        public HtmlElement(string name, RazorFile file) : base(file)
        {
            Name = name;
        }

        public HtmlElement AddAttribute(string name, string value)
        {
            Attributes[name] = value;
            return this;
        }

        public HtmlElement AddAttributeIfNotEmpty(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return this;
            }
            return AddAttribute(name, value);
        }

        public HtmlElement WithText(string text)
        {
            Text = text;
            return this;
        }

        public override string GetText(string indentation)
        {
            var sb = new StringBuilder();
            var requiresEndTag = !string.IsNullOrWhiteSpace(Text) || Nodes.Any();
            sb.Append($"{indentation}<{Name}{FormatAttributes(indentation)}{(!requiresEndTag ? "/" : "")}>{(Nodes.Any() || Attributes.Count > 1 ? Environment.NewLine : "")}");

            if (!string.IsNullOrWhiteSpace(Text))
            {
                if (Nodes.Any() || Attributes.Count > 1)
                {
                    sb.AppendLine($"{indentation}    {Text}");
                }
                else
                {
                    sb.Append(Text);
                }
            }

            foreach (var e in Nodes)
            {
                sb.Append(e.GetText($"{indentation}    "));
            }

            if (requiresEndTag)
            {
                if (Attributes.Count > 1 || Nodes.Any())
                {
                    sb.Append($"{(!Nodes.Any() && Attributes.Count <= 1 ? Environment.NewLine : "")}{indentation}</{Name}>");
                }
                else
                {
                    sb.Append($"</{Name}>");
                }
            }

            sb.AppendLine();

            return sb.ToString();
        }

        private string FormatAttributes(string indentation)
        {
            return string.Join($"{Environment.NewLine}{indentation}{new string(' ', Name.Length + 1)}", Attributes.Select(attribute => $" {attribute.Key}=\"{attribute.Value}\""));
        }

        public override string ToString()
        {
            return GetText("");
        }
    }
}