using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class DatabaseImport : ModuleTaskSingleInputBase<DatabaseImportModel>
{
    private readonly IMetadataManager _metadataManager;

    public DatabaseImport(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.DatabaseImport";
    public override string TaskTypeName => "SqlServer Database Import";

    protected override ValidationResult ValidateInputModel(DatabaseImportModel inputModel)
    {
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out _, out var errorMessage))
        {
            return ValidationResult.ErrorResult(errorMessage);
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(DatabaseImportModel importModel)
    {
        var executionResult = new ExecuteResult();

        PrepareInputModel(importModel);

        var sqlImportSettings = JsonSerializer.Serialize(importModel).Replace("\"", "\\\"");

        var errorMessage = new StringBuilder();
        SqlSchemaExtractorRunner.Run($@"--serialized-config ""{sqlImportSettings}""", (output, process) =>
        {
            if (output.Data?.Trim().StartsWith("Error:") == true)
            {
                var error = output.Data.Trim().RemovePrefix("Error:");
                errorMessage.AppendLine(error);
            }
            else if (output.Data?.Trim().StartsWith("Warning:") == true)
            {
                var warning = output.Data.Trim().RemovePrefix("Warning:");
                executionResult.Warnings.Add(warning);
            }
            else if (errorMessage.Length > 0)
            {
                if (output.Data.Trim().Equals("."))
                {
                    process.Kill(true);
                    return;
                }
                errorMessage.AppendLine(output.Data);
            }
        });

        if (errorMessage.Length > 0)
        {
            executionResult.Errors.Add(errorMessage.ToString());
        }

        if (executionResult.Errors.Count == 0)
        {
            SettingsHelper.PersistSettings(importModel);
        }

        return executionResult;
    }

    private void PrepareInputModel(DatabaseImportModel inputModel)
    {
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out var package, out _))
        {
            return;
        }

        // Required for the underlying CLI tool
        inputModel.PackageFileName = package.FileLocation;
        
        if (!Path.IsPathRooted(inputModel.ImportFilterFilePath) && !string.IsNullOrWhiteSpace(package.FileLocation))
        {
            var packageDirectory = Path.GetDirectoryName(package.FileLocation);
            if (!string.IsNullOrWhiteSpace(packageDirectory))
            {
                inputModel.ImportFilterFilePath = Path.Combine(packageDirectory, inputModel.ImportFilterFilePath);
            }
        }

        if (string.IsNullOrWhiteSpace(inputModel.StoredProcedureType))
        {
            inputModel.StoredProcedureType = "Default";
        }
    }
}