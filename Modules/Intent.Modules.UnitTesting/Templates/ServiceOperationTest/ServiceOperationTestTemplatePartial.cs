using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
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

namespace Intent.Modules.UnitTesting.Templates.ServiceOperationTest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ServiceOperationTestTemplate : CSharpTemplateBase<ServiceModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.UnitTesting.ServiceOperationTest";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceOperationTestTemplate(IOutputTarget outputTarget, ServiceModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetOperationNormalizedPath())
               .AddClass($"{Model.Name}Tests", @class =>
               {
                   @class.AddAttribute(CSharpIntentManagedAttribute.Merge());
                   @class.AddConstructor(ctor => ctor.AddAttribute(CSharpIntentManagedAttribute.Ignore()));
               });

            CSharpFile.AfterBuild((@file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                var serviceTemplates = ExecutionContext.FindTemplateInstances("Intent.Application.ServiceImplementations.ServiceImplementation", model);
                var serviceTemplate = serviceTemplates.FirstOrDefault(t => t.CanRunTemplate());

                if (serviceTemplate != null && serviceTemplate is ICSharpFileBuilderTemplate csharpTemplate)
                {
                    TestHelpers.PopulateTestConstructor(this, ctor, serviceTemplate, csharpTemplate, false);
                    @class.AddField(GetTypeName(serviceTemplate), "_service", @field =>
                    {
                        @field.PrivateReadOnly();
                    });

                    // for each operation which is either flagged with the unit test stereo, or if the service is flagged with it
                    foreach (var operation in Model.Operations
                        .Where(o => model.HasStereotype(ServiceModelStereotypeExtensions.UnitTest.DefinitionId) ||
                            o.HasStereotype(OperationModelStereotypeExtensions.UnitTest.DefinitionId)))
                    {
                        TestHelpers.AddDefaultSuccessTest(this, @class, TestHelpers.SuccessTestDetails.CreateServiceDetails(operation));
                    }
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