using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBRepositoryBaseTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBRepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBRepositoryBaseTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetDependencies.IEvangelistAzureCosmosRepository);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Azure.Cosmos")
                .AddClass("CosmosDBRepositoryBase", @class =>
                {
                    @class
                        .Internal()
                        .Abstract()
                        .AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TPersistence", out var tPersistence)
                        .AddGenericParameter("TDocument", out var tDocument)
                        .ImplementsInterface($"{this.GetCosmosDBRepositoryInterfaceName()}<{tDomain}, {tPersistence}>")
                        .AddGenericTypeConstraint(tPersistence, c => c
                            .AddType(tDomain))
                        .AddGenericTypeConstraint(tDocument, c => c
                            .AddType(tPersistence)
                            .AddType($"{this.GetCosmosDBDocumentInterfaceName()}<{tDocument}, {tDomain}>")
                            .AddType("new()"))
                        ;

                    @class.AddConstructor(ctor =>
                    {
                        ctor.Protected();
                        ctor.AddParameter(this.GetCosmosDBUnitOfWorkName(), "unitOfWork",
                            p => p.IntroduceReadonlyField());
                        ctor.AddParameter(UseType($"Microsoft.Azure.CosmosRepository.IRepository<{tDocument}>"),
                            "cosmosRepository", p => p.IntroduceReadonlyField());
                        ctor.AddParameter("string", "idFieldName", p => p.IntroduceReadonlyField());
                    });

                    @class.AddProperty(this.GetCosmosDBUnitOfWorkInterfaceName(), "UnitOfWork", p => p
                        .WithoutSetter()
                        .Getter.WithExpressionImplementation("_unitOfWork")
                    );

                    @class.AddMethod("void", "Add", m => m
                        .AddParameter(tDomain, "entity")
                        .AddStatement("_unitOfWork.Track(entity);", s => s.SeparatedFromPrevious())
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);", c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement(
                                        "await _cosmosRepository.CreateAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod("void", "Update", m => m
                        .AddParameter(tDomain, "entity")
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);", c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement(
                                        "await _cosmosRepository.UpdateAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod("void", "Remove", m => m
                        .AddParameter(tDomain, "entity")
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);", c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement(
                                        "await _cosmosRepository.DeleteAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", m => m
                        .Async()
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement("var documents = await _cosmosRepository.GetAsync(_ => true, cancellationToken);", c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                        .AddStatement("var results = documents.Cast<TDomain>().ToList();")
                        .AddStatement("Track(results);")
                        .AddStatement("return results;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<{tDomain}?>", "FindByIdAsync", m => m
                        .Async()
                        .AddParameter("string", "id")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement(
                            "var document = await _cosmosRepository.GetAsync(id, cancellationToken: cancellationToken);", c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                        .AddStatement("Track(document);")
                        .AddStatement("return document;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method
                            .AddStatement(
                                "var documents = await _cosmosRepository.GetAsync(AdaptFilterPredicate(filterExpression), cancellationToken);"
                                , c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                            .AddStatement("var results = documents.Cast<TDomain>().ToList();")
                            .AddStatement("Track(results);")
                            .AddStatement("return results;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<IPagedResult<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddStatement("return await FindAllAsync(_ => true, pageNo, pageSize, cancellationToken);");
                    });

                    @class.AddMethod($"Task<IPagedResult<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method
                            .AddStatement(
                                "var pagedDocuments = await _cosmosRepository.PageAsync(AdaptFilterPredicate(filterExpression), pageNo, pageSize, true, cancellationToken);",
                                c => c.AddMetadata(MetadataNames.PagedDocumentsDeclarationStatement, true))
                            .AddStatement("Track(pagedDocuments.Items.Cast<TDomain>());")
                            .AddStatement("return new CosmosPagedList<TDomain, TDocument>(pagedDocuments, pageNo, pageSize);", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindByIdsAsync", m => m
                        .AddParameter("IEnumerable<string>", "ids")
                        .Async()
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement(
                            @"var queryDefinition = new QueryDefinition($""SELECT * from c WHERE ARRAY_CONTAINS(@ids, c.{_idFieldName})"")
                .WithParameter(""@ids"", ids);",
                            c => c.AddMetadata(MetadataNames.QueryDefinitionDeclarationStatement, true))
                        .AddStatement(
                            "var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);")
                        .AddStatement("var results = documents.Cast<TDomain>().ToList();")
                        .AddStatement("Track(results);")
                        .AddStatement("return results;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod("Expression<Func<TDocument, bool>>", "AdaptFilterPredicate", method =>
                    {
                        method
                            .Private()
                            .Static()
                            .AddParameter("Expression<Func<TPersistence, bool>>", "expression")
                            .WithComments(new[]
                            {
                                "/// <summary>",
                                "/// Adapts a <typeparamref name=\"TPersistence\"/> predicate to a <typeparamref name=\"TDocument\"/> predicate.",
                                "/// </summary>"
                            });

                        method.AddStatement("if (!typeof(TPersistence).IsAssignableFrom(typeof(TDocument))) throw new Exception($\"{typeof(TPersistence)} is not assignable from {typeof(TDocument)}.\");");
                        method.AddStatement("var beforeParameter = expression.Parameters.Single();");
                        method.AddStatement("var afterParameter = Expression.Parameter(typeof(TDocument), beforeParameter.Name);");
                        method.AddStatement("var visitor = new SubstitutionExpressionVisitor(beforeParameter, afterParameter);");
                        method.AddStatement("return Expression.Lambda<Func<TDocument, bool>>(visitor.Visit(expression.Body)!, afterParameter);");

                    });

                    @class.AddMethod("void", "Track", method =>
                    {
                        method.AddParameter($"IEnumerable<{tDomain}>", "items");

                        method.AddForEachStatement("item", "items", stmt => stmt.AddStatement("_unitOfWork.Track(item);"));

                    });

                    @class.AddMethod("void", "Track", method =>
                    {
                        method.AddParameter($"{tDomain}", "item");
                        method.AddStatement("_unitOfWork.Track(item);");
                    });


                    @class.AddNestedClass("SubstitutionExpressionVisitor", nestClass =>
                    {
                        nestClass
                            .Private()
                            .WithBaseType("ExpressionVisitor");
                        nestClass.AddConstructor(ctor =>
                        {
                            ctor
                                .AddParameter("Expression", "before", p => p.IntroduceReadonlyField())
                                .AddParameter("Expression", "after", p => p.IntroduceReadonlyField());
                        });

                        nestClass.AddMethod("Expression?", "Visit", method =>
                        {
                            method
                                .Override()
                                .AddParameter("Expression?", "node");

                            method.AddStatement("return node == _before ? _after : base.Visit(node);");
                        });
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            this.ApplyAppSetting("RepositoryOptions", new
            {
                CosmosConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                DatabaseId = ExecutionContext.GetApplicationConfig().Name,
                ContainerId = "Container"
            });
            ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.CosmosDb.Name)
                .WithProperty(Infrastructure.CosmosDb.Property.ConnectionStringSettingPath, "RepositoryOptions:CosmosConnectionString"));
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}