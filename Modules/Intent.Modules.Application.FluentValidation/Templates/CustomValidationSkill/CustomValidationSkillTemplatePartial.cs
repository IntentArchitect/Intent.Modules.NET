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

namespace Intent.Modules.Application.FluentValidation.Templates.CustomValidationSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomValidationSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.FluentValidation.CustomValidationSkillTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CustomValidationSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", relativeLocation: "fluent-validation-custom-validation")
                .FromMarkdown($"""
---
name: fluent-validation-custom-validation
description: Use when writing or changing a custom or async validation rule in a FluentValidation validator (*Validator.cs).
template-id: {TemplateId}
---

# FluentValidation Custom Validation

Fill in a custom validation method in an existing validator. Its signature is generated; only the body is yours.

## Rules

- Mark the custom validation method with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body. Don't change its signature.
- Implement the rule the codebase states: XML documentation, comments, the domain model or tests. If nothing states it, ask the user what the rule is rather than writing a check that always passes.
- Report a failure with `validationContext.AddFailure(...)`, in the wording nearby validators use. Don't throw.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a validator.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation so the designer can map to it; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`ExistsBy…Async`, `IsAvailableAsync`), not for storage.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the validator are fine.
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
