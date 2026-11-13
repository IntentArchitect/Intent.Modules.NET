---
description: Capture a feature's intent and its contract — vision, jobs-to-be-done, user journeys, glossary, scope boundaries, success metrics, and precise testable EARS acceptance criteria with stable ids (requirements.md) — step 1 of Intent Architect's built-in, model-native SDD flow. Must ONLY be called explicitly by the user, or by the /sdd-extract or /sdd-run orchestrator the user invoked.
short-description: Capture a feature's intent and testable acceptance criteria — SDD step 1.
intent-updates: automatic
requiredTools:
  - read_spec
  - write_spec
  - ask_user_question
  - advance_spec_phase
group: Spec-driven development
---

# SDD — Requirements

You are running the **requirements** step of Intent Architect's built-in spec-driven flow. Produce a `requirements.md` for the feature the user described, then raise the phase gate so they can approve it.

The document has **two jobs**, and is finished only when both are done:

1. **Capture the intent** — why the feature exists, who it is for, its journeys, its vocabulary, its boundary, and how you'd know it worked. `/sdd-design` maps your glossary onto model elements and treats your non-goals as a hard boundary; `/sdd-verify` checks the journeys, not just the criteria.
2. **Capture the contract** — criteria **impossible to misread**: a developer who never spoke to the user builds the right thing from this document alone, and a tester turns every criterion into a pass/fail check **without a follow-up question**. A line that can be satisfied two ways is not done.

**This step is an interview, not a one-shot draft.** Grill the user until every material ambiguity is settled, _then_ write. Never pad the document with `[NEEDS CLARIFICATION]` markers or hand-wavy Assumptions for questions you never asked: **a correct run always ends with an empty Open Questions section.** If the user genuinely won't decide a point, pick a sensible default and record it under `## Assumptions`.

