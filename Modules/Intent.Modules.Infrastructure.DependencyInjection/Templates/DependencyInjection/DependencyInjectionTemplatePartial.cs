using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Infrastructure.DependencyInjection.Templates.DependencyInjection
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DependencyInjectionTemplate : CSharpTemplateBase<object, DependencyInjectionDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Infrastructure.DependencyInjection.DependencyInjection";

        private readonly IList<ContainerRegistrationRequest> _registrationRequests = new List<ContainerRegistrationRequest>();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DependencyInjectionTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<ContainerRegistrationRequest>(HandleEvent);
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

        private string DefineServiceRegistration(ContainerRegistrationRequest x)
        {
            var registrationType = x.Lifetime switch
            {
                ContainerRegistrationRequest.LifeTime.Singleton => "AddSingleton",
                ContainerRegistrationRequest.LifeTime.PerServiceCall => "AddScoped",
                ContainerRegistrationRequest.LifeTime.Transient => "AddTransient",
                _ => "AddTransient"
            };

            var usesGenerics = !x.ConcreteType.StartsWith("typeof(");
            var hasInterface = x.InterfaceType != null;
            var useProvider = x.ResolveFromContainer;

            var concreteType = usesGenerics
                ? NormalizeNamespace(x.ConcreteType)
                : x.ConcreteType;
            var interfaceType = usesGenerics && hasInterface
                ? NormalizeNamespace(x.InterfaceType)
                : x.InterfaceType;

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            return useProvider switch
            {
                false when !hasInterface && !usesGenerics => $"services.{registrationType}({concreteType});",
                false when !hasInterface && usesGenerics => $"services.{registrationType}<{concreteType}>();",
                false when hasInterface && !usesGenerics => $"services.{registrationType}({interfaceType}, {concreteType});",
                false when hasInterface && usesGenerics => $"services.{registrationType}<{interfaceType}, {concreteType}>();",
                true when !hasInterface && !usesGenerics => $"services.{registrationType}(provider => provider.GetService({concreteType}));",
                true when !hasInterface && usesGenerics => $"services.{registrationType}(provider => provider.GetService<{concreteType}>());",
                true when hasInterface && !usesGenerics => $"services.{registrationType}({interfaceType}, provider => provider.GetService({concreteType}));",
                true when hasInterface && usesGenerics => $"services.{registrationType}<{interfaceType}>(provider => provider.GetService<{concreteType}>());",
                _ => throw new InvalidOperationException()
            };
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
        }
    }
}