using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.WebAssembly.Templates.Program
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ProgramTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        private readonly IList<ContainerRegistrationRequest> _containerRegistrationRequests = new List<ContainerRegistrationRequest>();
        private readonly IList<ServiceConfigurationRequest> _serviceConfigurationRequests = new List<ServiceConfigurationRequest>();

        public const string TemplateId = "Intent.Blazor.WebAssembly.ProgramTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProgramTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreComponentsWebAssembly(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreComponentsWebAssemblyDevServer(outputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Components.Web")
                .AddUsing("Microsoft.AspNetCore.Components.WebAssembly.Hosting")
                .AddClass($"Program", @class =>
                {
                    @class.AddMethod("Task", "Main", method =>
                    {
                        method.Async()
                            .Static()
                            .AddParameter("string[]", "args");
                        method.AddStatement("var builder = WebAssemblyHostBuilder.CreateDefault(args);");
                        method.AddStatement("builder.RootComponents.Add<App>(\"#app\");");
                        method.AddStatement("builder.RootComponents.Add<HeadOutlet>(\"head::after\");");
                        method.AddStatement("await LoadAppSettings(builder);");
                        method.AddStatement("builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });");
                        method.AddStatement("await builder.Build().RunAsync();", stmt => stmt.AddMetadata("run-builder", "true"));
                    });

                    @class.AddMethod("Task", "LoadAppSettings", method =>
                    {
                        method.Async()
                            .Static()
                            .AddParameter("WebAssemblyHostBuilder", "builder");

                        method.AddStatement("var configProxy = new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };");
                        method.AddStatement("using var response = await configProxy.GetAsync(\"appsettings.json\");");
                        method.AddStatement("using var stream = await response.Content.ReadAsStreamAsync();");
                        method.AddStatement("builder.Configuration.AddJsonStream(stream);");
                    });
                })
                .AfterBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var method = priClass.FindMethod("Main");
                    var runStatement = method.FindStatement(p => p.HasMetadata("run-builder"));
                    foreach (var registration in _containerRegistrationRequests.OrderBy(x => x.Priority))
                    {
                        runStatement.InsertAbove(DefineServiceRegistration(registration));
                    }

                    foreach (var registration in _serviceConfigurationRequests.OrderBy(x => x.Priority))
                    {
                        runStatement.InsertAbove(ServiceConfigurationRegistration(registration));
                    }
                }, 1000);

            ExecutionContext.EventDispatcher.Subscribe<ContainerRegistrationRequest>(HandleEvent);
            ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(HandleEvent);
        }

        private void HandleEvent(ContainerRegistrationRequest @event)
        {
            if (@event.Concern != "BlazorClient")
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
            if (@event.Concern != "BlazorClient")
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
                false when !hasInterface && !usesTypeOfFormat => $"builder.Services.{registrationType}<{concreteType}>();",
                false when !hasInterface && usesTypeOfFormat => $"builder.Services.{registrationType}({concreteType});",
                false when hasInterface && !usesTypeOfFormat => $"builder.Services.{registrationType}<{interfaceType}, {concreteType}>();",
                false when hasInterface && usesTypeOfFormat => $"builder.Services.{registrationType}({interfaceType}, {concreteType});",
                true when !hasInterface && !usesTypeOfFormat => throw new InvalidOperationException(
                    $"Using a service provider for resolution during registration without an interface can cause an infinite loop. Concrete Type: {concreteType}"),
                true when !hasInterface && usesTypeOfFormat => throw new InvalidOperationException(
                    $"Using a service provider for resolution during registration without an interface can cause an infinite loop. Concrete Type: {concreteType}"),
                // These configurations can cause an infinite loop.
                // true when !hasInterface && !usesTypeOfFormat => $"services.{registrationType}(provider => provider.GetRequiredService<{concreteType}>());",
                // true when !hasInterface && usesTypeOfFormat => $"services.{registrationType}(provider => provider.GetRequiredService({concreteType}));",
                true when hasInterface && !usesTypeOfFormat => $"builder.Services.{registrationType}<{interfaceType}>(provider => provider.GetRequiredService<{concreteType}>());",
                true when hasInterface && usesTypeOfFormat => $"builder.Services.{registrationType}({interfaceType}, provider => provider.GetRequiredService({concreteType}));",
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

            return $"builder.Services.{registration.ExtensionMethodName}({GetExtensionMethodParameterList()});";
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            ExecutionContext.EventDispatcher.Publish(new LaunchProfileRegistrationRequest
            {
                Name = this.OutputTarget.GetProject().ApplicationName() + ".BlazorClient",
                CommandName = "Project",
                DotnetRunMessages = true,
                LaunchBrowser = true,
                InspectUri = "{wsProtocol}://{url.hostname}:{url.port}/_framework/debug/ws-proxy?browser={browserInspectUri}",
                ApplicationUrl = "https://localhost:{HttpsPort};http://localhost:{HttpPort}",
                PublishAllPorts = true,
            });
            ExecutionContext.EventDispatcher.Publish(new LaunchProfileRegistrationRequest
            {
                Name = "IIS Express",
                CommandName = "IISExpress",
                LaunchBrowser = true,
                InspectUri = "{wsProtocol}://{url.hostname}:{url.port}/_framework/debug/ws-proxy?browser={browserInspectUri}",
                PublishAllPorts = true,
            });
        }

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