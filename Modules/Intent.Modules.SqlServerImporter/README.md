# Intent.SqlServerImporter

This module added to the Domain Designer allowing you to import / reverse engineer domain models from SQl Server databases.

## Pre-requisites

This module works in conjunction with our `Intent.SQLSchemaExtractor` CLI tool.

## Installing `Intent.SQLSchemaExtractor` CLI tool

The tool is available as a [.NET Tool](https://docs.microsoft.com/dotnet/core/tools/global-tools) and can be installed with the following command:

```powershell
dotnet tool install Intent.SQLSchemaExtractor --global --prerelease
```

> [!NOTE]
> If `dotnet tool install` fails with an error to the effect of `The required NuGet feed can't be accessed, perhaps because of an Internet connection problem.` and it shows a private NuGet feed URL, you can try add the `--ignore-failed-sources` command line option ([source](https://learn.microsoft.com/dotnet/core/tools/troubleshoot-usage-issues#nuget-feed-cant-be-accessed)).

You should see output to the effect of:

```text
You can invoke the tool using the following command: intent-sqlschema-extractor
Tool 'intent-sqlschema-extractor' (version 'x.x.x') was successfully installed.
```

## Domain Designer

In the `Domain Designer`, right click on your domain package and select the `Database Import` context menu option.

![Database Import context menu item](./docs/images/db-import.png)

Selecting this option will provide you with the following dialog:

![Database Import dialog](./docs/images/db-import-dialog.png)

### Dialog options

#### Connection String

The connection string for the SQL Server database you wish to import.

#### Entity name convention

The setting controls the naming convention of the entities which will be created in the Domain Designer.

- `Singularized table name`,  entity names will be the Sql Table names, singularized. e.g. Customers -> Customer.
- `Table name, as is`, entity names will be the Sql Table names, as is.

#### Apply table stereotypes

The setting controls under which conditions Table stereotypes are applied to the Entities. Tables stereotypes use use to specify the underlying SQL Table name.
Sometimes Entity names may not be directly translatable back to the original table name due to differences in allowable character sets.

- `If They Differ`,  only introduce Table stereotypes if the Entity name does not translate back to the original table name.
- `Always`, always add explict tables names.

#### Schema Filter

A `;` seperated list of SQL Schemas to export from, allowing you to subset your export if required.

#### Include Type (s)

Select which SQL Types you would like to export e.g. Tables, Views and / or Stored Procedues.

#### Persist Settings

The dialog can remeber your configuration for the next time you want to run it. If you choose to persiste the settings, they are saved in the `Domain Package` which is committed in your source code repository.
If you have any concerns around committing connection string in your code base, use the relevant otpion to avoid this.

- `(None)`,  settings will not be persisted and remembered. Previously saved configuration will be deleted.
- `All (with Sanitized connection string, no password)`,  All settings will be persisted, if the connection string has a password in it, the password will not be persisted.
- `All (without connection string)`,  All settings, except for the connection string, will be persisted.
- `All`, All settings will be persisted.