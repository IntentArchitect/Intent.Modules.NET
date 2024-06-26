﻿using Intent.Engine;
using Intent.Plugins;
using Intent.Utils;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;


namespace Intent.Modules.OpenApi.Importer.Tasks
{
	public class OpenApiImport : IModuleTask
	{
		private readonly IMetadataManager _metadataManager;
		private readonly IApplicationConfigurationProvider _configurationProvider;

		public OpenApiImport(IMetadataManager metadataManager, IApplicationConfigurationProvider configurationProvider)
		{
			_metadataManager = metadataManager;
			_configurationProvider = configurationProvider;
		}

		public string TaskTypeId => "Intent.Modules.OpenApi.Importer.Tasks.OpenApiImport";

		public string TaskTypeName => "OpenApi Document Import";

		public int Order => 0;

		public string Execute(params string[] args)
		{
			if (!ValiadateRequest(args, out var openPpiImportSettings, out var errorMessage))
			{
				return Fail(errorMessage!);
			}


			var toolDirectory = Path.Combine(Path.GetDirectoryName(typeof(OpenApiImport).Assembly.Location), @"../content/tool");
			var executableName = "dotnet";
			var executableArgs = $"\"{Path.Combine(toolDirectory, "Intent.MetadataSynchronizer.OpenApi.CLI.dll")}\" --serialized-config \"{openPpiImportSettings}\"";

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
					if (args.Data?.Trim().Contains("Package saved successfully.") == true)
					{
						succeeded = true;
					}
					else if (args.Data?.Trim().Contains("Error:") == true)
					{
						Logging.Log.Failure(args.Data);
						succeeded = false;
						errorMessage = args.Data?.Trim();
						process.Kill(true);
					}
					else
					{
						if (args.Data?.Trim().Contains("Warning:") == true)
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
						return $"{{\"warnings\": \"{warnings.Replace(Environment.NewLine, "\\n").ToString()}\"}}";
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
				Logging.Log.Failure($@"Failed to execute: ""Intent.MetadataSynchronizer.OpenApi.CLI.dll"".
Please see reasons below:");
				Logging.Log.Failure(e);
				return Fail(e.GetBaseException().Message);
			}
		}

		private bool ValiadateRequest(string[] args, out string? openApiImportSettings, out string? errorMessage)
		{
			openApiImportSettings = null;
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

			var application = _configurationProvider.GetApplicationConfig();

			var designer = _metadataManager.GetDesigner(application.Id, "Services");
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

			settings.OpenApiSpecificationFile = settings.OpenApiSpecificationFile.Trim('"');

			settings.ApplicationName = application.Name;

			var file = Directory.GetFiles(_configurationProvider.GetSolutionConfig().SolutionRootLocation, "*.isln").First();
			settings.IslnFile = file;
			settings.PackageId = package.Id;
			settings.IsAzureFunctions = application.Modules.Any(m => m.ModuleId == "Intent.AzureFunctions");

			var sending = JsonSerializer.Serialize(settings);
			openApiImportSettings = sending.Replace("\"", "\\\"");
			return true;
		}

		private string Fail(string reason)
		{
			Logging.Log.Failure(reason);
			return $"{{\"errorMessage\": \"{reason}\"}}";
		}
	}
}

