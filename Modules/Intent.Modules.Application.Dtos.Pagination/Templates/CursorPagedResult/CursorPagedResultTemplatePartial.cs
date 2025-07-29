using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Pagination.Templates.CursorPagedResult
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CursorPagedResultTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Dtos.Pagination.CursorPagedResult";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CursorPagedResultTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"CursorPagedResult", @class =>
                {
                    @class.AddGenericParameter("T", out var tDataType);

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddObjectInitStatement("Data", "null!;");
                    });

                    AddUsing("System.Collections.Generic");
                    AddUsing("System.Linq");

                    @class.AddProperty($"IEnumerable<{tDataType}>", "Data");
                    @class.AddProperty("string?", "CursorToken");
                    @class.AddProperty("int", "PageSize");
                    @class.AddProperty("bool", "HasMoreResults", prop =>
                    {
                        prop.Getter.WithExpressionImplementation("!string.IsNullOrEmpty(CursorToken)");
                        prop.WithoutSetter();
                    });

                    @class.AddMethod($"CursorPagedResult<{tDataType}>", "Create", method =>
                    {
                        method.Static();
                        method.AddParameter("int", "pageSize")
                        .AddParameter("string?", "cursorToken")
                        .AddParameter($"IEnumerable<{tDataType}>", "data");

                        var returnStatement = new CSharpObjectInitializerBlock($"new CursorPagedResult<{tDataType}>")
                            .AddInitStatement("CursorToken", "cursorToken")
                            .AddInitStatement("PageSize", "pageSize")
                            .AddInitStatement("Data", "data");

                        method.AddReturn(returnStatement);
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

        public override bool CanRunTemplate()
        {
            var cursorResult = "CursorPagedResult";

            var isPagingUsed = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id)
                        .GetQueryModels().Any(x => x.TypeReference?.Element?.Name == cursorResult) ||
                    ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id)
                        .GetServiceModels().Any(x => x.Operations.Any(o => o.TypeReference?.Element?.Name == cursorResult));

            return base.CanRunTemplate() && isPagingUsed;
        }
    }
}