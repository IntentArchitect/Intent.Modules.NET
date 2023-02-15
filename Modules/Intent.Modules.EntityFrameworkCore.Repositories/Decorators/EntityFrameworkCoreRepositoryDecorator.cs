using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.UnitOfWorkInterface;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Decorators
{
    public class EntityFrameworkCoreRepositoryDecorator : DecoratorBase
    {
        public const string DecoratorId = "Intent.EntityFrameworkCore.Repositories.EntityFrameworkCoreRepositoryDecorator";

        private readonly DbContextTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EntityFrameworkCoreRepositoryDecorator(DbContextTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            _template.CSharpFile.OnBuild(file =>
            {
                file.Classes.First().ImplementsInterface(_template.GetUnitOfWorkInterfaceName());
            });

            // GCB - this should surely not be here?
            _template.ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(_template)
                .ForInterface(_template.GetTemplate<IClassProvider>(UnitOfWorkInterfaceTemplate.TemplateId))
                .ForConcern("Infrastructure")
                .WithResolveFromContainer()
                .WithPerServiceCallLifeTime());
        }
    }
}