# Finding and naming this codebase's review risks

Use this while scanning (phase 2) and when turning the findings into classifications.

## Contents

1. The principle: the themes come from the code
2. Scan lenses (coverage prompts, not categories)
3. From findings to themes
4. Review Importance
5. What counts as evidence
6. Colours
7. Scan brief for sub-agents

## 1. The principle: the themes come from the code

Every system gets its own map. A classification names a place in _this_ system where a wrong change hurts: a subsystem, a flow, a kind of data, a partner it talks to. It uses the words the team uses. There is no default list. Don't start from one, and don't carry themes over from another codebase you have classified, including any in this skill's examples.

Some concerns recur across systems: who may do what, where secrets live, data that must not leak. When the code really has one, draw its boundary and name it around how _this_ code does it. A token issued by an identity service and trusted by every API is "Identity & Token Trust". A lone `[Authorize]` attribute is not a theme of its own; it belongs to whatever flow it protects. Overlap with other systems' maps is fine. Copying them is not.

**Self-test before showing the user anything.** Read your theme names with the evidence hidden. If the list could describe almost any web application ("Access Control, Secrets, Personal Data, Integrations…"), it is a checklist, not an analysis. Regroup the findings around this system's subsystems and failure modes, and rename.

## 2. Scan lenses (coverage prompts, not categories)

These questions make sure the scan doesn't miss ground. They are **not theme names** and must not be handed to anyone as a list to sort findings into. A lens can produce no theme, one theme or several, and one theme often draws on several lenses.

- **Trust and authority:** who may call what and act on whose data, how identity is established and passed on, and what bypasses it.
- **Secrets and environments:** where credentials, keys and connection strings live, and what changes between environments.
- **Sensitive data:** which data must not leak or be altered (personal, payment, health, regulated), and where it flows: responses, logs, messages, exports, third parties.
- **Value and correctness:** what computes money, prices, quantities, scores or decisions, and whether any of it trusts values the client supplied.
- **State and workflow:** the lifecycles of core entities, sagas and cross-service flows, retries, idempotency, and what can get stuck or run twice.
- **Boundaries:** which systems, services, queues, webhooks and schedulers this code serves or depends on, and which contracts are shared (routes, messages, protos, schemas, packages) and how they change.
- **Data:** who owns the schema, migrations, raw queries, transactions and seeding.
- **Process-wide behaviour:** startup, middleware, hosting, and shared defaults that every service inherits.
- **AI and automation:** which models are called, what data reaches them, and how their output is trusted.
- **Build and delivery:** dependencies and their sources, pipelines, deployment configuration, agent instruction files.
- **Context and noise:** tests, presentation, static assets, leftovers.

## 3. From findings to themes

1. **Pool the findings.** Each one has files, what the code does, the failure mode if a change goes wrong, evidence, and a candidate name in the system's own words.
2. **Cluster by the reviewer's question.** Group findings a reviewer would check the same way, even across projects. Split findings in one folder that raise different questions.
3. **Name each cluster after the system's concept,** for example "Order Saga", "Card Data (PCI)", "Webhook Delivery", "Ledger Posting" or "Tenant Isolation". Prefer names the team would recognise. Avoid generic security-category names unless that genuinely is how this system is organised.
4. **Write the reviewer question** in this system's terms. "Can an order get stuck between Ordering, Catalog and Payment?" beats "Is state handled correctly?".
5. **Check the result:**
   - every finding lands in a theme;
   - every theme rests on evidence;
   - no two themes ask the same question (merge them);
   - no theme mixes unrelated questions (split it).
6. **Size it to the system.** A small service may need five themes; a distributed system fifteen or more. Merge themes a reviewer would never filter separately.
7. **Add context themes** where they help reviewers hide things, such as tests, UI or static assets. Put them at Low or None, again named for what they are here.

## 4. Review Importance

Change Review pairs a developer's review with AI review, so importance answers one question: **who has to review a change here?**

| Importance | Meaning                                                                                                                                                                                                                                                                                                             |
| ---------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| High       | If an AI changes this file, a developer must review the change themselves; it is not left to AI review. Reserve it for files where a wrong change fails silently and seriously (lost money, granted access, leaked or corrupted data, a broken live integration), or where a present defect makes the area fragile. |
| Medium     | A developer should look, but AI review is a sound first pass. Use it for catch-alls around High files, shared contracts and data shape.                                                                                                                                                                             |
| Low        | AI review is enough; the file is shown for context, as with helpers, tests and UI.                                                                                                                                                                                                                                  |
| None       | No review needed: noise.                                                                                                                                                                                                                                                                                            |

Treat this as the default meaning and confirm it with the user in phase 3. They may want High stricter or broader. Don't aim for a fixed share of High files: report the share the plan produces and let the user move the line. A strict reading often lands in single digits.

- **Catch-alls:** give every cluster of High files a Medium catch-all for its folder, so a new file there is classified too.
- **One row per file:** a file shows every theme it belongs to at its highest importance. A reviewer filters themes and importance separately, so it still surfaces under each theme.

## 5. What counts as evidence

High needs a concrete failure mode that no compiler or test would catch, cited as `file:line`. The shape to aim for, shown with illustrative examples from unrelated systems:

- "The refund handler trusts the amount in the request, so a client can refund more than it paid (`RefundHandler.cs:42`)."
- "Messages are acknowledged before the handler commits, so a crash loses the event (`Bus.cs:118`)."
- "The signing key is committed and loaded in every environment, so anyone with the repository can mint tokens (`Program.cs:29`)."

A file name or a folder alone is not evidence. Present defects (bugs and security holes) raise the importance of the files involved. List them separately for the report; fixing them is not this skill's job.

## 6. Colours

No colour belongs to a name. Give each theme a distinct colour, taking them from this palette in order. Each is dark enough for white text, and the MDX generator switches to dark text on the light ones. Use grey for noise and context themes, and keep the colours of existing classifications.

`#C62828` `#1565C0` `#2E7D32` `#6A1B9A` `#E65100` `#00838F` `#AD1457` `#5D4037` `#283593` `#F9A825` `#455A64` `#558B2F` `#D81B60` `#00695C` `#4E342E` `#757575` `#BDBDBD`

If there are more themes than colours, add six-digit hex colours that are clearly distinct from the ones in use. The MDX generator picks white or dark text for each by its luminance.

## 7. Scan brief for sub-agents

Give each read-only sub-agent one non-overlapping slice, such as a project or a top-level folder. Never include a list of theme names in the brief, whether from this file, another codebase or a previous run.

```text
READ-ONLY. Scan <slice> under <output folder> to find where a change would need careful review.
Do not sort findings into a predefined set of categories; describe each risk in this system's own terms.
First read sections 2, 4 and 5 of <skill folder>/references/finding-risk.md (lenses, importance, evidence).
For each finding report: path or folder glob (relative to the output folder), what the code does there,
the concrete failure mode if a change goes wrong, evidence as file:line, proposed importance, and a short
candidate name for the risk area in this system's vocabulary. Propose a Medium catch-all glob for each
cluster of High files, and list Low/None areas (tests, UI, static assets, leftovers).
List genuine defects separately, one line each with file:line. Do not modify anything.
Return compact tables, no file dumps, under about 1,200 words.
```
