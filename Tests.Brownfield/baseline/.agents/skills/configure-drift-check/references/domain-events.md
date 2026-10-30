---
contentHash: BBD4BC61E83A165588C3DAA7DEAE980322CDFD4FB36CD456C81A71211567C3BE
---
# Domain events: the Domain recipe for the events entities raise

Add this recipe to `guardrails/domain.drift.json` when the Domain model links constructors or operations to the Domain Events they raise (`Domain Event Origin`, from Intent.Modelers.Domain.Events). It is tested against entities in each style below. Copy its `facts` into the recipe file you started from the database guide, then adapt it to this codebase's own conventions. Don't write it again from scratch.

## Recognise it

Domain event classes (`OrderStartedDomainEvent`, often implementing `INotification` or deriving from `DomainEvent`), and entities that collect them for dispatch after saving. Each link is an `intent.raises` fact about the constructor or operation it starts from. The recipe finds that member, `class:{Entity} > method:{Member}` (a constructor is found by its class's name, and the parameter names choose between overloads), and reads its body for the event in any of these forms:

- `DomainEvents.Add(new OrderStartedDomainEvent(…))`, the list Intent's own entities have;
- `AddDomainEvent(new …)` or `RaiseDomainEvent(new …)`, a base entity's method;
- `_domainEvents.Add(new …)`, the entity's own list;
- a call of a private helper named after the event, `AddOrderStartedDomainEvent(…)`, as eShop's `Order` has.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| Do the entities live where the recipe file's `files` looks? | The database recipes may look only at the configuration | Give the recipe its own `files` naming the entities' project |
| Are events built by a factory, or raised by a helper named otherwise? | `OrderEvents.Started(this)`, `RaiseStarted()` | A case for that call, value `{DomainEvent}` |

## The recipe file

```json
{
  "files": "src/**/*.cs",
  "facts": {
    "intent.raises": {
      "find": "class:{Entity} > method:{Member}",
      "read": [
        { "part": "body", "match": "\\bnew ({DomainEvent})\\b" },
        { "part": "body", "match": "\\bAdd({DomainEvent})\\(" }
      ],
      "default": "",
      "unstated": "no `new {DomainEvent}` or `Add{DomainEvent}(…)` in `{Member}`"
    }
  }
}
```

## How each form is modelled

The recipe reads the entities in the Domain designer's terms, so the model must use the same terms (reverse-engineer-domain). Every `intent.raises` fact is a link the member's code should have, so its value comes from the code: one from `default` is a member that doesn't raise the event, or a form the recipe doesn't read yet.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `intent.raises` | A constructor or operation that adds a new event in any form above, or calls the helper that does: a `Domain Event Origin` from that `Class Constructor` or `Operation` to the `Domain Event`, one per event it raises | Code |

## Adapting to house style

| You see | Change |
| --- | --- |
| An event raised in a private helper not named after it (`MarkStarted()` adding `OrderStartedDomainEvent`) | A case for the helper's call, value `{DomainEvent}`, as long as only that event is raised there: `\\bMarkStarted\\(` |
| Events raised in a base class's method (`base.Complete()`) | Nothing reads it there: link the event to the member whose body raises it |

## What it can't read

- Whether the event is dispatched, only that the member's code adds it.
- An event raised by another class the member calls (a child entity, a domain service): it reads as missing. Link the event to the member that raises it.
- An event the code raises and the model doesn't link: no fact states it, so nothing reports it. Link it when you see it.
- The same event added twice in one member: the probe changes the first and warns that the second still reads the same. The reading is right; tell the developer rather than weakening the recipe.
