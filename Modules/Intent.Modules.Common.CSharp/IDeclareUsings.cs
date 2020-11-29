using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common
{
    public interface IDeclareUsings
    {
        IEnumerable<string> DeclareUsings();
    }

    public class ContainerRegistrationRequest
    {
        private readonly List<ITemplateDependency> _templateDependencies = new List<ITemplateDependency>();
        private readonly List<string> _requiredNamespaces = new List<string>();

        private ContainerRegistrationRequest(IClassProvider concreteType)
        {
            ConcreteType = concreteType.FullTypeName();
            Lifetime = LifeTime.Transient;
            _templateDependencies.Add(TemplateDependency.OnTemplate(concreteType));
        }

        public static ContainerRegistrationRequest ToRegister(IClassProvider concreteType)
        {
            return new ContainerRegistrationRequest(concreteType);
        }

        public ContainerRegistrationRequest ForInterface(IClassProvider interfaceType)
        {
            InterfaceType = interfaceType.FullTypeName();
            _templateDependencies.Add(TemplateDependency.OnTemplate(interfaceType));
            return this;
        }

        public ContainerRegistrationRequest ForInterface(string interfaceType)
        {
            InterfaceType = interfaceType;
            return this;
        }

        public ContainerRegistrationRequest WithLifeTime(string lifetime)
        {
            Lifetime = lifetime;
            return this;
        }

        public ContainerRegistrationRequest WithPerServiceCallLifeTime()
        {
            Lifetime = LifeTime.PerServiceCall;
            return this;
        }

        public ContainerRegistrationRequest WithSingletonLifeTime()
        {
            Lifetime = LifeTime.Singleton;
            return this;
        }

        public ContainerRegistrationRequest RequiresUsingNamespace(params string[] namespaces)
        {
            _requiredNamespaces.AddRange(namespaces);
            return this;
        }

        public IEnumerable<string> RequiredNamespaces => _requiredNamespaces;

        public string Group { get; private set; }

        public string InterfaceType { get; private set; }

        public string ConcreteType { get; private set; }

        public string Lifetime { get; private set; }

        public IEnumerable<ITemplateDependency> TemplateDependencies => _templateDependencies;

        public bool IsHandled { get; private set; } = false;

        public static class LifeTime
        {
            public const string Transient = "Transient";
            public const string Singleton = "Singleton";
            public const string PerServiceCall = "PerServiceCall";
        }
    }
}