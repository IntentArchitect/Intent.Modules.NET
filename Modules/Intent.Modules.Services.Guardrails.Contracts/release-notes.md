### Version 1.0.1

- Improvement: The Services drift check reports through Intent.Common.DriftReporting: its findings are the Services Area of the application's one drift report (`.cache/drift/<application>/DriftReport.mdx`), and a run with drift fails with one error naming the report. The per-designer `ServicesDriftReport.mdx` and its template are gone; a `ServicesDriftReporter` factory extension contributes the designer instead.
- Improvement: Extracted and generalized Drift Module

### Version 1.0.0

- New Feature: Create authoritative design visuals for existing code in the Services designer: services, commands, queries, DTOs, enums, integration events and commands, event handlers and API clients.
- New Feature: Drift detection report when the Services designer's visuals and the code go out of sync, including what each handler is modelled to do: its entity actions, invocations, published events and sent commands.
- New Feature: Link a subscriber's own copy of an integration event or command to the message it copies, so the event flow between applications stays modelled.
- Improvement: The drift check, its always-on rule and its skills now come from the Intent.Common.Drift module, installed with this one, so every Guardrails designer shares one copy.
- New Feature: Use the `initialize-brown-field` or `reverse-engineer-services` skill to set up your project, modelling the existing code and configuring the drift check from tested recipes for ASP.NET Core controllers, minimal APIs, FastEndpoints and handler intent.
