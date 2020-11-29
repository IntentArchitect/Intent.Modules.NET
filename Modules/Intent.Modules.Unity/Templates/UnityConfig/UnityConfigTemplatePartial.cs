using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Eventing;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Unity.Templates.PerServiceCallLifetimeManager;

namespace Intent.Modules.Unity.Templates.UnityConfig
{
    partial class UnityConfigTemplate : CSharpTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasDecorators<IUnityRegistrationsDecorator>, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.Unity.Config";

        private IList<IUnityRegistrationsDecorator> _decorators = new List<IUnityRegistrationsDecorator>();
        private readonly IList<ContainerRegistrationRequest> _registrations = new List<ContainerRegistrationRequest>();

        public UnityConfigTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, null)
        {
            eventDispatcher.Subscribe<ContainerRegistrationRequest>(Handle);
        }

        public IEnumerable<IOutputTarget> ApplicationProjects => ExecutionContext.OutputTargets.Select(x => x.GetTargetPath()[0]).Distinct();

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"UnityConfig",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public override string DependencyUsings => "";

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _registrations.SelectMany(x => x.TemplateDependencies);
            //.Where(x => x.InterfaceType != null && x.InterfaceTypeTemplateDependency != null)
            //.Select(x => x.InterfaceTypeTemplateDependency)
            //.Union(_registrations
            //    .Where(x => x.ConcreteTypeTemplateDependency != null)
            //    .Select(x => x.ConcreteTypeTemplateDependency))
            //.ToList();
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new INugetPackageInfo[]
            {
                NugetPackages.Unity,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public string Registrations()
        {
            var registrations = _registrations
                .Where(x => x.InterfaceType != null || !x.Lifetime.Equals(ContainerRegistrationRequest.LifeTime.Transient, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            var output = registrations.Any() ? registrations.Select(GetRegistrationString).Aggregate((x, y) => x + y) : string.Empty;

            return output + Environment.NewLine + GetDecorators().Aggregate(x => x.Registrations());
        }

        private string GetRegistrationString(ContainerRegistrationRequest x)
        {
            return x.InterfaceType != null
                ? $"{Environment.NewLine}            container.RegisterType<{NormalizeNamespace(x.InterfaceType)}, {NormalizeNamespace(x.ConcreteType)}>({GetLifetimeManager(x)});"
                : $"{Environment.NewLine}            container.RegisterType<{NormalizeNamespace(x.ConcreteType)}>({GetLifetimeManager(x)});";
        }

        private string GetLifetimeManager(ContainerRegistrationRequest registration)
        {
            switch (registration.Lifetime)
            {
                case ContainerRegistrationRequest.LifeTime.Singleton:
                    return "new ContainerControlledLifetimeManager()";
                case ContainerRegistrationRequest.LifeTime.PerServiceCall:
                    return $"new {Project.Application.FindTemplateInstance<IClassProvider>(TemplateDependency.OnTemplate(PerServiceCallLifetimeManagerTemplate.Identifier)).ClassName}()";
                case ContainerRegistrationRequest.LifeTime.Transient:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }

        public void AddDecorator(IUnityRegistrationsDecorator decorator)
        {
            _decorators.Add(decorator);

        }

        public IEnumerable<IUnityRegistrationsDecorator> GetDecorators()
        {
            return _decorators;
        }

        public string GetUsingsFromDecorators()
        {
            return string.Join(Environment.NewLine, _decorators
                .SelectMany(s => s.DeclareUsings())
                .Distinct()
                .Select(s => $"using {s};"));
        }

        private void Handle(ContainerRegistrationRequest @event)
        {
            _registrations.Add(@event);
            //    _registrations.Add(new ContainerRegistration(
            //        interfaceType: @event.TryGetValue(ContainerRegistrationEvent.InterfaceTypeKey),
            //        concreteType: @event.GetValue(ContainerRegistrationEvent.ConcreteTypeKey),
            //        lifetime: @event.TryGetValue(ContainerRegistrationEvent.LifetimeKey),
            //        interfaceTypeTemplateDependency: @event.TryGetValue(ContainerRegistrationEvent.InterfaceTypeTemplateIdKey) != null ? TemplateDependency.OnTemplate(@event.TryGetValue(ContainerRegistrationEvent.InterfaceTypeTemplateIdKey)) : null,
            //        concreteTypeTemplateDependency: @event.TryGetValue(ContainerRegistrationEvent.ConcreteTypeTemplateIdKey) != null ? TemplateDependency.OnTemplate(@event.TryGetValue(ContainerRegistrationEvent.ConcreteTypeTemplateIdKey)) : null));
        }
    }

    //internal class ContainerRegistration
    //{
    //    public ContainerRegistration(string interfaceType, string concreteType, string lifetime, ITemplateDependency interfaceTypeTemplateDependency, ITemplateDependency concreteTypeTemplateDependency)
    //    {
    //        InterfaceType = interfaceType;
    //        ConcreteType = concreteType;
    //        Lifetime = lifetime ?? Constants.ContainerRegistrationEvent.TransientLifetime;
    //        InterfaceTypeTemplateDependency = interfaceTypeTemplateDependency;
    //        ConcreteTypeTemplateDependency = concreteTypeTemplateDependency;
    //    }

    //    public string InterfaceType { get; private set; }
    //    public string ConcreteType { get; private set; }
    //    public string Lifetime { get; private set; }
    //    public ITemplateDependency InterfaceTypeTemplateDependency { get; private set; }
    //    public ITemplateDependency ConcreteTypeTemplateDependency { get; }
    //}
}
