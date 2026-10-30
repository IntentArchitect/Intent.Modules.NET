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

The document has **two jobs**, and it is only finished when both are done:

1. **Capture the intent** — why this feature exists, who it is for, the journeys it enables, the vocabulary it uses, where its boundary lies, and how you'd know it worked. Everything downstream inherits these: `/sdd-design` maps your glossary onto model elements and treats your non-goals as a hard boundary; `/sdd-verify` checks the journeys, not just the criteria. A criteria list alone cannot supply any of it.
2. **Capture the contract** — acceptance criteria that are **impossible to misread**. A developer who never spoke to the user must be able to build the right thing from this document alone, and a tester must be able to turn every acceptance criterion into a pass/fail check **without asking a single follow-up question**. If a line can be satisfied two different ways, it is not done.

**Resolve before you write — this step is an interview, not a one-shot draft.** Grill the user until every material ambiguity is settled, _then_ write `requirements.md`. The single biggest failure of this step is shipping a document padded with `[NEEDS CLARIFICATION]` markers and hand-wavy Assumptions for questions you never actually asked. If you can see an open question, the answer is to **put it to the user** — not to write it into the doc and move on. **This step may not finish with any question unanswered:** a correct requirements run **always** ends with an **empty** Open Questions section because you asked everything. If the user genuinely won't decide a point, pick a sensible default and record it under `## Assumptions` — never leave the question open.

## Honour the solution's governance

When the spec's framework declares governance documents — Spec Kit's `.specify/memory/constitution.md`, say — `read_spec` returns them as `governanceDocuments` and the Specs panel lists them. **Read them first and treat their rules as binding on what you produce.** They are the user's standing constraints, not background reading. If a requirement and a governance rule genuinely conflict, say so and ask rather than silently picking one.

## The quality bar — every acceptance criterion is

- **Testable** — phrased as observable behaviour a test could assert. "Think like a tester": what input produces what observable result?
- **Unambiguous** — exactly one reasonable interpretation. No pronouns with unclear referents, no "should probably", no "where appropriate".
- **Measurable / concrete** — carries the actual bounds: limits, lengths, ranges, formats, units, allowed values, timeouts, and the **exact** response on success _and_ on each failure.
- **Atomic** — one behaviour (one response), written in exactly one EARS pattern:
  - _Ubiquitous_ — `THE <subsystem> SHALL <response>.`
  - _Event_ — `WHEN <trigger>, THE <subsystem> SHALL <response>.`
  - _State_ — `WHILE <state>, THE <subsystem> SHALL <response>.`
  - _Unwanted_ — `IF <condition>, THEN THE <subsystem> SHALL <response>.`
  - _Optional_ — `WHERE <feature is present>, THE <subsystem> SHALL <response>.`

  Keep a criterion under about **300 characters and at most 2 conditions** (WHEN / IF / WHILE / WHERE). Past that, split it into sibling criteria (`R1.3`, `R1.4`…) along its behaviours, so each sibling can be tested on its own. A definition or scope note that only qualifies another criterion ("For R1.3, …") is not a behaviour: keep it in the criterion it qualifies, even if that runs a little over the limit. What the system deliberately refuses is still a criterion (`SHALL NOT`), but a limit of what it can observe or check, which no test could assert, belongs under Assumptions. `write_spec` reports any criterion over the limit. Each unhappy path gets **its own** criterion, not another clause on the happy one.

**Banned unless immediately quantified:** fast, quick, scalable, secure, robust, reliable, user-friendly, intuitive, efficient, appropriate, reasonable, properly, gracefully, as needed, etc., and so on, and/or. Each of these hides a decision — replace it with the number, list, or rule it stands for.

> ❌ "The system should validate the email and handle errors gracefully." ✅ "IF the submitted email does not match RFC 5322, THEN THE Customer_Service SHALL reject the request with a validation error naming the `email` field and SHALL NOT create the customer."

