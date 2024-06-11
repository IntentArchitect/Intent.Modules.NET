using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Dapper.Templates;
using Intent.Modules.Dapper.Templates.EntityRepositoryInterface;
using Intent.Modules.Dapper.Templates.Repository;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Dapper.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityRepositoryOperationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Dapper.EntityRepositoryOperationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var repositoryModels = application.MetadataManager.Domain(application).GetRepositoryModels();
            var classRepositories = repositoryModels
                .Where(x => x.TypeReference.Element.IsClassModel())
                .Select(x => (Entity: x.TypeReference.Element.AsClassModel(), Repository: x));

            foreach (var entry in classRepositories.Where(p => p.Repository.Operations.Any()))
            {
                var interfaceTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(EntityRepositoryInterfaceTemplate.TemplateId, entry.Entity);
                if (interfaceTemplate is not null)
                {
                    interfaceTemplate.CSharpFile.OnBuild(file =>
                    {
                        RepositoryOperationHelper.ApplyMethods(interfaceTemplate, interfaceTemplate.CSharpFile.Interfaces.First(), entry.Repository);
                    });
                    // so that this template can be found when searched for by its various roles with the repository model (e.g. DomainInteractions with repositories):
                    foreach (var role in application.GetRolesForTemplate(interfaceTemplate))
                    {
                        application.RegisterTemplateInRoleForModel(role, entry.Repository, interfaceTemplate);
                    }

                    // so that this template can be found when searched for by Id and the repository model (e.g. DomainInteractions with repositories):
                    application.RegisterTemplateInRoleForModel(interfaceTemplate.Id, entry.Repository, interfaceTemplate);
                }


                var repositoryTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(RepositoryTemplate.TemplateId, entry.Entity);
                if (repositoryTemplate is not null)
                {
                    repositoryTemplate.CSharpFile.OnBuild(file =>
                    {
                        RepositoryOperationHelper.ApplyMethods(repositoryTemplate, repositoryTemplate.CSharpFile.Classes.First(), entry.Repository);
                    });
                }
            }
        }
    }
}