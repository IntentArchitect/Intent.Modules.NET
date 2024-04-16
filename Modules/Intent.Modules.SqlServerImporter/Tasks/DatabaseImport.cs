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
			if (!ValiadateRequest(args, out var sqlImportSettings, out var errorMessage ))
			{
				return Fail(errorMessage!);
			}


			var toolDirectory = Path.Combine( Path.GetDirectoryName( typeof(DatabaseImport).Assembly.Location), @"../content/tool");
			var executableName = "dotnet";
			var executableArgs = $"\"{Path.Combine(toolDirectory, "Intent.SQLSchemaExtractor.dll")}\" --serialized-config \"{sqlImportSettings}\"";

			var warnings = new StringBuilder();
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
					else if (args.Data?.Trim().StartsWith("Error:") == true)
					{
						Logging.Log.Failure(args.Data);
						succeeded = false;
						errorMessage = args.Data?.Trim();
						process.Kill(true);
					}
					else
					{
						if (args.Data?.Trim().StartsWith("Warning:") == true)
						{
							warnings.AppendLine(args.Data);
						}
						Logging.Log.Info(args.Data);
					}
				};

				process.Start();
				process.BeginOutputReadLine();
				process.WaitForExit();

				if (succeeded)
				{
					if (warnings.Length > 0)
					{
						return $"{{\"warnings\": \"{warnings.Replace("\"", "\\\"").Replace(Environment.NewLine, "\\n").ToString()}\"}}";
					}
					return "{}";
				}
				else
				{
					return Fail(errorMessage);
				}
			}
			catch (Exception e)
			{
				Logging.Log.Failure($@"Failed to execute: ""Intent.SQLSchemaExtractor.dll"".
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

		private string Fail(string reason)
		{
			Logging.Log.Failure(reason);
			return $"{{\"errorMessage\": \"{reason}\"}}"; 
		}

	}
}
