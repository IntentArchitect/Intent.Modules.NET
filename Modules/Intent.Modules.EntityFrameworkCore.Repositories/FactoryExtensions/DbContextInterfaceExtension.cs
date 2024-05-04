using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.UnitOfWorkInterface;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DbContextInterfaceExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.Repositories.DbContextInterfaceExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var dbContextTemplates = application.FindTemplateInstances<DbContextTemplate>(TemplateRoles.Infrastructure.Data.ConnectionStringDbContext);
            foreach (var dbContextTemplate in dbContextTemplates)
            {
                dbContextTemplate.CSharpFile.OnBuild(file =>
                {
                    file.Classes.First().ImplementsInterface(dbContextTemplate.GetUnitOfWorkInterfaceName());
                });

                if (!dbContextTemplate.Model.IsApplicationDbContext)
                {
                    continue;
                }

                dbContextTemplate.ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                    .ToRegister(dbContextTemplate)
                    .ForInterface(dbContextTemplate.GetTemplate<IClassProvider>(UnitOfWorkInterfaceTemplate.TemplateId))
                    .ForConcern("Infrastructure")
                    .WithResolveFromContainer()
                    .WithPerServiceCallLifeTime());
            }
        }
    }
}