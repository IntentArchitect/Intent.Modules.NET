---
description: Route a styling/theme update into the right intake path — replace a supplied design.md, detect an out-of-band manual edit against the CSS, or interview the user when nothing has actually changed — then dispatch a coding sub-agent to run the project's `*-ux-theme-sync` skill so the CSS reflects the result. Use when the user asks to update styling, colours/branding, typography, or `design.md` itself.
contentHash: 0D246A200D4ED1401019C200EDCD1484EB5E820828A1D676F8147FA386D569F9
---
# Updating Styling

You are the **intake** step for a styling/theme change. Your only job is to get `design.md` into the state the user wants and then hand off to the project's own theme-sync skill — **never** translate design intent into CSS yourself; that belongs to the sync skill and is out of scope here.

## Find the pieces first

1. Locate the project's `design.md` (or equivalent design specification file) — typically `.agents/design.md` in an Intent Architect project, but confirm with `glob`/`list_directory` rather than assuming the path.
2. Locate the project's theme-sync skill — search for a `SKILL.md` whose name matches `*-ux-theme-sync` (e.g. `mudblazor-ux-theme-sync`). Read it in full so you know exactly which CSS file(s) it owns and which categories it extracts from `design.md` (palette, typography, spacing, radius, motion, component-level rules) — you reuse this same checklist below, both to check for drift and to structure a freshly-authored `design.md`.
3. If no `*-ux-theme-sync` skill exists in the project, **stop and tell the user** — this skill only orchestrates intake and hand-off; it does not itself know how to turn design intent into CSS.

## Preserve design.md's metadata block across every write

`design.md` may carry a leading YAML frontmatter block (`---` ... `---` at the very top) with fields like `name`, `paths`, `contentHash`. That block is not part of the design content — it's bookkeeping used elsewhere (e.g. `contentHash` decides whether the file gets treated as changed) — and it is easy to lose by accident: if you construct the new file purely from the supplied/authored content, the old frontmatter never enters the picture and silently disappears. Do not rely on remembering to "carry it over" while composing the new content — follow this as a literal, mandatory sequence instead, in every path that writes `design.md`:

