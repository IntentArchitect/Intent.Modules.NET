using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.Templates.CompositeMessageBusConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CompositeMessageBusConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Contracts.CompositeMessageBusConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CompositeMessageBusConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass("CompositeMessageBusConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod("IServiceCollection", "ConfigureCompositeMessageBus", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddStatement($"// Create the central registry instance (mutable during startup)");
                        method.AddStatement($"var registry = new {this.GetMessageBrokerRegistryName()}();");
                        method.AddStatement("");
                        method.AddStatement("// Configure all message broker providers, passing the registry to each");
                        
                        // Add configuration calls - these will be populated dynamically by extensions
                        method.AddStatement("// Provider configurations will be added here", stmt => stmt.SeparatedFromPrevious());

                        method.AddStatement($"// Register the now-populated registry as a singleton", stmt => stmt.SeparatedFromPrevious());
                        method.AddStatement("services.AddSingleton(registry);");

                        method.AddStatement($"// Register the resolver (scoped, uses IServiceProvider to resolve providers per request)", stmt => stmt.SeparatedFromPrevious());
                        method.AddStatement($"services.AddScoped<{this.GetMessageBrokerResolverName()}>();");

                        method.AddStatement($"// Register CompositeMessageBus as {this.GetBusInterfaceName()} (scoped per request)", stmt => stmt.SeparatedFromPrevious());
                        method.AddStatement($"services.AddScoped<{this.GetBusInterfaceName()}, {this.GetCompositeMessageBusName()}>();");

                        method.AddStatement("return services;", stmt => stmt.SeparatedFromPrevious());
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            var templates = ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>("Eventing.MessageBusProvider").ToList();
            return templates.Count >= 2;
        }

        public override void BeforeTemplateExecution()
        {
            if (!CanRunTemplate())
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureCompositeMessageBus", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this));
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
