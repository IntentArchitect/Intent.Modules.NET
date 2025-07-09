using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Intent.Modules.Common.Templates;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.Plugins;
using Intent.Utils;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class StoredProcList : ModuleTaskSingleInputBase<StoredProcListInputModel>
{
    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.StoredProcList";
    public override string TaskTypeName => "SqlServer Stored Procedure List";

    protected override ValidationResult ValidateInputModel(StoredProcListInputModel importModel)
    {
        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(StoredProcListInputModel importModel)
    {
        var executionResult = new ExecuteResult();
        
        var storedProcs = new List<string>();
        var capture = false;
        SqlSchemaExtractorRunner.Run($@"list-stored-proc --connection ""{importModel.ConnectionString}""", (output, process) =>
        {
            if (output.Data?.Trim().StartsWith("Stored Procedures:") == true)
            {
                capture = true;
            }
            else if (output.Data?.Trim().StartsWith("Error:") == true)
            {
                executionResult.Errors.Add(output.Data.Trim().RemovePrefix("Error:"));
                process.Kill(true);
            }
            else if (capture && output.Data?.Trim() == ".")
            {
                capture = false;
                process.Kill(true);
            }
            else if (capture)
            {
                storedProcs.Add(output.Data ?? "");
            }
        });

        if (executionResult.Errors.Count == 0)
        {
            var resultModel = new StoredProcListResultModel
            {
                StoredProcs = storedProcs
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Select(s =>
                    {
                        var parts = s.Split(".");
                        var schema = parts[0];
                        var name = parts[1];
                        return new { Schema = schema, Name = name };
                    })
                    .GroupBy(k => k.Schema, v => v.Name)
                    .ToDictionary(k => k.Key, v => v.ToArray())
            };
            executionResult.ResultModel = resultModel;
        }

        return executionResult;
    }
}