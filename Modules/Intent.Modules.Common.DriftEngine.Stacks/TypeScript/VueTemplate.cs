using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.Markup;

namespace Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

/// <summary>
/// A Vue single-file component's <c>&lt;template&gt;</c> as markup, read as Vue's own compiler parses a single-file component
/// (<c>@vue/compiler-dom</c>'s <c>parse</c> in its <c>sfc</c> mode): the top-level <c>&lt;template&gt;</c> is HTML, every
/// other block is raw text. Elements close at their end tag, at <c>/&gt;</c>, or as void elements; an end tag closes the
/// elements left open inside its element, and one that closes nothing is ignored. A tag is a component as Vue says: not an
/// HTML, SVG or MathML element, or starting with a capital letter. <c>v-*</c> attributes and the <c>:</c>, <c>@</c>,
/// <c>#</c> and <c>.</c> shorthands are directives, split as Vue splits them. A template in another language
/// (<c>lang="pug"</c>) or file (<c>src</c>) isn't read.
/// </summary>
internal static class VueTemplate
{
    // Vue's own lists (@vue/shared 3.5): what isn't one of these, nor slot, component or template, is a component.
    private const string HtmlTags = "html,body,base,head,link,meta,style,title,address,article,aside,footer,header,hgroup,h1,h2,h3,h4,h5,h6,nav,section,div,dd,dl,dt,figcaption,figure,picture,hr,img,li,main,ol,p,pre,ul,a,b,abbr,bdi,bdo,br,cite,code,data,dfn,em,i,kbd,mark,q,rp,rt,ruby,s,samp,small,span,strong,sub,sup,time,u,var,wbr,area,audio,map,track,video,embed,object,param,source,canvas,script,noscript,del,ins,caption,col,colgroup,table,thead,tbody,td,th,tr,button,datalist,fieldset,form,input,label,legend,meter,optgroup,option,output,progress,select,textarea,details,dialog,menu,summary,template,blockquote,iframe,tfoot";
    private const string SvgTags = "svg,animate,animateMotion,animateTransform,circle,clipPath,color-profile,defs,desc,discard,ellipse,feBlend,feColorMatrix,feComponentTransfer,feComposite,feConvolveMatrix,feDiffuseLighting,feDisplacementMap,feDistantLight,feDropShadow,feFlood,feFuncA,feFuncB,feFuncG,feFuncR,feGaussianBlur,feImage,feMerge,feMergeNode,feMorphology,feOffset,fePointLight,feSpecularLighting,feSpotLight,feTile,feTurbulence,filter,foreignObject,g,hatch,hatchpath,image,line,linearGradient,marker,mask,mesh,meshgradient,meshpatch,meshrow,metadata,mpath,path,pattern,polygon,polyline,radialGradient,rect,set,solidcolor,stop,switch,symbol,text,textPath,title,tspan,unknown,use,view";
    private const string MathTags = "annotation,annotation-xml,maction,maligngroup,malignmark,math,menclose,merror,mfenced,mfrac,mfraction,mglyph,mi,mlabeledtr,mlongdiv,mmultiscripts,mn,mo,mover,mpadded,mphantom,mprescripts,mroot,mrow,ms,mscarries,mscarry,msgroup,msline,mspace,msqrt,msrow,mstack,mstyle,msub,msubsup,msup,mtable,mtd,mtext,mtr,munder,munderover,none,semantics";
    private const string VoidTags = "area,base,br,col,embed,hr,img,input,link,meta,param,source,track,wbr";

    private static readonly HashSet<string> Native = new($"{HtmlTags},{SvgTags},{MathTags}".Split(','), StringComparer.Ordinal);
    private static readonly HashSet<string> Void = new(VoidTags.Split(','), StringComparer.Ordinal);

