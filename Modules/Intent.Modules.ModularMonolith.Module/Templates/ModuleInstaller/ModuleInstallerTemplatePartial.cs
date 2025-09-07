using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Common.CSharp.AppStartup.IAppStartupFile;
using static Intent.Modules.Constants.TemplateRoles.Application;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Module.Templates.ModuleInstaller
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ModuleInstallerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        private readonly List<ContainerRegistrationRequest> _containerRegistrationRequests = new();
        private readonly List<ServiceConfigurationRequest> _serviceConfigurationRequests = new();
        private IHasCSharpStatements? _configureServicesBlock;
        private IServiceConfigurationContext _serviceConfigurationContext;

        public const string TemplateId = "Intent.ModularMonolith.Module.ModuleInstallerTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleInstallerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var isBuilt = false;
            OnEmitOrPublished<ServiceConfigurationRequest>(request =>
            {
                if (isBuilt)
                {
                    return;
                }

                _serviceConfigurationRequests.Add(request);
            });

            OnEmitOrPublished<ContainerRegistrationRequest>(request =>
            {
                if (isBuilt)
                {
                    return;
                }

                _containerRegistrationRequests.Add(request);
            });

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Swashbuckle.AspNetCore.SwaggerGen")
                .AddUsing("MassTransit")
                .AddUsing("Microsoft.AspNetCore.Mvc.ApplicationParts")
                .AddClass($"ModuleInstaller", @class =>
                {
                    AddFrameworkDependency("Microsoft.AspNetCore.App");
                    @class.ImplementsInterface(this.GetModuleInstallerInterfaceName());
                    @class.AddMethod("void", "ConfigureContainer", method =>
                    {
                        _configureServicesBlock = method;
                        _serviceConfigurationContext = new ServiceConfigurationContext("configuration", "services");

                        //Want Project Dependency
                        GetTemplate<ICSharpFileBuilderTemplate>("Intent.Infrastructure.DependencyInjection.DependencyInjection");

                        method.AddParameter("IServiceCollection", "services");
                        method.AddParameter("IConfiguration", "configuration");
#warning these may need to move later
                        method.AddStatement("var builder = services.AddControllers();");
                        method.AddStatement("builder.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(ModuleInstaller).Assembly));");
                    });
                    @class.AddMethod("void", "ConfigureSwagger", method =>
                    {
                        string moduleInstallerTypeName = GetFullTypeName(TemplateId);

                        method.AddParameter("SwaggerGenOptions", "options");
                        method.AddStatement($"AddCommentFile(options, Path.Combine(AppContext.BaseDirectory, $\"{{typeof({moduleInstallerTypeName}).Assembly.GetName().Name}}.xml\"));");
#warning Would like a better way to get this assembly name Also Can't use name resolution and Careful of Conflict with Host
                        method.AddStatement($"AddCommentFile(options, Path.Combine(AppContext.BaseDirectory, $\"{{typeof({GetFullTypeName("Intent.Application.DependencyInjection.DependencyInjection")}).Assembly.GetName().Name}}.Contracts.xml\"));");

                    });
                    @class.AddMethod("void", "ConfigureIntegrationEventConsumers", method =>
                    {
                        method.AddParameter("IRegistrationConfigurator", "cfg");
                        method.AddStatement($"{GetTypeName("Intent.Eventing.MassTransit.MassTransitConfiguration")}.AddConsumers(cfg);");
                    });

                    @class.AddMethod("void", "AddCommentFile", method =>
                    {
                        method
                            .Private()
                            .Static()
                            .AddParameter("SwaggerGenOptions", "options")
                            .AddParameter("string", "filename");
                        method.AddStatement($"string? docFile = Path.Combine(AppContext.BaseDirectory, filename);");
                        method.AddIfStatement("File.Exists(docFile)", ifs => { ifs.AddStatement("options.IncludeXmlComments(docFile);"); });
                    });

                    foreach (var request in _serviceConfigurationRequests)
                    {
                        ProcessServiceConfigurationRequest(request);
                    }

                    foreach (var request in _containerRegistrationRequests)
                    {
                        ProcessContainerRegistrationRequest(request);
                    }

                    isBuilt = true;

                    // Our previous subscriptions are configured to only process if "isBuilt" is false,
                    // and we make new subscriptions below. This is because other templates which are
                    // also subscribed may have been (by chance) instantiated after this class,
                    // resulting in their handlers being registered after and thus would only get to
                    // handle their subscriptions after this class which will read them all as
                    // unhandled. So we make new subscriptions which we know will process requests
                    // after the other handlers.
                    OnEmitOrPublished<ServiceConfigurationRequest>(ProcessServiceConfigurationRequest);
                    OnEmitOrPublished<ContainerRegistrationRequest>(ProcessContainerRegistrationRequest);

                });
        }

        private void ProcessServiceConfigurationRequest(ServiceConfigurationRequest request)
        {
            if (request.IsHandled)
            {
                return;
            }
            request.MarkAsHandled();
            
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

            AddServiceConfiguration<CSharpStatement>(ctx =>
            {
                var parameterList = new List<string>();

                foreach (var parameter in request.ExtensionMethodParameterList)
                {
                    switch (parameter)
                    {
                        case ServiceConfigurationRequest.ParameterType.Configuration:
                            parameterList.Add(ctx.Configuration);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                paramName: nameof(request.ExtensionMethodParameterList),
                                actualValue: parameter,
                                message: "Type specified in parameter list is not known or supported");
                    }
                }

                return $"{ctx.Services}.{request.ExtensionMethodName}({string.Join(", ", parameterList)});";
            }, priority: request.Priority);
        }

        public void AddServiceConfiguration<TStatement>(
            Func<IServiceConfigurationContext, TStatement> create,
            ConfigureServiceStatement<TStatement>? configure = null,
            int? priority = null) where TStatement : CSharpStatement
        {
            ConfigureServices((statements, context) =>
            {
                var statement = create(context);
                Insert(statements, statement, StatementType.ConfigureServices, priority);
                configure?.Invoke(statement, context);
            });
        }

        public void ConfigureServices(Action<IHasCSharpStatements, IServiceConfigurationContext> configure)
        {

            configure(_configureServicesBlock, _serviceConfigurationContext);
        }


        private static void Insert(
        IHasCSharpStatements hasCSharpStatements,
        CSharpStatement statement,
        StatementType type,
        int? priority)
        {
            priority ??= int.MaxValue;

            statement
                .AddMetadata("startup-statement-type", type)
                .AddMetadata("startup-statement-priority", priority);

            var insertBelow = hasCSharpStatements.Statements
                .Where(x => x.TryGetMetadata<StatementType>("startup-statement-type", out var metadataType) &&
                            metadataType == type)
                .LastOrDefault(x => (int)x.GetMetadata("startup-statement-priority") <= priority);

            if (insertBelow == null && type == StatementType.ContainerRegistration)
            {
                insertBelow = hasCSharpStatements.Statements
                    .LastOrDefault(x => x.TryGetMetadata<StatementType>("startup-statement-type", out var metadataType) &&
                                        metadataType == StatementType.ConfigureServices);
            }

            if (insertBelow == null)
            {
                hasCSharpStatements.InsertStatement(0, statement);
                return;
            }

            insertBelow.InsertBelow(statement);

            if (insertBelow.HasMetadata("startup-statement-requires-line-after"))
            {
                statement.SeparatedFromPrevious();
            }
        }


        private void ProcessContainerRegistrationRequest(ContainerRegistrationRequest request)
        {
            if (request.IsHandled)
            {
                return;
            }
            request.MarkAsHandled();

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

            AddContainerRegistration<CSharpStatement>(ctx =>
            {

                return (request.ConcreteType.StartsWith("typeof("), request.InterfaceType != null) switch
                {
                    (false, false) => $"{ctx.Services}.{RegistrationType(request)}<{UseType(request.ConcreteType)}>();",
                    (false, true) => $"{ctx.Services}.{RegistrationType(request)}<{UseType(request.InterfaceType!)}, {UseType(request.ConcreteType)}>();",
                    (true, false) => $"{ctx.Services}.{RegistrationType(request)}({UseTypeOf(request.ConcreteType)});",
                    (true, true) => $"{ctx.Services}.{RegistrationType(request)}({UseTypeOf(request.InterfaceType!)}, {UseTypeOf(request.ConcreteType)});"
                };

                string UseTypeOf(string type)
                {
                    var typeName = type.Substring("typeof(".Length, type.Length - "typeof()".Length);
                    return $"typeof({UseType(typeName)})";
                }

                static string RegistrationType(ContainerRegistrationRequest registration)
                {
                    return registration.Lifetime switch
                    {
                        ContainerRegistrationRequest.LifeTime.Singleton => "AddSingleton",
                        ContainerRegistrationRequest.LifeTime.PerServiceCall => "AddScoped",
                        _ => "AddTransient"
                    };
                }
            }, priority: request.Priority);
        }

        public void AddContainerRegistration<TStatement>(
            Func<IServiceConfigurationContext, TStatement> create,
            ConfigureServiceStatement<TStatement>? configure = null,
            int? priority = null) where TStatement : CSharpStatement
        {
            ConfigureServices((statements, context) =>
            {
                var statement = create(context);
                Insert(statements, statement, StatementType.ContainerRegistration, priority);
                configure?.Invoke(statement, context);
            });
        }


        private string GetFullTypeName(string templateId)
        {
            var template = GetTemplate<ICSharpFileBuilderTemplate>(templateId);
            return template.Namespace + "." + template.ClassName;
        }

        private enum StatementType
        {
            ConfigureServices,
            ContainerRegistration,
            AppConfiguration
        }
        private record ServiceConfigurationContext(string Configuration, string Services) : IServiceConfigurationContext;


        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}