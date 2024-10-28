using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Pagination.Templates.PagedResult
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PagedResultTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Dtos.Pagination.PagedResult";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PagedResultTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            this.AddUsing("System.Collections.Generic");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"PagedResult", @class =>
                {
                    @class.AddGenericParameter("T");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddObjectInitStatement("Data", "null!;");
                    });

                    @class.AddProperty("int", "TotalCount");
                    @class.AddProperty("int", "PageCount");
                    @class.AddProperty("int", "PageSize");
                    @class.AddProperty("int", "PageNumber");
                    @class.AddProperty("IEnumerable<T>", "Data");

                    @class.AddMethod("PagedResult<T>", "Create", mthd =>
                    {
                        mthd.Static();

                        mthd.AddParameter("int", "totalCount");
                        mthd.AddParameter("int", "pageCount");
                        mthd.AddParameter("int", "pageSize");
                        mthd.AddParameter("int", "pageNumber");
                        mthd.AddParameter("IEnumerable<T>", "data");

                        mthd.AddReturn(new CSharpStatement(@"new PagedResult<T>
            {
                TotalCount = totalCount,
                PageCount = pageCount,
                PageSize = pageSize,
                PageNumber = pageNumber,
                Data = data,
            }"));
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"PagedResult",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}