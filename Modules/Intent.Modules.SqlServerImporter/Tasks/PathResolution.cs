using System;
using System.IO;
using Intent.Engine;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class PathResolution : ModuleTaskSingleInputBase<PathResolutionInputModel>
{
    private readonly IMetadataManager _metadataManager;

    public PathResolution(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }
    
    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.PathResolution";
    public override string TaskTypeName => "SqlServer Path Resolution";

    protected override ValidationResult ValidateInputModel(PathResolutionInputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.SelectedFilePath))
        {
            return ValidationResult.ErrorResult("Selected file path is required");
        }
        
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out _, out var errorMessage))
        {
            return ValidationResult.ErrorResult(errorMessage);
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(PathResolutionInputModel inputModel)
    {
        var executionResult = new ExecuteResult();
        
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out var package, out _))
        {
            return executionResult;
        }
        
        try
        {
            // Normalize paths to handle different path separators
            var selectedFilePath = Path.GetFullPath(inputModel.SelectedFilePath);
            var packageFilePath = Path.GetFullPath(package.FileLocation);
            
            // Get directories
            var selectedFileDirectory = Path.GetDirectoryName(selectedFilePath);
            var packageFileDirectory = Path.GetDirectoryName(packageFilePath);
            
            if (selectedFileDirectory == null || packageFileDirectory == null)
            {
                executionResult.Errors.Add("Unable to determine directory paths");
                return executionResult;
            }

            string resolvedPath;
            if (string.Equals(selectedFileDirectory, packageFileDirectory))
            {
                // Same directory - return relative path with "./"
                var fileName = Path.GetFileName(selectedFilePath);
                resolvedPath = $"./{fileName}";
            }
            else
            {
                // Different directories - return absolute path
                resolvedPath = selectedFilePath;
            }

            executionResult.ResultModel = new 
            { 
                ResolvedPath = resolvedPath,
                IsRelative = resolvedPath.StartsWith("./")
            };
        }
        catch (Exception ex)
        {
            executionResult.Errors.Add($"Error resolving path: {ex.Message}");
            executionResult.ResultModel = new { ResolvedPath = inputModel.SelectedFilePath, IsRelative = false };
        }

        return executionResult;
    }
} 