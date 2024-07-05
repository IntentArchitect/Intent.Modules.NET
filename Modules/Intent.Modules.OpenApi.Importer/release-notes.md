### Version 1.1.0

- Improvement: Added support for OpenAPI `Required` property . 
- Improvement: Rest services import have their `Http Settings` property `Return Type Mediatype` default to `Default` rather than `application/json`.

### Version 1.0.3

- Improvement: Improved the algorithm for assigning end point names. 
- Improvement: Added warning for array based bodies which can't be realized as Command's. 
- Improvement: Updated to latest `Microsoft.OpenApi.Readers`. 
- Fixed: An issue where tool would not block some valid versions of OpenAPI documents.
- Fixed: An issue rest end points would be imported as the same Operation / Command / Query .

### Version 1.0.2

- Fixed: A `Value cannot be null. (Parameter: key)` error would sometimes occur.

### Version 1.0.1

- Improvement: Fetch schema information from HTTP / HTTPS endpoints.
- Fixed: If a Type starts with a number it will be prefixed. Like "200Success" will become "_200Success".

### Version 1.0.0

- New Feature: Module released.