using System;
using System.IO;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class PackageLocationGet : ModuleTaskSingleInputBase<PackageLocationInputModel>
{
    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.PackageLocationGet";
    public override string TaskTypeName => "SqlServer Package Location Get";

    protected override ValidationResult ValidateInputModel(PackageLocationInputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.ApplicationId))
        {
            return ValidationResult.ErrorResult("Application ID is required");
        }

        if (string.IsNullOrWhiteSpace(inputModel.PackageId))
        {
            return ValidationResult.ErrorResult("Package ID is required");
        }
        
        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(PackageLocationInputModel inputModel)
    {
        var executionResult = new ExecuteResult();
        
        try
        {
            // This task uses the Intent Architect API to get the package file path
            // We'll return the package directory path for file operations
            executionResult.ResultModel = new 
            { 
                PackageDirectory = ".", // Default to current directory, this will be resolved by the frontend
                ApplicationId = inputModel.ApplicationId,
                PackageId = inputModel.PackageId
            };
        }
        catch (Exception ex)
        {
            executionResult.Errors.Add($"Error getting package location: {ex.Message}");
            executionResult.ResultModel = new { PackageDirectory = "" };
        }

        return executionResult;
    }
} 