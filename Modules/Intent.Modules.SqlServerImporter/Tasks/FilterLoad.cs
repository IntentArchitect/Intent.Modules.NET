using System;
using System.IO;
using System.Text.Json;
using Intent.Engine;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class FilterLoad : ModuleTaskSingleInputBase<FilterLoadInputModel>
{
    private readonly IMetadataManager _metadataManager;

    public FilterLoad(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }
    
    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.FilterLoad";
    public override string TaskTypeName => "SqlServer Filter Load";

    protected override ValidationResult ValidateInputModel(FilterLoadInputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.ImportFilterFilePath))
        {
            return ValidationResult.ErrorResult("Import filter file path is required");
        }
        
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out _, out var errorMessage))
        {
            return ValidationResult.ErrorResult(errorMessage);
        }
        
        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(FilterLoadInputModel inputModel)
    {
        var executionResult = new ExecuteResult();
        
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out var package, out _))
        {
            return executionResult;
        }
        
        try
        {
            var filePath = inputModel.ImportFilterFilePath;
            
            // Handle relative paths relative to package directory
            if (!Path.IsPathRooted(filePath) && !string.IsNullOrWhiteSpace(package.FileLocation))
            {
                var packageDirectory = Path.GetDirectoryName(package.FileLocation);
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