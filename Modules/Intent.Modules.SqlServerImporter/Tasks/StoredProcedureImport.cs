using System.Linq;
using System.Text.Json;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class RepositoryImport : ModuleTaskSingleInputBase<RepositoryImportModel>
{
    private readonly IMetadataManager _metadataManager;

    public RepositoryImport(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.StoredProcedureImport";
    public override string TaskTypeName => "SqlServer Stored Procedure Import";

    protected override ValidationResult ValidateInputModel(RepositoryImportModel inputModel)
    {
        var designer = _metadataManager.GetDesigner(inputModel.ApplicationId, "Domain");
        if (designer == null)
        {
            return ValidationResult.ErrorResult("Unable to find domain designer in application");
        }

        var package = designer.Packages.FirstOrDefault(p => p.Id == inputModel.PackageId);
        if (package == null)
        {
            return ValidationResult.ErrorResult($"Unable to find package with Id : {inputModel.PackageId}");
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(RepositoryImportModel importModel)
    {
        var executionResult = new ExecuteResult();

        PrepareInputModel(importModel);

        var sqlImportSettings = JsonSerializer.Serialize(importModel).Replace("\"", "\\\"");

        SqlSchemaExtractorRunner.Run($@"--serialized-config ""{sqlImportSettings}""", (output, process) =>
        {
            if (output.Data?.Trim().StartsWith("Error:") == true)
            {
                var error = output.Data.Trim().RemovePrefix("Error:");
                executionResult.Errors.Add(error);
                process.Kill(true);
            }
            else if (output.Data?.Trim().StartsWith("Warning:") == true)
            {
                var warning = output.Data.Trim().RemovePrefix("Warning:");
                executionResult.Warnings.Add(warning);
            }
        });

        if (executionResult.Errors.Count == 0)
        {
            SettingsHelper.PersistSettings(importModel);
        }

        return executionResult;
    }

    private void PrepareInputModel(RepositoryImportModel importModel)
    {
        // After validation, we can safely assume the following lookups:
        var designer = _metadataManager.GetDesigner(importModel.ApplicationId, "Domain");
        var package = designer.Packages.First(p => p.Id == importModel.PackageId);

        importModel.EntityNameConvention = "SingularEntity";
        importModel.TableStereotype = "WhenDifferent";
        importModel.TypesToExport = ["StoredProcedure"];
        importModel.PackageFileName = package.FileLocation;

        if (importModel.SettingPersistence == RepositorySettingPersistence.InheritDb)
        {
            SettingsHelper.HydrateDbSettings(importModel);
        }

        if (string.IsNullOrWhiteSpace(importModel.StoredProcedureType))
        {
            importModel.StoredProcedureType = "Default";
        }
    }
}