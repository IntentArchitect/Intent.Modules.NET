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

namespace Intent.Modules.Application.Mediator.Templates.CommandHandlerSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommandHandlerSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Mediator.CommandHandlerSkill";

        public const string SkillName = "mediator-command-handler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CommandHandlerSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            var consolidated = ExecutionContext.ConsolidateIntoSingleFile();
            var handlerExample = consolidated
                ? "the *CommandHandler class inside a *Command.cs file"
                : "*CommandHandler.cs";
            var handlerFiles = consolidated
                ? "`*Command.cs` files (each command's `*CommandHandler` class is in the command's own file)"
                : "`*CommandHandler.cs` files";
            var fileLayoutRule = consolidated
                ? "\n- Each command, its handler and its validator share one file, the command's own `*Command.cs`. Keep them there: never move the handler or validator into a file of its own."
                : "";
            MarkdownFile = new MarkdownFile($"SKILL", relativeLocation: SkillName)
                .FromMarkdown($$"""
---
name: {{SkillName}}
description: Implements the whole body of a Mediator command handler from the interactions modelled on the command. MUST load before creating, editing or revisiting any command handler e.g. {{handlerExample}}, and after any Software Factory run that generates a new command handler stub.
template-id: {{TemplateId}}
---

# Mediator Command Handler

Write the `Handle` body of a command handler. The generator only emits a stub that throws `NotImplementedException`; the associations modelled on the command in the Services designer are the specification.

## Finding the handlers to write

- After every Software Factory run, search the Application project for {{handlerFiles}} whose `Handle` still throws `NotImplementedException`, and write every one.
- When you add, remove, reorder or change an association on a command, update its handler in the same piece of work. No generated file changes, so nothing else will remind you.

## Rules

- Mark `Handle` with `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body. Never change its name, return type or parameters, and keep it `async` even when it awaits nothing.{{fileLayoutRule}}
- The handler implements `ICommandHandler<TCommand, TResponse>` from the `Mediator` library and returns `ValueTask<TResponse>`. A command with no response returns `ValueTask<Unit>` and ends every normal path with `return Unit.Value;`.
- Inject only Domain and Application types. No Entity Framework, Dapper, `DbContext`, SQL client or other infrastructure type belongs in a handler.
- Load aggregates through their repository interface and change them through their domain behaviour, not by setting state ad hoc. Let entities raise domain events themselves; never dispatch one by hand.
- Follow the nearest similar handler for result types, error handling and the not-found pattern.
- Never aggregate in memory (Count, Sum, GroupBy and the like). Call a repository method that returns the result.
- When a repository method is missing: if a designer will use it, for example mapped from a command or query, model it as a repository operation; otherwise add it by hand to the repository interface and the repository. Name it for the domain intent (`FindByIdAsync`, `ExistsBy…Async`), not for storage, and keep storage details such as includes, joins and SQL out of the handler.
- Publish or send messages only through the message bus (usually `IMessageBus`). The pipeline flushes it after the unit of work commits, so never flush it yourself and add no transaction or outbox code.
- Pass the `cancellationToken` to every async call.
- Private helper methods in the handler are fine when they make the flow easier to read.

## Read the model first

Load the `exploring-the-model` skill and call `run_designer_query` with `getElementDetails` on the command. Implement its associations in the order the designer shows them; later steps may use what earlier ones loaded or created. Re-read them whenever you revisit the handler.

Field mappings are not modelled. Ignore any minimal mapping or missing-mapping warning on an association, and add none: decide which command field fills which attribute, constructor argument or message property by name and meaning.

- Create Entity: construct the entity through its constructor or factory with the matching command fields, and add it through its repository.
- Update Entity: load it by the identity on the command, fail with the not-found pattern when it is missing, then apply the command's fields, through domain methods where they exist.
- Delete Entity: load it by the identity on the command, fail with the not-found pattern when it is missing, then remove it through the repository.
- Query Entity: load the matching instances through the repository for the steps that follow.
- Call Service Operation (Invoke): call the operation through its Application interface. When it targets a command or query, dispatch it with `Mediator.ISender`.
- Publish Integration Event or Send Integration Command: populate every property of the message from the command and earlier results, then publish or send it through the message bus.
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
