using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.UpdateCommandHandlerTests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class UpdateCommandHandlerTestsTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.CRUD.Tests.UpdateCommandHandlerTests";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UpdateCommandHandlerTestsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "exampleParam", param =>
                        {
                            param.IntroduceReadonlyField();
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