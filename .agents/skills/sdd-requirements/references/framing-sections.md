# The framing sections — when to include each, and how to write them

The sections above `## Requirements` turn a criteria list into a spec someone can act on. They are **not boilerplate to fill in**: an empty-calorie journey or a metric nobody will measure is worse than the omitted section, because a reader has to work out it says nothing. Include a section when it carries weight, omit it entirely when it doesn't — and never pad one to look complete.

| Section | Include it |
| --- | --- |
| **Introduction** | Always. |
| **Vision** | When the _why_ isn't self-evident from the Introduction — a new product surface, a behaviour change users will notice, a strategic bet. Skip for a plumbing change. |
| **Target Users & Jobs To Be Done** | When more than one kind of user is involved, or the motivation shapes the behaviour. Skip when there is exactly one obvious actor doing one obvious thing. |
| **Key User Journeys** (`UJ-N`) | Whenever there is a multi-step flow, a UI, an authentication or hand-off assumption, or an ordering the user experiences. **This is the highest-value section** — the beats force specificity a criteria list never surfaces (a journey's edge case is usually where the "adding it twice is a no-op" rule comes from). Skip only for a single-call library/API/CLI change with no flow. |
| **Glossary** | Always — even two or three terms. `/sdd-design` maps these nouns onto Domain elements, so this section is what stops the model and the spec drifting apart in vocabulary. |
| **Non-Goals** | Always, even if it's two bullets. Cheapest scope-creep prevention there is: it stops the "let me also add this nearby thing" failure mode at design, tasks _and_ code. |
| **MVP Scope** | When the feature ships in stages, or something has been deliberately deferred. Fold into the Introduction's out-of-scope note when it's a single bullet. |
| **Success Metrics** (`SM-N`) | When someone will ask "did this work?" — a user-facing capability, a performance or adoption goal. Skip for internal plumbing. When you include primaries, include at least one **counter-metric**: it is what stops the design optimizing the wrong thing. |

## How to write them

- **Journeys are named-persona narratives, not restated criteria.** One line of title, then persona + context, entry state, a 3–5 beat path, the climax (the moment value lands and how the user knows), the resolution, and — where there is one — a real edge case. Number them `UJ-1 … UJ-N`. Where the journey is genuinely just a job-to-be-done restated, one sentence (`{Persona}, {context}, {what they do and why}.`) is the right length; don't inflate it.
- **Metrics cross-reference requirements.** `**SM-1**: <metric, definition, target>. Validates R1, R3.` Counter-metrics say what they counterbalance.