**No implementation or solution detail.** Requirements state _what_ the system does and _why_ — never _how_ (no frameworks, libraries, tech choices, and no model design: entities, properties, associations). The design step owns the _how_. A criterion states the observable outcome: tool names, internal caps, animations and call batching belong in `design.mdx`. A number stays only when it is the user-visible contract ("within 2s"). **This rule binds the framing sections too** — a vision or a journey that names a screen technology, a table, or a class has become a design document. The glossary is the one place a domain noun is pinned down, and it pins down its _meaning_ and _cardinality_, never its fields, types, keys or associations; field-level detail belongs in acceptance criteria.

## The framing sections — scale them to the feature

The sections above `## Requirements` are what turn a criteria list into a spec someone can act on. They are **not boilerplate to fill in**: an empty-calorie journey or a metric nobody will measure is worse than the omitted section, because a reader has to work out it says nothing. Include a section when it carries weight, omit it entirely when it doesn't — and never pad one to look complete.

| Section                            | Include it                                                                                                                                                                                                                                                                                                                                                                               |
| ---------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Introduction**                   | Always.                                                                                                                                                                                                                                                                                                                                                                                  |
| **Vision**                         | When the _why_ isn't self-evident from the Introduction — a new product surface, a behaviour change users will notice, a strategic bet. Skip for a plumbing change.                                                                                                                                                                                                                      |
| **Target Users & Jobs To Be Done** | When more than one kind of user is involved, or the motivation shapes the behaviour. Skip when there is exactly one obvious actor doing one obvious thing.                                                                                                                                                                                                                               |
| **Key User Journeys** (`UJ-N`)     | Whenever there is a multi-step flow, a UI, an authentication or hand-off assumption, or an ordering the user experiences. **This is the highest-value section** — the beats force specificity a criteria list never surfaces (a journey's edge case is usually where the "adding it twice is a no-op" rule comes from). Skip only for a single-call library/API/CLI change with no flow. |
| **Glossary**                       | Always — even two or three terms. `/sdd-design` maps these nouns onto Domain elements, so this section is what stops the model and the spec drifting apart in vocabulary.                                                                                                                                                                                                                |
| **Non-Goals**                      | Always, even if it's two bullets. Cheapest scope-creep prevention there is: it stops the "let me also add this nearby thing" failure mode at design, tasks _and_ code.                                                                                                                                                                                                                   |
| **MVP Scope**                      | When the feature ships in stages, or something has been deliberately deferred. Fold into the Introduction's out-of-scope note when it's a single bullet.                                                                                                                                                                                                                                 |
| **Success Metrics** (`SM-N`)       | When someone will ask "did this work?" — a user-facing capability, a performance or adoption goal. Skip for internal plumbing. When you include primaries, include at least one **counter-metric**: it is what stops the design optimizing the wrong thing.                                                                                                                              |

Rules the framing sections must hold to:

- **Journeys are named-persona narratives, not restated criteria.** One line of title, then persona + context, entry state, a 3–5 beat path, the climax (the moment value lands and how the user knows), the resolution, and — where there is one — a real edge case. Number them `UJ-1 … UJ-N`. Where the journey is genuinely just a job-to-be-done restated, one sentence (`{Persona}, {context}, {what they do and why}.`) is the right length; don't inflate it.
- **Every journey must be realized by at least one requirement, and every requirement should name the journey(s) it realizes** (`**Realizes:** UJ-1`). A journey with no requirement is a gap in the contract; a requirement realizing no journey is either infrastructure/NFR (fine — say so) or scope creep (delete it).
- **The Glossary owns the vocabulary.** Every domain noun the document uses is defined there once, with its relationship to the other terms and its cardinality where it matters ("one Wishlist per Customer"). Thereafter the whole document — journeys, requirements, criteria, metrics — uses those terms **verbatim**. Introducing a synonym anywhere is a defect. If a requirement introduces a new noun, add it to the Glossary in the same pass.
- **Metrics cross-reference requirements.** `**SM-1**: <metric, definition, target>. Validates R1, R3.` Counter-metrics say what they counterbalance.

## What to do

1. **Guard.** You always write a **Change**: one piece of work that, once done, is folded into the **Capabilities** that describe the system as it is. A Capability is never reopened here; new work on it is a new Change aimed at it. Call `read_spec()` with no slug to list the specs with their kind. For any whose title or description sounds related, call `read_spec(slug, artifact: "traceability")` and check its files and elements against the feature's area. When an existing Change or Capability already covers that area, stop and `ask_user_question`: extend that Change (reuse its slug), or start a new Change. When you go on, name every overlapping Capability as a spec link, `[[other-slug]]`, in the Introduction, saying what this Change does to it. For a new Change, pick a stable kebab-case `slug` (e.g. `add-customer-aggregate`).
2. **Ground yourself first.** Skim the relevant designers read-only (`getDesignerModelStructure()`, `searchElements()`, `getElementDetails()`) so requirements reuse the existing domain vocabulary and don't re-specify what already exists. The glossary you write should agree with the names already in the model wherever the concept is the same one. **This whole step is read-only toward the model** — inspect it, never mutate it: no designer scripts, no module installs, no Software Factory runs, no code changes. Mutation starts at implementation.
3. **Frame the feature before you specify it.** Work out — and where you can't, ask — the following. This is the part a criteria-only draft skips, and it is what makes the rest specific:
   - **Why now, and what changes for the user** — the one or two paragraphs of vision. If you can't say what is better afterwards, you don't yet know what to build.
   - **Who** — each kind of user and the job they're hiring this feature to do.
   - **The journeys** — for each job, the actual scene: where they start, what they do, when they know it worked, what they're left with, and what goes wrong. Draft these and put them to the user to correct: people spot a wrong journey far faster than they spot a missing criterion.
   - **The vocabulary** — every domain noun, defined once, with cardinality. Reconcile synonyms **now**, before they multiply through the document.
   - **The boundary** — the non-goals, and (where it applies) what is in and out of the MVP. Ask explicitly: "what should this deliberately _not_ do?" is one of the highest-yield questions in this step.
   - **How you'd know it worked** — the metric, its target, and the counter-metric that stops it being gamed.
4. **Probe the negative space.** Most vagueness comes from questions nobody asked. Walk this checklist against the feature and, for each item that applies, either specify a concrete criterion or ask the user — never let it default silently:
   - **The data each thing captures** — for every entity the feature introduces or touches (e.g. a "quote"), pin down _exactly_ what it records: each field, whether it's required, its type/format/ bounds, and whether it's entered or derived. "Manage quotes" without stating what a quote stores (number, buyer, line items, status, validity/expiry, totals, tax, notes, timestamps, …) is **not specified** — resolve it and turn the answer into acceptance criteria.
   - **Actors & permissions** — who can do this; what each role may and may not do.
   - **Inputs** — for every field: type, required/optional, format, length/range, default.
   - **Validation & failure behaviour** — the _exact_ response to each invalid input and each failure (error shown, state left behind, what is _not_ done). "Handle errors" is not an answer.
   - **Empty / boundary / max states** — nothing yet, one, the maximum, over the maximum.
   - **Duplicates, concurrency, idempotency** — repeat submissions, simultaneous edits.
   - **Data lifecycle** — create/update/delete semantics, soft- vs hard-delete, retention, audit history.
   - **State transitions** — the allowed states and what triggers each move.
   - **Non-functional (only where it matters)** — performance with an actual target, authorization, volume.
   - **Each journey's edge case** — walk every `UJ-N` beat by beat and ask what happens when that beat fails. Journeys and this checklist feed each other: a beat with no criterion is an unspecified behaviour.
5. **Grill the user — in focused rounds — until nothing material is unresolved.** For every unknown the framing and the probe surfaced, put the question to the user; when their answers raise new unknowns, ask those too. Keep looping, and do **not** advance to writing while a material question is still open. **Default to the `ask_user_question` tool** — clicking an option is far cheaper for the user than typing:
   - **Prefer `ask_user_question` (options).** Whenever a question has a small set of plausible answers — even if _you_ have to enumerate them — ask it as options so the user just picks. This covers most requirements decisions: soft- vs hard-delete; standalone invoice vs linked order; sync vs async; **which fields an entity captures** (offer them as a multi-select checklist); which statuses/transitions apply; **which of the journeys you drafted are the real ones**; **which candidate non-goals to lock in**. If you can frame it as a choice, use the tool. Batch several such questions in one round.
   - **Free-text (a plain chat message) is the fallback ONLY for a genuinely open-ended question** that has no clear set of answers (e.g. "describe the business goal", "state the exact validity-period rule"). Keep these to **one or two at a time** — text answers are expensive.

   Rule of thumb: **any question good enough to write as a `[NEEDS CLARIFICATION]` marker is good enough to ask right now — and if it can be options, make it options.**
6. **Only once everything material is answered, fold it in.** When you write:
   - Turn each resolved answer into **concrete acceptance criteria** (real fields, values, bounds) — not a restatement that "the user chose X".
   - `## Assumptions` holds **low-stakes defaults** you picked for the user to rubber-stamp (e.g. money is a decimal) — stated in plain language, never a material decision you dodged.
   - `## Open Questions` **must be empty when you finish** — this step may not conclude while any question about the feature is unanswered. Every question you surface goes to the user and gets an answer; fold each answer into acceptance criteria. If the user genuinely can't or won't decide one, do **not** leave it open — pick a sensible default and record it under `## Assumptions` for them to correct. Something truly outside this feature's scope belongs in `## Non-Goals`, not here. A non-empty Open Questions section (or any surviving `[NEEDS CLARIFICATION]` marker) means you are not done asking — go back to step 5.
7. **Write immediately — do NOT preview and ask for confirmation.** Call `write_spec` right after the self-review gate passes. Do not present a draft in-chat and ask "shall I save this?" — the review happens _after_ it is written, at the phase gate you raise in the final step. Write the requirements via `write_spec(slug, "requirements", <content>, description=<summary>, kind: "change")`. **It defaults to `.mdx`**, so load the **authoring-rich-documents** skill before you draft — do not author MDX from memory. Which block carries which section is prescribed below under **Write it as MDX**; follow it rather than deciding block-by-block as you draft. The EARS structure below is unchanged — it is prose, and prose is still plain Markdown inside an `.mdx` file. Two MDX rules bite here: the skeleton's `<…>` placeholders are instructions to replace, never text to copy through, and any surviving bare `<` or `{` in prose (`< 24 hours`, `List<Order>`) must be backticked or it fails the whole document. Pass `format: "md"` only if the user asked for plain markdown. Also pass a `description`: a single plain-language sentence (~10-15 words) distilled from the Introduction that says what the feature does (it becomes the spec card's always-visible subtitle, so keep it concise and jargon-free). Use this **Kiro-compatible EARS** structure, dropping whichever framing sections don't carry weight (see the scope dial above):

   ```markdown
   # Requirements Document

   ## Introduction

   <1–2 paragraphs: what the feature is, who it serves, and what's explicitly out of scope.>

   ## Vision

   <1–2 paragraphs: what this changes for the user and why it matters. Stands alone.>

   ## Target Users & Jobs To Be Done

   - **<user type>** — <the job they're hiring this feature to do, and the context they do it in>.

   ## Key User Journeys

   - **UJ-1. <one-line title — a named persona doing the thing>.**
     - **Persona + context:** <one line, grounded enough to explain the why>
     - **Entry state:** <authenticated? on which surface? arriving from where?>
     - **Path:** <3–5 concrete beats — the actions, screens and decisions>
     - **Climax:** <the moment value is delivered, and how the user knows>
     - **Resolution:** <the state they're left in, and what's next>
     - **Edge case:** <one real failure mode, and what the user does next>

   ## Glossary

   - **<Term>** — <definition; relationship to other terms; cardinality where it matters>.

   ## Non-Goals

   - <what this feature explicitly is not, and will not do>

   ## MVP Scope

   ### In Scope

   - <crisp bullets>

   ### Out of Scope for MVP

   - <deferred item — with a one-line reason where the reason matters>

   ## Success Metrics

   **Primary**

   - **SM-1**: <metric — definition and target>. Validates R1, R2.

   **Counter-metrics (do not optimize)**

   - **SM-C1**: <what must not be traded away to move SM-1>. Counterbalances SM-1.

   ## Requirements

   ### Requirement 1: <short title>

   **User Story:** As a <role>, I want <capability>, so that <benefit>.

   **Realizes:** UJ-1

   #### Acceptance Criteria

   1. THE <subsystem> SHALL <behaviour, with concrete bounds / formats / limits>.
   2. WHEN <trigger>, THE <subsystem> SHALL <response>.
   3. IF <condition>, THEN THE <subsystem> SHALL <response> [and SHALL NOT <forbidden effect>].

   ## Assumptions

   - <a default you chose because the description was silent — stated so the user can correct it>

   ## Open Questions

   <!-- Normally empty (and omitted). This step may not finish with an unanswered question — ask the user,
         or fold an undecided point into ## Assumptions as a chosen default. -->
   ```

   - **Current behaviour only:** the document states what the feature does now, never the history of a change — git holds the history.
   - **Ids:** Requirement 1 → `R1`, a criterion → `R1.2`. The design and tasks reference these ids for traceability. Refer to another requirement or criterion only by its full id, `R<requirement>` or `R<requirement>.<criterion>`, even within the same requirement (never "criterion <criterion>"), so the document viewer can link it. A criterion that no longer applies is **deleted**, and the criteria after it keep their numbers: a criterion number is **spent** and never renumbered or reused, because tests cite it (`[spec:<slug> R1.2]`). A requirement that goes away is deleted with its heading, and its number is **spent**: a new requirement takes one past the highest number ever used (a gap in the headings marks a removed one; when the last was removed, its id is still in the spec's traceability or git history). `UJ-N` and `SM-N` follow the same rule as requirement ids.
   - Keep `### Requirement N: <title>` headings exact — the catalog parser keys off them, and so does the rendered document: Intent Architect decorates each requirement heading with that requirement's LIVE coverage chip (covered / uncovered / stale / broken), its verify verdict, and an expandable tree of the model elements and files linked to it. A heading that drifts from the convention silently loses its overlay, so `requirements.md` stops being the dashboard it is meant to be.
   - **Keep the section order.** Every framing section goes **above** `## Requirements`; only `## Assumptions` and `## Open Questions` go below it. The catalog parser takes a requirement's body as everything up to the next `### Requirement N` heading, so a section placed after the requirements is silently absorbed into the last one's statement — and into the drift hash that marks its traceability links stale.
   - Cover the **happy path and the unhappy paths** in the criteria: a separate criterion for each invalid input, empty state, and failure you surfaced in step 4 — including each journey's edge case.
   - Omit any framing section, `## Assumptions` or `## Open Questions` that would be empty — omit the heading too, don't leave a stub.

## Write it as MDX — which block carries which section

The skeleton and heading conventions above are **fixed** — the catalog parser and the live coverage overlays key off them, so never restructure a heading to suit a block. What MDX adds is the form each section's content takes _within_ those headings. Reach for a block **where the content shape calls for it**, not to hit a count: a genuinely narrative section that stays prose is a judgement, not an omission.

| Section                                               | Carried by                                                                                                                                                                                                                                                                                                               |
| ----------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Introduction**, **Glossary**                        | Prose and bullets as the skeleton has them — but **every entity that already exists in the model** is an inline `<ModelRef>` rather than bare text, so the reader sees at a glance which nouns are new and which already exist (and whether they still do). A Glossary term the model doesn't have yet stays plain text. A related or overlapping spec is a spec link by its slug, `[[other-spec]]`, never a path (see "Linking documents" in authoring-rich-documents). |
| **Key User Journeys**                                 | One `` ```mermaid `` `flowchart TD` **per journey**, placed after that journey's bullets, encoding the same beats — entry state, each path beat, the climax, the resolution, **and the edge case as a branch**. The bullets stay: the diagram is the shape, the bullets are the detail, and a reader needs both.         |
| **A validation gauntlet or a lifecycle**              | `<Diagram>` where the layout genuinely needs more than mermaid gives — a multi-stage validation funnel, a state machine with parallel guards. Otherwise a `` ```mermaid `` `stateDiagram-v2` is less to maintain.                                                                                                        |
| **A requirement's bounds**                            | A table immediately **before** the criteria it quantifies — field, type, required, format, length/range, default. This is the step-4 "inputs" probe rendered rather than dissolved into a dozen SHALL clauses.                                                                                                       |
| **A requirement's unhappy paths**                     | A failure-matrix table — condition, exact response, state left behind, what is NOT done — wherever the failures are enumerable. Each failure still gets its own criterion; the table is what makes a gap visible.                                                                                                            |
| **A risk or a decision that qualifies a requirement** | `<Callout tone="risk">` / `<Callout tone="decision">` placed **after** the criteria it qualifies, never before them — a callout above the criteria reads as the requirement itself.                                                                                                                                      |
| **Assumptions**                                       | A `<Checklist>`, so each assumption can be worked through and its tick persists across opens.                                                                                                                                                                                                                            |
| **Open Questions** (rare — normally empty)            | `<QuestionForm>`. Its answers post straight back into this conversation, so anything that surfaces _after_ the write still gets resolved rather than sitting there. It does not license finishing with open questions.                                                                                                   |

**The convention that makes every later step mechanical: back-reference the criterion.** Every diagram node, table row, caption and callout names the criterion id it belongs to (`R2.3`), so `/sdd-design` can map a block to the requirement it realizes and `/sdd-verify` can tell whether a block was covered. A block that floats free of the ids is decoration.

**Number every table `Table N.N`, never with a criterion id.** Caption a table `Table 13.1 — what each permission mode gates` (the requirement's number, then its own running index), and cite it in prose as `Table 13.1`; the viewer links that cite to the table. A caption written as a criterion id (`R13.1 — …`) collides with the real criterion `R13.1`, so a cite of it can't tell the two apart.

**Nothing normative may live only inside a block.** The criteria are the contract, so every table must be cited by a criterion. A bounds table is cited by reference ("THE Customer_Service SHALL reject a field outside the limits in Table 2.1"), not restated bound by bound. A table that no criterion cites is one `/sdd-verify` has nothing to check against.

## Before you call write_spec — self-review

Run this gate over the draft and fix anything that fails; then state in your closing message that it passed:

- [ ] I interviewed the user and resolved every material ambiguity — including **what data each entity captures** — rather than deferring it.
- [ ] `## Open Questions` is **empty** — every question I surfaced was put to the user and answered, or (where the user wouldn't decide) resolved into an `## Assumptions` default. No `[NEEDS CLARIFICATION]` marker survives.
- [ ] The framing sections that carry weight are present and specific; the ones that don't are **omitted, not padded**. Non-Goals and a Glossary are there either way.
- [ ] Every `UJ-N` is realized by at least one requirement, and every requirement either names the journey(s) it realizes or is explicitly infrastructure/NFR.
- [ ] Every domain noun the document uses is defined once in the Glossary and used **verbatim** everywhere — no synonyms, no undefined nouns.
- [ ] Every `SM-N` names the requirement(s) it validates, and any primary metric has a counter-metric.
- [ ] No banned/vague word survives unquantified.
- [ ] Every acceptance criterion is testable, has one interpretation, and carries concrete bounds.
- [ ] Every criterion is atomic: one EARS pattern, under ~300 characters, at most 2 conditions.
- [ ] Each failure / empty / boundary state from the step-4 probe — and each journey's edge case — has a criterion.
- [ ] No implementation or model-design detail leaked in, including into the vision, journeys or glossary.
- [ ] The document states current behaviour only; no `R`, `UJ`, `SM` or criterion id reuses or renumbers a removed one, and every framing section sits above `## Requirements`.

Then the document itself — for an `.mdx` requirements artifact, every item below is one a block-free document fails:

- [ ] **Every entity that already exists in the model** and is named in prose is an inline `<ModelRef>` carrying `application` — not bare text, and not a plain-`.md` link in an `.mdx` document.
- [ ] **Every `UJ-N` has a diagram** — a `` ```mermaid `` `flowchart TD` (or a `<Diagram>` where the layout needs it) encoding its beats including its edge case, alongside its bullets.
- [ ] Wherever a requirement's inputs or failures are enumerable, they are a **table** — bounds before the criteria, failure matrix beside them — rather than dissolved into prose.
- [ ] Every diagram node, table row, caption and callout **back-references its criterion id**.
- [ ] Every table is captioned `Table N.N` (not a criterion id) and cited in prose by that label.
- [ ] **Nothing normative lives only inside a block** — every table is cited by a criterion (by reference to `Table N.N`).
- [ ] Where a section has NO block, that is a judgement I would defend — the content genuinely is narrative — rather than an omission.

## As-built mode (extraction)

When `/sdd-extract` loads this skill, the user has decided to **document a feature that already exists** — that is a user decision, so these deltas override the interview rules above:

- **A Capability, not a Change.** `/sdd-extract` runs its own guard and makes the first write with `kind: "capability"`, so skip step 1.
- **No interview.** Every open question becomes an item in the `## Assumptions` `<Checklist>`, stated as what the code does today. `## Open Questions` stays empty, and you do not call `ask_user_question` here — `/sdd-extract` owns the questions.
- **Describe, never propose.** Every criterion states what the code does now, quirks included. A behaviour that looks wrong is still written down as it behaves; `/sdd-extract` raises suspected bugs separately.
- **Evidence or nothing.** Every criterion must rest on a behaviour from the inventory you were handed, which carries `file:line` evidence. Do not add a criterion the code can't show.
- **Keep the prose non-technical.** No file names, class names or paths in the document — the evidence lives in design and traceability.
- The Introduction says in one sentence that this spec was **extracted from the existing implementation**.
- Journeys, Glossary and Non-Goals still apply: Non-Goals are the neighbouring features the scope pass put out of bounds.
- The self-review items about interviewing the user are satisfied by the inventory instead.

## Then raise the phase gate

Tell the user the requirements are ready to review — call out the Assumptions explicitly so they can correct them, along with the **journeys and the non-goals**, which are the two things a reader is most likely to disagree with. Keep this to a **brief summary**: never paste `requirements.md` into the chat, because every surface reads the artifact off disk.

Then, with the artifact written and no question left open, call **`advance_spec_phase(slug, artifact: "requirements", toPhase: "design")`** — this is the approval gate, and it works from every surface. In-app it raises an approval card and blocks until the user decides; under a spec whose manifest carries `gates: auto` it advances immediately. On **reject**, revise `requirements.md` per the user's feedback with `write_spec` and call the gate again. Phases advance **one gate at a time** (the tool refuses a jump), so this call moves the spec to **design** and nothing further — do not start design yourself.
