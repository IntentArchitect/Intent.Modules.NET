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

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public EFRepositorySkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", relativeLocation: SkillName)
                .FromMarkdown($"""
---
name: {SkillName}
description: Guidance for extending Entity Framework Repositories. Use when a wanting to modify or extend existing Entity Framework repository functionality (Interfaces or Concretes).
template-id: {TemplateId}
---

# EF Repository Extension

- Repositories should encapsulate all logic for retrieving and persisting entities. This includes any necessary joins, filtering, or other data access logic.
- Aggregations of data should be handled by the repository layer, not by services or other layers. This ensures that all data access logic is centralized and can be optimized as needed.
- Only add additional methods to the repository for querying aggregations or complex queries. Otherwise just use the existing methods.

## Repository Interface

### Instructions

- Only add additional methods to the repository for querying aggregations or complex queries. Otherwise just use the existing methods.
- Always read the base repository interface to understand what is already provided and available before adding new methods.
- If you add a new method to the repository interface, do not put any [IntentManaged] attributes on it, especially not [IntentManaged(Mode.Fully)].

### Rules when adding methods

- Always add the method signature to the repository interface contract first, then implement it in the repository implementation.
- Never return tuples. If a complex return type is required, create a new Contract record in this file (below this interface) and add a `[IntentIgnore]` attribute over it.

## Repository Implementation (Concrete)

### Rules when adding methods

- Always add the method signature to the repository interface contract first, then implement it in the repository implementation.
- Always add the `[IntentIgnore]` attribute to any method added.
- Always read the base repository methods to understand what is already provided and available before adding new methods.
- Optimize for query performance and maintainability when adding new methods.
- Never return tuples. If a complex return type is required, create a new Contract record in the Application layer and return that instead.
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}