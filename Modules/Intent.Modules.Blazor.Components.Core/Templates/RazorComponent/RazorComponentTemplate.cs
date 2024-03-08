using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.Core.Templates.RazorComponent
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RazorComponentTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var razorFile = new RazorFile();
            razorFile.AddUsing("Microsoft.AspNetCore.Components.Forms");
            razorFile.AddHtmlElement("h1", e => e.Text = "@Mode Contact");
            razorFile.AddHtmlElement("hr");
            razorFile.AddCodeBlock("if (Contact is not null)", block =>
            {
                block.AddHtmlElement("EditForm", e =>
                {
                    e.AddAttribute("Model", "Contract")
                        .AddAttribute("OnInvalidSubmit", "(async () => await HandleSubmitAsync(false))");
                });
            });
            foreach (var component in Model.View.InternalElement.ChildElements)
            {
                razorFile.Nodes.Add(_componentResolver.ResolveFor(component).Render(component));
            }

            return razorFile.ToString();
        }
    }

    public class RazorFile : RazorFileNodeBase<RazorFile>
    {
        public IList<CSharpUsing> Usings { get; } = new List<CSharpUsing>();

        public RazorFile AddUsing(string @namespace)
        {
            Usings.Add(new CSharpUsing(@namespace));
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var index = 0; index < Usings.Count; index++)
            {
                var @using = Usings[index];
                sb.AppendLine($"@{@using}");

                if (index == Usings.Count - 1)
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
    }

    public class RazorFileNodeBase<T>
        where T : RazorFileNodeBase<T>
    {
        public IList<IRazorFileNode> Nodes { get; } = new List<IRazorFileNode>();

        public T AddHtmlElement(string name, Action<HtmlElement> configure = null)
        {
            var htmlElement = new HtmlElement(name);
            Nodes.Add(htmlElement);
            configure?.Invoke(htmlElement);
            return (T)this;
        }


        public T AddCodeBlock(CSharpStatement expression, Action<RazorCodeBlock> configure = null)
        {
            var razorCodeBlock = new RazorCodeBlock(expression);
            Nodes.Add(razorCodeBlock);
            configure?.Invoke(razorCodeBlock);
            return (T)this;
        }
    }

    public class RazorCodeBlock : RazorFileNodeBase<RazorCodeBlock>, IRazorFileNode
    {
        public ICSharpExpression Expression { get; set; }

        public RazorCodeBlock()
        {
        }

        public RazorCodeBlock(ICSharpExpression expression)
        {
            Expression = expression;
        }

        public string GetText(string indentation)
        {
            return $@"{indentation}@{Expression.GetText(indentation)?.TrimStart() ?? "code"} {{
{indentation}{string.Join("", Nodes.Select(x => x.GetText($"{indentation}    ")))}
{indentation}}}
";
        }
    }

    public class HtmlElement : RazorFileNodeBase<HtmlElement>, IRazorFileNode
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        public HtmlElement() { }

        public HtmlElement(string name)
        {
            Name = name;
        }

        public HtmlElement AddAttribute(string name, string value)
        {
            Attributes[name] = value;
            return this;
        }

        public HtmlElement WithText(string text)
        {
            Text = text;
            return this;
        }

        public string GetText(string indentation)
        {
            var sb = new StringBuilder();
            var requiresEndTag = !string.IsNullOrWhiteSpace(Text) || Nodes.Any();
            sb.Append($"{indentation}<{Name}{FormatAttributes(indentation)}{(!requiresEndTag ? "/" : "")}>{(Nodes.Any() ? Environment.NewLine : "")}");

            if (!string.IsNullOrWhiteSpace(Text))
            {
                if (Nodes.Any())
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
                    sb.Append($"{(!Nodes.Any() ? Environment.NewLine : "")}{indentation}</{Name}>");
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