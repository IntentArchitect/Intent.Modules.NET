using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.NetTopologySuite.Templates.GeoJsonSchemaSwaggerFilter;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.NetTopologySuite.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SwaggerConfigExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.NetTopologySuite.SwaggerConfigExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            foreach (var template in application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Distribution.SwashbuckleConfiguration"))
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();

                    var configureSwaggerOptionsBlock = GetConfigureSwaggerOptionsBlock(@class);
                    if (configureSwaggerOptionsBlock is null)
                    {
                        return;
                    }

                    // The filter is placed by its own "Startup" role, independently of where Swashbuckle is
                    // configured, so a Swagger-enabled host may have no filter it can reach. Resolve the
                    // reachable instance here and name it directly: the template-id GetTypeName overload falls
                    // back to an unscoped lookup that picks up (or trips over) other hosts' filters.
                    if (template.OutputTarget.FindTemplateInstance(GeoJsonSchemaSwaggerFilterTemplate.TemplateId) is not { } filterTemplate ||
                        !filterTemplate.CanRunTemplate())
                    {
                        Logging.Log.Warning(
                            $"NetTopologySuite: project '{template.OutputTarget.GetProject().Name}' configures Swagger but has no GeoJSON schema filter it can reference, " +
                            "so geometry properties in its OpenAPI document will be described as NetTopologySuite object graphs rather than GeoJSON. " +
                            $"To include it, add a Template Output for {GeoJsonSchemaSwaggerFilterTemplate.TemplateId} under that project in the Codebase Structure designer.");
                        return;
                    }

                    configureSwaggerOptionsBlock.AddStatement($@"options.SchemaFilter<{template.GetTypeName(filterTemplate)}>();");
                });
            }
        }

        private static CSharpLambdaBlock? GetConfigureSwaggerOptionsBlock(CSharpClass @class)
        {
            var configureSwaggerMethod = @class.FindMethod("ConfigureSwagger");
            var addSwaggerGen = configureSwaggerMethod?.FindStatement(p => p.HasMetadata("AddSwaggerGen")) as CSharpInvocationStatement;
            var cSharpLambdaBlock = addSwaggerGen?.Statements.First() as CSharpLambdaBlock;
            return cSharpLambdaBlock;
        }
    }
}
