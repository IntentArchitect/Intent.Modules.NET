---
description: Fold a done Change into the Capabilities it belongs to — merge its requirements, traceability and lasting design decisions into each Capability, have the user review each one (a Capability under auto gates is approved at once), archive the Change, then verify each Capability. Use when a Change reaches done, or when the Specs panel's "Fold into capabilities" fills the chat.
short-description: Fold a done Change into its Capabilities, archive it, then verify them.
argument-hint: <change slug>
intent-updates: automatic
requiredTools:
  - read_spec
  - write_spec
  - record_spec_traceability
  - request_spec_review
  - advance_spec_phase
  - ask_user_question
  - create_sub_agent
group: Spec-driven development
---

# SDD — Fold

A **Change** is one piece of work. Once it is done, what it established belongs in the **Capabilities**, the living specs of how each area of the system behaves now. You move it there, archive the Change, then verify the Capabilities.

## 1. Read the Change

Resolve the slug with `read_spec()` if you weren't handed one; never guess it. The Change must be a `change` at `done`; otherwise stop and say why. Read its `requirements`, `design` and `traceability`. For a spec another framework owns, its own markers of what changed are your hints: OpenSpec's `## ADDED`, `## MODIFIED`, `## REMOVED` and `## RENAMED Requirements`, or the equivalent in the framework's documents.

## 2. Map each requirement to a Capability

From the `read_spec()` listing, read the `requirements` and `traceability` of each Capability whose title or description sounds related. For each of the Change's requirements, judge from its wording and its linked files and elements which Capability it belongs to, and whether it **adds** behaviour, **changes** an existing requirement there, or **removes** one. When more than one Capability fits, `ask_user_question`. When none fits, `ask_user_question` first, offering **Create a new Capability** (with a proposed slug and title) and **Skip these requirements** (they stay unfolded and the report names them); never create a Capability without that answer. Either question says what follows: the Change is archived, each Capability is then verified, and the tests citing the Change are re-bound to it. Once the user agrees, create it: `write_spec(newSlug, "requirements", …, description: …, kind: "capability", parent: <area slug>)`. Take the parent from the `kind: area` specs in the `read_spec()` listing whose description covers the Capability. When more than one fits, `ask_user_question`. When none fits, leave `parent` out and say in your report that the Capability is Ungrouped and that **sdd-area** can group it. Follow **sdd-requirements** for the document's shape.

## 3. Write each Capability

Per Capability, with `write_spec` searchBlock/replaceBlock on its `requirements`:

- **Add:** a new requirement takes one past the highest id the Capability has ever used, and a new criterion one past the highest in its requirement. Ids are spent, never reused (see **sdd-requirements**).
- **Change:** reword the existing criteria in place, keeping their ids. State current behaviour only.
- **Remove:** delete the criterion or requirement; the ids after it keep their numbers.
- **Cite by id:** a criterion names another only by its full id (`R3.2`), never "criterion 2". A requirement or criterion of another spec is a spec link anchored at its full id, `[[other-slug#R3.2]]`, never `other-slug R3.2`, which the viewer links to this spec's own R3. A criterion moved from the Change gets a new id in the Capability, so rewrite every id it cites to the Capability's numbering. Fix whatever `write_spec`'s feedback lists under requirement id problems.

Then re-record traceability against the Capability's ids, following **sdd-record-traceability**. A record replaces every link under its id, so pass the Capability's existing targets for that id plus the Change's; clear a removed requirement's links with `clear: true`. Merge the Change's lasting design decisions (why the code is shaped the way it is, not the plan for building it) into the Capability's `design`. A Capability you created always gets a `design`, in **sdd-design**'s as-built shape, because its verification gate needs one.

**Re-bind the tests.** Tests find their spec by the slug in their title, `[spec:<slug> R2.1]`, so after a fold every test citing the Change cites an archived spec and the Capability shows its criteria untested. `read_spec(changeSlug, artifact: "tests")` lists them. Retitle each citation to the Capability's slug and the id that criterion now has there, which differs from the Change's id when you renumbered it in **Add**; a criterion you skipped or removed keeps no citation. Change only the citation token, never the test's body. A test method Intent generates keeps a retitle only when the method is protected from regeneration (in C#, `[IntentManaged(Mode.Ignore)]`), so add that where it is missing. Then check `read_spec(capabilitySlug, artifact: "tests")` shows each re-bound criterion cited, and `read_spec(changeSlug, artifact: "tests")` shows none left.

**Account for every criterion.** Archiving hides the Change, so nothing it established may be dropped silently. List every criterion id of the Change (`read_spec(changeSlug, artifact: "catalog")`) and give each its destination: `R2.3 → <capabilitySlug> R5.3`, or `skipped` where the user chose **Skip these requirements**. A criterion with neither is unfinished work: fold it, or ask the user about it, before you go on. The re-binding above uses this same map.

## 4. Review, then archive

Call `request_spec_review(capabilitySlug)` once per existing Capability you changed. A Capability under `gates: auto` approves at once, its changes left marked Unreviewed. On feedback, revise each commented or rejected block and request again. A Capability you created has no HEAD to diff against, so its approval is its phase gates instead: `advance_spec_phase(newSlug, "requirements", "design")`, then `advance_spec_phase(newSlug, "design", "verification")`. Only when every review and gate is approved, and every criterion has a destination in the map, call `advance_spec_phase(slug, artifact: "requirements", toPhase: "archived")` for the Change.

## 5. Verify the Capabilities

A folded Capability is unverified until its own verify runs, so once the Change is archived, verify each Capability you created or changed, without asking: the user asked for the fold, and this finishes it. `use_skill` **sdd-sync** and follow only its step 5, "Verify and hand off", for those Capabilities, with two differences:

- **No "Verify now / Not now" question.** Go straight to the dispatch.
- **Target what the fold moved.** A Capability with no verdict yet, including one you created, gets a full verify. Otherwise target the requirements its `movedSinceVerdict` lists and the criteria whose test status is `stale` or `unconfirmed` (`read_spec(capabilitySlug, artifact: "tests")`), which re-bound tests often are.

End with this skill's Report, not step 5's closing line, naming each Capability still unverified or with gaps and the step that fixes it: `/sdd-run <capabilitySlug>`. Never report the fold as finished work while one is open without that line.

## Dispatched by /sdd-run

As a `/sdd-run` sub-agent, do steps 1–3, including re-binding the tests, and stop: the orchestrator raises the reviews, verifies the Capabilities and archives the Change. End with the report your instructions ask for, naming each Capability you changed or created.

## Report

Each Capability changed or created, the ids added, changed and removed in each, the criterion map (every Change criterion to its Capability id or `skipped`; under /sdd-run it may exceed the ten-line limit), how many tests you re-bound and any you could not, whether the Change was archived, and, outside a Run, each Capability's verify: its gap count, how the user settled each disagreement and what heal changed. End with either **"Folded Capabilities verified with no gaps."** or each one still open.
