# R8.6 / R8.7 — delete-and-regenerate evidence

Spec: `object-mapper-hardening` · Run date: 2026-08-15 · Modules under test:
`Intent.Application.Dtos.ObjectMapping` 1.0.0-pre.4, `Intent.Application.DomainInteractions` 1.2.10-pre.0.

This is the run record R8.6 asks for. It cannot be a script — the Software Factory is driven from
Intent Architect, not the command line — so the procedure and its actual results are recorded here.

## Procedure

1. Deleted, in **both** flavour applications, every file the Object Mapper module owns plus the four
   query handlers whose Call Sites Domain Interactions generates:
   - `{App}.Application/Mappings/Orders/*.cs` — 6 files per application
   - `{App}.Application/Orders/{Query}/{Query}Handler.cs` — 4 files per application
2. Ran the Software Factory once per application.
3. Applied the staged changes and ran the Software Factory a **second** consecutive time.
4. Diffed the regenerated output against the `intent/.specs/object-mapper-hardening/baseline/`
   snapshots taken in T1.9 / T2.9.
5. Built and tested both solutions, then ran `verify-objectmapping-flavour-equivalence.ps1`.

## Results

| Check | ObjectMapping.Strict | ObjectMapping.Lenient |
|---|---|---|
| Files reproduced by run 1 | 10 of 10 (`create`) | 10 of 10 (`create`) |
| Run 2 changes proposed | **0** | **0** |
| Solution build | exit 0, 0 warnings, 0 errors | exit 0, 0 warnings, 0 errors |
| Test suite | 40 passed / 0 failed | 44 passed / 0 failed |
| Flavour equivalence script | exit 0 — 6 mapping files compared, differences confined to `CouponId`, `CustomerCity`, `CouponPercentOff`, `CouponKind` | — |

### Diff against the T1.9 / T2.9 baselines

Mapping Extension Classes — semantically identical in all 12 files. The only differences are two
cosmetic artefacts of the hand-authored baselines: a trailing comma after the last object-initializer
entry, and a trailing newline at end of file. No expression, member, ordering or type differs.

Query handlers — the regenerated bodies carry `[IntentManaged(Mode.Fully, Body = Mode.Fully)]` where
the hand-written baselines carried `Body = Mode.Merge`. This is the R9.6 / R8.7 result the exercise
existed to prove: the Call Sites are now **fully** module-generated, with no merge-managed region in
which a hand edit could survive regeneration and masquerade as generated output. Their statements are
otherwise identical to the baselines.

### R8.7 — the one remaining merge-managed body

`Order.GetDisplayLabel()` in both applications keeps a hand-written body
(`return $"Order {OrderNumber} [{Status}]";`) inside a merge-managed region; the Domain Entity template
emits a `NotImplementedException` stub there. This is by design and is not module-owned output — the
Domain designer cannot model a C# operation body, and this one exists only as the test fixture Mapping
Shape 13 maps through. R8.7 was amended to state that exception explicitly.
