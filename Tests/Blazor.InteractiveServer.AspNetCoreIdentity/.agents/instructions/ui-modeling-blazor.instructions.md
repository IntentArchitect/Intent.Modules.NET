---
name: ui-modeling-blazor.instructions
description: General guidance for AI on how to model UIs for Blazor.
contentHash: EEB3AF205FB9AA644C86280B27C1621E1CA3A4F3CC5186098BCF0CB32B684692
---
## General Rules

- UI Pages / Components should be modeled in the UI Designer.
- Any navigation between Pages should be modeled.
- Pages / Components which are designer to talk to services should model those service interactions.
- If Pages / Components talk to Services the Service Package (where the services are modeled) should be added as a reference in the UI Designer (if its not already there).
