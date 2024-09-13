using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.Engine;
using Intent.IArchitect.CrossPlatform.IO;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.Plugins;
using Intent.Utils;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class RepositoryImport : IModuleTask
{
    private readonly IMetadataManager _metadataManager;
    
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
    
    public RepositoryImport(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    public string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.StoredProcedureImport";
    public string TaskTypeName => "SqlServer Stored Procedure Import";
    public int Order => 0;

    public string Execute(params string[] args)
    {
        try
        {
            if (!ValidateRequest(args, out var importModel, out var errorMessage))
            {
                return Fail(errorMessage!);
            }

            var sqlImportSettings = JsonSerializer.Serialize(importModel);
            sqlImportSettings = sqlImportSettings.Replace("\"", "\\\"");

            Logging.Log.Info(sqlImportSettings);
            
            var toolDirectory = Path.Combine(Path.GetDirectoryName(typeof(RepositoryImport).Assembly.Location)!, @"../content/tool");
            const string executableName = "dotnet";
            var executableArgs = $"\"{Path.Combine(toolDirectory, "Intent.SQLSchemaExtractor.dll")}\" --serialized-config \"{sqlImportSettings}\"";

            var warnings = new StringBuilder();
            var succeeded = false;

            Logging.Log.Info($"Executing: {executableName} {executableArgs}");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executableName,
                    Arguments = executableArgs,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    WorkingDirectory = toolDirectory,
                    EnvironmentVariables =
                    {
                        ["DOTNET_CLI_UI_LANGUAGE"] = "en"
                    }
                }
            };

            process.OutputDataReceived += (_, eventArgs) =>
            {
                if (eventArgs.Data?.Trim().StartsWith("Package saved successfully.") == true)
                {
                    succeeded = true;
                }
                else if (eventArgs.Data?.Trim().StartsWith("Error:") == true)
                {
                    Logging.Log.Failure(eventArgs.Data);
                    succeeded = false;
                    errorMessage = eventArgs.Data?.Trim();
                    process.Kill(true);
                }
                else
                {
                    if (eventArgs.Data?.Trim().StartsWith("Warning:") == true)
                    {
                        warnings.AppendLine(eventArgs.Data);
                    }

                    Logging.Log.Info(eventArgs.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            if (!succeeded)
            {
                return Fail(errorMessage!);
            }
            
            SettingsHelper.PersistSettings(importModel!);
            
            return warnings.Length > 0 ? JsonSerializer.Serialize(new { warnings = warnings.ToString() }, SerializerOptions) : "{}";
        }
        catch (Exception e)
        {
            Logging.Log.Failure($@"Failed to execute: ""Intent.SQLSchemaExtractor.dll"".
Please see reasons below:");
            Logging.Log.Failure(e);
            return Fail(e.GetBaseException().Message);
        }
    }

    private bool ValidateRequest(string[] args, out RepositoryImportModel? importModel, out string? errorMessage)
    {
        importModel = null;
        errorMessage = null;

        if (args.Length < 1)
        {
            errorMessage = "Expected 1 argument received 0";
            return false;
        }

        var settings = JsonSerializer.Deserialize<RepositoryImportModel>(args[0], SerializerOptions);
        if (settings == null)
        {
            errorMessage = $"Unable to deserialize : {args[0]}";
            return false;
        }
        
        importModel = settings;

        var designer = _metadataManager.GetDesigner(settings.ApplicationId, "Domain");
        if (designer == null)
        {
            errorMessage = "Unable to find domain designer in application";
            return false;
        }

        var package = designer.Packages.FirstOrDefault(p => p.Id == settings.PackageId);
        if (package == null)
        {
            errorMessage = $"Unable to find package with Id : {settings.PackageId}";
            return false;
        }

        settings.EntityNameConvention = "SingularEntity";
        settings.TableStereotype = "WhenDifferent";
        settings.TypesToExport = ["StoredProcedure"];
        settings.SchemaFilter = [];
        settings.PackageFileName = package.FileLocation;
        
        if (settings.SettingPersistence == RepositorySettingPersistence.InheritDb)
        {
            SettingsHelper.HydrateDbSettings(settings);
        }

        if (string.IsNullOrWhiteSpace(settings.StoredProcedureType))
        {
            settings.StoredProcedureType = "Default";
        }
        
        return true;
    }

    private static string Fail(string reason)
    {
        Logging.Log.Failure(reason);
        var errorObject = new { errorMessage = reason };
        var json = JsonSerializer.Serialize(errorObject);
        return json;
    }
}