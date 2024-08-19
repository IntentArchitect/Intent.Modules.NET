using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Infrastructure.DependencyInjection.Templates.DependencyInjection
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class DependencyInjectionTemplate : CSharpTemplateBase<object, DependencyInjectionDecorator>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Infrastructure.DependencyInjection.DependencyInjection";

        private readonly IList<ContainerRegistrationRequest> _containerRegistrationRequests = new List<ContainerRegistrationRequest>();
        private readonly IList<ServiceConfigurationRequest> _serviceConfigurationRequests = new List<ServiceConfigurationRequest>();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DependencyInjectionTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftExtensionsConfigurationAbstractions(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsDependencyInjection(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsConfigurationBinder(outputTarget));

            ExecutionContext.EventDispatcher.Subscribe<ContainerRegistrationRequest>(HandleEvent);
            ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(HandleEvent);
            CSharpFile = new CSharpFile(OutputTarget.GetNamespace(), "")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass("DependencyInjection", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "AddInfrastructure", method =>
                    {
                        method.Static();
                        method.AddParameter("this IServiceCollection", "services");
                        method.AddParameter("IConfiguration", "configuration");

                        // delay execution
                        CSharpFile.AfterBuild(file =>
                        {
                            foreach (var decorator in GetDecorators())
                            {
                                method.AddStatement(decorator.ServiceRegistration().Trim());
                            }

                            foreach (var registration in _containerRegistrationRequests.OrderBy(x => x.Priority))
                            {
                                method.AddStatement(DefineServiceRegistration(registration));
                            }

                            foreach (var registration in _serviceConfigurationRequests.OrderBy(x => x.Priority))
                            {
                                method.AddStatement(ServiceConfigurationRegistration(registration));
                            }

                            method.AddStatement("return services;");
                        });
                    });
                });
        }

        public CSharpFile CSharpFile { get; }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            ExecutionContext.EventDispatcher.Publish(
                ServiceConfigurationRequest.ToRegister(
                        extensionMethodName: "AddInfrastructure",
                        extensionMethodParameterList: ServiceConfigurationRequest.ParameterType.Configuration)
                    .HasDependency(this));
        }

        private void HandleEvent(ContainerRegistrationRequest @event)
        {
            if (@event.Concern != "Infrastructure")
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

            foreach (var ns in @event.RequiredNamespaces)
            {
                AddUsing(ns);
            }
        }

        private void HandleEvent(ServiceConfigurationRequest @event)
        {
            if (@event.Concern != "Infrastructure")
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

            foreach (var ns in @event.RequiredNamespaces)
            {
                AddUsing(ns);
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DependencyInjection",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
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

            // This is a 3 way truth table to string mapping:
            return useProvider switch
            {
                false when !hasInterface && !usesTypeOfFormat => $"services.{registrationType}<{concreteType}>();",
                false when !hasInterface && usesTypeOfFormat => $"services.{registrationType}({concreteType});",
                false when hasInterface && !usesTypeOfFormat => $"services.{registrationType}<{interfaceType}, {concreteType}>();",
                false when hasInterface && usesTypeOfFormat => $"services.{registrationType}({interfaceType}, {concreteType});",
                true when !hasInterface && !usesTypeOfFormat => throw new InvalidOperationException($"Using a service provider for resolution during registration without an interface can cause an infinite loop. Concrete Type: {concreteType}"),
                true when !hasInterface && usesTypeOfFormat => throw new InvalidOperationException($"Using a service provider for resolution during registration without an interface can cause an infinite loop. Concrete Type: {concreteType}"),
                // These configurations can cause an infinite loop.
                // true when !hasInterface && !usesTypeOfFormat => $"services.{registrationType}(provider => provider.GetRequiredService<{concreteType}>());",
                // true when !hasInterface && usesTypeOfFormat => $"services.{registrationType}(provider => provider.GetRequiredService({concreteType}));",
                true when hasInterface && !usesTypeOfFormat => $"services.{registrationType}<{interfaceType}>(provider => provider.GetRequiredService<{concreteType}>());",
                true when hasInterface && usesTypeOfFormat => $"services.{registrationType}({interfaceType}, provider => provider.GetRequiredService({concreteType}));",
                _ => throw new InvalidOperationException()
            };
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
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
                        case ServiceConfigurationRequest.ParameterType.Configuration:
                            paramList.Add("configuration");
                            break;
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