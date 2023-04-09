using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainServices.Templates.DomainServiceInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.DomainServices.Templates.DomainServiceImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DomainServiceImplementationTemplate : CSharpTemplateBase<DomainServiceModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.DomainServices.DomainServiceImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainServiceImplementationTemplate(IOutputTarget outputTarget, DomainServiceModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.AddMetadata("model", model.InternalElement);
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge().WithBodyIgnored());
                    });

                    foreach (var operation in Model.Operations)
                    {
                        @class.AddMethod(GetTypeName(operation), operation.Name.ToPascalCase(), method =>
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                            foreach (var parameter in operation.Parameters)
                            {
                                method.AddParameter(GetTypeName(parameter), parameter.Name.ToCamelCase());
                            }
                            method.AddStatement("throw new NotImplementedException(\"Implement your domain service logic here...\");");
                        });
                    }
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Application")
                .ForInterface(GetTemplate<IClassProvider>(DomainServiceInterfaceTemplate.TemplateId, Model))
                .WithPriority(99));
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}