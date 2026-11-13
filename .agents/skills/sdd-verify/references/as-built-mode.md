# As-built mode (extraction)

When `/sdd-extract` loads this skill, the spec documents code that already existed, so **the code is the truth**. That is a user decision, and these deltas to `SKILL.md` apply:

- **Pass `refA: "HEAD"`** to `get_change_review`. The feature was committed long before the spec, so the branch range would list every unrelated change on it as untraced. With `HEAD`, the untraced gate covers only uncommitted work, which is what the extraction itself touched. Say in the report that the range is deliberately narrow.
- **A behavioural gap means the requirement misdescribes the code**, not that the code is wrong. Record it as normal: `expected` is the criterion as written and `actual` is what the code does. `/sdd-extract` rewords the criterion to match `actual` and verifies again. If the extra behaviour would push the criterion over the atomicity limit in **sdd-requirements**, it becomes a new criterion id instead. It does not heal.
- Code changes happen only through the suspected-bug questions `/sdd-extract` asks after the verdict, never from here.
