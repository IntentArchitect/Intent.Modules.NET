using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
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
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class StartupTemplate : CSharpTemplateBase<object, StartupDecorator>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)] public const string TemplateId = "Intent.AspNetCore.Startup";

        private readonly List<ContainerRegistrationRequest> _containerRegistrationRequests = new();
        private readonly List<ServiceConfigurationRequest> _serviceConfigurationRequests = new();
        private readonly List<ApplicationBuilderRegistrationRequest> _applicationBuilderRegistrationRequests = new();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public StartupTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<ContainerRegistrationRequest>(Handle);
            ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(Handle);
            ExecutionContext.EventDispatcher.Subscribe<ApplicationBuilderRegistrationRequest>(Handle);

            CSharpFile = new CSharpFile(OutputTarget.GetNamespace(), "")
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddUsing("Microsoft.AspNetCore.Hosting")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("Microsoft.Extensions.Options")
                .AfterBuild(file =>
                {
                    file.AddClass("Startup", @class =>
                    {
                        @class.AddAttribute("[IntentManaged(Mode.Merge)]");
                        @class.AddConstructor(ctor =>
                        {
                            ctor.AddParameter("IConfiguration", "configuration",
                                param => param.IntroduceProperty(prop => { prop.ReadOnly(); }));
                        });
                        @class.AddMethod("void", "ConfigureServices", method =>
                        {
                            method.AddParameter("IServiceCollection", "services");

                            method.AddStatements(GetServiceConfigurations("            "));
                            method.AddStatements(GetContainerRegistrations());
                        });
                        @class.AddMethod("void", "Configure", method =>
                        {
                            method.WithComments("// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.");

                            method.AddParameter("IApplicationBuilder", "app");
                            method.AddParameter("IWebHostEnvironment", "env");

                            method.AddStatement(@"if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }");
                            method.AddStatements(GetApplicationConfigurations(), s => s.FirstOrDefault()?.SeparatedFromPrevious());
                        });
                    });
                }, int.MinValue); // always run first

        }
        public CSharpFile CSharpFile { get; }

        private void Handle(ContainerRegistrationRequest request)
        {
            _containerRegistrationRequests.Add(request);
        }

        private void Handle(ServiceConfigurationRequest request)
        {
            _serviceConfigurationRequests.Add(request);
        }

        private void Handle(ApplicationBuilderRegistrationRequest request)
        {
            _applicationBuilderRegistrationRequests.Add(request);
        }

        /// <remarks>
        /// It's a bit of a smell that a "Get" adds dependencies, but we can't process AddUsings in
        /// the Handle methods as not events might have been published yet.
        /// </remarks>
        private IEnumerable<string> GetServiceConfigurations(string baseIndent)
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

                serviceConfigElements.Add((GetServiceConfigurationExtensionMethodStatement(request), request.Priority));
            }

            return serviceConfigElements
                .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                .OrderBy(x => x.Priority)
                .Select(x => x.Code.Trim());
        }

        public bool IsNetCore2App()
        {
            return OutputTarget.IsNetCore2App();
        }

        private static string GetServiceConfigurationExtensionMethodStatement(ServiceConfigurationRequest request)
        {
            string GetExtensionMethodParameterList()
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

            return $"services.{request.ExtensionMethodName}({GetExtensionMethodParameterList()});";
        }

        private IEnumerable<CSharpStatement> GetApplicationConfigurations()
        {
            var appConfigElements = new List<(CSharpStatement Code, int Priority)>();
            appConfigElements.Add(("app.UseHttpsRedirection();", -30));
            appConfigElements.Add(("app.UseRouting();", -20));
            appConfigElements.Add(("app.UseAuthorization();", -5));

            appConfigElements.Add((new EndpointsStatement(GetEndPointMappings()), 0));

            appConfigElements.AddRange(GetDecorators().Select(s => (new CSharpStatement(s.Configuration()), s.Priority)));

            var applicationBuilderRegistrationRequests = _applicationBuilderRegistrationRequests;

            foreach (var request in applicationBuilderRegistrationRequests)
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

                appConfigElements.Add((GetApplicationBuilderExtensionMethodStatement(request), request.Priority));
            }

            return appConfigElements
                .Where(x => !string.IsNullOrWhiteSpace(x.Code.ToString()))
                .OrderBy(x => x.Priority)
                .Select(x => x.Code);
        }

        private string GetApplicationBuilderExtensionMethodStatement(ApplicationBuilderRegistrationRequest request)
        {
            string GetExtensionMethodParameterList()
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
                        case ApplicationBuilderRegistrationRequest.ParameterType.Configuration:
                            paramList.Add("Configuration");
                            break;
                        case ApplicationBuilderRegistrationRequest.ParameterType.HostEnvironment:
                        case ApplicationBuilderRegistrationRequest.ParameterType.WebHostEnvironment:
                            paramList.Add("env");
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

            return $"app.{request.ExtensionMethodName}({GetExtensionMethodParameterList()});";
        }

        private IEnumerable<CSharpStatement> GetEndPointMappings()
        {
            var endpointMappings = new List<(string Code, int Priority)>();

            endpointMappings.AddRange(GetDecorators().Select(s => (s.EndPointMappings(), s.Priority)));

            return endpointMappings
                .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                .OrderBy(x => x.Priority)
                .Select(x => new CSharpStatement(x.Code.Trim()));
        }

        //private string GetCodeInNeatLines(IEnumerable<(string Code, int Priority)> codeSections, string baseIndent)
        //{
        //    var sb = new StringBuilder();
        //    PushIndent(baseIndent);

        //    foreach (var element in codeSections.OrderBy(x => x.Priority))
        //    {
        //        var codeLines = element.Code
        //            .Trim()
        //            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        //        foreach (var line in codeLines)
        //        {
        //            sb.Append(CurrentIndent).AppendLine(line);
        //        }
        //    }

        //    PopIndent();
        //    return sb.ToString();
        //}

        /// <remarks>
        /// It's a bit of a smell that a "Get" adds dependencies, but we can't process AddUsings in
        /// the Handle methods as not events might have been published yet.
        /// </remarks>
        public IEnumerable<string> GetContainerRegistrations()
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

            return containerRegistrationRequests.Select(DefineServiceRegistration);
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
                    ? $"services.{RegistrationType(x)}({UseTypeOf(x.InterfaceType)}, {UseTypeOf(x.ConcreteType)});"
                    : $"services.{RegistrationType(x)}({UseTypeOf(x.ConcreteType)});";
            }

            return x.InterfaceType != null
                ? $"services.{RegistrationType(x)}<{UseType(x.InterfaceType)}, {UseType(x.ConcreteType)}>();"
                : $"services.{RegistrationType(x)}<{UseType(x.ConcreteType)}>();";
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

    }

    public class EndpointsStatement : CSharpStatement, IHasCSharpStatements
    {
        public EndpointsStatement() : base(null)
        {
        }

        public EndpointsStatement(IEnumerable<CSharpStatement> configStatements) : this()
        {
            Statements = configStatements.ToList();
        }

        public IList<CSharpStatement> Statements { get; }

        public override string GetText(string indentation)
        {
            return $@"{indentation}app.UseEndpoints(endpoints =>
{indentation}{{
{string.Join($@"
", Statements.Select(x => x.GetText($"{indentation}    ")))}
{indentation}}});";
        }

        public void AddEndpointConfiguration(CSharpStatement statement)
        {
            Statements.Add(statement);
        }
    }
}