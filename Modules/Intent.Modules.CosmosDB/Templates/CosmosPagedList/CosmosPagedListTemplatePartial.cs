using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosPagedList
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosPagedListTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosPagedList";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosPagedListTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("Microsoft.Azure.CosmosRepository")
                .AddUsing("Microsoft.Azure.CosmosRepository.Paging")
                .AddClass($"CosmosPagedList", @class =>
                {
                    @class
                        .Internal()
                        .AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TDocument", out var tDocument)
                        .WithBaseType($"List<{tDomain}>")
                        .ImplementsInterface($"{this.GetPagedResultInterfaceName()}<{tDomain}>")
                        .AddGenericTypeConstraint(tDocument, c => c
                            .AddType(tDomain)
                            .AddType("IItem"))
                        ;

                    @class.AddProperty("int", "TotalCount", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageCount", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageNo", prop => prop.PrivateSetter());
                    @class.AddProperty("int", "PageSize", prop => prop.PrivateSetter());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IPageQueryResult<{tDocument}>", "pagedResult")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize");

                        ctor.AddStatement($"TotalCount = pagedResult.Total ?? 0;");
                        ctor.AddStatement($"PageCount = pagedResult.TotalPages ?? 0;");
                        ctor.AddStatement($"PageNo = pageNo;");
                        ctor.AddStatement($"PageSize = pageSize;");

                        ctor.AddForEachStatement("result", "pagedResult.Items", stmt =>
                        {
                            stmt.AddStatement("Add(result);");
                        });
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
