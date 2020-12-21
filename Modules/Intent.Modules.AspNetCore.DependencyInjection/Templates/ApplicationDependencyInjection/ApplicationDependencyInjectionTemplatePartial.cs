using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.DependencyInjection.Templates.ApplicationDependencyInjection
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApplicationDependencyInjectionTemplate : CSharpTemplateBase<object, ApplicationDependencyInjectionDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.DependencyInjection.ApplicationDependencyInjection";

        private readonly IList<ContainerRegistrationRequest> _registrationRequests = new List<ContainerRegistrationRequest>();

        public ApplicationDependencyInjectionTemplate(IOutputTarget outputTarget, object model) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<ContainerRegistrationRequest>(HandleEvent);
        }

        private void HandleEvent(ContainerRegistrationRequest @event)
        {
            if (@event.Concern != "Application")
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

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DependencyInjection",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        private string DefineServiceRegistration(ContainerRegistrationRequest x)
        {
            if (x.ConcreteType.StartsWith("typeof("))
            {
                return x.InterfaceType != null
                    ? $"services.{RegistrationType(x)}({(x.InterfaceType)}, {(x.ConcreteType)});"
                    : $"services.{RegistrationType(x)}({(x.ConcreteType)});";
            }
            return x.InterfaceType != null
                ? $"services.{RegistrationType(x)}<{NormalizeNamespace(x.InterfaceType)}, {NormalizeNamespace(x.ConcreteType)}>();"
                : $"services.{RegistrationType(x)}<{NormalizeNamespace(x.ConcreteType)}>();";
        }

        private string RegistrationType(ContainerRegistrationRequest registration)
        {
            switch (registration.Lifetime)
            {
                case ContainerRegistrationRequest.LifeTime.Singleton:
                    return "AddSingleton";
                case ContainerRegistrationRequest.LifeTime.PerServiceCall:
                    return "AddScoped";
                case ContainerRegistrationRequest.LifeTime.Transient:
                    return "AddTransient";
                default:
                    return "AddTransient";
            }
        }
    }
}