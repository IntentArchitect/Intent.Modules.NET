### Version 4.4.0

- Improvement: The filename of DTOs with generic types will now contain the generic type(s) (e.g. the filename for the DTO ProductDto\<T> will now be named ProductOfT). 

### Version 4.3.2

- Fixed: Constructor nullability assignment omitted for generic type properties.

### Version 4.3.1

- Improvement: Updated module NuGet packages infrastructure.

### Version 4.3.0

- Fixed: An issue where `Event` package enums generated multiple files. 
> ⚠️ **NOTE**
>
> This may move some enums moving to different locations and namespaces.

### Version 4.2.1

- Improvement: TypeSources added for Contract Clients.

### Version 4.2.0

- Improvement: Upgraded module to support new 4.1 SDK features.

### Version 4.1.1

- Fixed: Protected Constructor added when parameter constructor is introduced for mapping libraries to instantiate using reflection.

### Version 4.0.8

- Fixed: Constructor introduced when non-public property accessor setting is configured.

### Version 4.0.7

- Improvement: Description Attributes can be applied to `Enum` literals through the usage of the Description Stereotype.

### Version 4.0.6

- Fixed: DTO static factory methods didn't have generic type parameters.

### Version 4.0.4

- Upgrade : Removed various compiler warnings.

### Version 4.0.3

- Upgrade : Added configurable `sealed` access modifier support for `DTOs`.

### Version 4.0.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Fixed: `[DataMember]` attributes on DTO properties causing incorrect C# syntax errors.

### Version 4.0.0

- Upgrade : `DtoModelTemplate` upgraded to use the `ICSharpFileBuilderTemplate` paradigm.
- Enums will now generate comments captured in designers.


### Version 3.3.10

- It's now possible to control the `JsonPropertyName` attribute on DTO properties by applying the `Serialization Settings` stereotype which is available from the `3.3.11` version, or higher, of the `Intent.Metadata.WebApi` module. The stereotype can be applied either on a `DTO-Field` or the `DTO` to cascade on all `DTO-Field`s.
- Enums will now also be generated.

### Version 3.3.9

- It's now possible to have DTOs inherit from other DTOs.
