### Version 1.0.1

- Improvement: The Domain drift check reports through Intent.Common.DriftReporting: its findings are the Domain Area of the application's one drift report (`.cache/drift/<application>/DriftReport.mdx`), and a run with drift fails with one error naming the report. The per-designer `DomainDriftReport.mdx` and its template are gone; a `DomainDriftReporter` factory extension contributes the designer instead.
- Improvement: Extracted and generalized Drift Module

### Version 1.0.0

- New Feature: Create authoritative design visuals for existing code in the Domain designer: entities, value objects, enums, repositories, data contracts, domain services and domain events.
- New Feature: Drift detection report when the Domain designer's visuals and the code go out of sync, including the domain events each constructor and operation is modelled to raise.
- Improvement: The drift check, its always-on rule and its skills now come from the Intent.Common.Drift module, installed with this one, so every Guardrails designer shares one copy.
- New Feature: Use the `initialize-brown-field` or `reverse-engineer-domain` skill to set up your project, modelling the existing code and configuring the drift check from tested recipes for EF Core, MongoDB, Azure Cosmos DB and domain events.
