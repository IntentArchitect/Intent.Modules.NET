---
description: Keep SDD specs true when you change code they trace — act on the Spec hints naming the requirements a changed file realises, update the spec first when the user changes traced behaviour, and run sdd-sync before finishing.
alwaysApply: true
intent-updates: automatic
group: Rules
---

## Keeping specs true

This applies only when the solution has specs, meaning `read_spec()` lists at least one. Each spec links its requirements to the files and model elements that realise them.

- **A `Spec:` line names the requirements a file realises.** Intent's file tools add one to their results, and a `<spec_link_hints>` message after your turn names them for files your own editor changed. Keep those criteria true unless the user asked you to change that behaviour.
- **When the user asks for a change to traced behaviour, update the spec first.** Say in one line which criteria will change, e.g. "This changes ai-chat-surface R7.3." A new criterion goes in the spec whose description in `read_spec()` covers the behaviour, not just a spec that traces the file; ask when more than one fits. When none fits, create a Capability for it with the `sdd-extract` skill, not by writing its requirements alone: extract writes its as-built design too, so a clean verify moves it to done. Patch them with `write_spec`, tell the user in one line why a review is coming (e.g. "The requirements changed, so please review the new wording before I continue."), then call `request_spec_review(slug)`. Write the code only to the wording the user approved. If the review comes back with feedback, revise the wording and request the review again.
- **When you record links for model changes** with `record_spec_traceability`, take the targets from every script's `changes`, including new members of an element linked earlier, rather than from memory. A re-record replaces the task's links, so include all of them.
- **After changing traced code**, load the `sdd-sync` skill before you end the turn. It writes the spec updates, has the user review them with `request_spec_review`, and says when to skip it.
