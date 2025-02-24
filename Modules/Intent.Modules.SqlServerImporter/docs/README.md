﻿# Intent.SqlServerImporter

This module added to the Domain Designer allowing you to import / reverse engineer domain models from SQl Server databases.

## Domain Designer

In the `Domain Designer`, right-click on your domain package and select the `Database Import` context menu option.

![Database Import context menu item](images/db-import.png)

Selecting this option will provide you with the following dialog:

![Database Import dialog](images/db-import-dialog.png)

### Dialog options

#### Connection String

The connection string for the SQL Server database you wish to import.

#### Entity name convention

The setting controls the naming convention of the entities which will be created in the Domain Designer.

- `Singularized table name`,  entity names will be the SQL Table names, singularized. e.g. Customers -> Customer.
- `Table name, as is`, entity names will be the SQL Table names, as is.

#### Apply table stereotypes

The setting controls under which conditions Table stereotypes are applied to the Entities. Tables stereotypes use to specify the underlying SQL Table name.
Sometimes Entity names may not be directly translatable back to the original table name due to differences in allowable character sets.

- `If They Differ`,  only introduce Table stereotypes if the Entity name does not translate back to the original table name.
- `Always`, always add explicit tables names.

#### Include Type (s)

Select which SQL Types you would like to export e.g. Tables, Views and / or Stored Procedures.

#### Import Filter File

Specify a JSON file path **(that may be relative file path to the Package file being imported into)** that of the following structure to assist with importing only certain objects from SQL Server.


```json
{
  "schemas": [
    "dbo"
  ],
  "include_tables": [
    {
      "name": "ExistingTableName",
      "exclude_columns": [
        "LegacyColumn"
      ]
    }
  ],
  "include_views": [
    {
      "name": "ExistingViewName",
      "exclude_columns": [
        "LegacyColumn"
      ]
    }
  ],
  "include_stored_procedures": [
    "ExistingStoredProcedureName"
  ],
  "exclude_tables": [
    "LegacyTableName"
  ],
  "exclude_views": [
    "LegacyViewName"
  ],
  "exclude_stored_procedures": [
    "LegacyStoredProcedureName"
  ]
}
```

| JSON Field                | Description                                                                                                    |
|---------------------------|----------------------------------------------------------------------------------------------------------------|
| schemas                   | Database Schema names to import (rest is filtered out).                                                        |
| include_tables            | Database Tables to import (rest is filtered out).                                                              |
| include_views             | Database Views to import (rest is filtered out).                                                               |
| include_stored_procedures | Database Stored Procedures to import (rest is filtered out).                                                   |
| exclude_tables            | Database Tables to exclude from import (include takes preference above this if same name is found).            |
| exclude_views             | Database Views to exclude from import (include takes preference above this if same name is found).             |
| exclude_stored_procedures | Database Stored Procedures to exclude from import (include takes preference above this if same name is found). |

#### Stored Procedure Representations

Choose between using Repository Elements and Repository Operations to represent your Stored Procedures.

- Stored Procedure Element
   ![Stored Procedure Element](images/stored-procedure-element.png)

- Stored Procedure Operation
  ![Stored Procedure Operation](images/stored-procedure-operation.png)

#### Persist Settings

The dialog can remember your configuration for the next time you want to run it. If you choose to persist the settings, they are saved in the `Domain Package` which is committed in your source code repository.
If you have any concerns around committing connection string in your code base, use the relevant option to avoid this.

- `(None)`,  settings will not be persisted and remembered. Previously saved configuration will be deleted.
- `All (with Sanitized connection string, no password)`,  All settings will be persisted, if the connection string has a password in it, the password will not be persisted.
- `All (without connection string)`,  All settings, except for the connection string, will be persisted.
- `All`, All settings will be persisted.