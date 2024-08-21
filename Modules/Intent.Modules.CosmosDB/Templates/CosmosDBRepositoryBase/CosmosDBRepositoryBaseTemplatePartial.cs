using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using Intent.Modules.CosmosDB.Settings;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Modelers.Domain.Settings;
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
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            var useOptimisticConcurrency = ExecutionContext.Settings.GetCosmosDb().UseOptimisticConcurrency();
            AddNugetDependency(NugetPackages.IEvangelistAzureCosmosRepository(outputTarget));
            AddNugetDependency(NugetPackages.NewtonsoftJson(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Azure.Cosmos")
                .AddUsing("Microsoft.Azure.Cosmos.Linq")
                .AddUsing("Microsoft.Azure.CosmosRepository.Extensions")
                .AddUsing("Microsoft.Azure.CosmosRepository.Options")
                .AddUsing("Microsoft.Azure.CosmosRepository.Providers")
                .AddUsing("Microsoft.Extensions.Options")

                .AddClass("CosmosDBRepositoryBase", @class =>
                {
                    @class
                        .Internal()
                        .Abstract()
                        .AddGenericParameter("TDomain", out var tDomain);

                    var tDomainState = tDomain;
                    if (createEntityInterfaces)
                    {
                        @class
                            .AddGenericParameter("TDomainState", out tDomainState);
                    }

                    @class
                        .AddGenericParameter("TDocument", out var tDocument)
                        .AddGenericParameter("TDocumentInterface", out var tDocumentInterface);

                    @class
                        .ImplementsInterface($"{this.GetCosmosDBRepositoryInterfaceName()}<{tDomain}, {tDocumentInterface}>")
                        .AddGenericTypeConstraint(tDomain, c => c
                            .AddType("class"));

                    if (createEntityInterfaces)
                    {
                        @class
                            .AddGenericTypeConstraint(tDomainState, c => c
                                .AddType("class")
                                .AddType(tDomain));
                    }

                    var tDomainStateConstraint = createEntityInterfaces
                        ? $", {tDomainState}"
                        : string.Empty;
                    @class
                        .AddGenericTypeConstraint(tDocument, c => c
                            .AddType($"{this.GetCosmosDBDocumentOfTInterfaceName()}<{tDomain}{tDomainStateConstraint}, {tDocument}>")
                            .AddType(tDocumentInterface)
                            .AddType("new()"))
                        ;

                    if (useOptimisticConcurrency)
                    {
                        @class.AddField("Dictionary<string, string?>", "_eTags", f => f
                            .PrivateReadOnly()
                            .WithAssignment(new CSharpStatement("new Dictionary<string, string?>()")));
                    }
                    @class.AddField("string", "_documentType", f => f.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.Protected();
                        ctor.AddParameter(this.GetCosmosDBUnitOfWorkName(), "unitOfWork",
                            p => p.IntroduceReadonlyField());
                        ctor.AddParameter(UseType($"Microsoft.Azure.CosmosRepository.IRepository<{tDocument}>"),
                            "cosmosRepository", p => p.IntroduceReadonlyField());
                        ctor.AddParameter("string", "idFieldName", p => p.IntroduceReadonlyField());
                        ctor.AddParameter(UseType($"ICosmosContainerProvider<{tDocument}>"), "containerProvider", p => p.IntroduceReadonlyField());
                        ctor.AddParameter("IOptionsMonitor<RepositoryOptions>", "optionsMonitor", p => p.IntroduceReadonlyField());
                        ctor.AddStatement("_documentType = typeof(TDocument).GetNameForDocument();");

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
                            var getEntityArgument = useOptimisticConcurrency
                                ? ", _ => null"
                                : string.Empty;

                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity{getEntityArgument});", c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement(
                                        "await _cosmosRepository.CreateAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    var updatePopulateFromEntityStatement = useOptimisticConcurrency
                        ? $"var document = new {tDocument}().PopulateFromEntity(entity, _eTags.GetValueOrDefault);"
                        : $"var document = new {tDocument}().PopulateFromEntity(entity);";
                    @class.AddMethod("void", "Update", m => m
                        .AddParameter(tDomain, "entity")
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement(updatePopulateFromEntityStatement, c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement(
                                        "await _cosmosRepository.UpdateAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    var removePopulateFromEntityStatement = useOptimisticConcurrency
                        ? $"var document = new {tDocument}().PopulateFromEntity(entity, _eTags.GetValueOrDefault);"
                        : $"var document = new {tDocument}().PopulateFromEntity(entity);";
                    @class.AddMethod("void", "Remove", m => m
                        .AddParameter(tDomain, "entity")
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement(removePopulateFromEntityStatement, c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement(
                                        "await _cosmosRepository.DeleteAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod($"Task<{tDomain}?>", "FindAsync", method =>
                    {
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method
                            .AddStatement(
                                "var documents = await _cosmosRepository.GetAsync(AdaptFilterPredicate(filterExpression), cancellationToken).ToListAsync();"
                                , c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                            .AddIfStatement("!documents.Any()", stmt =>
                            {
                                stmt.AddStatement("return default;");
                            })
                            .AddStatement("var entity = LoadAndTrackDocument(documents.First());")
                            .AddStatement("return entity;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", m => m
                        .Async()
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement("var documents = await _cosmosRepository.GetAsync(_ => true, cancellationToken);", c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                        .AddStatement("var results = LoadAndTrackDocuments(documents).ToList();")
                        .AddStatement("return results;", s => s.SeparatedFromPrevious())
                    );


                    @class.AddMethod($"Task<{tDomain}?>", "FindByIdAsync", m => m
                        .Async()
                        .Protected()
                        .AddParameter("string", "id")
                        .AddParameter("string?", "partitionKey", p => p.WithDefaultValue("default"))
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddTryBlock(tryBlock =>
                        {
                            tryBlock
                                .AddStatement(
                                    "var document = await _cosmosRepository.GetAsync(id, partitionKey, cancellationToken: cancellationToken);",
                                    c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                .AddStatement("var entity = LoadAndTrackDocument(document);")
                                .AddStatement("return entity;", s => s.SeparatedFromPrevious());
                        })
                        .AddCatchBlock(UseType("Microsoft.Azure.Cosmos.CosmosException"), "ex", c =>
                        {
                            c.WithWhenExpression($"ex.StatusCode == {UseType("System.Net.HttpStatusCode")}.NotFound");
                            c.AddStatement("return null;");
                        })
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method
                            .AddStatement(
                                "var documents = await _cosmosRepository.GetAsync(AdaptFilterPredicate(filterExpression), cancellationToken);"
                                , c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                            .AddStatement("var results = LoadAndTrackDocuments(documents).ToList();")
                            .AddStatement("return results;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddStatement("return await FindAllAsync(_ => true, pageNo, pageSize, cancellationToken);");
                    });

                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        var tDomainStateGenericTypeArgument = createEntityInterfaces
                            ? $", {tDomainState}"
                            : string.Empty;
                        method
                            .AddStatement(
                                "var pagedDocuments = await _cosmosRepository.PageAsync(AdaptFilterPredicate(filterExpression), pageNo, pageSize, true, cancellationToken);",
                                c => c.AddMetadata(MetadataNames.PagedDocumentsDeclarationStatement, true))
                            .AddStatement("var entities = LoadAndTrackDocuments(pagedDocuments.Items).ToList();")
                            .AddStatement("var totalCount = pagedDocuments.Total ?? 0;", s => s.SeparatedFromPrevious())
                            .AddStatement("var pageCount = pagedDocuments.TotalPages ?? 0;")
                            .AddStatement($"return new {this.GetCosmosPagedListName()}<{tDomain}{tDomainStateGenericTypeArgument}, {tDocument}>(entities, totalCount, pageCount, pageNo, pageSize);", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindByIdsAsync", m => m
                        .Async()
                        .AddParameter("IEnumerable<string>", "ids")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement(
                            @"var queryDefinition = new QueryDefinition($""SELECT * from c WHERE ARRAY_CONTAINS(@ids, c.{_idFieldName})"")
                .WithParameter(""@ids"", ids);",
                            c => c.AddMetadata(MetadataNames.QueryDefinitionDeclarationStatement, true))
                        .AddStatement("return await FindAllAsync(queryDefinition);", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<{tDomain}?>", "FindAsync", method => method
                        .Async()
                        .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "queryOptions")
                        .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        .AddStatement("var queryable = await CreateQuery(queryOptions);")
                        .AddStatement("var documents = await ProcessResults(queryable, cancellationToken);")
                        .AddIfStatement("!documents.Any()", ifs => ifs.AddStatement("return default;"))
                        .AddStatement("var entity = LoadAndTrackDocument(documents.First());")
                        .AddStatement("return entity;", s => s.SeparatedFromPrevious())
                    );
                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                        .Async()
                        .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "queryOptions")
                        .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        .AddStatement("var queryable = await CreateQuery(queryOptions);")
                        .AddStatement("var documents =  await ProcessResults(queryable, cancellationToken);")
                        .AddStatement("var results = LoadAndTrackDocuments(documents).ToList();")
                        .AddStatement("return results;", s => s.SeparatedFromPrevious())
                    );
                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        var tDomainStateGenericTypeArgument = createEntityInterfaces
                            ? $", {tDomainState}"
                            : string.Empty;

                        method
                            .Async()
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "queryOptions")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                            .AddStatement("var queryable = await CreateQuery(queryOptions, new QueryRequestOptions(){MaxItemCount = pageSize});")
                            .AddStatement("var countResponse = await queryable.CountAsync(cancellationToken);")
                            .AddStatement(@"queryable = queryable
                    .Skip(pageSize * (pageNo - 1))
                    .Take(pageSize);")
                            .AddStatement("var documents = await ProcessResults(queryable, cancellationToken);", s => s.SeparatedFromPrevious())
                            .AddStatement("var entities = LoadAndTrackDocuments(documents).ToList();")
                            .AddStatement("var totalCount = countResponse ?? 0;", s => s.SeparatedFromPrevious())
                            .AddStatement("var pageCount = (int)Math.Abs(Math.Ceiling(totalCount / (double)pageSize));")
                            .AddStatement($"return new {this.GetCosmosPagedListName()}<{tDomain}{tDomainStateGenericTypeArgument}, {tDocument}>(entities, totalCount, pageCount, pageNo, pageSize);", s => s.SeparatedFromPrevious());
                    });
                    @class.AddMethod("Task<int>", "CountAsync", method => method
                        .Async()
                        .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>?", "queryOptions", param => param.WithDefaultValue("default"))
                        .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        .AddStatement("var queryable = await CreateQuery(queryOptions);")
                        .AddStatement("return await queryable.CountAsync(cancellationToken);", s => s.SeparatedFromPrevious())
                    );
                    @class.AddMethod("Task<bool>", "AnyAsync", method => method
                        .Async()
                        .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>?", "queryOptions", param => param.WithDefaultValue("default"))
                        .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        .AddStatement("var queryable = await CreateQuery(queryOptions);")
                        .AddStatement("return await queryable.CountAsync(cancellationToken) > 0;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", m => m
                        .Protected()
                        .Async()
                        .AddParameter("QueryDefinition", "queryDefinition")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement("var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);")
                        .AddStatement("var results = LoadAndTrackDocuments(documents).ToList();")
                        .AddStatement("return results;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<{tDomain}?>", "FindAsync", m => m
                        .Protected()
                        .Async()
                        .AddParameter("QueryDefinition", "queryDefinition")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement("var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);")
                        .AddIfStatement("!documents.Any()", ifs => ifs.AddStatement("return default;"))
                        .AddStatement("var entity = LoadAndTrackDocument(documents.First());")
                        .AddStatement("return entity;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<IQueryable<TDocumentInterface>>", "CreateQuery", m => m
                        .Protected()
                        .Async()
                        .AddParameter("Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>>?", "queryOptions", p => p.WithDefaultValue("default"))
                        .AddParameter("QueryRequestOptions?", "requestOptions", p => p.WithDefaultValue("default"))
                        .AddStatement("var container = await _containerProvider.GetContainerAsync();")
                        .AddStatement("var queryable = (IQueryable<TDocumentInterface>)container.GetItemLinqQueryable<TDocumentInterface>(requestOptions: requestOptions, linqSerializerOptions: _optionsMonitor.CurrentValue.SerializationOptions);")
                        .AddStatement("queryable = queryOptions == null ? queryable : queryOptions(queryable);")
                        .AddStatement("//Filter by document type")
                        .AddStatement("queryable = queryable.Where(d => ((IItem)d!).Type == _documentType);")
                        .AddStatement("return queryable;", stmt => stmt.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<List<TProjection>>", "ProcessResults", m => m
                        .Protected()
                        .Async()
                        .AddGenericParameter("TProjection", out var tProjection)
                        .AddParameter($"IQueryable<{tProjection}>", "query")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement($"var results = new List<{tProjection}>();", stmt => stmt.SeparatedFromNext())
                        .AddStatement("using var feedIterator = query.ToFeedIterator();")
                        .AddWhileStatement("feedIterator.HasMoreResults", w => w.AddStatement("results.AddRange(await feedIterator.ReadNextAsync(cancellationToken));"))
                        .AddStatement("return results;", stmt => stmt.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<List<{tDocument}>>", "ProcessResults", m => m
                        .Protected()
                        .Async()
                        .AddParameter($"IQueryable<{tDocumentInterface}>", "query")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement($"var results = new List<{tDocument}>();", stmt => stmt.SeparatedFromNext())
                        .AddStatement($"using var feedIterator = query.Select(x => ({tDocument})x).ToFeedIterator();")
                        .AddWhileStatement("feedIterator.HasMoreResults", w => w.AddStatement("results.AddRange(await feedIterator.ReadNextAsync(cancellationToken));"))
                        .AddStatement("return results;", stmt => stmt.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Expression<Func<{tDocument}, bool>>", "AdaptFilterPredicate", method =>
                    {
                        method
                            .Private()
                            .Static()
                            .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "expression")
                            .WithComments(new[]
                            {
                                "/// <summary>",
                                $"/// Adapts a <typeparamref name=\"{tDocumentInterface}\"/> predicate to a <typeparamref name=\"{tDocument}\"/> predicate.",
                                "/// </summary>"
                            });

                        method.AddStatement("var beforeParameter = expression.Parameters.Single();");
                        method.AddStatement($"var afterParameter = Expression.Parameter(typeof({tDocument}), beforeParameter.Name);");
                        method.AddStatement("var visitor = new SubstitutionExpressionVisitor(beforeParameter, afterParameter);");
                        method.AddStatement($"return Expression.Lambda<Func<{tDocument}, bool>>(visitor.Visit(expression.Body)!, afterParameter);");

                    });

                    @class.AddMethod(tDomain, "LoadAndTrackDocument", method =>
                    {
                        method.AddParameter($"{tDocument}", "document");

                        method.AddStatement("var entity = document.ToEntity();");

                        method.AddStatement("_unitOfWork.Track(entity);", s => s.SeparatedFromPrevious());

                        if (useOptimisticConcurrency)
                        {
                            method.AddStatement("_eTags[document.Id] = document.Etag;");
                        }

                        method.AddStatement("return entity;", s => s.SeparatedFromPrevious());

                    });

                    @class.AddMethod($"IEnumerable<{tDomain}>", "LoadAndTrackDocuments", method =>
                    {
                        method.AddParameter($"IEnumerable<{tDocument}>", "documents");

                        method.AddForEachStatement("document", "documents", stmt => stmt.AddStatement("yield return LoadAndTrackDocument(document);"));
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

            this.ApplyAppSetting("RepositoryOptions", GetRepositoryOptions());

            ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.CosmosDb.Name)
                .WithProperty(Infrastructure.CosmosDb.Property.ConnectionStringSettingPath, "RepositoryOptions:CosmosConnectionString"));
        }

        private object GetRepositoryOptions()
        {
            if (DocumentTemplateHelpers.IsSeparateDatabaseMultiTenancy(ExecutionContext.Settings))
            {
                return new
                {
                    DatabaseId = ExecutionContext.GetApplicationConfig().Name,
                    ContainerId = "Container"
                };
            }
            else
            {
                return new
                {
                    CosmosConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                    DatabaseId = ExecutionContext.GetApplicationConfig().Name,
                    ContainerId = "Container"
                };
            }
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