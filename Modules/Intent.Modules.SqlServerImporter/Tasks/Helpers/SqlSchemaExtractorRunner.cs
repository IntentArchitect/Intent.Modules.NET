using System;
using System.Diagnostics;
using System.IO;
using Intent.Utils;

namespace Intent.Modules.SqlServerImporter.Tasks.Helpers;

public static class SqlSchemaExtractorRunner
{
    private static readonly string ToolDirectory = Path.Combine(Path.GetDirectoryName(typeof(SqlSchemaExtractorRunner).Assembly.Location)!, @"../content/tool");
    private static readonly string ToolExecutable = Path.Combine(ToolDirectory, "Intent.SQLSchemaExtractor.dll");
    
    public static void Run(string arguments, Action<DataReceivedEventArgs, Process>? outputDataReceived)
    {
        const string executableName = "dotnet";
        var executableArguments = $"\"{ToolExecutable}\" {arguments}";
        Logging.Log.Info($"Executing: {executableName} {executableArguments}");
        
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executableName,
                Arguments = executableArguments,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = false,
                UseShellExecute = false,
                WorkingDirectory = ToolDirectory,
                EnvironmentVariables =
                {
                    ["DOTNET_CLI_UI_LANGUAGE"] = "en"
                }
            }
        };

        if (outputDataReceived is not null)
        {
            process.OutputDataReceived += (_, eventArgs) =>
            {
                outputDataReceived(eventArgs, process);
            };
        }

        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();
    }
}