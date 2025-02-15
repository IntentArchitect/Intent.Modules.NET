using System.Linq;
using Intent.Engine;
using Intent.Entities.SoftDelete.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryBase;
using Intent.Modules.Entities.SoftDelete.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Entities.SoftDelete.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CosmosDbExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Entities.SoftDelete.CosmosDbExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.FindTemplateInstances<IIntentTemplate>("Domain.UnitOfWork.CosmosDB").Any())
            {
                return;
            }

            InstallSoftDeleteOnEntities(application);
            InstallSoftDeleteOnEntityDocumentInterfaces(application);
            InstallSoftDeleteOnCosmosDBRepositoryBase(application);
        }

        private static void InstallSoftDeleteOnEntities(IApplication application)
        {
            var entities = application
                .FindTemplateInstances<IIntentTemplate<ClassModel>>(
                    TemplateDependency.OnTemplate(TemplateRoles.Domain.Entity.Primary))
                .Where(p => p.Model.HasSoftDeleteEntity())
                .Cast<ICSharpFileBuilderTemplate>()
                .ToArray();
            foreach (var entity in entities)
            {
                entity.CSharpFile.OnBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var softDeleteInterfaceName = entity.GetSoftDeleteInterfaceName();
                    if (priClass.Interfaces.All(x => x != softDeleteInterfaceName))
                    {
                        priClass.ImplementsInterface(softDeleteInterfaceName);
                    }

                    priClass.AddMethod("void", "ISoftDelete.SetDeleted", method =>
                    {
                        method.WithoutAccessModifier();
                        method.AddParameter("bool", "isDeleted");
                        method.AddStatement("IsDeleted = isDeleted;");
                    });
                });
            }
        }

        private static void InstallSoftDeleteOnEntityDocumentInterfaces(IApplication application)
        {
            var docInterfaces = application.FindTemplateInstances<IIntentTemplate<ClassModel>>("Intent.CosmosDB.CosmosDBDocumentInterface")
                .Where(p => p.Model.HasSoftDeleteEntity())
                .Cast<ICSharpFileBuilderTemplate>()
                .ToArray();
            foreach (var docInterface in docInterfaces)
            {
                docInterface.CSharpFile.OnBuild(file =>
                {
                    var @interface = file.Interfaces.First();
                    var softDeleteInterfaceName = $"{docInterface.GetSoftDeleteInterfaceName()}ReadOnly";
                    @interface.ImplementsInterfaces(softDeleteInterfaceName);
                    var existingDeleteProperty = @interface.Properties.FirstOrDefault(p => p.Name == "IsDeleted");
                    if (existingDeleteProperty is not null)
                    {
                        @interface.Properties.Remove(existingDeleteProperty);
                    }
                }, 1001);
            }
        }

        private static void InstallSoftDeleteOnCosmosDBRepositoryBase(IApplication application)
        {
            var repo = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.CosmosDB.CosmosDBRepositoryBase");
            if (repo is null)
            {
                return;
            }

            repo.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var removeMethod = @class.FindMethod("Remove");
                if (removeMethod is not null)
                {
                    var existing = removeMethod.Statements.ToArray();
                    removeMethod.Statements.Clear();
                    removeMethod.AddIfStatement($"entity is {repo.GetSoftDeleteInterfaceName()} softDeleteEntity", @if =>
                    {
                        @if.AddStatement("softDeleteEntity.SetDeleted(true);");
                        @if.AddStatement("Update(entity);");
                    });
                    removeMethod.AddElseStatement(@else =>
                    {
                        @else.AddStatements(existing);
                    });
                }

                var findAllMethod = @class.Methods.FirstOrDefault(m => m.Name == "FindAllAsync" && m.Parameters.Count == 1);
                if (findAllMethod is not null)
                {
                    var targetStmt = findAllMethod.FindStatement(x => x.HasMetadata(MetadataNames.DocumentsDeclarationStatement));
                    if (targetStmt is not null)
                    {
                        targetStmt.Replace("var documents = await _cosmosRepository.GetAsync(AdaptFilterPredicate(_ => true), cancellationToken);");
                    }
                }

                var createQueryMethod = @class.FindMethod("CreateQuery");
                if (createQueryMethod is not null)
                {
                    var targetStmt = createQueryMethod.Statements.LastOrDefault(x => x.GetText("") == "return queryable;");
                    if (targetStmt is not null)
                    {
                        targetStmt.InsertAbove(new CSharpIfStatement($"typeof({repo.GetSoftDeleteInterfaceName()}).IsAssignableFrom(typeof(TDocumentInterface))")
                            .AddStatement($"queryable = queryable.Where(d => (({repo.GetSoftDeleteInterfaceName()})d!).IsDeleted == false);"));
                    }
                }

                var adaptFilterPredicateMethod = @class.FindMethod("AdaptFilterPredicate");
                if (adaptFilterPredicateMethod is not null)
                {
                    var targetStmt = adaptFilterPredicateMethod.Statements.LastOrDefault(x => x.GetText("") == "return Expression.Lambda<Func<TDocument, bool>>(visitor.Visit(expression.Body)!, afterParameter);");
                    if (targetStmt is not null)
                    {
                        targetStmt.InsertAbove("var adaptedBody = visitor.Visit(expression.Body)!;");
                        targetStmt.InsertAbove(new CSharpIfStatement($"typeof({repo.GetSoftDeleteInterfaceName()}).IsAssignableFrom(typeof(TDocumentInterface))")
                            .AddStatement($"var convertToSoftDelete = Expression.Convert(afterParameter, typeof({repo.GetSoftDeleteInterfaceName()}));")
                            .AddStatement($"var isDeletedProperty = Expression.Property(convertToSoftDelete, nameof({repo.GetSoftDeleteInterfaceName()}.IsDeleted));")
                            .AddStatement("var isDeletedCheck = Expression.Equal(isDeletedProperty, Expression.Constant(false));")
                            .AddStatement("adaptedBody = Expression.AndAlso(adaptedBody, isDeletedCheck);"));
                        targetStmt.Replace("return Expression.Lambda<Func<TDocument, bool>>(adaptedBody, afterParameter);");
                    }
                }
            }, 1);
        }
    }
}