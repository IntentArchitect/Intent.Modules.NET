### Version 1.0.2

- Fixed: Connection string names were being PascalCased before being stored, so the generated constant's value no longer matched the actual connection string name (breaking the runtime lookup), and the search that swaps a hardcoded string literal in the Dependency Injection class for the constant reference failed to match whenever the connection string name wasn't already PascalCase (e.g. contained spaces). Both the constant's value and the substitution now use the original connection string name; only the generated field's identifier is PascalCased.

### Version 1.0.1

- Improvement: The `Constants` class is now named based on the application name, ensuring uniqueness across different projects.

### Version 1.0.0

- New Feature: Generates a `Constants` class in the Infrastructure project and substitutes all hardcoded strings with constants in the Dependency Injection class.