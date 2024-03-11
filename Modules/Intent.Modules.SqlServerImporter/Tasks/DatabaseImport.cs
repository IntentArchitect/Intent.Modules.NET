using Intent.Engine;
using Intent.Plugins;
using Intent.Utils;
using Microsoft.VisualBasic;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Intent.Modules.SqlServerImporter.Tasks
{
	public class DatabaseImport : IModuleTask
	{
		private readonly IMetadataManager _metadataManager;

		public DatabaseImport(IMetadataManager metadataManager)
        {				
			_metadataManager = metadataManager;
        }

        public string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.DatabaseImport";

		public string TaskTypeName => "SqlServer Database Import";

		public int Order => 0;

		public string Execute(params string[] args)
		{
			string toolname = "intent-sql-schema-extractor";
			var minimumToolVersion = new NuGetVersion(1, 1, 2);

			if (!IsToolInstalled(toolname))
			{
				return Fail($"({toolname}) not installed. You can install it from a Terminal as follows: \\r\\ndotnet tool install Intent.SQLSchemaExtractor --global");
			}
			if (!CheckToolVersion(toolname, minimumToolVersion))
			{
				return Fail($"({toolname}) needs to be updated to at least ({minimumToolVersion.ToString()}). You can install it from a Terminal as follows: \\r\\ndotnet tool update Intent.SQLSchemaExtractor --global");
			}

			if (!ValiadateRequest(args, out var sqlImportSettings, out var errorMessage ))
			{
				return Fail(errorMessage!);
			}


			var toolDirectory = Path.Combine( Path.GetDirectoryName( typeof(DatabaseImport).Assembly.Location), @"../content/tool");
			var executableName = "dotnet";
			var executableArgs = $"\"{Path.Combine(toolDirectory, "Intent.SQLSchemaExtractor.dll")}\" --serialized-config \"{sqlImportSettings}\"";

			Logging.Log.Info($"Executing: {executableName} {executableArgs} ");
			var succeeded = false;
			try
			{
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

				process.OutputDataReceived += (_, args) =>
				{
					if (args.Data?.Trim().StartsWith("Package saved successfully.") == true)
					{
						succeeded = true;
					}
					else if (args.Data?.Trim().StartsWith("Error :") == true)
					{
						Logging.Log.Failure(args.Data);
						succeeded = false;
						errorMessage = args.Data?.Trim();
						process.Kill(true);
					}
					else
					{ 
						Logging.Log.Info(args.Data);
					}
				};

				process.Start();
				process.BeginOutputReadLine();
				process.WaitForExit();

				if (succeeded)
				{
					return "{}";
				}
				else
				{
					return Fail(errorMessage);
				}
			}
			catch (Exception e)
			{
				Logging.Log.Failure($@"Failed to execute: ""{toolname}"".
Please see reasons below:");
				Logging.Log.Failure(e);
				return Fail(e.GetBaseException().Message);
			}
		}

		private bool ValiadateRequest(string[] args, out string? sqlImportSettings, out string? errorMessage)
		{
			sqlImportSettings = null;
			errorMessage = null;
			
			var serializerOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			};
			if (args.Length < 1)
			{
				errorMessage = $"Expected 1 argument received 0";
				return false;
			}
			var settings = JsonSerializer.Deserialize<ImportSettings>(args[0], serializerOptions);
			if (settings == null)
			{
				errorMessage = $"Unable to deserialize : {args[0]}";
				return false;
			}

			var designer = _metadataManager.GetDesigner(settings.ApplicationId, "Domain");
			if (designer == null)
			{
				errorMessage = $"Unable to find domain designer in application";
				return false;
			}

			var package = designer.Packages.FirstOrDefault(p => p.Id == settings.PackageId);
			if (package == null)
			{
				errorMessage = $"Unable to find package with Id : {settings.PackageId}";
				return false;
			}

			settings.PackageFileName = package.FileLocation;
			
			var sending = JsonSerializer.Serialize(settings);
			sqlImportSettings = sending.Replace("\"", "\\\"");
			return true;
		}

		private bool CheckToolVersion(string toolname, NuGetVersion minimumVersion)
		{
			var processStartInfo = new ProcessStartInfo
			{
				FileName = toolname,
				Arguments = "--version",
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				CreateNoWindow = false,
				UseShellExecute = false
			};

			var cmd = Process.Start(processStartInfo)!;
			cmd.WaitForExit(5000);
			if (!cmd.HasExited)
			{
				throw new Exception("Timeout exceeded when performing \"intent-sql-schema-extractor --version\"");
			}

			var output = cmd.StandardOutput.ReadToEnd();

			var versionString = output.Trim();
			if (versionString.Contains('\n'))
			{
				versionString = versionString.Substring(versionString.LastIndexOf('\n') + 1);
			}
			var currentVerion =  NuGetVersion.Parse(versionString);
			Logging.Log.Info($"using {toolname}:{versionString}");
			return currentVerion >= minimumVersion;
		}

		static bool IsToolInstalled(string toolName)
		{
			try
			{
				string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";

				Process process = new Process();
				process.StartInfo.FileName = command;
				process.StartInfo.Arguments = toolName;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.CreateNoWindow = true;
				process.Start();

				string output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();

				// Check if the tool executable path is found in the output
				return !string.IsNullOrEmpty(output) && output.Contains(toolName);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error checking for {toolName}: {ex.Message}");
				return false;
			}
		}

		private string Fail(string reason)
		{
			Logging.Log.Failure(reason);
			return $"{{\"errorMessage\": \"{reason}\"}}"; 
		}

	}
}
