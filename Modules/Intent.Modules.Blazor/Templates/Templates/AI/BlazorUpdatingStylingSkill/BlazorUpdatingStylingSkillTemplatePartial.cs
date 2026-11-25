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

namespace Intent.Modules.Blazor.Templates.Templates.AI.BlazorUpdatingStylingSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BlazorUpdatingStylingSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.AI.BlazorUpdatingStylingSkillTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BlazorUpdatingStylingSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", "md", "updating-styling")
                .FromMarkdown("""
                    ---
                    description: Route a styling/theme update into the right intake path — replace a supplied design.md, detect an out-of-band manual edit against the CSS, or interview the user when nothing has actually changed — then dispatch a coding sub-agent to run the project's `*-ux-theme-sync` skill so the CSS reflects the result. Use when the user asks to update styling, colours/branding, typography, or `design.md` itself.
                    ---

                    # Updating Styling

                    You are the intake step for a styling change: get `design.md` into the state the user wants, then hand off to the project's theme-sync skill. Never turn design intent into CSS or edit a CSS file yourself; that is the sync skill's job.

                    ## 1. Find the pieces

                    1. `design.md` (or the equivalent design spec), typically `.agents/design.md`. Confirm the path with `glob` or `list_directory`; don't assume it.
                    2. The theme-sync skill: a `SKILL.md` named `*-ux-theme-sync` (e.g. `mudblazor-ux-theme-sync`). Read it in full for the CSS files it owns and the categories it extracts (palette, typography, spacing, radius, motion, component rules). Those categories are your checklist below.
                    3. No such skill: stop and tell the user.

                    ## 2. Writing design.md

                    `design.md` may start with a YAML frontmatter block (`name`, `paths`, `contentHash`, ...). It is bookkeeping, not design content (`contentHash` decides whether the file counts as changed), and it is lost whenever the new file is built only from new content. Every time you write `design.md`:

                    1. If it exists, re-read it from disk now, as its own step: in Path 1 too, never from memory.
                    2. If it starts with frontmatter, copy that block byte for byte, `contentHash` included.
                    3. Write that block followed by the new body. With no old block, or no file, write the body alone. Never invent, recompute or drop a block.
                    4. Re-open the written file and check the block is byte-identical. A missing or altered `contentHash` is a failed write: redo it.

                    ## 3. Pick the intake path

                    **Path 1: the user supplied a design this turn** (a pasted or attached design.md, brand guide, Figma export). Replace `design.md` in full with it: no merge, append or diff. If none exists, create it at the location from step 1, asking the user only if that can't be inferred. Then dispatch.

                    **Path 2: nothing supplied, and `design.md` exists.** Compare the values it states (palette, typography, spacing and radius, component rules) with what the sync skill's CSS files implement.
                    - They differ: `design.md` was edited directly and is authoritative, so ask nothing. Report the deltas (e.g. "design.md now specifies #FF5733 as primary; ux-tokens.css still has #0EA5E9"), then dispatch.
                    - They match, or there is no `design.md`: go to Path 3.

                    **Path 3: nothing supplied, nothing changed.** Use `ask_user_question` to ask how they want to supply the styling:
                    - **Paste it as text:** write their next message into `design.md`, using the sync skill's categories to spot what's missing.
                    - **A few high-level questions:** primary brand colour(s), mood or tone, light or dark default, font preference (or none). Fill every category with internally consistent defaults derived from the answers, and record each filled gap as an assumption in `design.md`.
                    - **A detailed set of questions:** every category the sync skill extracts. Full palette (primary, secondary, accent, with hover and pressed), semantic colours per surface layer, status colours, typography (families, scale, weights, line heights), spacing, radius tiers, motion, and component rules (buttons, cards, dialogs, tables, forms, navigation). Batch related questions into focused rounds. Use `ask_user_question` where the answers are a small set, free text only for open-ended ones.

                    Fold the answers into `design.md`, keeping an existing version's structure and tone so the sync skill sees a legible diff. Then dispatch.

                    ## 4. Dispatch

                    Dispatch a coding sub-agent to run the theme-sync skill end to end: read every file it requires, extract, map, apply, and work through its own Definition of Done. Don't re-derive the CSS mapping yourself. Give it the `design.md` path, the skill's name and location, and this invariant:

                    > Unless the user asked for it this turn, or `design.md` calls for asymmetry (e.g. a header that stays dark as an accent), each theme variant must be internally consistent: light theme, light header, nav and surfaces; dark theme, dark ones. After a polarity flip, renaming `[data-theme]` selectors is not enough: check header, nav and toolbar rules for hardcoded colour literals (a header gradient, a hero overlay) that don't go through `var(...)` and so won't follow the flip.

                    Wait for it to finish.

                    ## 5. Report

                    Say which path you took (supplied, drift, or interview), the deltas or answers behind the change, and the sub-agent's summary of what it touched. Surface anything it couldn't resolve (a missing CSS file, a conflicting value) rather than accepting a partial result.
                    """);
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