    // Elements whose content is text, not markup; in textarea and title, Vue still reads {{ }}.
    private static readonly HashSet<string> RawText = new(["script", "style", "textarea", "title"], StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> Interpolated = new(["textarea", "title"], StringComparer.OrdinalIgnoreCase);

    /// <param name="path">The file, for each node's place.</param>
    /// <param name="text">The file as written.</param>
    /// <returns>The elements at the template's top level; empty when it has no template Vue would parse as HTML.</returns>
    public static IReadOnlyList<MarkupNode> Read(string path, string text) => new Parser(path, text).Template();

    private abstract record Node(int Start, int End);

    private sealed record Text(int Start, int End) : Node(Start, End);

    private sealed record Interpolation(int Start, int End, string Code) : Node(Start, End);

    private sealed record Attribute(string Name, int Start, int End, string? Value, int ValueStart);

    private sealed record Element(string Tag, int Start, int End, IReadOnlyList<Attribute> Attributes, IReadOnlyList<Node> Children) : Node(Start, End);

    private sealed class Open(string tag, int start, IReadOnlyList<Attribute> attributes)
    {
        public string Tag { get; } = tag;

        public int Start { get; } = start;

        public IReadOnlyList<Attribute> Attributes { get; } = attributes;

        public List<Node> Children { get; } = [];

        /// <summary>Closed where its end tag ends; one left open ends with what it holds.</summary>
        public Element Close(int? end) => new(Tag, Start, end ?? (Children.Count > 0 ? Children[^1].End : Start), Attributes, Children);
    }

    private sealed class Parser(string path, string text)
    {
        private int _i;

        public IReadOnlyList<MarkupNode> Template()
        {
            while (_i < text.Length)
            {
                if (At("<!--"))
                {
                    SkipPast("-->");
                }
                else if (text[_i] == '<' && _i + 1 < text.Length && char.IsLetter(text[_i + 1]))
                {
                    var (tag, start, attributes, selfClosing, _) = OpenTag();
                    if (selfClosing)
                    {
                        continue;
                    }

                    var lang = attributes.FirstOrDefault(x => x.Name == "lang")?.Value;
                    if (tag == "template" && attributes.All(x => x.Name != "src") && lang is null or "html")
                    {
                        var template = Children(new Open(tag, start, attributes));
                        return template.Children.OfType<Element>().Select(Markup).ToList();
                    }

                    // Every other block is raw text, up to its end tag.
                    SkipRaw(tag);
                }
                else
                {
                    _i++;
                }
            }

            return [];
        }

        /// <summary>Reads markup until the end tag that closes <paramref name="root"/>, or the end of the file.</summary>
        private Element Children(Open root)
        {
            var stack = new List<Open> { root };
            var textStart = -1;
            while (_i < text.Length)
            {
                var top = stack[^1];
                var markup = text[_i] == '<' && _i + 1 < text.Length && (char.IsLetter(text[_i + 1]) || text[_i + 1] is '/' or '!');
                if (!markup && !At("{{"))
                {
                    textStart = textStart < 0 ? _i : textStart;
                    _i++;
                    continue;
                }

                if (textStart >= 0)
                {
                    top.Children.Add(new Text(textStart, _i));
                    textStart = -1;
                }

                if (At("{{"))
                {
                    var end = text.IndexOf("}}", _i + 2, StringComparison.Ordinal);
                    end = end < 0 ? text.Length : end;
                    top.Children.Add(new Interpolation(_i, Math.Min(end + 2, text.Length), text[(_i + 2)..end].Trim()));
                    _i = Math.Min(end + 2, text.Length);
                }
                else if (At("<!--"))
                {
                    SkipPast("-->");
                }
                else if (text[_i + 1] == '!')
                {
                    SkipPast(">");
                }
                else if (text[_i + 1] == '/')
                {
                    if (_i + 2 >= text.Length || !char.IsLetter(text[_i + 2]))
                    {
                        SkipPast(">");
                        continue;
                    }

                    var name = Name(_i + 2);
                    SkipPast(">");
                    var open = stack.FindLastIndex(x => string.Equals(x.Tag, name, StringComparison.OrdinalIgnoreCase));
                    if (open < 0)
                    {
                        // An end tag that closes nothing.
                        continue;
                    }

                    // It closes the elements left open inside its element.
                    for (var i = stack.Count - 1; i > open; i--)
                    {
                        stack[i - 1].Children.Add(stack[i].Close(null));
                    }

                    stack.RemoveRange(open + 1, stack.Count - open - 1);
                    if (open == 0)
                    {
                        return root.Close(_i);
                    }

                    var closed = stack[open].Close(_i);
                    stack.RemoveAt(open);
                    stack[^1].Children.Add(closed);
                }
                else
                {
                    var (tag, start, attributes, selfClosing, openEnd) = OpenTag();
                    if (selfClosing || Void.Contains(tag))
                    {
                        top.Children.Add(new Element(tag, start, openEnd, attributes, []));
                    }
                    else if (RawText.Contains(tag))
                    {
                        var contentStart = _i;
                        var contentEnd = SkipRaw(tag);
                        top.Children.Add(new Element(tag, start, _i, attributes, Interpolated.Contains(tag) ? TextAndValues(contentStart, contentEnd) : contentEnd > contentStart ? [new Text(contentStart, contentEnd)] : []));
                    }
                    else
                    {
                        stack.Add(new Open(tag, start, attributes));
                    }
                }
            }

            if (textStart >= 0)
            {
                stack[^1].Children.Add(new Text(textStart, text.Length));
            }

            for (var i = stack.Count - 1; i > 0; i--)
            {
                stack[i - 1].Children.Add(stack[i].Close(null));
            }

            return root.Close(null);
        }

        /// <summary>Reads a start tag at <c>&lt;</c>: its name, attributes, whether it closes itself, and where it ends.</summary>
        private (string Tag, int Start, IReadOnlyList<Attribute> Attributes, bool SelfClosing, int End) OpenTag()
        {
            var start = _i;
            var tag = Name(_i + 1);
            _i += 1 + tag.Length;
            var attributes = new List<Attribute>();
            while (_i < text.Length)
            {
                if (char.IsWhiteSpace(text[_i]))
                {
                    _i++;
                }
                else if (text[_i] == '>')
                {
                    _i++;
                    return (tag, start, attributes, false, _i);
                }
                else if (At("/>"))
                {
                    _i += 2;
                    return (tag, start, attributes, true, _i);
                }
                else if (text[_i] == '/')
                {
                    _i++;
                }
                else
                {
                    attributes.Add(AttributeAt());
                }
            }

            return (tag, start, attributes, false, _i);
        }

        private Attribute AttributeAt()
        {
            var start = _i;
            // A dynamic argument (:[key]) may hold what would end a name.
            while (_i < text.Length && !(char.IsWhiteSpace(text[_i]) || text[_i] is '=' or '>' || (text[_i] == '/' && !InBrackets(start))))
            {
                _i++;
            }

            var name = text[start.._i];
            var afterName = _i;
            while (_i < text.Length && char.IsWhiteSpace(text[_i]))
            {
                _i++;
            }

            if (_i >= text.Length || text[_i] != '=')
            {
                _i = afterName;
                return new Attribute(name, start, afterName, null, afterName);
            }

            _i++;
            while (_i < text.Length && char.IsWhiteSpace(text[_i]))
            {
                _i++;
            }

            if (_i < text.Length && text[_i] is '"' or '\'')
            {
                var quote = text[_i];
                var end = text.IndexOf(quote, _i + 1);
                end = end < 0 ? text.Length : end;
                var value = new Attribute(name, start, Math.Min(end + 1, text.Length), text[(_i + 1)..end], _i + 1);
                _i = Math.Min(end + 1, text.Length);
                return value;
            }

            var valueStart = _i;
            while (_i < text.Length && !char.IsWhiteSpace(text[_i]) && text[_i] != '>')
            {
                _i++;
            }

            return new Attribute(name, start, _i, text[valueStart.._i], valueStart);
        }

        /// <summary>Text with <c>{{ }}</c> values in it, between two positions.</summary>
        private List<Node> TextAndValues(int start, int end)
        {
            var nodes = new List<Node>();
            while (start < end)
            {
                var open = text.IndexOf("{{", start, end - start, StringComparison.Ordinal);
                var close = open < 0 ? -1 : text.IndexOf("}}", open + 2, end - open - 2, StringComparison.Ordinal);
                if (close < 0)
                {
                    nodes.Add(new Text(start, end));
                    break;
                }

                if (open > start)
                {
                    nodes.Add(new Text(start, open));
                }

                nodes.Add(new Interpolation(open, close + 2, text[(open + 2)..close].Trim()));
                start = close + 2;
            }

            return nodes;
        }

        private bool InBrackets(int start)
        {
            var open = text.IndexOf('[', start, _i - start);
            return open >= 0 && text.IndexOf(']', open, _i - open) < 0;
        }

        private string Name(int start)
        {
            var end = start;
            while (end < text.Length && !char.IsWhiteSpace(text[end]) && text[end] is not ('/' or '>'))
            {
                end++;
            }

            return text[start..end];
        }

        /// <summary>Skips a raw text element's content up to and past its end tag; returns where the content ends.</summary>
        private int SkipRaw(string tag)
        {
            var end = text.IndexOf("</" + tag, _i, StringComparison.OrdinalIgnoreCase);
            if (end < 0)
            {
                _i = text.Length;
                return text.Length;
            }

            _i = end;
            SkipPast(">");
            return end;
        }

        private void SkipPast(string what)
        {
            var end = text.IndexOf(what, _i + 1, StringComparison.Ordinal);
            _i = end < 0 ? text.Length : end + what.Length;
        }

        private bool At(string what) => string.CompareOrdinal(text, _i, what, 0, what.Length) == 0;

        private MarkupNode Markup(Element element) => new()
        {
            Tag = element.Tag,
            Component = ComponentOf(element.Tag),
            Attributes = element.Attributes.Select(AttributeOf).ToList(),
            Children = element.Children.OfType<Element>().Select(Markup).ToList(),
            Content = element.Children.Any(x => x is Element)
                ? null
                : MarkupParts.Content(element.Children.Select(x => x is Interpolation interpolation
                    ? new MarkupParts.Part(true, interpolation.Code, SpanAt(x.Start, x.End))
                    : new MarkupParts.Part(false, text[x.Start..x.End], SpanAt(x.Start, x.End))).ToList()),
            At = SpanAt(element.Start, element.End),
            TextOf = () => CanonicalText.OfMarkup(text[element.Start..element.End])
        };

        /// <summary>
        /// The component a tag names: by its PascalCase name (<c>stock-pill</c> is <c>StockPill</c>), or as written when it is
        /// one of the script's names and a member of it (<c>ui.Badge</c>). Null for an element, and for <c>&lt;component
        /// :is&gt;</c>, which names none.
        /// </summary>
        private static TypeRefNode? ComponentOf(string tag)
        {
            if (tag is "slot" or "component" or "template" || !char.IsUpper(tag[0]) && Native.Contains(tag))
            {
                return null;
            }

            var name = tag.Contains('.') ? tag : Pascal(tag);
            return new TypeRefNode { Text = tag, Name = name[(name.LastIndexOf('.') + 1)..], QualifiedName = name, Language = TypeScriptLanguage.LanguageName };
        }

        private static string Pascal(string name) => string.Concat(name.Split('-', StringSplitOptions.RemoveEmptyEntries).Select(x => char.ToUpperInvariant(x[0]) + x[1..]));

        /// <summary>
        /// An attribute, its directive split as Vue splits it: <c>v-model:title.trim</c> is <c>model</c>, <c>title</c> and
        /// <c>trim</c>; <c>:to</c> is <c>bind</c> and <c>to</c>, <c>@click</c> <c>on</c> and <c>click</c>, <c>#item</c>
        /// <c>slot</c> and <c>item</c>, and <c>.value</c> <c>bind</c> and <c>value</c> with <c>prop</c>.
        /// </summary>
        private MarkupAttributeNode AttributeOf(Attribute attribute)
        {
            var name = attribute.Name;
            string? directive = null;
            var rest = "";
            List<string> modifiers = [];
            if (name.Length > 2 && name.StartsWith("v-", StringComparison.Ordinal))
            {
                var end = name.IndexOfAny([':', '.'], 2);
                directive = end < 0 ? name[2..] : name[2..end];
                rest = end < 0 ? "" : name[end..];
            }
            else if (name.Length > 0 && name[0] is ':' or '@' or '#' or '.')
            {
                directive = name[0] switch { '@' => "on", '#' => "slot", _ => "bind" };
                rest = ":" + name[1..];
                if (name[0] == '.')
                {
                    modifiers.Add("prop");
                }
            }

            string? argument = null;
            if (rest.StartsWith(':'))
            {
                // A dynamic argument (:[key]) is written in brackets, which may hold dots.
                var close = rest.Length > 1 && rest[1] == '[' ? rest.IndexOf(']') : -1;
                var end = rest.IndexOf('.', close < 0 ? 1 : close);
                argument = end < 0 ? rest[1..] : rest[1..end];
                rest = end < 0 ? "" : rest[end..];
            }

            modifiers.AddRange(rest.Split('.', StringSplitOptions.RemoveEmptyEntries));
            return new MarkupAttributeNode
            {
                Name = name,
                Directive = directive,
                Argument = argument is "" ? null : argument,
                Modifiers = modifiers,
                Value = attribute.Value,
                Literal = directive is null && attribute.Value is { } value
                    ? MarkupParts.Value([new MarkupParts.Part(false, value, SpanAt(attribute.ValueStart, attribute.ValueStart + value.Length))])
                    : null,
                At = SpanAt(attribute.Start, attribute.End)
            };
        }

        private Span SpanAt(int start, int end)
        {
            var (line, column) = Position(start);
            var (endLine, endColumn) = Position(Math.Max(start, end));
            return new Span(path, line, column, endLine, endColumn);
        }

        private (int Line, int Column) Position(int index)
        {
            var line = 1;
            var lineStart = 0;
            for (var i = 0; i < index && i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    line++;
                    lineStart = i + 1;
                }
            }

            return (line, index - lineStart + 1);
        }
    }
}
