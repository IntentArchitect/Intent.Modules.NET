using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.UnitTesting.Templates.IntegrationEventHandlerTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IntegrationEventHandlerTestTemplate : CSharpTemplateBase<IntegrationEventHandlerModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.UnitTesting.IntegrationEventHandlerTest";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationEventHandlerTestTemplate(IOutputTarget outputTarget, IntegrationEventHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}Tests", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    @class.AddConstructor(ctor => ctor.AddAttribute(CSharpIntentManagedAttribute.Ignore()));
                });
            
            CSharpFile.AfterBuild((@file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                var handlerTemplates = ExecutionContext.FindTemplateInstances(TemplateRoles.Application.Eventing.EventHandler, model);
                var handlerTemplate = handlerTemplates.FirstOrDefault(t => t.CanRunTemplate());

                if (handlerTemplate != null && handlerTemplate is ICSharpFileBuilderTemplate csharpTemplate)
                {
                    TestHelpers.PopulateTestConstructor(this, ctor, handlerTemplate, csharpTemplate);

                    @class.AddField(GetTypeName(handlerTemplate), "_handler", @field =>
                    {
                        @field.PrivateReadOnly();
                    });

                    TestHelpers.AddDefaultSuccessTest(this, @class, TestHelpers.SuccessTestDetails.CreateIntegrationEventDetails(model));
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