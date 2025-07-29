using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Entities.Repositories.Api.Templates.CursorPagedListInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CursorPagedListInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Entities.Repositories.Api.CursorPagedListInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CursorPagedListInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Repository.Interface.CursorPagedList);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"ICursorPagedList", @interface =>
                {
                    AddUsing("System.Collections.Generic");

                    @interface.ImplementsInterfaces("IList<T>");
                    @interface.AddGenericParameter("T");

                    @interface.AddProperty("int", "PageSize", prop =>
                    {
                        prop.WithoutSetter();
                    });

                    @interface.AddProperty("string?", "CursorToken", prop =>
                    {
                        prop.WithoutSetter();
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

            var isPagingUsed = ExecutionContext.MetadataManager.GetDesigner(ExecutionContext.GetApplicationConfig().Id, "81104ae6-2bc5-4bae-b05a-f987b0372d81")
                       .GetElementsOfType("e71b0662-e29d-4db2-868b-8a12464b25d0").ToList().Any(x => x.TypeReference?.Element?.Name == cursorResult) ||
                    ExecutionContext.MetadataManager.GetDesigner(ExecutionContext.GetApplicationConfig().Id, "81104ae6-2bc5-4bae-b05a-f987b0372d81")
                        .GetElementsOfType("b16578a5-27b1-4047-a8df-f0b783d706bd")
                            .Any(x => x.ChildElements.GetElementsOfType("e030c97a-e066-40a7-8188-808c275df3cb").Any(o => o.TypeReference?.Element?.Name == cursorResult));

            return base.CanRunTemplate() && isPagingUsed;
        }
    }
}