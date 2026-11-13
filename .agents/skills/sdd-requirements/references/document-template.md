# The requirements.md skeleton (Kiro-compatible EARS)

Use this structure, dropping whichever framing sections don't carry weight (see `references/framing-sections.md`). The `<…>` placeholders are instructions to replace, never text to copy through. The heading, id and section-order rules in SKILL.md apply to everything below.

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
