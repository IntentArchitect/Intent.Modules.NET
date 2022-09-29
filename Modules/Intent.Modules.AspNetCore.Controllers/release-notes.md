### Version 3.3.11

- New: Http Settings' Return Type Mediatype setting will determine if the primitive return type should be wrapped in a JsonResponse object or not.
- Fixed: Controllers will now add usings for enums.

### Version 3.3.10

- Update: Decorators can add attributes to controllers.
- Fixed: Controller actions that made use of Http Headers didn't specify header names.