1. **Before writing anything**, if `design.md` already exists on disk, read the file as it currently is — even in Path 1, even though you already have the replacement content in hand. Do this read as its own explicit step, not from memory of an earlier read in the conversation.
2. Check whether that on-disk content starts with a frontmatter block. If it does, copy that block out **verbatim** (byte-for-byte, including the `contentHash` value) — this is the block you will reuse, not one you reconstruct from memory.
3. Build the final file content as: `[verbatim old frontmatter block, if one existed]` + `[the new/supplied body]`. Never write the new body as a standalone file and never regenerate, recompute, or omit the frontmatter block as a side effect of pasting in new content.
4. If the on-disk file had **no** frontmatter block (or didn't exist yet), write the new body with no frontmatter block — do not invent one.
5. After writing, re-open the file you just wrote and confirm the frontmatter block (if any) is present and byte-identical to what you copied in step 2. Treat a missing or altered `contentHash` as a failed write — redo it.

## Decide which of the three intake paths applies

### Path 1 — A new design.md was supplied in the conversation

The user pasted, attached, or dragged in a full replacement this turn — new design.md content, a brand guide, a Figma export, etc. — directly in the chat.

- Overwrite the existing `design.md` **in full** with the supplied content. Do not merge, append, or diff against the old version — but do follow the metadata-preservation procedure above so the old frontmatter block (if any) survives the overwrite.
- If no `design.md` exists yet, create it at the location found in step 1 (or confirm the location with the user if it genuinely can't be inferred).
- Then go straight to **Dispatch**.

### Path 2 — No new content supplied, but design.md may have been edited out-of-band

Nothing was pasted this turn, but a `design.md` already exists on disk. Check whether it has drifted from what the CSS currently expresses:

- Read `design.md` and the CSS file(s) the theme-sync skill owns (from step 2 above).
- Compare the values `design.md` states — palette, typography, spacing/radius, component rules — against what the CSS tokens actually implement.
- **If they differ**, someone edited `design.md` directly without syncing the CSS yet. Treat the on-disk `design.md` as authoritative — it already reflects the desired end state, so no further user input is needed. Report the specific deltas you found (e.g. `"design.md now specifies #FF5733 as primary; ux-tokens.css still has #0EA5E9"`) so the user can see what's about to change, then go straight to **Dispatch**.
- **If they match** (or `design.md` doesn't exist at all), nothing has actually changed — fall through to Path 3.

### Path 3 — Nothing supplied, nothing changed

Ask the user how they want to supply the styling information. Use `ask_user_question` with these options:

- **Paste it in as text** — the user supplies the design directly (prose, a brand guide, hex values, whatever they have) in their next message; write it into `design.md`, using the theme-sync skill's extraction categories as a checklist for what's missing.
- **A few high-level questions** — a short, focused round covering the essentials only: primary brand colour(s), overall mood/tone, light vs dark default, and font preference (or "no preference"). Fill every category the theme-sync skill extracts with sensible, internally-consistent defaults derived from those answers, and record each filled-in gap as an assumption in `design.md` so the user can correct it later.
- **A detailed set of questions** — a thorough interview across every category the theme-sync skill extracts: full palette (primary/secondary/accent plus hover/pressed variants), semantic colours per surface layer, status colours, typography (families, full type scale, weights, line-heights), spacing scale, border-radius tiers, motion (durations/easing), and component-level rules (buttons, cards, dialogs, tables, forms, navigation, etc.). Prefer `ask_user_question` whenever a question has a small set of plausible answers; use free text only for genuinely open-ended ones (e.g. "describe the overall aesthetic in your own words"). Batch related questions into focused rounds rather than asking one at a time.

Whichever option the user picks, fold the answers into `design.md` — reusing its existing structure/tone if a prior version exists, so the diff the sync skill sees stays legible. Follow the metadata-preservation procedure above when writing it to disk, then go straight to **Dispatch**.

## Dispatch

Once `design.md` reflects the desired end state (via whichever path applied), dispatch a coding sub-agent to execute the `*-ux-theme-sync` skill you found in step 2. Give it the `design.md` path, the theme-sync skill's name/location, and instruct it to run that skill's process end-to-end (read every file it requires, extract intent, map to tokens, apply changes, work through its own Definition of Done) rather than re-deriving the CSS mapping yourself. Do not hand-edit any CSS file in this skill — that translation is entirely the sync skill's responsibility.

- *Theme consistency invariant — always pass this to the sub-agent:** unless the user explicitly asked for it this turn, or `design.md` itself explicitly calls for asymmetric treatment (e.g. a header that deliberately stays dark as an accent regardless of theme), each theme variant must come out internally consistent — light theme → light header/nav/surfaces, dark theme → dark header/nav/surfaces. This matters most whenever a design.md swap flips the default polarity (dark-default ↔ light-default): mechanically renaming `[data-theme="..."]` selectors and swapping token values is not sufficient on its own, because component-level rules that bake in a hardcoded colour literal (a header gradient, a hero overlay) instead of resolving through `var(...)` tokens won't track the flip, and can leave the header showing the *previous* theme's polarity even though the rest of the page flipped correctly. Tell the sub-agent to explicitly check header/nav/toolbar rules for this after any polarity change, not just rename selectors and stop.

Wait for the sub-agent to finish before reporting back.

## Then stop

Tell the user: which path you took (supplied replacement / detected drift / interviewed them), the deltas or answers that drove the change, and the sync sub-agent's summary of what it actually touched. If the sub-agent reported anything it couldn't resolve (e.g. a missing CSS file, a conflicting value), surface that too instead of silently accepting a partial result.
