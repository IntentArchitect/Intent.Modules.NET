### Version 1.0.3

- Fixed: Fails when Entity Interfaces are enabled.

### Version 1.0.1

- Fixed: Integrating with DbContext's SaveChanges now will execute after the possible DomainEvents dispatching. 

### Version 1.0.0

- New Feature: Adds basic auditing fields to an Entity using the `ICurrentUserService` to resolve the user that interacted with the Entity. Fields that are included are:
  * Created By
  * Created Date
  * Updated By
  * Updated Date