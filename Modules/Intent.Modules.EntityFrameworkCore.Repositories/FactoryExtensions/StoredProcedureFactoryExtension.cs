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
    public class StoredProcedureFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.Repositories.StoredProcedureFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var repositoryModels = application.MetadataManager.Domain(application).GetRepositoryModels();

            var dataContractResults = repositoryModels
                .SelectMany(StoredProcedureHelpers.GetStoredProcedureModels)
                .Select(x => x.TypeReference?.Element.AsDataContractModel())
                .Where(x => x != null)
                .Distinct()
                .ToArray();

            if (dataContractResults.Any() &&
                TryGetTemplate<ICSharpFileBuilderTemplate>(application, TemplateRoles.Infrastructure.Data.DbContext, out var dbContextTemplate))
            {
                dbContextTemplate.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.Single();
                    var onModelCreating = @class.Methods.Single(x => x.Name == "OnModelCreating");

                    foreach (var dc in dataContractResults)
                    {
                        var typeName = dbContextTemplate.GetTypeName(TemplateRoles.Domain.DataContract, dc);

                        @class.AddProperty(
                            type: $"DbSet<{typeName}>",
                            name: typeName.Pluralize(),
                            configure: prop => prop.AddMetadata("model", dc));

                        onModelCreating.AddStatement($"modelBuilder.Entity<{typeName}>().HasNoKey().ToView(null);");
                    }
                });
            }

            var classRepositories = repositoryModels
                .Where(x => x.TypeReference.Element.IsClassModel())
                .Select(x => (Entity: x.TypeReference.Element.AsClassModel(), Repository: x));

            foreach (var (entity, repository) in classRepositories)
            {
                var storedProcedures = repository.GetStoredProcedureModels();
                if (!storedProcedures.Any())
                {
                    continue;
                }

                if (TryGetTemplate<EntityRepositoryInterfaceTemplate>(application, EntityRepositoryInterfaceTemplate.TemplateId, entity, out var interfaceTemplate))
                {
                    StoredProcedureHelpers.ApplyInterfaceMethods<EntityRepositoryInterfaceTemplate, ClassModel>(interfaceTemplate, storedProcedures);
                }

                if (!TryGetTemplate<RepositoryTemplate>(application, RepositoryTemplate.TemplateId, entity, out var implementationTemplate))
                {
                    continue;
                }

                if (storedProcedures.Any(x => x.TypeReference.Element?.Id != implementationTemplate.Model.Id))
                {
                    implementationTemplate.CSharpFile.AfterBuild(file =>
                    {
                        var @class = file.Classes.First();

                        var parameters = @class.Constructors
                            .SelectMany(x => x.Parameters)
                            .Where(x => x.Name == "dbContext");

                        foreach (var parameter in parameters)
                        {
                            parameter.IntroduceReadonlyField();
                        }
                    });
                }

                StoredProcedureHelpers.ApplyImplementationMethods<RepositoryTemplate, ClassModel>(implementationTemplate, storedProcedures);
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

        private static bool TryGetTemplate<T>(
            ISoftwareFactoryExecutionContext application,
            string templateId,
            out T template) where T : class
        {
            template = application.FindTemplateInstance<T>(templateId);
            return template != null;
        }
    }
}