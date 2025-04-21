using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.StoredProcedures.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.SqlDatabaseProject.Api;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.SqlDatabaseProject.Templates.StoredProcedure
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class StoredProcedureTemplate : IntentTemplateBase<SqlStoredProcedureModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.SqlDatabaseProject.StoredProcedureTemplate";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public StoredProcedureTemplate(IOutputTarget outputTarget, SqlStoredProcedureModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"{Model.StoredProcedureModel.Name}",
                fileExtension: "sql",
                relativeLocation: GetLocation()
            );
        }

        private string GetLocation()
        {
            var schema = Model.Schema;
            return Path.Combine(schema, "Stored Procedures");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var fullProcedureName = GetFullProcedureName();
            var parametersDefinition = GenerateParametersDefinition();

            return $"""
                    CREATE PROCEDURE {fullProcedureName}
                    {parametersDefinition}
                    AS
                    SET NOCOUNT ON;
                    
                    """;
        }

        private string GetProcedureName()
        {
            return Model.StoredProcedureModel.Name;
        }

        private string GetFullProcedureName()
        {
            var schema = string.IsNullOrWhiteSpace(Model.Schema) ? "dbo" : Model.Schema;
            return $"[{schema}].[{GetProcedureName()}]";
        }

        private string GenerateParametersDefinition()
        {
            if (Model.StoredProcedureModel.Parameters != null && Model.StoredProcedureModel.Parameters.Any())
            {
                var parameters = Model.StoredProcedureModel.Parameters.Select(p => $"    @{p.Name} {ConvertToSqlType(p)}");
                return string.Join("," + Environment.NewLine, parameters);
            }

            return string.Empty;
        }

        private static string ConvertToSqlType(StoredProcedureParameterModel parameter)
        {
            if (!string.IsNullOrWhiteSpace(parameter.GetStoredProcedureParameterSettings()?.SQLDataType()))
            {
                return parameter.GetStoredProcedureParameterSettings().SQLDataType();
            }

            if (!parameter.TypeReference.HasStringType() && SqlHelper.TryGetSqlType(parameter.TypeReference, out var sqlType))
            {
                return sqlType!;
            }

            if (parameter.TypeReference.HasStringType())
            {
                return "NVARCHAR(MAX)";
            }

            throw new NotSupportedException($"Could not convert parameter type '{parameter.TypeReference.Element?.Name}' to SQL Type");
        }
    }
}
