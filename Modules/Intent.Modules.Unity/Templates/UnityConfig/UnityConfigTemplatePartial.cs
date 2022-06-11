using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Unity.Templates.PerServiceCallLifetimeManager;
using Intent.Templates;
using ModelHasFolderTemplateExtensions = Intent.Modules.Common.CSharp.Templates.ModelHasFolderTemplateExtensions;

namespace Intent.Modules.Unity.Templates.UnityConfig
{
    partial class UnityConfigTemplate : CSharpTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasDecorators<IUnityRegistrationsDecorator>, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.Unity.Config";

        private readonly IList<IUnityRegistrationsDecorator> _decorators = new List<IUnityRegistrationsDecorator>();
        private readonly IList<ContainerRegistrationRequest> _registrations = new List<ContainerRegistrationRequest>();

        public UnityConfigTemplate(IOutputTarget outputTarget, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, outputTarget, null)
        {
            eventDispatcher.Subscribe<ContainerRegistrationRequest>(Handle);
        }

        public IEnumerable<IOutputTarget> ApplicationProjects => ExecutionContext.OutputTargets.Where(x => x.IsVSProject()).Select(x => x.GetTargetPath()[0]).Distinct();

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "UnityConfig",
                @namespace: this.GetNamespace(),
                relativeLocation: ModelHasFolderTemplateExtensions.GetFolderPath(this));
        }

        public override string DependencyUsings => "";

        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _registrations
                .SelectMany(x => x.TemplateDependencies)
                .Union(base.GetTemplateDependencies());
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

            output += Environment.NewLine + GetDecorators().Aggregate(x => x.Registrations());

            return output.Trim();
        }

        private string GetRegistrationString(ContainerRegistrationRequest x)
        {
            return x.InterfaceType != null
                ? $"{Environment.NewLine}            container.RegisterType<{UseType(x.InterfaceType)}, {UseType(x.ConcreteType)}>({GetLifetimeManager(x)});"
                : $"{Environment.NewLine}            container.RegisterType<{UseType(x.ConcreteType)}>({GetLifetimeManager(x)});";
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
        }
    }
}
