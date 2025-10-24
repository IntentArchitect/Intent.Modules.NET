using System;
using System.Collections.Generic;
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

namespace Intent.Modules.Aws.Common.Templates.AwsConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AwsConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.Common.AwsConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AwsConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpClassMethod? configureMethod = null;

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"AwsConfiguration", @class =>
                {
                    AddNugetDependency(NugetPackages.AWSSDKExtensionsNETCoreSetup(OutputTarget));

                    @class.Static();

                    @class.AddMethod("IServiceCollection", "ConfigureAws", method =>
                    {
                        configureMethod = method;

                        method.Static();
                        method.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddStatement("services.AddDefaultAWSOptions(configuration.GetAWSOptions());");
                    });
                })
                .AfterBuild(_ =>
                {
                    configureMethod!.AddStatement("return services;", s => s.SeparatedFromPrevious());
                });
        }

        public override void AfterTemplateRegistration()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureAws", ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));
            
            base.AfterTemplateRegistration();
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