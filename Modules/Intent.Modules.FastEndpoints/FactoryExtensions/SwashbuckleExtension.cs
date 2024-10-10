using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.FastEndpoints.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SwashbuckleExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.FastEndpoints.SwashbuckleExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var configTemplate =
                application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                    TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration"));

            configTemplate?.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Asp.Versioning");
                file.AddUsing("Asp.Versioning.ApiExplorer");
                file.AddUsing("Microsoft.Extensions.Options");
                file.AddUsing("Swashbuckle.AspNetCore.SwaggerGen");
                file.AddUsing("System.Linq");

                var priClass = file.Classes.First();

                UpdateConfigureSwaggerMethod(priClass, configTemplate.GetApiVersionSwaggerGenOptionsName());
                UpdateUseSwashbuckleMethod(priClass);
                IntroduceAddSwaggerEndpointMethod(priClass);
            });
        }

        private static void UpdateConfigureSwaggerMethod(CSharpClass priClass, string apiVersionSwaggerGenOptionsName)
        {
            var method = priClass.FindMethod("ConfigureSwagger");
            method.Statements.Insert(0, $"services.AddTransient<IConfigureOptions<SwaggerGenOptions>, {apiVersionSwaggerGenOptionsName}>();");
            var addSwaggerGen = (CSharpInvocationStatement)method.FindStatement(p => p.HasMetadata("AddSwaggerGen"));
            var addSwaggerGenOptions = (CSharpLambdaBlock)addSwaggerGen?.Statements.FirstOrDefault();
            addSwaggerGenOptions?.FindStatement(p => p.GetText("").Contains("options.SwaggerDoc")).Remove();
        }

        private static void UpdateUseSwashbuckleMethod(CSharpClass priClass)
        {
            var method = priClass.FindMethod("UseSwashbuckle");
            var useSwaggerUi = (CSharpInvocationStatement)method.FindStatement(p => p.HasMetadata("UseSwaggerUI"));
            var useSwaggerUiOptions = (CSharpLambdaBlock)useSwaggerUi?.Statements.FirstOrDefault();
            useSwaggerUiOptions?.FindStatement(p => p.GetText("").Contains("options.SwaggerEndpoint")).Remove();

            useSwaggerUiOptions.AddInvocationStatement("AddSwaggerEndpoints", stmt => stmt
                .AddArgument("app")
                .AddArgument("options"));
        }

        private static void IntroduceAddSwaggerEndpointMethod(CSharpClass priClass)
        {
            priClass.AddMethod("void", "AddSwaggerEndpoints", method =>
            {
                method.Private().Static();
                method.AddParameter("IApplicationBuilder", "app");
                method.AddParameter("SwaggerUIOptions", "options");
                method.AddStatement(
                    "var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();");
                method.AddForEachStatement("description", "provider.ApiVersionDescriptions.OrderByDescending(o => o.ApiVersion)", stmt => stmt
                    .AddStatement(
                        @"options.SwaggerEndpoint($""/swagger/{description.GroupName}/swagger.json"", $""{options.OAuthConfigObject.AppName} {description.GroupName}"");"));
            });
        }
    }
}