using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.Blazor.Api
{
    public class BlazorFile : RazorFile
    {
        private Action<BlazorFile> _configure;
        public IList<RazorCodeBlock> CodeBlocks => ChildNodes.OfType<RazorCodeBlock>().Where(x => x.Expression == null).ToList();

        public BlazorFile(ICSharpTemplate template) : base(template)
        {
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
            var razorCodeBlock = new RazorCodeBlock(RazorFile);
            ChildNodes.Add(razorCodeBlock);
            configure?.Invoke(razorCodeBlock);
            return this;
        }

        public BlazorFile Configure(Action<BlazorFile> configure)
        {
            _configure = configure;
            return this;
        }

        public override RazorFile Build()
        {
            _isBuilt = true;
            _configure?.Invoke(this);
            return this;
        }
    }

    public class RazorFile : RazorFileNodeBase<RazorFile>, ICSharpFile
    {
        private Action<RazorFile> _configure;

        public RazorFile(ICSharpTemplate template) : base(null)
        {
            Template = template;
            File = this;
            RazorFile = this;
        }

        public ICSharpTemplate Template { get; protected set; }
        public IList<RazorDirective> Directives { get; } = new List<RazorDirective>();

        public RazorFile AddUsing(string @namespace)
        {
            Directives.Add(new RazorDirective("using", new CSharpStatement(@namespace.Replace("using ", ""))));
            return this;
        }

        public string GetModelType<TModel>(TModel model) where TModel : IMetadataModel, IHasName
        {
            if (Template == null)
            {
                throw new InvalidOperationException("Cannot add property with model. Please add the template as an argument to this CSharpFile's constructor.");
            }

            var type = model switch
            {
                IHasTypeReference hasType => Template.GetTypeName(hasType.TypeReference),
                ITypeReference typeRef => Template.GetTypeName(typeRef),
                _ => throw new ArgumentException($"model {model.Name} must implement either IHasTypeReference or ITypeReference", nameof(model))
            };
            return type;
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

        protected bool _isBuilt = false;
        public virtual RazorFile Build()
        {
            _configure(this);
            _isBuilt = true;
            return this;
        }

        public override string GetText(string indentation)
        {
            if (!_isBuilt)
            {
                throw new Exception("RazorFile has not been built. Call .Build() before this invocation.");
            }
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

            foreach (var node in ChildNodes)
            {
                sb.Append(node.GetText(""));
            }

            return sb.ToString();
        }
    }

    public interface IRazorFileNode
    {
        string GetText(string indentation);
        void AddChildNode(IRazorFileNode node);
    }

    public abstract class RazorFileNodeBase<T> : CSharpMetadataBase<T>, IRazorFileNode where T : RazorFileNodeBase<T>
    {
        public RazorFileNodeBase(RazorFile file)
        {
            File = file;
            RazorFile = file;
        }

        public RazorFile RazorFile { get; protected set; }

        public IList<IRazorFileNode> ChildNodes { get; } = new List<IRazorFileNode>();

        public T AddHtmlElement(string name, Action<HtmlElement> configure = null)
        {
            var htmlElement = new HtmlElement(name, RazorFile);
            ChildNodes.Add(htmlElement);
            configure?.Invoke(htmlElement);
            return (T)this;
        }

        public T AddHtmlElement(HtmlElement htmlElement)
        {
            ChildNodes.Add(htmlElement);
            return (T)this;
        }

        public T AddEmptyLine()
        {
            ChildNodes.Add(new EmptyLine());
            return (T)this;
        }

        public T AddCodeBlock(CSharpStatement expression, Action<RazorCodeDirective> configure = null)
        {
            var razorCodeBlock = new RazorCodeDirective(expression, RazorFile);
            ChildNodes.Add(razorCodeBlock);
            configure?.Invoke(razorCodeBlock);
            return (T)this;
        }

        public abstract string GetText(string indentation);


        public void AddChildNode(IRazorFileNode node)
        {
            ChildNodes.Add(node);
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


    public class RazorCodeDirective : RazorFileNodeBase<RazorCodeDirective>, IRazorFileNode
    {
        public ICSharpExpression Expression { get; set; }
        public RazorCodeDirective(ICSharpExpression expression, RazorFile file) : base(file)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public override string GetText(string indentation)
        {
            return $@"{indentation}@{Expression.GetText(indentation)?.TrimStart()} {{
{string.Join("", ChildNodes.Select(x => x.GetText($"{indentation}    ")))}
{indentation}}}
";
        }
    }

    public class RazorCodeBlock : RazorFileNodeBase<RazorCodeBlock>, IRazorFileNode, IBuildsCSharpMembers
    {
        public ICSharpExpression Expression { get; set; }
        public IList<ICodeBlock> Declarations { get; } = new List<ICodeBlock>();

        public RazorCodeBlock(RazorFile file) : base(file)
        {
        }

        public IBuildsCSharpMembers AddField(string type, string name, Action<CSharpField> configure = null)
        {
            var field = new CSharpField(type, name)
            {
                BeforeSeparator = CSharpCodeSeparatorType.NewLine,
                AfterSeparator = CSharpCodeSeparatorType.NewLine
            };
            Declarations.Add(field);
            configure?.Invoke(field);
            return this;
        }

        public IBuildsCSharpMembers AddProperty(string type, string name, Action<CSharpProperty> configure = null)
        {
            var property = new CSharpProperty(type, name, RazorFile)
            {
                BeforeSeparator = CSharpCodeSeparatorType.EmptyLines,
                AfterSeparator = CSharpCodeSeparatorType.EmptyLines
            };
            Declarations.Add(property);
            configure?.Invoke(property);
            return this;
        }

        public IBuildsCSharpMembers AddMethod(string type, string name, Action<CSharpClassMethod> configure = null)
        {
            var method = new CSharpClassMethod(type, name, RazorFile)
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
            return $@"{indentation}@code {{
{string.Join("", ChildNodes.Select(x => x.GetText($"{indentation}    ")))}{string.Join(@"
", Declarations.ConcatCode(indentation + "    "))}
{indentation}}}
";
        }
    }

    public class EmptyLine : IRazorFileNode
    {
        public string GetText(string indentation)
        {
            return Environment.NewLine;
        }

        public void AddChildNode(IRazorFileNode node)
        {
            throw new NotImplementedException();
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

        public HtmlElement AddAttribute(string name, string value = null)
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
            var requiresEndTag = !string.IsNullOrWhiteSpace(Text) || ChildNodes.Any() || Name is "script";
            sb.Append($"{indentation}<{Name}{FormatAttributes(indentation)}{(!requiresEndTag ? " /" : "")}>{(requiresEndTag && (ChildNodes.Any() || Attributes.Count > 1) ? Environment.NewLine : "")}");

            if (!string.IsNullOrWhiteSpace(Text))
            {
                if (ChildNodes.Any() || Attributes.Count > 1)
                {
                    sb.AppendLine($"{indentation}    {Text}");
                }
                else
                {
                    sb.Append(Text);
                }
            }

            foreach (var e in ChildNodes)
            {
                sb.Append(e.GetText($"{indentation}    "));
            }

            if (requiresEndTag)
            {
                if (Attributes.Count > 1 || ChildNodes.Any())
                {
                    sb.Append($"{(!ChildNodes.Any() && Attributes.Count <= 1 ? Environment.NewLine : "")}{indentation}</{Name}>");
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
            var separateLines = Name is not "link" and not "meta";
            return string.Join(separateLines ? $"{Environment.NewLine}{indentation}{new string(' ', Name.Length + 1)}" : "", Attributes.Select(attribute => $" {attribute.Key}{(attribute.Value != null ? $"=\"{attribute.Value}\"" : "")}"));
        }

        public override string ToString()
        {
            return GetText("");
        }
    }
}