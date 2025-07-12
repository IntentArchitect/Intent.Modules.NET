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

    protected override ValidationResult ValidateInputModel(DatabaseImportModel importModel)
    {
        var designer = _metadataManager.GetDesigner(importModel.ApplicationId, "Domain");
        if (designer == null)
        {
            return ValidationResult.ErrorResult("Unable to find domain designer in application");
        }

        var package = designer.Packages.FirstOrDefault(p => p.Id == importModel.PackageId);
        if (package == null)
        {
            return ValidationResult.ErrorResult($"Unable to find package with Id : {importModel.PackageId}");
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

    private void PrepareInputModel(DatabaseImportModel importModel)
    {
        // After validation, we can safely assume the following lookups:
        var designer = _metadataManager.GetDesigner(importModel.ApplicationId, "Domain");
        var package = designer.Packages.First(p => p.Id == importModel.PackageId);

        // Required for the underlying CLI tool
        importModel.PackageFileName = package.FileLocation;

        if (string.IsNullOrWhiteSpace(importModel.StoredProcedureType))
        {
            importModel.StoredProcedureType = "Default";
        }
    }
}