using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
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

        private readonly List<ContainerRegistrationRequest> _containerRegistrationRequests = new();
        private readonly List<ServiceConfigurationRequest> _serviceConfigurationRequests = new();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public StartupTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<ContainerRegistrationRequest>(Handle);
            ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(Handle);
        }

        private void Handle(ContainerRegistrationRequest request)
        {
            _containerRegistrationRequests.Add(request);
        }

        private void Handle(ServiceConfigurationRequest request)
        {
            _serviceConfigurationRequests.Add(request);
        }

        /// <remarks>
        /// It's a bit of a smell that a "Get" adds dependencies, but we can't process AddUsings in
        /// the Handle methods as not events might have been published yet.
        /// </remarks>
        private string GetServiceConfigurations(string baseIndent)
        {
            var serviceConfigElements = new List<(string Code, int Priority)>();

            serviceConfigElements.AddRange(GetDecorators()
                .Select(s => (s.ConfigureServices(), s.Priority)));

            var serviceConfigurationRequests = _serviceConfigurationRequests
                .Where(x => !x.IsHandled)
                .ToArray();

            foreach (var request in serviceConfigurationRequests)
            {
                foreach (var dependency in request.TemplateDependencies)
                {
                    var classProvider = GetTemplate<IClassProvider>(dependency);

                    AddTemplateDependency(dependency);
                    AddUsing(classProvider.Namespace);
                }

                foreach (var @namespace in request.RequiredNamespaces)
                {
                    AddUsing(@namespace);
                }

                serviceConfigElements.Add((GetExtensionMethodInvocationStatement(request), request.Priority));
            }

            return GetCodeInNeatLines(serviceConfigElements, baseIndent);
        }

        public bool IsNetCore2App()
        {
            return OutputTarget.IsNetCore2App();
        }

        private static string GetExtensionMethodInvocationStatement(ServiceConfigurationRequest request)
        {
            return $"services.{request.ExtensionMethodName}({GetExtensionMethodParameterList(request)});";
        }

        private static string GetExtensionMethodParameterList(ServiceConfigurationRequest request)
        {
            if (request.ExtensionMethodParameterList?.Any() != true)
            {
                return string.Empty;
            }

            var paramList = new List<string>();

            foreach (var param in request.ExtensionMethodParameterList)
            {
                switch (param)
                {
                    case ServiceConfigurationRequest.ParameterType.Configuration:
                        paramList.Add("Configuration");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            paramName: nameof(request.ExtensionMethodParameterList),
                            actualValue: param,
                            message: "Type specified in parameter list is not known or supported");
                }
            }

            return string.Join(", ", paramList);
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

        private string GetCodeInNeatLines(IEnumerable<(string Code, int Priority)> codeSections, string baseIndent)
        {
            var sb = new StringBuilder();
            PushIndent(baseIndent);

            foreach (var element in codeSections.OrderBy(x => x.Priority))
            {
                var codeLines = element.Code
                    .Trim()
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in codeLines)
                {
                    sb.Append(CurrentIndent).AppendLine(line);
                }
            }

            PopIndent();
            return sb.ToString();
        }

        /// <remarks>
        /// It's a bit of a smell that a "Get" adds dependencies, but we can't process AddUsings in
        /// the Handle methods as not events might have been published yet.
        /// </remarks>
        public string GetContainerRegistrations()
        {
            var containerRegistrationRequests = _containerRegistrationRequests
                .Where(x => !x.IsHandled)
                .ToArray();

            foreach (var request in containerRegistrationRequests)
            {
                foreach (var dependency in request.TemplateDependencies)
                {
                    var classProvider = GetTemplate<IClassProvider>(dependency);

                    AddTemplateDependency(dependency);
                    AddUsing(classProvider.Namespace);
                }

                foreach (var @namespace in request.RequiredNamespaces)
                {
                    AddUsing(@namespace);
                }
            }

            return string.Concat(containerRegistrationRequests.Select(DefineServiceRegistration));
        }

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

        private static string RegistrationType(ContainerRegistrationRequest registration)
        {
            return registration.Lifetime switch
            {
                ContainerRegistrationRequest.LifeTime.Singleton => "AddSingleton",
                ContainerRegistrationRequest.LifeTime.PerServiceCall => "AddScoped",
                _ => "AddTransient"
            };
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "Startup",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}