using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
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
using Intent.Modules.EntityFrameworkCore.Repositories.Api;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.DataContractEntityTypeConfiguration;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Modules.Modelers.Domain.StoredProcedures.Api;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using System.Linq;
using static Intent.Modules.Constants.TemplateRoles.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomRepositoryMethodsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.Repositories.CustomRepositoryMethodsExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 2;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var repositoryModels = application.MetadataManager.Domain(application).GetRepositoryModels();

            // Add EF specific db context methods etc
            var dbContextTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.ConnectionStringDbContext);
            foreach (var dbContextTemplate in dbContextTemplates)
            {
                var hasTypeDefinitionResults = repositoryModels
                    .SelectMany(EntityFrameworkRepositoryHelpers.GetStoredProcedureModels)
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

                var dataContractResults = EntityFrameworkRepositoryHelpers.GetEntityTypeConfigurationDataContractModels(application);

                if (dataContractResults.Count != 0)
                {
                    dbContextTemplate.CSharpFile.AfterBuild(file =>
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

                            // try find a EntityTypeConfiguration instance for the DataContract model
                            var entityTypeConfigTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(DataContractEntityTypeConfigurationTemplate.TemplateId, dc);
                            var @configClass = entityTypeConfigTemplate?.CSharpFile.Classes.FirstOrDefault();
                            var configureMethod = @configClass?.Methods.FirstOrDefault(x => x.Name == "Configure");

                            if (configureMethod is null)
                            {
                                var toViewStatement = $"modelBuilder.Entity<{typeName}>().HasNoKey().ToView(null);";
                                if (!onModelCreating.Statements.Any(s => s.Text == toViewStatement))
                                {
                                    onModelCreating.AddStatement(toViewStatement);
                                }
                                AddAttributeDecimalConfiguration(file, onModelCreating, dc, $"modelBuilder.Entity<{typeName}>()");
                            }
                            else
                            {
                                var applyConfigStatement = $"modelBuilder.ApplyConfiguration(new {file.Template.GetTypeName(entityTypeConfigTemplate.Id, dc)}());";
                                if (!onModelCreating.Statements.Any(s => s.Text == applyConfigStatement))
                                {
                                    onModelCreating.AddStatement(applyConfigStatement, config => config.AddMetadata("model", dc));
                                }

                                var toViewStatement = $"builder.HasNoKey().ToView(null);";
                                if (!configureMethod.Statements.Any(s => s.Text == toViewStatement))
                                {
                                    configureMethod.AddStatement(toViewStatement);
                                }
                                AddAttributeDecimalConfiguration(file, configureMethod, dc, $"builder");
                            }
                        }
                    });
                }
            }

            var classlessRepositories = repositoryModels
                .Where(repositoryModel => repositoryModel.TypeReference.Element == null);

            // add EF specific concerns to class less repositories (such as stored procedure calls)
            foreach (var repository in classlessRepositories)
            {
                if (TryGetTemplate(application, "Intent.Entities.Repositories.Api.CustomRepositoryInterface", repository, out ICSharpFileBuilderTemplate interfaceTemplate))
                {
                    EntityFrameworkRepositoryHelpers.ApplyEFInterfaceMethods<ICSharpFileBuilderTemplate, ClassModel>(interfaceTemplate, repository);
                }

                if (TryGetTemplate(application, "Intent.Entities.Repositories.Api.CustomRepository", repository, out ICSharpFileBuilderTemplate implementationTemplate))
                {
                    EntityFrameworkRepositoryHelpers.ApplyEFImplementationMethods<ICSharpFileBuilderTemplate, ClassModel>(implementationTemplate, repository);
                }
            }

            // get all repositories which have class type reference
            var classRepositories = repositoryModels
                .Where(repositoryModel => repositoryModel.TypeReference.Element.IsClassModel())
                .Select(repositoryModel => (Entity: repositoryModel.TypeReference.Element.AsClassModel(), Repository: repositoryModel));

            foreach (var (entity, repository) in classRepositories)
            {
                if (TryGetTemplate<EntityRepositoryInterfaceTemplate>(application, EntityRepositoryInterfaceTemplate.TemplateId, entity, out var interfaceTemplate))
                {
                    EntityFrameworkRepositoryHelpers.ApplyEFInterfaceMethods<EntityRepositoryInterfaceTemplate, ClassModel>(interfaceTemplate, repository);
                }

                if (!TryGetTemplate<RepositoryTemplate>(application, RepositoryTemplate.TemplateId, entity, out var implementationTemplate))
                {
                    continue;
                }

                var requiresDbContextField = repository.InternalElement.ChildElements.Any(element =>
                {
                    if (element.TypeReference.Element?.IsClassModel() == true &&
                        element.TypeReference.Element?.Id == implementationTemplate.Model.Id)
                    {
                        return false;
                    }

                    if (element.IsStoredProcedureModel())
                    {
                        return true;
                    }

                    var operationModel = Intent.Modelers.Domain.Api.OperationModelExtensions.AsOperationModel(element);
                    if (operationModel == null)
                    {
                        return false;
                    }

                    if (operationModel.TryGetStoredProcedure(out _))
                    {
                        return true;
                    }

                    if (operationModel.StoredProcedureInvocationTargets().Any(x => x.TypeReference.Element?.IsStoredProcedureModel() == true))
                    {
                        return true;
                    }

                    return false;
                });

                if (requiresDbContextField)
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

                EntityFrameworkRepositoryHelpers.ApplyEFImplementationMethods<ICSharpFileBuilderTemplate, ClassModel>(implementationTemplate, repository);
            }
        }

        private static void AddAttributeDecimalConfiguration(CSharpFile file, CSharpClassMethod method, DataContractModel dc, string statementPrefix)
        {
            foreach (var decimalAttribute in dc.Attributes.Where(a => a.TypeReference.HasDecimalType()))
            {
                var precision = "18,2";

                if (file.Template.ExecutionContext.Settings.GetDatabaseSettings().TryGetDecimalPrecisionAndScale(out var constraints))
                {
                    precision = $"{constraints.Precision},{constraints.Scale}";
                }

                if (decimalAttribute.TryGetDecimalConstraints(out var attributeConstraints))
                {
                    precision = $"{attributeConstraints.Precision() ?? 18},{attributeConstraints.Scale() ?? 2}";
                }

                var statement = $"{statementPrefix}.Property(x => x.{decimalAttribute.Name.ToPascalCase()}).HasPrecision({precision});";

                if (!method.Statements.Any(s => s.Text == statement))
                {
                    method.AddStatement(statement);
                }
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