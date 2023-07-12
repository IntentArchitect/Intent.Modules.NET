# Intent.EntityFrameworkCore.BasicAuditing

Extend Domain Entities to have fields that record which user created / updated them and at what time.

> [!NOTE]
> This is not an Audit Trail but merely a way to determine who touched an Entity and when.

Select an Entity in the Domain Designer.

![Domain Entity without Basic Auditing](docs/images/person-without-auditing.png)

Right click and select `Apply Stereotype` (or press F3).

![Auditing Stereotype](docs/images/basic-auditing-stereotype.png)

Select the `Basic Auditing` stereotype.

![Domain Entity with Basic Auditing](docs/images/person-with-auditing.png)

Your Entity will now be extended with the following attributes:

* CreatedBy - User name that created this Entity instance.
* CreatedDate - Timestamp when creation took place.
* UpdatedBy - User name that updated this Entity instance.
* UpdatedDate - Timestamp when creation took place.

> [!NOTE]
> It is worth noting that the "updated" attributes remain null upon creation and only get populated when an update has taken place.
