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

namespace Intent.Modules.Entities.Templates.DomainEntitySkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DomainEntitySkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Entities.DomainEntitySkill";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEntitySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", relativeLocation: "domain-entity")
                .FromMarkdown($"""
---
name: domain-entity
description: Use when writing or changing the body of a constructor or operation on a domain entity or aggregate in the Domain project, including the generated stubs that throw NotImplementedException.
template-id: {TemplateId}
---

# Domain Entity

Fill in a constructor or operation on a domain entity. The class, its properties, and its constructor and operation signatures come from the Domain designer; only the bodies are yours.

## Rules

- Properties belong to the model. To add or change a property, constructor or operation, or a signature, change it in the Domain designer and regenerate.
- A modelled constructor or operation is generated with `[IntentManaged(Mode.Fully, Body = Mode.Merge)]` and a stub: `// IntentInitialGen`, a TODO, and `throw new NotImplementedException(...)`. When you write the body, replace the stub and change the attribute to `Body = Mode.Ignore`, so the next run leaves it alone.
- A constructor leaves the entity valid. An operation enforces the invariants it touches and updates related state together.
- Depend only on Domain types. No Application, infrastructure or framework type belongs in an entity.
- For a rule violation, throw the exception type the codebase already uses for domain rules.
- Private helper methods you add to the entity are kept.
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
