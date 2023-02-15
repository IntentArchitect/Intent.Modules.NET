using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.MongoDb.Api;
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
            UpdateRepositoryInterfaceTemplate(application);
        }
        
        private static void UpdateRepositoryInterfaceTemplate(IApplication application)
        {
            var repositoryTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(EntityRepositoryInterfaceTemplate.TemplateId));
            foreach (var repositoryTemplate in repositoryTemplates)
            {
                repositoryTemplate.CSharpFile.AddUsing("System.Linq.Expressions");
                var inter = repositoryTemplate.CSharpFile.Interfaces.First();
                var model = inter.GetMetadata<ClassModel>("model");

                if (model.InternalElement?.Package?.SpecializationTypeId != MongoDomainPackageModel.SpecializationTypeId)
                {
                    continue;
                }
                
                inter.AddMethod("object", "Update", method =>
                {
                    method.AddAttribute("[IntentManaged(Mode.Fully)]");
                    method.AddParameter($"Expression<Func<{repositoryTemplate.GetTypeName("Domain.Entity", model)}, bool>>", "predicate")
                        .AddParameter(repositoryTemplate.GetTypeName("Domain.Entity.Interface", model), "entity");
                });
            }
        }
    }
}