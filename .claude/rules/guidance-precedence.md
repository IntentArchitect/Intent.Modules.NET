---
description: Precedence between explicit user instructions, skills, and an orchestrator's own inferences — read before delegating to a subagent or reconciling a skill against a task prompt.
alwaysApply: true
intent-updates: automatic
---

## Guidance precedence

1. Explicit user instructions are final.
2. Skills in `skills` are authoritative for HOW to implement, and outrank
   an orchestrating agent's own summary, paraphrase, or inference of them.
3. If a task prompt conflicts with a skill on HOW to implement: follow the
   SKILL, unless the prompt explicitly attributes the instruction to a user
   decision — then follow the user. When it's unclear whether an unattributed
   instruction is a real user decision or the orchestrator's own inference,
   don't silently pick the skill — ask if you can, otherwise state the
   assumption explicitly in your response.

Orchestrators: when delegating, point subagents at the relevant skill by name.
Do not paraphrase a skill's rules into the prompt — quote or reference it. Attribute
user decisions explicitly ("the user specified X") so subagents can tell your
inferences from their instructions.
