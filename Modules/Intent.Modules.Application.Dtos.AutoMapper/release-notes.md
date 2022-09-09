### Version 3.3.6

- Fixed: Mapping from a DTO field that is of a complex type to a Domain Entity association that is also of Complex type no longer results in trying to map a surrogate key to the DTO Field.

### Version 3.3.4

- New: DTO Field mappings that map from an Entity on an association level can now be mapped to a primitive type that represents a surrogate key and it will automatically map the Ids.