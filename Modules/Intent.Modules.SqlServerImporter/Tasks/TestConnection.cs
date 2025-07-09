using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.Modules.Common.Templates;
using Intent.Modules.SqlServerImporter.Tasks.Helpers;
using Intent.Modules.SqlServerImporter.Tasks.Models;
using Intent.Plugins;
using Intent.Utils;
using Serilog;

namespace Intent.Modules.SqlServerImporter.Tasks;

public class TestConnection : ModuleTaskSingleInputBase<TestConnectionInputModel>
{
    public override string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.TestConnection";
    public override string TaskTypeName => "SqlServer Database Connection Tester";

    protected override ValidationResult ValidateInputModel(TestConnectionInputModel importModel)
    {
        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(TestConnectionInputModel importModel)
    {
        var executionResult = new ExecuteResult();
        
        SqlSchemaExtractorRunner.Run($@"test-connection --connection ""{importModel.ConnectionString}""", (output, process) =>
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
        
        return executionResult;
    }
}