using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.AspNetCore.Templates.Startup
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class StartupTemplate : CSharpTemplateBase<object, StartupDecorator>
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.AspNetCore.Startup";

        private readonly IList<ContainerRegistrationRequest> _registrations = new List<ContainerRegistrationRequest>();

        private readonly IList<ServiceConfigurationRequest> _serviceConfigurations =
            new List<ServiceConfigurationRequest>();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public StartupTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<ContainerRegistrationRequest>(HandleServiceRegistration);
            ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(HandleServiceConfiguration);
            //eventDispatcher.Subscribe(ContainerRegistrationForDbContextEvent.EventId, HandleDbContextRegistration);
            //eventDispatcher.Subscribe(ServiceConfigurationRequiredEvent.EventId, HandleServiceConfiguration);
            //eventDispatcher.Subscribe(InitializationRequiredEvent.EventId, HandleInitialization);
        }

        private void HandleServiceRegistration(ContainerRegistrationRequest @event)
        {
            _registrations.Add(@event);
        }

        private void HandleServiceConfiguration(ServiceConfigurationRequest request)
        {
            _serviceConfigurations.Add(request);
        }

        public bool IsNetCore2App()
        {
            return OutputTarget.IsNetCore2App();
        }

        private string GetServiceConfigurations(string baseIndent)
        {
            var serviceConfigElements = new List<(string Code, int Priority)>();
            
            serviceConfigElements.AddRange(GetDecorators()
                .Select(s => (s.ConfigureServices(), s.Priority)));
            serviceConfigElements.AddRange(_serviceConfigurations
                .Select(s => (GetExtensionMethodInvocationStatement(s), s.Priority)));

            return GetCodeInNeatLines(serviceConfigElements, baseIndent);
        }

        private string GetExtensionMethodInvocationStatement(ServiceConfigurationRequest request)
        {
            return
                $"services.{request.ExtensionMethodName}({(request.SupplyConfiguration ? "Configuration" : string.Empty)});";
        }

        private string GetApplicationConfigurations(string baseIndent)
        {
            var appConfigElements = new List<(string Code, int Priority)>();
            if (IsNetCore2App())
            {
                appConfigElements.Add(("app.UseHttpsRedirection();", -30));
            }
            else
            {
                appConfigElements.Add(("app.UseHttpsRedirection();", -30));
                appConfigElements.Add(("app.UseRouting();", -20));
                appConfigElements.Add(("app.UseAuthorization();", -5));
            }

            appConfigElements.Add(($@"
app.UseEndpoints(endpoints =>
{{
    {GetEndPointMappings()}
}});", 0));

            appConfigElements.AddRange(GetDecorators().Select(s => (s.Configuration(), s.Priority)));

            return GetCodeInNeatLines(appConfigElements, baseIndent);
        }

        private string GetEndPointMappings()
        {
            var endpointMappings = new List<(string Code, int Priority)>();

            endpointMappings.AddRange(GetDecorators().Select(s => (s.EndPointMappings(), s.Priority)));

            return GetCodeInNeatLines(endpointMappings, "                ");
        }

        private string GetCodeInNeatLines(List<(string Code, int Priority)> codeSections, string baseIndent)
        {
            codeSections.Sort(Comparer<(string Code, int Priority)>
                .Create((x, y) => x.Priority.CompareTo(y.Priority)));

            var sb = new StringBuilder();
            base.PushIndent(baseIndent);

            foreach (var element in codeSections)
            {
                var codeLines = element.Code
                    .Trim()
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in codeLines)
                {
                    sb.Append(base.CurrentIndent).AppendLine(line);
                }
            }

            base.PopIndent();
            return sb.ToString();
        }

        //private void HandleDbContextRegistration(ApplicationEvent @event)
        //{
        //    var registration = new DbContextContainerRegistration(
        //        @event.TryGetValue(ContainerRegistrationForDbContextEvent.UsingsKey),
        //        @event.GetValue(ContainerRegistrationForDbContextEvent.ConcreteTypeKey),
        //        @event.TryGetValue(ContainerRegistrationForDbContextEvent.ConcreteTypeTemplateIdKey) != null ? TemplateDependency.OnTemplate(@event.TryGetValue(ContainerRegistrationForDbContextEvent.ConcreteTypeTemplateIdKey)) : null,
        //        @event.TryGetValue(ContainerRegistrationForDbContextEvent.OptionsKey),
        //        @event.TryGetValue(ContainerRegistrationForDbContextEvent.NugetDependency),
        //        @event.TryGetValue(ContainerRegistrationForDbContextEvent.NugetDependencyVersion));
        //    _dbContextRegistrations.Add(registration);
        //    if (registration.NugetPackage != null)
        //    {
        //        AddNugetDependency(registration.NugetPackage);
        //    }
        //}

        //private void HandleServiceConfiguration(ApplicationEvent @event)
        //{
        //    _serviceConfigurations.Add(new Initializations(
        //        usings: @event.GetValue(ServiceConfigurationRequiredEvent.UsingsKey),
        //        code: @event.GetValue(ServiceConfigurationRequiredEvent.CallKey),
        //        method: @event.TryGetValue(ServiceConfigurationRequiredEvent.MethodKey),
        //        priority: int.TryParse(@event.TryGetValue(ServiceConfigurationRequiredEvent.PriorityKey), out var priority) ? priority : 0,
        //        templateDependency: null));
        //}

        //private void HandleInitialization(ApplicationEvent @event)
        //{
        //    _initializations.Add(new Initializations(
        //        usings: @event.GetValue(InitializationRequiredEvent.UsingsKey),
        //        code: @event.GetValue(InitializationRequiredEvent.CallKey),
        //        method: @event.TryGetValue(InitializationRequiredEvent.MethodKey),
        //        priority: int.TryParse(@event.TryGetValue(InitializationRequiredEvent.PriorityKey), out var priority) ? priority : 0,
        //        templateDependency: @event.TryGetValue(InitializationRequiredEvent.TemplateDependencyIdKey) != null ? TemplateDependency.OnTemplate(@event.TryGetValue(InitializationRequiredEvent.TemplateDependencyIdKey)) : null));
        //}

        //public string ServiceConfigurations()
        //{
        //    var configurations = _serviceConfigurations.Select(x => x.Code).ToList();

        //    if (!configurations.Any())
        //    {
        //        return string.Empty;
        //    }

        //    const string tabbing = "            ";
        //    return Environment.NewLine +
        //           configurations
        //               .Select(x => x.Trim())
        //               .Select(x => x.StartsWith("#") ? x : $"{tabbing}{x}")
        //               .Aggregate((x, y) => $"{x}{Environment.NewLine}" +
        //                                    $"{y}");
        //}

        //public void AddDecorator(StartupDecorator decorator)
        //{
        //    _decorators.Add(decorator);
        //}

        //public IEnumerable<StartupDecorator> GetDecorators()
        //{
        //    return _decorators;
        //}

        //public string Configurations()
        //{
        //    var configurations = _initializations.Select(x => x.Code).ToList();

        //    if (!configurations.Any())
        //    {
        //        return string.Empty;
        //    }

        //    const string tabbing = "            ";
        //    return Environment.NewLine +
        //           configurations
        //               .Select(x => x.Trim())
        //               .Select(x => x.StartsWith("#") ? x : $"{tabbing}{x}")
        //               .Aggregate((x, y) => $"{x}{Environment.NewLine}" +
        //                                    $"{y}");
        //}

        private IEnumerable<ContainerRegistrationRequest> GetServiceRegistrations()
        {
            return _registrations.Where(x => !x.IsHandled);
        }

        public string Registrations()
        {
            string registrations = string.Empty;
            //if (_dbContextRegistrations.Any())
            //{
            //    registrations += $"{Environment.NewLine}            ConfigureDbContext(services);";
            //}

            registrations += GetServiceRegistrations().Any()
                ? GetServiceRegistrations().Select(DefineServiceRegistration).Aggregate((x, y) => x + y)
                : string.Empty;

            return registrations; // + Environment.NewLine + GetDecorators().Aggregate(x => x.Registrations());
        }

        //public string Methods()
        //{
        //    var methods = _initializations.Concat(_serviceConfigurations)
        //        .Where(x => !string.IsNullOrWhiteSpace(x.Method))
        //        .OrderBy(x => x.Priority)
        //        .Select(x => x.Method)
        //        .ToList();

        //    if (_dbContextRegistrations.Any())
        //    {
        //        var dbContextRegistration = string.Empty;
        //        dbContextRegistration += $"{Environment.NewLine}        private void ConfigureDbContext(IServiceCollection services)";
        //        dbContextRegistration += $"{Environment.NewLine}        {{";
        //        dbContextRegistration += _dbContextRegistrations.Select(DefineDbContextRegistration).Aggregate((x, y) => x + y);
        //        dbContextRegistration += $"{Environment.NewLine}        }}";
        //        methods.Add(dbContextRegistration);
        //    }

        //    if (!methods.Any())
        //    {
        //        return string.Empty;
        //    }

        //    const string tabbing = "        ";
        //    return Environment.NewLine +
        //           Environment.NewLine +
        //           methods
        //               .Select(x => x.Trim())
        //               .Select(x => $"{tabbing}{x}")
        //               .Aggregate((x, y) => $"{x}{Environment.NewLine}" +
        //                                    $"{Environment.NewLine}" +
        //                                    $"{y}");
        //}

        //private string DefineDbContextRegistration(DbContextContainerRegistration x)
        //{
        //    return $"{Environment.NewLine}            services.AddDbContext<{UseType(x.ConcreteType)}>({(x.Options != null ? $"x => x{x.Options}" : string.Empty)});";
        //}

        private string DefineServiceRegistration(ContainerRegistrationRequest x)
        {
            string UseTypeOf(string type)
            {
                var typeName = type.Substring("typeof(".Length, type.Length - "typeof()".Length);
                return $"typeof({UseType(typeName)})";
            }

            if (x.ConcreteType.StartsWith("typeof("))
            {
                return x.InterfaceType != null
                    ? $"{Environment.NewLine}            services.{RegistrationType(x)}({UseTypeOf(x.InterfaceType)}, {UseTypeOf(x.ConcreteType)});"
                    : $"{Environment.NewLine}            services.{RegistrationType(x)}({UseTypeOf(x.ConcreteType)});";
            }

            return x.InterfaceType != null
                ? $"{Environment.NewLine}            services.{RegistrationType(x)}<{UseType(x.InterfaceType)}, {UseType(x.ConcreteType)}>();"
                : $"{Environment.NewLine}            services.{RegistrationType(x)}<{UseType(x.ConcreteType)}>();";
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"Startup",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        //public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        //{
        //    return base.GetTemplateDependencies().Concat(GetServiceRegistrations().SelectMany(x => x.TemplateDependencies)
        //        //.Where(x => x.InterfaceType != null && x.InterfaceTypeTemplateDependency != null)
        //        //.Select(x => x.InterfaceTypeTemplateDependency)
        //        //.Union(_registrations
        //        //    .Where(x => x.ConcreteTypeTemplateDependency != null)
        //        //    .Select(x => x.ConcreteTypeTemplateDependency))
        //        .Union(_initializations
        //            .Where(x => x.TemplateDependancy != null)
        //            .Select(x => x.TemplateDependancy))
        //        .Union(_dbContextRegistrations
        //            .Where(x => x.ConcreteTypeTemplateDependency != null)
        //            .Select(x => x.ConcreteTypeTemplateDependency))
        //        .ToList());
        //}

        //public IEnumerable<string> DeclareUsings()
        //{
        //    return _dbContextRegistrations.Select(x => x.Usings)
        //        .Concat(_initializations.Select(x => x.Usings))
        //        .Concat(_serviceConfigurations.Select(x => x.Usings))
        //        .Select(x => x.Split(';'))
        //        .SelectMany(x => x)
        //        .Where(x => !string.IsNullOrWhiteSpace(x))
        //        .Select(x => x.Trim());
        //}

        //private bool IsNetCore2App() // Dirty way to get this info. Should not have this dependency
        //{
        //    return Project.TargetFramework().StartsWith("netcoreapp2");
        //}

        //internal class ContainerRegistration
        //{
        //    public ContainerRegistration(string interfaceType, string concreteType, string lifetime, ITemplateDependency interfaceTypeTemplateDependency, ITemplateDependency concreteTypeTemplateDependency)
        //    {
        //        InterfaceType = interfaceType;
        //        ConcreteType = concreteType;
        //        Lifetime = lifetime ?? "Transient";
        //        InterfaceTypeTemplateDependency = interfaceTypeTemplateDependency;
        //        ConcreteTypeTemplateDependency = concreteTypeTemplateDependency;
        //    }

        //    public string InterfaceType { get; }
        //    public string ConcreteType { get; }
        //    public string Lifetime { get; }
        //    public ITemplateDependency InterfaceTypeTemplateDependency { get; }
        //    public ITemplateDependency ConcreteTypeTemplateDependency { get; }
        //}

        //internal class DbContextContainerRegistration
        //{
        //    public DbContextContainerRegistration(string usings, string concreteType,
        //        ITemplateDependency concreteTypeTemplateDependency, string options, string nugetDependency,
        //        string nugetDependencyVersion)
        //    {
        //        Usings = usings;
        //        ConcreteType = concreteType;
        //        ConcreteTypeTemplateDependency = concreteTypeTemplateDependency;
        //        Options = options;
        //        if (!string.IsNullOrWhiteSpace(nugetDependency) && !string.IsNullOrWhiteSpace(nugetDependencyVersion))
        //        {
        //            NugetPackage = new NugetPackageInfo(nugetDependency, nugetDependencyVersion);
        //        }
        //    }

        //    public string Usings { get; }
        //    public string ConcreteType { get; }
        //    public ITemplateDependency ConcreteTypeTemplateDependency { get; }
        //    public string Options { get; }
        //    public NugetPackageInfo NugetPackage { get; }
        //}

        //internal class Initializations
        //{
        //    public string Usings { get; }
        //    public string Code { get; }
        //    public string Method { get; }
        //    public int Priority { get; }
        //    public ITemplateDependency TemplateDependancy { get; }

        //    public Initializations(string usings, string code, string method, int priority, ITemplateDependency templateDependency)
        //    {
        //        Usings = usings;
        //        Code = code;
        //        Method = method;
        //        Priority = priority;
        //        TemplateDependancy = templateDependency;
        //    }
        //}
    }
}