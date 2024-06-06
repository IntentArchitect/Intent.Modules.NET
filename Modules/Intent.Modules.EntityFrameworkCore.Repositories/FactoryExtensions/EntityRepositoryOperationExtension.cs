using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityRepositoryOperationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.Repositories.EntityRepositoryOperationExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var repositoryModels = application.MetadataManager.Domain(application).GetRepositoryModels();
            var classRepositories = repositoryModels
                .Where(x => x.TypeReference.Element.IsClassModel())
                .Select(x => (Entity: x.TypeReference.Element.AsClassModel(), Repository: x));

            foreach (var entry in classRepositories.Where(p => p.Repository.Operations.Any()))
            {
                var interfaceTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(EntityRepositoryInterfaceTemplate.TemplateId, entry.Entity);
                interfaceTemplate?.CSharpFile.OnBuild(file =>
                {
                    RepositoryOperationHelper.ApplyMethods(interfaceTemplate, interfaceTemplate?.CSharpFile.Interfaces.FirstOrDefault(), entry.Repository);
                });
                var repositoryTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(RepositoryTemplate.TemplateId, entry.Entity);
                repositoryTemplate?.CSharpFile.OnBuild(file =>
                {
                    RepositoryOperationHelper.ApplyMethods(repositoryTemplate, repositoryTemplate?.CSharpFile.Classes.FirstOrDefault(), entry.Repository);
                });
            }
        }
    }
}