**Governance.** When `read_spec` returns `governanceDocuments` (e.g. Spec Kit's constitution), read them first; their rules are binding. If a requirement genuinely conflicts with one, say so and ask.

## References — read only what you need

Paths are relative to this skill's directory; `read_file` them.

| When | Reference |
| --- | --- |
| Steps 3–5 in full: framing questions, the whole negative-space probe, how to phrase questions | `references/interview.md` |
| Which framing sections to include; how to write journeys and metrics | `references/framing-sections.md` |
| The full Kiro-compatible document skeleton | `references/document-template.md` |
| Which MDX block carries which section, and the `.mdx` self-review — **read before drafting MDX** | `references/mdx-blocks.md` |
| Loaded by `/sdd-extract` to document an existing feature | `references/as-built.md` |

## The quality bar — every acceptance criterion is

- **Testable** — observable behaviour a test could assert. **Unambiguous** — one reasonable reading; no unclear pronouns, "should probably", "where appropriate".
- **Concrete** — actual limits, lengths, ranges, formats, units, allowed values, timeouts, and the **exact** response on success _and_ each failure.
- **Atomic** — one behaviour in exactly one EARS pattern: `THE <subsystem> SHALL …` · `WHEN <trigger>, THE … SHALL …` · `WHILE <state>, …` · `IF <condition>, THEN THE … SHALL …` · `WHERE <feature is present>, …`

Keep a criterion under about **300 characters and at most 2 conditions** (WHEN / IF / WHILE / WHERE); `write_spec` reports any over. Past that, split it into sibling criteria (`R1.3`, `R1.4`…) along its behaviours, each testable alone. A definition or scope note that only qualifies another criterion ("For R1.3, …") is not a behaviour: keep it in the criterion it qualifies, even if a little over. A deliberate refusal is a criterion (`SHALL NOT`); a limit of what the system can observe, which no test could assert, is an Assumption. Each unhappy path gets **its own** criterion.

**Banned unless quantified:** fast, quick, scalable, secure, robust, reliable, user-friendly, intuitive, efficient, appropriate, reasonable, properly, gracefully, as needed, etc., and so on, and/or — replace each with the number, list or rule it hides.

> ❌ "The system should validate the email and handle errors gracefully." ✅ "IF the submitted email does not match RFC 5322, THEN THE Customer_Service SHALL reject the request with a validation error naming the `email` field and SHALL NOT create the customer."

**No implementation or solution detail** — _what_ and _why_, never _how_: no frameworks, libraries, tech choices or model design (entities, properties, associations). Tool names, internal caps, animations and call batching belong in `design.mdx`; a number stays only when it is the user-visible contract ("within 2s"). **This binds the framing sections too.** The glossary pins a noun's _meaning_ and _cardinality_, never its fields, types, keys or associations — field detail goes in criteria.

**Framing sections** (above `## Requirements`) scale to the feature: include one only when it carries weight, never pad. **Introduction, Glossary and Non-Goals are always present.** **Key User Journeys (`UJ-N`) are present whenever there is a multi-step flow, a UI, an auth or hand-off assumption, or an ordering the user experiences** — skip only for a single-call library/API/CLI. Success Metrics with primaries include at least one **counter-metric**. Always:

- **Every journey is realized by at least one requirement, and every requirement names the journey(s) it realizes** (`**Realizes:** UJ-1`) or is explicitly infrastructure/NFR — otherwise it is scope creep; delete it.
- **The Glossary owns the vocabulary.** Every domain noun is defined there once, with cardinality where it matters ("one Wishlist per Customer"), and used **verbatim** everywhere. A synonym is a defect; a new noun joins the Glossary in the same pass.

## What to do

1. **Guard.** You always write a **Change**: one piece of work that, once done, is folded into the **Capabilities** that describe the system as it is. A Capability is never reopened here; new work on it is a new Change aimed at it. Call `read_spec()` with no slug to list the specs with their kind. For any whose title or description sounds related, call `read_spec(slug, artifact: "traceability")` and check its files and elements against the feature's area. When an existing Change or Capability already covers that area, stop and `ask_user_question`: extend that Change (reuse its slug), or start a new Change. When you go on, name every overlapping Capability as a spec link, `[[other-slug]]`, in the Introduction, saying what this Change does to it. For a new Change, pick a stable kebab-case `slug` (e.g. `add-customer-aggregate`).
2. **Ground yourself.** Skim the relevant designers (`getDesignerModelStructure()`, `searchElements()`, `getElementDetails()`) so you reuse existing vocabulary and don't re-specify what exists; the glossary agrees with the model's names wherever the concept is the same. **This step is read-only toward the model** — no designer scripts, module installs, Software Factory runs or code changes.
3. **Frame the feature**: why now, who and their jobs, the journeys (draft them for the user to correct), the vocabulary (reconcile synonyms now), the boundary ("what should this deliberately _not_ do?"), and the metric with its counter-metric.
4. **Probe the negative space** — for each that applies, specify a criterion or ask; never default silently: **the data each entity captures** (every field, required, type/format/bounds, entered or derived), actors & permissions, inputs, the exact response to each invalid input and failure, empty/boundary/max states, duplicates/concurrency/idempotency, data lifecycle, state transitions, non-functional targets where they matter, and **each journey's edge case** beat by beat.
5. **Grill the user in focused rounds until nothing material is unresolved.** **Default to `ask_user_question` with options** whenever answers can be enumerated — even by you (fields to capture as a multi-select, which drafted journeys are real, which non-goals to lock in); batch several per round. Free text only for genuinely open-ended questions, one or two at a time. **Any question good enough to write as a `[NEEDS CLARIFICATION]` marker is good enough to ask right now — and if it can be options, make it options.**
6. **Fold the answers in** as **concrete criteria** (real fields, values, bounds), not "the user chose X". `## Assumptions` holds only **low-stakes defaults** to rubber-stamp, never a dodged decision. `## Open Questions` **must be empty**; out-of-scope items go in `## Non-Goals`. A surviving question or marker means back to step 5.
7. **Write immediately — do NOT preview and ask for confirmation.** Once the self-review passes, call `write_spec(slug, "requirements", <content>, description=<summary>, kind: "change")`; review happens at the gate. It **defaults to `.mdx`**: load **authoring-rich-documents** first (never MDX from memory) and read `references/mdx-blocks.md`. `<…>` placeholders are never copied through, and a bare `<` or `{` in prose (`< 24 hours`, `List<Order>`) must be backticked or the document fails. `format: "md"` only if the user asked. `description`: one plain sentence (~10–15 words) from the Introduction — the spec card's subtitle. Use the **Kiro-compatible EARS structure** in `references/document-template.md`, and hold to:
   - **Current behaviour only:** the document states what the feature does now, never the history of a change — git holds the history.
   - **Ids:** Requirement 1 → `R1`, a criterion → `R1.2`. Refer to another requirement or criterion only by its full id, `R<requirement>` or `R<requirement>.<criterion>`, even within the same requirement (never "criterion <criterion>"), so the viewer links it. A requirement or criterion of another spec is a spec link anchored at its full id, `[[other-slug#R1.2]]`, never `other-slug R1.2`, which the viewer links to this spec's own R1. A criterion that no longer applies is **deleted**, and the criteria after it keep their numbers: a criterion number is **spent** and never renumbered or reused, because tests cite it (`[spec:<slug> R1.2]`). A requirement that goes away is deleted with its heading, and its number is **spent**: a new requirement takes one past the highest number ever used (a heading gap marks a removed one; if the last was removed, its id is in traceability or git history). `UJ-N` and `SM-N` follow the same rule. `write_spec` sorts requirements and their criteria into numeric order, never renumbering them, and lists requirement id problems: repeated numbers, order it couldn't fix, and "criterion N" citations. Fix each one it lists.
   - **Keep `### Requirement N: <title>` headings exact** — the catalog parser and the live coverage/verdict overlay key off them; a drifted heading silently loses both.
   - **Keep the section order:** framing sections above `## Requirements`, only `## Assumptions` and `## Open Questions` below — a requirement's body runs to the next heading, so a later section is absorbed into the last requirement and its drift hash.
   - A separate criterion for every invalid input, empty state, failure and journey edge case from step 4.
   - Omit any empty section, heading included.
   - **MDX:** existing model entities are inline `<ModelRef>`s carrying `application`. A related or overlapping spec is a spec link by its slug, `[[other-spec]]`, never a path. Every diagram node, table row, caption and callout back-references its criterion id; tables are captioned `Table N.N`, never a criterion id; nothing normative lives only in a block — a criterion cites every table.
8. **Offer a mockup when a requirement changes what a user sees** in a web app (a new or changed screen, panel, dialog or control). Ask once, with `ask_user_question`: mock up the affected screens now, or skip. If the `mockup-kit` skill is not available to you, skip this step. On yes, load **mockup-kit** and follow its Part 2: before/after screens pinned to the criteria, embedded next to them in `requirements.mdx` with `write_spec`, and shown in your reply. When the app has no `intent/.mockups/<app>/snapshot.json`, take a snapshot first with Part 1 if you can run the app's build; otherwise tell the user a developer must take it, and carry on without the mockup. A `snapshot.json` whose `styles.css` is missing is only unbuilt: if you can run commands, build it first with the script mockup-kit names (`--prepare-if-needed`); otherwise build the mockup anyway, and the `<Mockup>` block offers the reader a Build button. A mockup illustrates criteria and never replaces one: anything it shows that matters is already a criterion. Skip this step for a feature with no UI, and in as-built mode.

## Before you call write_spec — self-review

Fix anything that fails, then say in your closing message that the gate passed. For `.mdx`, also run the checklist in `references/mdx-blocks.md`.

- [ ] Every material ambiguity — including **what data each entity captures** — was resolved with the user.
- [ ] `## Open Questions` is **empty**; no `[NEEDS CLARIFICATION]` survives.
- [ ] Weight-carrying framing sections are specific, the rest **omitted, not padded**; Non-Goals and Glossary present.
- [ ] Every `UJ-N` is realized; every requirement names its journeys or is infrastructure/NFR.
- [ ] Every domain noun is in the Glossary once and used **verbatim**.
- [ ] Every `SM-N` names the requirements it validates; primary metrics have a counter-metric.
- [ ] No banned word unquantified; every criterion testable, unambiguous, bounded, atomic (one EARS pattern, ~300 chars, ≤ 2 conditions).
- [ ] Every step-4 failure/empty/boundary state and journey edge case has a criterion.
- [ ] No implementation or model-design detail anywhere, framing included.
- [ ] Current behaviour only; no id reused or renumbered; framing sections above `## Requirements`.

## As-built mode (extraction)

When `/sdd-extract` loads this skill, read `references/as-built.md` first: the user decided to document an existing feature, so its deltas override the interview rules above.

## Then raise the phase gate

Tell the user the requirements are ready — call out the Assumptions, the **journeys** and the **non-goals**, the things a reader most likely disagrees with. **Brief summary only**: never paste `requirements.md` into the chat; every surface reads it off disk.

Then, with no question left open, call **`advance_spec_phase(slug, artifact: "requirements", toPhase: "design")`** — the approval gate, from every surface. In-app it raises an approval card and blocks until the user decides; under `gates: auto` it advances immediately. On **reject**, revise with `write_spec` and call the gate again. Phases advance **one gate at a time** (the tool refuses a jump), so this moves the spec to **design** only — do not start design yourself.
