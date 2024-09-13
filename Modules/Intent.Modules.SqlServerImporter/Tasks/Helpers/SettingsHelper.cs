using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Serialization;
using Intent.IArchitect.CrossPlatform.IO;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.Utils;
using Microsoft.Data.SqlClient;

namespace Intent.Modules.SqlServerImporter.Tasks.Helpers;

public static class SettingsHelper
{
    public static void PersistSettings(DatabaseImportModel importModel)
    {
        Logging.Log.Info($"PackageFileName: {importModel.PackageFileName}");
        var package = LoadPackage(importModel.PackageFileName!);
        
        if (importModel.SettingPersistence != DatabaseSettingPersistence.None)
        {
            package.AddMetadata("sql-import:entityNameConvention", importModel.EntityNameConvention);
            package.AddMetadata("sql-import:tableStereotypes", importModel.TableStereotype);
            package.AddMetadata("sql-import:typesToExport", importModel.TypesToExport.Any() ? string.Join(";", importModel.TypesToExport.Select(t => t.ToString())) : "");
            package.AddMetadata("sql-import:schemaFilter", importModel.SchemaFilter.Any() ? string.Join(";", importModel.SchemaFilter) : "");
            package.AddMetadata("sql-import:tableViewFilterFilePath", importModel.TableViewFilterFilePath);
            package.AddMetadata("sql-import:storedProcedureType", importModel.StoredProcedureType);
            ProcessConnectionStringSetting(package, importModel);
            package.AddMetadata("sql-import:settingPersistence", importModel.SettingPersistence.ToString());
        }
        else
        {
            package.RemoveMetadata("sql-import:entityNameConvention");
            package.RemoveMetadata("sql-import:tableStereotypes");
            package.RemoveMetadata("sql-import:typesToExport");
            package.RemoveMetadata("sql-import:schemaFilter");
            package.RemoveMetadata("sql-import:tableViewFilterFilePath");
            package.RemoveMetadata("sql-import:storedProcedureType");
            package.RemoveMetadata("sql-import:connectionString");
            package.RemoveMetadata("sql-import:settingPersistence");
        }
        
        package.Save();
        return;

        static void ProcessConnectionStringSetting(PackageModelPersistable package, DatabaseImportModel config)
        {
            var connectionString = config.ConnectionString;

            if (config.SettingPersistence == DatabaseSettingPersistence.AllSanitisedConnectionString)
            {
                var builder = new SqlConnectionStringBuilder();
                builder.ConnectionString = connectionString;

                var addPassword = builder.Remove("Password");

                var sanitisedConnectionString = builder.ConnectionString;
                if (addPassword)
                {
                    sanitisedConnectionString = "Password=  ;" + sanitisedConnectionString;
                }

                connectionString = sanitisedConnectionString;
            }

            if (config.SettingPersistence == DatabaseSettingPersistence.AllWithoutConnectionString)
            {
                package.RemoveMetadata("sql-import:connectionString");
            }
            else
            {
                package.AddMetadata("sql-import:connectionString", connectionString);
            }
        }
    }
    
    public static void PersistSettings(RepositoryImportModel importModel)
    {
        Logging.Log.Info($"PackageFileName: {importModel.PackageFileName}");
        var package = LoadPackage(importModel.PackageFileName!);

        if (importModel.SettingPersistence == RepositorySettingPersistence.None)
        {
            package.RemoveMetadata("sql-import-repository:storedProcedureType");
            package.RemoveMetadata("sql-import-repository:connectionString");
            package.RemoveMetadata("sql-import-repository:settingPersistence");
        }
        else
        {
            package.AddMetadata("sql-import-repository:storedProcedureType", importModel.StoredProcedureType);
            ProcessConnectionStringSetting(package, importModel);
            package.AddMetadata("sql-import-repository:settingPersistence", importModel.SettingPersistence.ToString());
        }
        
        package.Save();
        return;

        static void ProcessConnectionStringSetting(PackageModelPersistable package, RepositoryImportModel settings)
        {
            var connectionString = settings.ConnectionString;

            if (settings.SettingPersistence == RepositorySettingPersistence.AllSanitisedConnectionString)
            {
                var builder = new SqlConnectionStringBuilder();
                builder.ConnectionString = connectionString;

                var addPassword = builder.Remove("Password");

                var sanitisedConnectionString = builder.ConnectionString;
                if (addPassword)
                {
                    sanitisedConnectionString = "Password=  ;" + sanitisedConnectionString;
                }

                connectionString = sanitisedConnectionString;
            }

            if (settings.SettingPersistence == RepositorySettingPersistence.AllWithoutConnectionString)
            {
                package.RemoveMetadata("sql-import-repository:connectionString");
            }
            else
            {
                package.AddMetadata("sql-import-repository:connectionString", connectionString);
            }
        }
    }
    
    public static void HydrateDbSettings(RepositoryImportModel importModel)
    {
        var package = LoadPackage(importModel.PackageFileName!);

        if (string.IsNullOrWhiteSpace(importModel.StoredProcedureType))
        {
            importModel.StoredProcedureType = package.GetMetadataValue("sql-import:storedProcedureType");
        }

        importModel.ConnectionString = package.GetMetadataValue("sql-import:connectionString")!;
    }
    
    // We can't use PackageModelPersistable.Load since it uses the underlying cached versions
    // Also it should ONLY load the package as to prevent unfortunate model corruption
    private static PackageModelPersistable LoadPackage(string packagePath)
    {
        var package = XmlSerializationHelper.LoadFromFile<PackageModelPersistable>(packagePath, loadThisFileOnly: true, skipCache: true);
        foreach (var reference in package.References.Where(reference => !string.IsNullOrWhiteSpace(reference.RelativePath)))
        {
            reference.AbsolutePath = Path.GetFullPath(Path.Combine(package.DirectoryPath, reference.RelativePath));
        }
        return package;
    }
}