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

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.EFRepositorySkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class EFRepositorySkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.EntityFrameworkCore.Repositories.EFRepositorySkillTemplate";

        internal const string SkillName = "ef-repository";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public EFRepositorySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", relativeLocation: SkillName)
                .FromMarkdown($"""
---
name: {SkillName}
description: Use when adding or changing a query or persistence method on a repository, in an I*Repository.cs interface in the Domain project or its EF Core implementation in Infrastructure.
template-id: {TemplateId}
---

# EF Repository Extension

Add a query or persistence method to a repository: its interface in the Domain project and its EF Core implementation in Infrastructure.

## Rules

- Read the base repository interface first, and use an existing method when one fits. Add a method for an aggregate result or a query the base methods can't express.
- If a designer will use the method, for example mapped from a command or query, model it as a repository operation in the Domain designer and regenerate, so the designer can map to it. Otherwise add it by hand to the interface, then to the implementation.
- Put no `[IntentManaged]` attribute on a method you add to the interface. The generated methods are `Mode.Fully`, and a `Mode.Fully` method the model doesn't produce is deleted on the next run.
- Mark a method you add to the implementation with `[IntentIgnore]`.
- Do the joins, filtering and aggregation in the repository, so callers get the result rather than data to finish in memory.
- Never return a tuple. For a composite result, declare a record in the interface's file, below the interface, so it stays in the Domain layer, and mark it `[IntentIgnore]` or the next run removes it.
- Name the method for the domain intent (`GetForUpdateAsync`, `ExistsBy…Async`) and take a `CancellationToken`.
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}