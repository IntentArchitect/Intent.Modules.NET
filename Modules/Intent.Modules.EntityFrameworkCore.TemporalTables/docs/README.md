# Intent.EntityFrameworkCore.TemporalTables

This modules adds support for [SQL Server temporal tables](https://learn.microsoft.com/en-us/ef/core/providers/sql-server/temporal-tables). in Entity Framework Core. Temporal tables allow you to automatically track historical data changes, enabling you to easily query historical states of your data.

## Configure Temporal Table

To configure a class as a temporal table in the `Domain Designer`, apply the `Temporal Table` stereotype to the class.

### Temporal Table annotation

Once applied, the class will be annotated with an icon to indicate it has been configured as a _temporal table_:

![Annotated](images/temporal-annotation.png)

### Optional Configuration Properties

There are three optional properties available for customization of the temporal table configuration. You can leave these properties as is to use the default values:

![Properties](images/temporal-table-properties.png)

- **Period Start Column Name**: Override the default period start column name (if left blank, the default EntityFramework Core value of _PeriodStart_ will be used).
- **Period End Column Name**: Override the default period end column name (if left blank, the default EntityFramework Core value of _PeriodEnd_ will be used).
- **History Table Name**: Override the default history table name (if left blank, the default EntityFramework Core value of _{TableName}History_ will be used)

After configuring the temporal table, you will need to create an Entity Framework migration. This will generate the necessary schema changes in the database to reflect the temporal table configuration. Follow the usual migration creation process to achieve this.

## Access Temporal Information

By default, Intent Architect will generated a new method `FindHistoryAsync` on the repository of the specific entity, allowing for retrieval of historic information.

This methods accepts a `TemporalHistoryQueryOptions` instance as a parameter, which is used to determine how to retrieve the temporal information.

`TemporalHistoryQueryOptions` has three optional fields:
- QueryType: Defaults to `All` (see below), if not supplied.
- DateFrom: Defaults to `DateTime.MinValue` if not supplied.
- DateTo: Defaults to `DateTime.MaxValue` if not supplied.

`QueryType` can be one of the following 5 options, which is used in conjunction with the date fields to retrieve the information:
- **All**: The default. The date values are ignored (even if supplied) and all history rows are returned.
- **AsOf**: Returns rows that were active (current) at the given UTC time, _DateFrom_.
- **FromTo**:  Returns all rows that were active between the two given UTC times, _DateFrom_ and _DateTo_.
- **Between**:  The same as `FromTo` except that rows are included that became active on the upper boundary.
- **ContainedIn**:  Returns all rows that started being active and ended being active between the two given UTC times, _DateFrom_ and _DateTo_.

Additional information on context on these types can be found on the [Entity Framework Core documentation website.](https://learn.microsoft.com/en-us/ef/core/providers/sql-server/temporal-tables#querying-historical-data)

