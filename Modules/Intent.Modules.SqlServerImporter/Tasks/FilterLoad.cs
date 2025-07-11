using System;
using System.IO;
using System.Text.Json;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class FilterLoad : ModuleTaskSingleInputBase<FilterLoadInputModel>
{
    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.FilterLoad";
    public override string TaskTypeName => "SqlServer Filter Load";

    protected override ValidationResult ValidateInputModel(FilterLoadInputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.ImportFilterFilePath))
        {
            return ValidationResult.ErrorResult("Import filter file path is required");
        }
        
        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(FilterLoadInputModel inputModel)
    {
        var executionResult = new ExecuteResult();
        
        try
        {
            var filePath = inputModel.ImportFilterFilePath;
            
            // Handle relative paths relative to package directory
            if (!Path.IsPathRooted(filePath) && !string.IsNullOrWhiteSpace(inputModel.PackageFileName))
            {
                var packageDirectory = Path.GetDirectoryName(inputModel.PackageFileName);
                if (!string.IsNullOrWhiteSpace(packageDirectory))
                {
                    filePath = Path.Combine(packageDirectory, filePath);
                }
            }

            if (!File.Exists(filePath))
            {
                executionResult.Warnings.Add($"Filter file not found: {filePath}");
                executionResult.ResultModel = new ImportFilterModel();
                return executionResult;
            }

            var jsonContent = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                executionResult.ResultModel = new ImportFilterModel();
                return executionResult;
            }

            var filterModel = JsonSerializer.Deserialize<ImportFilterModel>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            executionResult.ResultModel = filterModel ?? new ImportFilterModel();
        }
        catch (Exception ex)
        {
            executionResult.Errors.Add($"Error loading filter file: {ex.Message}");
            executionResult.ResultModel = new ImportFilterModel();
        }

        return executionResult;
    }
} 