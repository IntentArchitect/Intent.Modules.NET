using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AzureFunctions.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.OpenApi.Templates.OpenApiConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class OpenApiConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.OpenApi.OpenApiConfigurationTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public OpenApiConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            switch (outputTarget.GetProject().GetMaxNetAppVersion().Major)
            {
                case >= 8:
                    AddNugetDependency(NugetPackages.MicrosoftAzureFunctionsWorkerExtensionsOpenApi(outputTarget));
                    AddNugetDependency(NugetPackages.MicrosoftCodeAnalysisWorkspacesCommon(outputTarget));
                    AddNugetDependency(NugetPackages.MicrosoftCodeAnalysisCSharpWorkspaces(outputTarget));

                    ExecutionContext.EventDispatcher.Publish(new RemoveNugetPackageEvent(NugetPackages.MicrosoftAzureWebJobsExtensionsOpenApiPackageName, outputTarget));
                    break;
                case >= 6:
                    AddNugetDependency(NugetPackages.MicrosoftAzureWebJobsExtensionsOpenApi(outputTarget));

                    ExecutionContext.EventDispatcher.Publish(new RemoveNugetPackageEvent(NugetPackages.MicrosoftAzureFunctionsWorkerExtensionsOpenApiPackageName, outputTarget));
                    break;
            }

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums")
                .AddUsing("Microsoft.OpenApi.Models")
                .AddUsing("System.Collections.Generic")
                .AddClass($"OpenApiConfigurationOptions", @class =>
                {
                    @class.ImplementsInterface(UseType("Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions.IOpenApiConfigurationOptions"));
                    @class.AddProperty(UseType("Microsoft.OpenApi.Models.OpenApiInfo"), "Info");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddObjectInitializerBlock($"Info = new OpenApiInfo", block =>
                        {
                            block.AddInitStatement("Title", $"\"{OutputTarget.ApplicationName()} API\"");
                            block.AddInitStatement("Version", "\"1.0.0\"");
                            block.WithSemicolon();
                        });
                    });

                    @class.AddProperty("List<OpenApiServer>", "Servers", property => property.WithInitialValue("new ()"));
                    @class.AddProperty("OpenApiVersionType", "OpenApiVersion", property => property.WithInitialValue("OpenApiVersionType.V3"));
                    @class.AddProperty("List<IDocumentFilter>", "DocumentFilters");
                    @class.AddProperty("bool", "IncludeRequestingHostName", property => property.WithInitialValue("false"));
                    @class.AddProperty("bool", "ForceHttp", property => property.WithInitialValue("true"));
                    @class.AddProperty("bool", "ForceHttps", property => property.WithInitialValue("false"));
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