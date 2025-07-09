using System;
using System.Collections.Generic;
using System.Linq;
using Intent.DomainServices.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.DomainServices.Templates.DomainServiceInterface;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.DomainServices.Api.DomainServiceModelStereotypeExtensions.ServiceRegistrationSettings;
using static Intent.Modules.DomainServices.Settings.DomainSettingsExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.DomainServices.Templates.DomainServiceImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DomainServiceImplementationTemplate : CSharpTemplateBase<DomainServiceModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.DomainServices.DomainServiceImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainServiceImplementationTemplate(IOutputTarget outputTarget, DomainServiceModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateRoles.Domain.Entity.Interface);
            AddTypeSource(TemplateRoles.Domain.ValueObject);
            AddTypeSource(TemplateRoles.Domain.DataContract);
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Domain.DomainServices.Interface);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.AddMetadata("model", model.InternalElement);
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.ImplementsInterface(this.GetDomainServiceInterfaceName());
                    @class.TryAddXmlDocComments(model.InternalElement);
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge().WithBodyIgnored());
                    });

                    foreach (var operation in Model.Operations)
                    {
                        @class.AddMethod(GetTypeName(operation), operation.Name.ToPascalCase(), method =>
                        {
                            if (operation.GenericTypes.Any())
                            {
                                foreach (var genericType in operation.GenericTypes)
                                {
                                    method.AddGenericParameter(genericType);
                                }
                            }

                            method.TryAddXmlDocComments(operation.InternalElement);
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());

                            foreach (var parameter in operation.Parameters)
                            {
                                method.AddParameter(GetTypeName(parameter), parameter.Name.ToParameterName(),
                                    parm => parm.WithDefaultValue(parameter.Value));
                            }

                            if (IsAsync(operation))
                            {
                                AddUsing("System.Threading.Tasks");
                                method
                                    .Async()
                                    .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", p => p.WithDefaultValue("default"));
                            }

                            method.AddStatement("// IntentInitialGen");
                            method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                            method.AddStatement("throw new NotImplementedException(\"Implement your domain service logic here...\");");
                        });
                    }
                });
        }

        private static bool IsAsync(OperationModel operation)
        {
            return operation.HasStereotype("Asynchronous") || operation.Name.EndsWith("Async", System.StringComparison.OrdinalIgnoreCase);
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

            var request = ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Application")
                .ForInterface(GetTemplate<IClassProvider>(DomainServiceInterfaceTemplate.TemplateId, Model))
                .WithPriority(99);

            request = ApplyRegistrationScope(request);

            ExecutionContext.EventDispatcher.Publish(request);
        }

        private ContainerRegistrationRequest ApplyRegistrationScope(ContainerRegistrationRequest request)
        {
            ServiceRegistrationScopeOptionsEnum? serviceScope = Model.HasServiceRegistrationSettings() && Model.GetServiceRegistrationSettings().ServiceRegistrationScope().Value != null ?
               Model.GetServiceRegistrationSettings().ServiceRegistrationScope().AsEnum() : null;
            DefaultDomainServiceScopeOptionsEnum globalScope = ExecutionContext.Settings.GetDomainSettings().DefaultDomainServiceScope().AsEnum();

            return DetermineAndApplyScope(request, serviceScope, globalScope);
        }

        private ContainerRegistrationRequest DetermineAndApplyScope(ContainerRegistrationRequest request, ServiceRegistrationScopeOptionsEnum? serviceScope, DefaultDomainServiceScopeOptionsEnum globalScope) =>
            (serviceScope, globalScope) switch
            {
                (null, DefaultDomainServiceScopeOptionsEnum.Transient) or
                (ServiceRegistrationScopeOptionsEnum.Transient, _) => request,
                (null, DefaultDomainServiceScopeOptionsEnum.Scoped) or
                (ServiceRegistrationScopeOptionsEnum.Scoped, _) => request.WithPerServiceCallLifeTime(),
                (ServiceRegistrationScopeOptionsEnum.Singleton, _) => request.WithSingletonLifeTime(),
                (_, _) => request
            };

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}