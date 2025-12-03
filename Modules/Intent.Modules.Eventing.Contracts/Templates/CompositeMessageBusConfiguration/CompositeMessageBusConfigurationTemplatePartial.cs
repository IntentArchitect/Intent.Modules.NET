using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
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
                    
                    var eventBusConfigTemplates = ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Eventing.MessageBusConfiguration).ToList();
                    foreach (var template in eventBusConfigTemplates)
                    {
                        if (!template.CanRunTemplate())
                        {
                            continue;
                        }
                        
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

        public override void BeforeTemplateExecution()
        {
            if (!this.RequiresCompositeMessageBus())
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureCompositeMessageBus", ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
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
