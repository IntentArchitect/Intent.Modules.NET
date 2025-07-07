using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.Plugins;
using Intent.Utils;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class StoredProcList : IModuleTask
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
    
    public string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.StoredProcList";
    public string TaskTypeName => "SqlServer Stored Procedure List";
    public int Order => 0;

    public string Execute(params string[] args)
    {
        try
        {
            if (!ValidateRequest(args, out var importModel, out var errorMessage))
            {
                return Fail(errorMessage!);
            }

            if (importModel is null)
            {
                return Fail("Problem validating request model.");
            }

            

            var toolDirectory = Path.Combine(Path.GetDirectoryName(typeof(DatabaseImport).Assembly.Location)!, @"../content/tool");
            const string executableName = "dotnet";
            var executableArgs = $@"""{Path.Combine(toolDirectory, "Intent.SQLSchemaExtractor.dll")}"" list-stored-proc --connection ""{importModel.ConnectionString}""";
            
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
            
            var succeeded = false;
            var storedProcs = new List<string>();
            var capture = false;

            process.OutputDataReceived += (_, eventArgs) =>
            {
                if (eventArgs.Data?.Trim().StartsWith("Stored Procedures:") == true)
                {
                    capture = true;
                }
                else if (capture && eventArgs.Data?.Trim() == ".")
                {
                    capture = false;
                    succeeded = true;
                    process.Kill(true);
                }
                else if (capture)
                {
                    storedProcs.Add(eventArgs.Data ?? "");
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            if (!succeeded)
            {
                return Fail(errorMessage!);
            }

            var resultModel = new StoredProcListResultModel
            {
                StoredProcs = storedProcs
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(s =>
                    {
                        var parts = s.Split(".");
                        var schema = parts[0];
                        var name = parts[1];
                        return new { Schema = schema, Name = name };
                    })
                    .GroupBy(k => k.Schema, v => v.Name)
                    .ToDictionary(k => k.Key, v => v.ToArray())
            };
            var resultJson = JsonSerializer.Serialize(resultModel, SerializerOptions);
            Logging.Log.Info(resultJson);
            return resultJson;
        }
        catch (Exception e)
        {
            Logging.Log.Failure($@"Failed to execute: ""Intent.SQLSchemaExtractor.dll"".
Please see reasons below:");
            Logging.Log.Failure(e);
            return Fail(e.GetBaseException().Message);
        }
    }

    private bool ValidateRequest(string[] args, out StoredProcListInputModel? importModel, out string? errorMessage)
    {
        importModel = null;
        errorMessage = null;

        if (args.Length < 1)
        {
            errorMessage = "Expected 1 argument received 0";
            return false;
        }

        var settings = JsonSerializer.Deserialize<StoredProcListInputModel>(args[0], SerializerOptions);
        if (settings == null)
        {
            errorMessage = $"Unable to deserialize : {args[0]}";
            return false;
        }
        
        importModel = settings;
        
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