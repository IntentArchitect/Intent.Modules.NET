using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.NetTopologySuite.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class StartupExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.NetTopologySuite.StartupExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Distribution.WebApi.Startup));
            template.AddNugetDependency(NugetPackages.NetTopologySuiteIoGeoJson4Stj);

            template?.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var configServicesMethod = @class.FindMethod("ConfigureServices");
                var lastConfigStatement = (CSharpInvocationStatement)configServicesMethod!.Statements.Last(p => p.HasMetadata("configure-services-controllers"));

                var addJsonOptionsStatement =
                    configServicesMethod.FindStatement(s => s.TryGetMetadata<string>("configure-services-controllers", out var v) && v == "json")
                        as CSharpInvocationStatement;
                if (addJsonOptionsStatement is null)
                {
                    addJsonOptionsStatement = new CSharpInvocationStatement(".AddJsonOptions");
                    addJsonOptionsStatement.AddMetadata("configure-services-controllers", "json");
                    lastConfigStatement.InsertBelow(addJsonOptionsStatement);
                }

                lastConfigStatement.WithoutSemicolon();

                var lambda = addJsonOptionsStatement.Statements.FirstOrDefault() as CSharpLambdaBlock;
                if (lambda is null)
                {
                    lambda = new CSharpLambdaBlock("options");
                    addJsonOptionsStatement.AddArgument(lambda);
                }

                lambda.AddStatement($@"options.JsonSerializerOptions.Converters.Add(new {template.UseType("NetTopologySuite.IO.Converters.GeoJsonConverterFactory")}());");
            }, 10);
        }
    }
}