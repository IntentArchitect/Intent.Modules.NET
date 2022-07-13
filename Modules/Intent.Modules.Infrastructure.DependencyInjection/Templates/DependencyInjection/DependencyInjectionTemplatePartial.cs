using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Infrastructure.DependencyInjection.Templates.DependencyInjection
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DependencyInjectionTemplate : CSharpTemplateBase<object, DependencyInjectionDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Infrastructure.DependencyInjection.DependencyInjection";

        private readonly IList<ContainerRegistrationRequest> _registrationRequests =
            new List<ContainerRegistrationRequest>();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DependencyInjectionTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId,
            outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<ContainerRegistrationRequest>(HandleEvent);
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(
                ServiceConfigurationRequest.ForExtensionMethod(this, "AddInfrastructure",
                    new[] { ServiceConfigurationParameterType.Configuration }));
        }

        private void HandleEvent(ContainerRegistrationRequest @event)
        {
            if (@event.Concern != "Infrastructure")
            {
                return;
            }

            @event.MarkAsHandled();
            _registrationRequests.Add(@event);
            foreach (var templateDependency in @event.TemplateDependencies)
            {
                AddTemplateDependency(templateDependency);
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DependencyInjection",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        private string DefineServiceRegistration(ContainerRegistrationRequest request)
        {
            string UseTypeOf(string type)
            {
                var typeName = type.Substring("typeof(".Length, type.Length - "typeof()".Length);
                return $"typeof({UseType(typeName)})";
            }

            var registrationType = request.Lifetime switch
            {
                ContainerRegistrationRequest.LifeTime.Singleton => "AddSingleton",
                ContainerRegistrationRequest.LifeTime.PerServiceCall => "AddScoped",
                ContainerRegistrationRequest.LifeTime.Transient => "AddTransient",
                _ => "AddTransient"
            };

            var usesTypeOfFormat = request.ConcreteType.StartsWith("typeof(");
            var hasInterface = request.InterfaceType != null;
            var useProvider = request.ResolveFromContainer;

            var concreteType = usesTypeOfFormat
                ? UseTypeOf(request.ConcreteType)
                : UseType(request.ConcreteType);

            var interfaceType = hasInterface
                ? usesTypeOfFormat
                    ? UseTypeOf(request.InterfaceType)
                    : UseType(request.InterfaceType)
                : null;

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            return useProvider switch
            {
                false when !hasInterface && !usesTypeOfFormat => $"services.{registrationType}<{concreteType}>();",
                false when !hasInterface && usesTypeOfFormat => $"services.{registrationType}({concreteType});",
                false when hasInterface && !usesTypeOfFormat =>
                    $"services.{registrationType}<{interfaceType}, {concreteType}>();",
                false when hasInterface && usesTypeOfFormat =>
                    $"services.{registrationType}({interfaceType}, {concreteType});",
                true when !hasInterface && !usesTypeOfFormat =>
                    $"services.{registrationType}(provider => provider.GetService<{concreteType}>());",
                true when !hasInterface && usesTypeOfFormat =>
                    $"services.{registrationType}(provider => provider.GetService({concreteType}));",
                true when hasInterface && !usesTypeOfFormat =>
                    $"services.{registrationType}<{interfaceType}>(provider => provider.GetService<{concreteType}>());",
                true when hasInterface && usesTypeOfFormat =>
                    $"services.{registrationType}({interfaceType}, provider => provider.GetService({concreteType}));",
                _ => throw new InvalidOperationException()
            };
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
        }
    }
}