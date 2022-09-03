using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DependencyInjectionTemplate : CSharpTemplateBase<object, DependencyInjectionDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.DependencyInjection.DependencyInjection";

        private readonly IList<ContainerRegistrationRequest> _containerRegistrationRequests = new List<ContainerRegistrationRequest>();
        private readonly IList<ServiceConfigurationRequest> _serviceConfigurationRequests = new List<ServiceConfigurationRequest>();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DependencyInjectionTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<ContainerRegistrationRequest>(HandleEvent);
            ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(HandleEvent);
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(
                ServiceConfigurationRequest
                    .ToRegister(
                        "AddApplication")
                        // Do we want to have Configuration as part of the Application registration?
                        // , ServiceConfigurationRequest.ParameterType.Configuration)
                    .HasDependency(this));
        }

        private void HandleEvent(ContainerRegistrationRequest @event)
        {
            if (@event.Concern != "Application")
            {
                return;
            }

            @event.MarkAsHandled();
            _containerRegistrationRequests.Add(@event);
            foreach (var templateDependency in @event.TemplateDependencies)
            {
                var template = GetTemplate<IClassProvider>(templateDependency);
                if (template != null)
                {
                    AddUsing(template.Namespace);
                }

                AddTemplateDependency(templateDependency);
            }
        }
        
        private void HandleEvent(ServiceConfigurationRequest @event)
        {
            if (@event.Concern != "Application")
            {
                return;
            }

            @event.MarkAsHandled();
            _serviceConfigurationRequests.Add(@event);
            foreach (var templateDependency in @event.TemplateDependencies)
            {
                var template = GetTemplate<IClassProvider>(templateDependency);
                if (template != null)
                {
                    AddUsing(template.Namespace);
                }

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

            if (request.ConcreteType.StartsWith("typeof("))
            {
                return request.InterfaceType != null
                    ? $"services.{registrationType}({UseTypeOf(request.InterfaceType)}, {UseTypeOf(request.ConcreteType)});"
                    : $"services.{registrationType}({UseTypeOf(request.ConcreteType)});";
            }

            return request.InterfaceType != null
                ? $"services.{registrationType}<{UseType(request.InterfaceType)}, {UseType(request.ConcreteType)}>();"
                : $"services.{registrationType}<{UseType(request.ConcreteType)}>();";
        }

        private string ServiceConfigurationRegistration(ServiceConfigurationRequest registration)
        {
            string GetExtensionMethodParameterList()
            {
                if (registration.ExtensionMethodParameterList?.Any() != true)
                {
                    return string.Empty;
                }

                var paramList = new List<string>();

                foreach (var param in registration.ExtensionMethodParameterList)
                {
                    switch (param)
                    {
                        // Do we want to have Configuration as part of the Application registration?
                        // case ServiceConfigurationRequest.ParameterType.Configuration:
                        //     paramList.Add("configuration");
                        //     break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                paramName: nameof(registration.ExtensionMethodParameterList),
                                actualValue: param,
                                message: "Type specified in parameter list is not known or supported");
                    }
                }

                return string.Join(", ", paramList);
            }
            
            return $"services.{registration.ExtensionMethodName}({GetExtensionMethodParameterList()});";
        }
    }
}