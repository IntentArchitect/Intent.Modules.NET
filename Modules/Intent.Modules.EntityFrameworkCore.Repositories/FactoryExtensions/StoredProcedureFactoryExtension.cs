using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository;
using Intent.Modules.EntityFrameworkCore.Templates;
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

            var dbContextTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.ConnectionStringDbContext);
            foreach (var dbContextTemplate in dbContextTemplates)
            {
                var hasTypeDefinitionResults = repositoryModels
                    .SelectMany(StoredProcedureHelpers.GetStoredProcedureModels)
                    .Select(x => x.TypeReference?.Element.AsTypeDefinitionModel())
                    .Any(x => x != null);

                if (hasTypeDefinitionResults)
                {
                    dbContextTemplate.CSharpFile.OnBuild(file =>
                    {
                        var @class = file.Classes.Single();
                        @class.AddMethod("T?", "ExecuteScalarAsync", method =>
                        {
                            method.AddGenericParameter("T", out var t);
                            method.Async();
                            method.AddParameter("string", "rawSql");
                            method.AddParameter($"{dbContextTemplate.UseType("System.Data.Common.DbParameter")}[]?", "parameters", p => p.WithParamsParameterModifier());

                            method.AddStatement("var connection = Database.GetDbConnection();");

                            method.AddStatement("// As per the note at https://learn.microsoft.com/ef/core/performance/advanced-performance-topics#managing-state-in-pooled-contexts,");
                            method.AddStatement("// we are responsible for leaving DbConnection states in the same way we found them.");
                            method.AddStatement($"var wasOpen = connection.State == {dbContextTemplate.UseType("System.Data.ConnectionState")}.Open;");
                            method.AddIfStatement("!wasOpen", @if =>
                            {
                                @if.AddStatement("await connection.OpenAsync();");
                                @if.SeparatedFromPrevious();
                            });

                            method.AddTryBlock(block =>
                            {
                                block.AddStatement("await using var command = connection.CreateCommand();");

                                block.AddStatement("command.CommandText = rawSql;", s => s.SeparatedFromPrevious());

                                block.AddForEachStatement("parameter", "parameters ?? []", @foreach =>
                                {
                                    @foreach.AddStatement("command.Parameters.Add(parameter);");
                                });
                                block.AddStatement($"return ({t}?)await command.ExecuteScalarAsync();");
                            });
                            method.AddFinallyBlock(block =>
                            {
                                block.AddIfStatement("!wasOpen", @if =>
                                {
                                    @if.AddStatement("await connection.CloseAsync();");
                                });
                            });
                        });
                    });
                }

                var dataContractResults = repositoryModels
                    .SelectMany(StoredProcedureHelpers.GetStoredProcedureModels)
                    .Select(x => x.TypeReference?.Element.AsDataContractModel())
                    .Where(x => x != null)
                    .Distinct()
                    .ToArray();

                if (dataContractResults.Any())
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
            }

            var classRepositories = repositoryModels
                .Where(x => x.TypeReference.Element.IsClassModel())
                .Select(x => (Entity: x.TypeReference.Element.AsClassModel(), Repository: x));

            foreach (var (entity, repository) in classRepositories)
            {
                var storedProcedures = repository.GetGeneralizedStoredProcedures();
                if (!storedProcedures.Any())
                {
                    continue;
                }

                if (TryGetTemplate<EntityRepositoryInterfaceTemplate>(application, EntityRepositoryInterfaceTemplate.TemplateId, entity, out var interfaceTemplate))
                {
                    // so that this template can be found when searched for by its various roles with the repository model (e.g. DomainInteractions with repositories):
                    foreach (var role in application.GetRolesForTemplate(interfaceTemplate))
                    {
                        application.RegisterTemplateInRoleForModel(role, repository, interfaceTemplate);
                    }

                    // so that this template can be found when searched for by Id and the repository model (e.g. DomainInteractions with repositories):
                    application.RegisterTemplateInRoleForModel(interfaceTemplate.Id, repository, interfaceTemplate);
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
                        var hasField = @class.Fields.Any(x => x.Name == "_dbContext");

                        foreach (var ctor in @class.Constructors)
                        {
                            var dbContextParameter = ctor.Parameters.SingleOrDefault(x => x.Name == "dbContext");
                            if (dbContextParameter == null)
                            {
                                continue;
                            }

                            if (!hasField)
                            {
                                dbContextParameter.IntroduceReadonlyField();
                                hasField = true;
                            }

                            if (ctor.Statements.All(x => !string.Equals(x.ToString(), "_dbContext = dbContext;")))
                            {
                                ctor.AddStatement("_dbContext = dbContext;");
                            }
                        }
                    }, 10);
                }

                StoredProcedureHelpers.ApplyImplementationMethods<RepositoryTemplate, ClassModel>(implementationTemplate, storedProcedures, DbContextManager.GetDbContext(entity));
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