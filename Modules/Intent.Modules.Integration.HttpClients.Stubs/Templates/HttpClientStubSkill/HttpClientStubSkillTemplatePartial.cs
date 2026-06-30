using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Stubs.Templates.HttpClientStubSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class HttpClientStubSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Integration.HttpClients.Stubs.HttpClientStubSkill";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientStubSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("SKILL", relativeLocation: "http-client-stub-sample-data")
                .FromMarkdown($$"""
---
name: http-client-stub-sample-data
description: customize Intent-generated stub HTTP client responses using DTO factories. Use when editing stub HTTP client methods, response DTO factories, or scenario helpers so integrations return plausible, contract-valid sample data.
template-id: {{TemplateId}}
---

# HTTP Client Stub Sample Data

Use generated DTO factories as the source of response structure. Shape stub responses so they are plausible, contract-valid, and easy to adjust for the behavior being exercised.

## Rules

- Do not change generated method signatures or DTO shapes in code.
- Start from factory defaults: `OrderDtoFactory.Create()` and `CreateList(...)`.
- Override fields that matter to the stub operation: identifiers, statuses, totals, dates, filters, or nested collections.
- Keep scenario helpers in the service's `Stubs` folder.
- Prefer stable, readable values over randomness, hashes, or hidden data-generation rules.
- Make empty, error, mixed-state, and nested-list cases explicit when they matter.

## Choosing A Style

- **Indexed Overrides** generate a requested number of similar records and vary selected fields by position. They are compact and easy to scale for search results, pages, or line items, but less suitable when each record represents a distinct case.
- **Curated Scenarios** define each record explicitly. They are clearer for named outcomes, mixed statuses, empty collections, edge cases, or nested structures with meaning, but become verbose for large uniform lists.
- Infer from the stub method name, parameters, comments, and nearby scenarios first.
- If both styles fit and the operation gives no signal, explain the difference and ask: "Should this return a repeatable list of similar records, or a small explicit set of meaningful cases?"

## Indexed Overrides

```csharp
var orders = OrderDtoFactory.CreateList(3, (order, orderIndex) =>
{
    order.Id = $"ORDER-{orderIndex + 1:000}";
    order.Items = OrderItemDtoFactory.CreateList(2, (item, itemIndex) =>
    {
        item.Sku = $"SKU-{itemIndex + 1:000}";
    });
});
```

## Curated Scenarios

```csharp
var orders = new List<OrderDto>
{
    OrderDtoFactory.Create(o =>
    {
        o.Id = "ORDER-001";
        o.Status = "Submitted";
    }),
    OrderDtoFactory.Create(o =>
    {
        o.Id = "ORDER-002";
        o.Status = "Cancelled";
        o.Items = [];
    })
};
```
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();
    }
}
