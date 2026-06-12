# Bug report: Razor merger crashes on `@if` blocks nested in markup (on regeneration)

## Summary
When the Software Factory **re-generates** a Razor file (the file already exists on disk, so the
Razor merger runs) and that file contains an `@if { ... }` control-flow block **nested inside a
markup element**, the merge throws and the SF cannot reach staging.

First generation (file absent → written raw, no merge) succeeds. The crash only happens on the
**second** run, when `RazorMerger.Merge(generated, existing, previousOutput)` is invoked.

## Environment
- Intent Architect **v5.1.x NIGHTLY**, build `5.1.999-nightly.2026-06-10-1112` (`Intent.SoftwareFactory.dll` 5.1.999.0)
- Merger lives in `Intent.Code.Weaving.Razor` (bundled in the IA agent, not a NuGet package)

## Exception
Two variants observed depending on the file's configured merge mode:

- Default (`Mode.Merge`):
  `An exception occurred when trying to merge the Razor file [...]: Unexpected type:
  Microsoft.AspNetCore.Razor.Language.Syntax.MarkupEphemeralTextLiteralSyntax`
- Forced `Mode.Fully` (`ConfigureRazorMerger(m => m.WithDefaultMode(Mode.Fully))`):
  `... Operation is not valid due to the current state of the object.`

Both fail at the same place:

```
at Intent.Code.Weaving.Razor.Merging.RazorTreeMerger.VisitBasedOnMode[T](T solutionNode, Func`2 baseVisit)
   in Modules/Intent.Code.Weaving.Razor/Merging/RazorTreeMerger.#.cs:line 65
at Intent.Code.Weaving.Razor.Merging.RazorTreeMerger.VisitMarkupBlock(MarkupBlockSyntax node)
   in Modules/Intent.Code.Weaving.Razor/Merging/RazorTreeMerger.MarkupBlock.cs:line 8
at Intent.Code.Weaving.Razor.RazorMerger.Merge(...)  Modules/Intent.Code.Weaving.Razor/RazorMerger.cs:line 85/69/48
at Intent.Code.Weaving.Razor.FactoryExtensions.RazorOutputTransformer.GetMergedContent(...)
```

i.e. `RazorTreeMerger` does not handle `MarkupEphemeralTextLiteralSyntax`, which the Razor parser
emits around the markup↔code transition of an `@if` block embedded in a `MarkupBlock`.

## Minimal reproduction
Any Razor file whose markup contains an `@if` block nested inside an element triggers it. The
smallest content:

```razor
<div>
    @if (true)
    {
        <span>x</span>
    }
</div>
```

### Quickest unit-style repro
Call the merger directly with identical generated/existing content (previousOutput can be empty
or equal), e.g.:

```csharp
var content =
    "<div>\r\n    @if (true)\r\n    {\r\n        <span>x</span>\r\n    }\r\n</div>\r\n";

// throws: Unexpected type ... MarkupEphemeralTextLiteralSyntax
RazorMerger.Merge(generated: content, existing: content, previousOutput: content);
```

### End-to-end repro (via a File Builder template)
1. Create a `RazorTemplateBase<object>` template whose body emits the markup above
   (e.g. `file.AddHtmlElement("div", d => { var c = IRazorCodeDirective.Create(new CSharpStatement("@if (true)"), file); c.AddHtmlElement("span", s => s.WithText("x")); d.AddChildNode(c); });`).
2. Run the Software Factory once and **apply** (file is written).
3. Run the Software Factory again → the merge of the now-existing file throws the exception above.

## Notes / expected behaviour
- `@if` blocks at the **document top level** and bare `@expression`s (e.g. `@Body`, `@(cond ? a : b)`)
  merge fine — only `@if`/code **blocks nested inside a markup element** crash.
- Expected: the merger should treat `MarkupEphemeralTextLiteralSyntax` like other literal/whitespace
  nodes (pass through) rather than throwing "Unexpected type".

## Workaround in the meantime
Avoid `@if { ... }` blocks in template-generated markup; express conditionals as a single razor
expression instead, e.g. `@(cond ? someRenderFragment : Body)`. (Static-content `.razor` files are
unaffected because they are written raw and never go through the merger.)
