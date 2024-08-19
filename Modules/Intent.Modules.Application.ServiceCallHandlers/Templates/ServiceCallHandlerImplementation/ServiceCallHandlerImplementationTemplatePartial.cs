using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceCallHandlerImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ServiceCallHandlerImplementationTemplate : CSharpTemplateBase<OperationModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.ServiceCallHandlers.ServiceCallHandlerImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceCallHandlerImplementationTemplate(IOutputTarget outputTarget, OperationModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultTypeCollectionFormat("List<{0}>");
            AddTypeSource(CSharpTypeSource.Create(ExecutionContext, DtoModelTemplate.TemplateId, "List<{0}>"));
            AddTypeSource(TemplateRoles.Application.Command);
            AddTypeSource(TemplateRoles.Application.Query);
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            FulfillsRole("Application.Implementation.Custom");

            var parentName = (model.ParentService.Name + "Handlers").ToPascalCase();

            CSharpFile = new CSharpFile(this.GetNamespace(parentName), this.GetFolderPath(parentName))
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Collections.Generic")
                .AddClass($"{Model.Name}SCH", @class =>
                {
                    @class.TryAddXmlDocComments(model.InternalElement);
                    @class.AddMetadata("model", model.ParentService);
                    @class.RepresentsModel(model);

                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    @class.AddConstructor(ctor => { ctor.AddAttribute(CSharpIntentManagedAttribute.Merge()); });

                    @class.AddMethod(GetOperationReturnType(model), "Handle", method =>
                    {
                        method.AddMetadata("model", model);

                        foreach (var parameter in model.Parameters)
                        {
                            method.AddParameter(GetTypeName(parameter.TypeReference), parameter.Name);
                        }

                        if (model.IsAsync())
                        {
                            AddUsing("System.Threading");
                            AddUsing("System.Threading.Tasks");
                            method.Async();
                            method.AddOptionalCancellationTokenParameter(this);
                        }

                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());

                        method.AddStatement(
                            @"throw new NotImplementedException(""Implement your business logic for this service call in the <#=ClassName#> (SCH = Service Call Handler) class."");");
                    });
                });
        }

        private string GetOperationReturnType(OperationModel o)
        {
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? UseType("System.Threading.Tasks.Task") : "void";
            }

            return o.IsAsync() ? $"{UseType("System.Threading.Tasks.Task")}<{GetTypeName(o.ReturnType)}>" : GetTypeName(o.TypeReference);
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Application")
            );
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