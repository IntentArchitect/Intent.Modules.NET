using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.DataContractExtensionMethods
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DataContractExtensionMethodsTemplate : CSharpTemplateBase<DataContractModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.Repositories.DataContractExtensionMethods";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DataContractExtensionMethodsTemplate(IOutputTarget outputTarget, DataContractModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateRoles.Domain.DataContract);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Data")
                .AddClass($"{Model.Name.EnsureSuffixedWith("ExtensionMethods")}", @class =>
                {
                    @class.Static().Internal();
                    @class.AddMethod("DataTable", "ToDataTable", method =>
                    {
                        method.Static();

                        var methodParameterName = Model.Name.Pluralize().ToLocalVariableName();
                        method.AddParameter(
                            type: $"IEnumerable<{GetTypeName(TemplateRoles.Domain.DataContract, Model)}>",
                            name: methodParameterName,
                            configure: parameter => parameter.WithThisModifier());

                        method.AddStatement("var dataTable = new DataTable();");

                        foreach (var attribute in Model.Attributes)
                        {
                            var typeName = GetTypeName((IElement)attribute.TypeReference.Element);
                            method.AddStatement($"dataTable.Columns.Add(\"{attribute.Name}\", typeof({typeName}));");
                        }

                        method.AddForEachStatement(
                            iterationVariable: "item",
                            sourceCollection: methodParameterName,
                            configure: @foreach =>
                            {
                                var values = Model.Attributes
                                    .Select(x => $"item.{x.Name.ToPascalCase()}");

                                @foreach.SeparatedFromPrevious();
                                @foreach.AddStatement($"dataTable.Rows.Add({string.Join(", ", values)});");
                            });

                        method.AddStatement("return dataTable;", s => s.SeparatedFromPrevious());
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}