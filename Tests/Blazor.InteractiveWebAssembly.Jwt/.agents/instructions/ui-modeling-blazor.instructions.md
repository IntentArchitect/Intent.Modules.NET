---
name: ui-modeling-blazor.instructions
description: General guidance for AI on how to model UIs for Blazor.
contentHash: 553E21AA6C0BAC147AF386ACD47186B197C3C62D5D76D9854A8A09A4BFC0224B
---
## General Rules

- UI Pages / Components should be modeled in the UI Designer.
- Any navigation between Pages should be modeled.
- Pages / Components which are designer to talk to services should model those service interactions.
- If Pages / Components talk to Services the Service Package (where the services are modeled) should be added as a reference in the UI Designer (if its not already there).

## WASM Specific Guidance

- In the service design Do not model services using explicit proxies, rather model against the actual commands and queries.
- Don't look or expect to find anything in the WASM Projects services designer.
- If you want to connect to services in other applications you must add a Service Reference in the WASM Applications UI Designer to the Service Application's Service package.
