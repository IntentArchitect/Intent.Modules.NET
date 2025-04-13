using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Entities.Repositories.Api.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomRepositoryMethodsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Entities.Repositories.Api.CustomRepositoryMethodsExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;


        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var repositoryModels = application.MetadataManager.Domain(application).GetRepositoryModels();

            var classRepositories = repositoryModels
                .Where(repositoryModel => repositoryModel.TypeReference.Element.IsClassModel())
                .Select(repositoryModel => (Entity: repositoryModel.TypeReference.Element.AsClassModel(), Repository: repositoryModel));

            foreach (var (entity, repository) in classRepositories)
            {
                if (TryGetTemplate<EntityRepositoryInterfaceTemplate>(application, EntityRepositoryInterfaceTemplate.TemplateId, entity, out var interfaceTemplate))
                {
                    // so that this template can be found when searched for by its various roles with the repository model (e.g. DomainInteractions with repositories):
                    foreach (var role in application.GetRolesForTemplate(interfaceTemplate))
                    {
                        application.RegisterTemplateInRoleForModel(role, repository, interfaceTemplate);
                    }

                    // so that this template can be found when searched for by Id and the repository model (e.g. DomainInteractions with repositories):
                    application.RegisterTemplateInRoleForModel(interfaceTemplate.Id, repository, interfaceTemplate);
                    CustomRepositoryHelper.ApplyInterfaceMethods<EntityRepositoryInterfaceTemplate, ClassModel>(interfaceTemplate, repository);
                }

                if (!TryGetTemplate<ICSharpFileBuilderTemplate>(application, TemplateRoles.Repository.Implementation.Entity, entity, out var implementationTemplate))
                {
                    continue;
                }

                CustomRepositoryHelper.ApplyImplementationMethods<ICSharpFileBuilderTemplate, RepositoryModel>(
                      template: implementationTemplate,
                      repositoryModel: repository);
            }
        }

        private static bool TryGetTemplate<T>(
            ISoftwareFactoryExecutionContext application,
            string templateId,
            object model,
            out T template) where T : class
        {
            template = application.FindTemplateInstance<T>(templateId, model);
            return template != null;
        }
    }
}