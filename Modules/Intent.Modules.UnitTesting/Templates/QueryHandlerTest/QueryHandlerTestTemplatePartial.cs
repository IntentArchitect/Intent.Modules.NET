using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.UnitTesting.Api;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.UnitTesting.Templates.QueryHandlerTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class QueryHandlerTestTemplate : CSharpTemplateBase<IElement>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.UnitTesting.QueryHandlerTest";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueryHandlerTestTemplate(IOutputTarget outputTarget, IElement model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetElementNamespace(), this.GetTestElementNormalizedPath())
                .AddClass($"{Model.Name}HandlerTests", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    @class.AddConstructor(ctor => ctor.AddAttribute(CSharpIntentManagedAttribute.Ignore()));
                });

            CSharpFile.AfterBuild((@file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                var handlerTemplates = ExecutionContext.FindTemplateInstances(TemplateRoles.Application.Handler.Query, model);
                var handlerTemplate = handlerTemplates.FirstOrDefault(t => t.CanRunTemplate());

                if (handlerTemplate != null && handlerTemplate is ICSharpFileBuilderTemplate csharpTemplate)
                {
                    var details = TestHelpers.SuccessTestDetails.CreateQueryDetails(model);
                    TestHelpers.PopulateTestConstructor(this, ctor, handlerTemplate, csharpTemplate, details);

                    @class.AddField(GetTypeName(handlerTemplate), "_handler", @field =>
                    {
                        @field.PrivateReadOnly();
                    });

                    TestHelpers.AddDefaultSuccessTest(this, @class, details);
                }
            }), 9999);
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