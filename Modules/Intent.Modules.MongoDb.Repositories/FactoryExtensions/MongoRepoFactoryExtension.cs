using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.UnitOfWorkInterface;
using Intent.Modules.MongoDb.Templates.ApplicationMongoDbContext;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.Repositories.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MongoRepoFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.Repositories.MongoRepoFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var dbContextTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(ApplicationMongoDbContextTemplate.TemplateId));
            if (dbContextTemplate == null)
            {
                return;
            }

            var intentTemplate = (CSharpTemplateBase<object>)dbContextTemplate;
            var @class = dbContextTemplate.CSharpFile.Classes.First();
            @class.ImplementsInterface(intentTemplate.GetUnitOfWorkInterfaceName());
            @class.AddMethod("Task<int>", "IUnitOfWork.SaveChangesAsync", method =>
            {
                method.Async();
                method.WithoutAccessModifier();
                method.AddParameter("CancellationToken", "cancellationToken");
                method.AddStatement("return (await base.SaveChangesAsync(cancellationToken)).Results.Count;");
            });

            dbContextTemplate.ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(intentTemplate)
                .ForInterface(intentTemplate.GetTemplate<IClassProvider>(UnitOfWorkInterfaceTemplate.TemplateId))
                .ForConcern("Infrastructure")
                .WithResolveFromContainer());
        }
    }
}