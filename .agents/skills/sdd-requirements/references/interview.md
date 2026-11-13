# The interview — framing, the negative-space probe, and how to ask

Steps 3–5 of SKILL.md in full. The rule they serve: resolve every material question with the user before you write.

## Step 3 — frame the feature before you specify it

Work out — and where you can't, ask — the following. This is the part a criteria-only draft skips, and it is what makes the rest specific:

- **Why now, and what changes for the user** — the one or two paragraphs of vision. If you can't say what is better afterwards, you don't yet know what to build.
- **Who** — each kind of user and the job they're hiring this feature to do.
- **The journeys** — for each job, the actual scene: where they start, what they do, when they know it worked, what they're left with, and what goes wrong. Draft these and put them to the user to correct: people spot a wrong journey far faster than they spot a missing criterion.
- **The vocabulary** — every domain noun, defined once, with cardinality. Reconcile synonyms **now**, before they multiply through the document.
- **The boundary** — the non-goals, and (where it applies) what is in and out of the MVP. Ask explicitly: "what should this deliberately _not_ do?" is one of the highest-yield questions in this step.
- **How you'd know it worked** — the metric, its target, and the counter-metric that stops it being gamed.

## Step 4 — probe the negative space

Most vagueness comes from questions nobody asked. Walk this checklist against the feature and, for each item that applies, either specify a concrete criterion or ask the user — never let it default silently:

- **The data each thing captures** — for every entity the feature introduces or touches (e.g. a "quote"), pin down _exactly_ what it records: each field, whether it's required, its type/format/bounds, and whether it's entered or derived. "Manage quotes" without stating what a quote stores (number, buyer, line items, status, validity/expiry, totals, tax, notes, timestamps, …) is **not specified** — resolve it and turn the answer into acceptance criteria.
- **Actors & permissions** — who can do this; what each role may and may not do.
- **Inputs** — for every field: type, required/optional, format, length/range, default.
- **Validation & failure behaviour** — the _exact_ response to each invalid input and each failure (error shown, state left behind, what is _not_ done). "Handle errors" is not an answer.
- **Empty / boundary / max states** — nothing yet, one, the maximum, over the maximum.
- **Duplicates, concurrency, idempotency** — repeat submissions, simultaneous edits.
- **Data lifecycle** — create/update/delete semantics, soft- vs hard-delete, retention, audit history.
- **State transitions** — the allowed states and what triggers each move.
- **Non-functional (only where it matters)** — performance with an actual target, authorization, volume.
- **Each journey's edge case** — walk every `UJ-N` beat by beat and ask what happens when that beat fails. Journeys and this checklist feed each other: a beat with no criterion is an unspecified behaviour.

## Step 5 — how to ask

Keep looping in focused rounds; when answers raise new unknowns, ask those too. **Default to the `ask_user_question` tool** — clicking an option is far cheaper for the user than typing:

- **Prefer `ask_user_question` (options).** Whenever a question has a small set of plausible answers — even if _you_ have to enumerate them — ask it as options so the user just picks. This covers most requirements decisions: soft- vs hard-delete; standalone invoice vs linked order; sync vs async; **which fields an entity captures** (offer them as a multi-select checklist); which statuses/transitions apply; **which of the journeys you drafted are the real ones**; **which candidate non-goals to lock in**. Batch several such questions in one round.
- **Free-text (a plain chat message) is the fallback ONLY for a genuinely open-ended question** that has no clear set of answers (e.g. "describe the business goal", "state the exact validity-period rule"). Keep these to **one or two at a time** — text answers are expensive.
