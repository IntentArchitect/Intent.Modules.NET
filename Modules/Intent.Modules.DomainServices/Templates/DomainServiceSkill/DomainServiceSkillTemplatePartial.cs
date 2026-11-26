using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.DomainServices.Templates.DomainServiceSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DomainServiceSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.DomainServices.DomainServiceSkill";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainServiceSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", relativeLocation: "domain-services")
                .FromMarkdown($"""
---
name: domain-services
description: Use when writing or changing a method body in a domain service, the interface and class in the Domain project's Services folder, including the generated stubs that throw NotImplementedException.
template-id: {TemplateId}
---

# Domain Service

Fill in the methods of an existing domain service. A domain service holds business rules that span several entities, or that belong to none of them. Its interface and method signatures come from the Domain designer; only the method bodies are yours.

## Rules

- Mark each method with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body.
- To add a method or change a signature, model the operation on the domain service in the Domain designer and regenerate. The interface is fully generated, so a hand edit to it is overwritten.
- Depend only on Domain types: entities, value objects, repository interfaces and other domain interfaces. No Application, infrastructure or framework type belongs in a domain service.
- Repositories are fine. Don't call `SaveChangesAsync` or use the unit of work: the pipeline that called the service commits.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation so the designer can map to it; otherwise add it by hand to the repository interface and the repository.
- When a rule concerns only one entity, put it on that entity and call it from the service.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the service are fine.
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
