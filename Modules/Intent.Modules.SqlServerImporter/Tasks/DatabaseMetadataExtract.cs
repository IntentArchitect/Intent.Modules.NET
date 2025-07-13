using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Intent.Modules.Common.Templates;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class DatabaseMetadataExtract : ModuleTaskSingleInputBase<DatabaseMetadataInputModel>
{
    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.DatabaseMetadataExtract";
    public override string TaskTypeName => "SqlServer Database Metadata Extract";

    protected override ValidationResult ValidateInputModel(DatabaseMetadataInputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.ConnectionString))
        {
            return ValidationResult.ErrorResult("Connection string is required");
        }
        
        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(DatabaseMetadataInputModel importModel)
    {
        var executionResult = new ExecuteResult();
        
        SqlSchemaExtractorRunner.Run($@"extract-metadata --connection ""{importModel.ConnectionString}""", (output, process) =>
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
            else if (output.Data?.Trim().StartsWith('{') == true)
            {
                var jsonResultPayload = output.Data.Trim();
                var result = JsonSerializer.Deserialize<ExtractMetadataResult>(jsonResultPayload, SerializerOptions);
                if (result is null)
                {
                    throw new Exception($"Unable to deserialize result from {jsonResultPayload}");
                }
                executionResult.ResultModel = new DatabaseMetadataResultModel
                {
                    Tables = GetGroupedSchemaElements(result.Tables),
                    Views = GetGroupedSchemaElements(result.Views),
                    StoredProcedures = GetGroupedSchemaElements(result.StoredProcedures)
                };
            }
        });

        return executionResult;
    }

    private static Dictionary<string, string[]> GetGroupedSchemaElements(List<string> schemaElements)
    {
        return schemaElements.Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(s =>
            {
                var parts = s.Split(".");
                var schema = parts[0];
                var name = parts[1];
                return new { Schema = schema, Name = name };
            })
            .GroupBy(k => k.Schema, v => v.Name)
            .ToDictionary(k => k.Key, v => v.ToArray());
    }

    class ExtractMetadataResult
    {
        public List<string> Tables { get; set; }
        public List<string> Views { get; set; }
        public List<string> StoredProcedures { get; set; }
    }
} 