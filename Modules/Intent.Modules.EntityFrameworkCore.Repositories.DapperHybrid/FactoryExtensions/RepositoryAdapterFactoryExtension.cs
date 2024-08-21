using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.DapperHybrid.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RepositoryAdapterFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.Repositories.DapperHybrid.RepositoryAdapterFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.Repositories.RepositoryBase");
            if (template == null)
            {
                return;
            }
            template.CSharpFile.OnBuild(file =>
            {
                template.AddNugetDependency(NugetPackages.Dapper(template.OutputTarget));
                var @class = template.CSharpFile.TypeDeclarations.First();

                @class.AddMethod(template.UseType("System.Data.IDbConnection"), "GetConnection", method =>
                {
                    method.Protected();
                    method.AddStatement("return _dbContext.Database.GetDbConnection();");
                });

            });
        }
    }
}