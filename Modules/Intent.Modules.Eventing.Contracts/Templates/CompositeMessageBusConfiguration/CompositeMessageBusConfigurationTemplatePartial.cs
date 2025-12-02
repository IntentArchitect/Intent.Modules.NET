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
                    });
                })
                .OnBuild(file =>
                {
                    var method = file.Classes.First().FindMethod("ConfigureCompositeMessageBus")!;
                    method.AddStatement($"var registry = new {this.GetMessageBrokerRegistryName()}();", s => s.SeparatedFromNext());
                    
                    var eventBusConfigTemplates = ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>("Eventing.MessageBusConfiguration").ToList();
                    foreach (var template in eventBusConfigTemplates)
                    {
                        var configMethodName = template.CSharpFile.Classes.First().FindMethod(m => m.Name.StartsWith("Add") && m.Name.EndsWith("Configuration"))?.Name;
                        if (configMethodName == null)
                        {
                            throw new Exception($"Could not find configuration method on template '{template.Id}'.");
                        }

                        method.AddInvocationStatement($"services.{configMethodName}", inv =>
                        {
                            inv.AddArgument("configuration");
                            inv.AddArgument("registry");
                        });
                        AddTemplateDependency(TemplateDependency.OnTemplate(template));
                    }

                    method.AddStatement("services.AddSingleton(registry);", s => s.SeparatedFromPrevious());

                    method.AddStatement($"services.AddScoped<{this.GetMessageBrokerResolverName()}>();", s => s.SeparatedFromPrevious());
                    method.AddStatement($"services.AddScoped<{this.GetBusInterfaceName()}, {this.GetCompositeMessageBusName()}>();");

                    method.AddReturn("services", stmt => stmt.SeparatedFromPrevious());
                }, 1);
        }

        // To prevent potential stack overflow since multiple other Message Bus Configuration templates
        // are querying the RequiresCompositeMessageBus() method which runs this template's CanRunTemplate() method
        // which will be querying other Message Bus Configuration templates again and so it goes...
        private bool _processingCanRunTemplate = false;
        public override bool CanRunTemplate()
        {
            if (_processingCanRunTemplate)
            {
                return false;
            }

            _processingCanRunTemplate = true;
            var result = RequiresCompositeMessageBus();
            _processingCanRunTemplate = false;
            return result;
        }

        public override void BeforeTemplateExecution()
        {
            if (!RequiresCompositeMessageBus())
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureCompositeMessageBus", ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));
        }
        
        private bool RequiresCompositeMessageBus()
        {
            // Don't use a different way to find templates as they would result in a cached lookup
            // and so it may miss some message brokers when queried for configuring them up.
            var templates = ExecutionContext.FindTemplateInstances("Eventing.MessageBusConfiguration")
                .Where(x => x.CanRunTemplate())
                .ToList();
            return templates.Count >= 2;
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
