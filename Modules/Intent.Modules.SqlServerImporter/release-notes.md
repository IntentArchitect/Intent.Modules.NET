### Version 1.0.1

Improvement: Improved support for Index importing.
Improvement: Importer now doesn't add explicit `Index`s on foreign keys, so the model is more how you would have modeled it. The Importer detects relationships which have been remodeled from `Aggregation`(white diamond) to `Composition`(black diamond) and does not recreate the `Aggrgation Relationship`.

### Version 1.0.0

New Feature: Module release.
