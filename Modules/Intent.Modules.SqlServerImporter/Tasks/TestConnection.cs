using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.Plugins;
using Intent.Utils;
using Serilog;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class TestConnection : IModuleTask
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
    
    public string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.TestConnection";
    public string TaskTypeName => "SqlServer Database Connection Tester";
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
            
            var toolDirectory = Path.Combine(Path.GetDirectoryName(typeof(TestConnection).Assembly.Location)!, @"../content/tool");
            const string executableName = "dotnet";
            var executableArgs = $@"""{Path.Combine(toolDirectory, "Intent.SQLSchemaExtractor.dll")}"" test-connection --connection ""{importModel.ConnectionString}""";
            
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

            process.OutputDataReceived += (_, eventArgs) =>
            {
                if (eventArgs.Data?.Trim().Equals("Successfully established a connection.") == true)
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
            };

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            var resultModel = new TestConnectionResultModel
            {
                Success = succeeded,
                Message = errorMessage
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

    private bool ValidateRequest(string[] args, out TestConnectionInputModel? importModel, out string? errorMessage)
    {
        importModel = null;
        errorMessage = null;

        if (args.Length < 1)
        {
            errorMessage = "Expected 1 argument received 0";
            return false;
        }

        var settings = JsonSerializer.Deserialize<TestConnectionInputModel>(args[0], SerializerOptions);
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