using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.MongoDbRepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbRepositoryBaseTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoDbRepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbRepositoryBaseTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            AddNugetDependency(NugetPackages.MongoDBDriver(outputTarget));
            RemoveNugetDependency(NugetPackages.MongoFrameworkPackageName);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq.Expressions")
                .AddUsing("MongoDB.Driver.Linq")
                .AddClass($"MongoRepositoryBase", @class =>
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
                        .ImplementsInterface($"{this.GetMongoDbRepositoryInterfaceName()}<{tDomain}, {tDocumentInterface}>")
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
                            .AddType($"{this.GetMongoDbDocumentOfTInterfaceName()}<{tDomain}{tDomainStateConstraint}, {tDocument}>")
                            .AddType(tDocumentInterface)
                            .AddType("new()"))
                        ;

                    @class.AddField("List<string>", "_objectIds", f => f.PrivateReadOnly().WithAssignment("new List<string>()"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.Protected();
                        ctor.AddParameter(UseType($"MongoDB.Driver.IMongoCollection<{tDocument}>"),
                            "collection", p => p.IntroduceReadonlyField());
                        ctor.AddParameter(this.GetMongoDbUnitOfWorkName(), "unitOfWork",
                            p => p.IntroduceReadonlyField());
                    });

                    @class.AddProperty(this.GetMongoDbUnitOfWorkInterfaceName(), "UnitOfWork", p => p
                        .WithoutSetter()
                        .Getter.WithExpressionImplementation("_unitOfWork")
                    );

                    @class.AddMethod("void", "Add", m => m
                        .Virtual()
                        .AddParameter(tDomain, "entity")
                        .AddStatement("_unitOfWork.Track(entity);", s => s.SeparatedFromPrevious())
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);")
                                    .AddStatement("await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);"));
                        })
                    );

                    @class.AddMethod("void", "Update", m => m
                        .Virtual()
                        .AddParameter(tDomain, "entity")
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);", c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement($"var filter = Builders<{tDocument}>.Filter.Eq(d => d.Id, document.Id);")
                                    .AddStatement(
                                        "await _collection.ReplaceOneAsync(filter, document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod("void", "Remove", m => m
                        .Virtual()
                        .AddParameter(tDomain, "entity")
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);", c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement($"var filter = Builders<{tDocument}>.Filter.Eq(d => d.Id, document.Id);")
                                    .AddStatement(
                                        "await _collection.DeleteOneAsync(filter, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod($"List<{tDomain}>", "SearchText", m => m
                        .Virtual()
                        .AddParameter("string", "searchText")
                        .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>?", "filterExpression", p => p.WithDefaultValue("null"))
                        .AddStatement(
                            $"var textFilter = Builders<{tDocument}>.Filter.Text(searchText);"
                            , c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                        .AddStatement(
                            $"FilterDefinition<{tDocument}> combinedFilter = textFilter;")
                        .AddIfStatement("filterExpression != null", stmt =>
                        {
                            stmt.AddStatement($"var adaptedFilter = Builders<{tDocument}>.Filter.Where(AdaptFilterPredicate(filterExpression));");
                            stmt.AddStatement($"combinedFilter = Builders<{tDocument}>.Filter.And(textFilter, adaptedFilter);");
                        })
                        .AddStatement("var documents = _collection.Find(combinedFilter).ToList();")
                        .AddStatement("return documents.Select(LoadAndTrackDocument).ToList();")
                        );


                    @class.AddMethod($"Task<{tDomain}?>", "FindAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method
                            .AddStatement(
                                "var documents = QueryInternal(filterExpression);"
                                , c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                            .AddIfStatement("!documents.Any()", stmt =>
                            {
                                stmt.AddStatement("return default;");
                            })
                            .AddStatement("var entity = LoadAndTrackDocument(documents.First());")
                            .AddStatement("return entity;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<{tDomain}?>", "FindAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method
                            .AddStatement(
                                "var documents = QueryInternal(filterExpression, linq);"
                                , c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                            .AddStatement(
                                "var document = await documents.FirstOrDefaultAsync(cancellationToken);")
                            .AddIfStatement("document == null", stmt =>
                            {
                                stmt.AddStatement("return default;");
                            })
                            .AddStatement("return LoadAndTrackDocument((TDocument)document);");
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", m => m
                        .Virtual()
                        .Async()
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement("var documents = QueryInternal(x => true);", c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                        .AddStatement("return LoadAndTrackDocuments(documents).ToList();", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method
                            .AddStatement(
                                " var documents = QueryInternal(filterExpression);"
                                , c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                            .AddIfStatement("!documents.Any()", stmt =>
                            {
                                stmt.AddStatement("return default;");
                            })
                            .AddStatement("return LoadAndTrackDocuments(documents).ToList();", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method
                            .AddStatement(
                                " var documents = QueryInternal(filterExpression, linq);"
                                , c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                            .AddIfStatement("documents == null", stmt =>
                            {
                                stmt.AddStatement("return default;");
                            })
                            .AddStatement($"return LoadAndTrackDocuments(documents.Select(d => ({tDocument})d)).ToList();", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddStatement("var query = QueryInternal(x => true);");
                        method.AddStatement("return await MongoPagedList<TDomain, TDocument>.CreateAsync(query.Cast<TDomain>(), pageNo, pageSize, cancellationToken);");
                    });

                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddStatement("var query = QueryInternal(filterExpression, linq);");
                        method.AddStatement("return await MongoPagedList<TDomain, TDocument>.CreateAsync(query.Cast<TDomain>(), pageNo, pageSize, cancellationToken);");
                    });

                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddStatement("var query = QueryInternal(filterExpression);");
                        method.AddStatement("return await MongoPagedList<TDomain, TDocument>.CreateAsync(query.Cast<TDomain>(), pageNo, pageSize, cancellationToken);");
                    });

                    @class.AddMethod($"Task<{tDomain}?>", "FindAsync", method => method
                        .Virtual()
                        .Async()
                        .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "queryOptions")
                        .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        .AddStatement("var documents = QueryInternal(x => true, queryOptions);")
                        .AddStatement("var document = await documents.FirstOrDefaultAsync(cancellationToken);")
                        .AddIfStatement("document == null", ifs => ifs.AddStatement("return default;"))
                        .AddStatement("return LoadAndTrackDocument((TDocument)document);", s => s.SeparatedFromPrevious())
                    );
                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                        .Virtual()
                        .Async()
                        .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "queryOptions")
                        .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        .AddStatement("var query = QueryInternal(x => true, queryOptions);")
                        .AddStatement("var documents = await query.ToListAsync(cancellationToken);")
                        .AddStatement("return LoadAndTrackDocuments(documents).ToList();", s => s.SeparatedFromPrevious())
                    );
                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        var tDomainStateGenericTypeArgument = createEntityInterfaces
                            ? $", {tDomainState}"
                            : string.Empty;

                        method
                            .Virtual()
                            .Async()
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "queryOptions")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                            .AddStatement("var query = QueryInternal(x => true, queryOptions);")
                            .AddStatement("return await MongoPagedList<TDomain, TDocument>.CreateAsync(query.Cast<TDomain>(), pageNo, pageSize, cancellationToken);");
                    });
                    @class.AddMethod("Task<int>", "CountAsync", method => method
                        .Virtual()
                        .Async()
                        .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                        .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        .AddStatement("return await QueryInternal(filterExpression).CountAsync(cancellationToken);", s => s.SeparatedFromPrevious())
                    );
                    @class.AddMethod("Task<int>", "CountAsync", method => method
                        .Virtual()
                        .Async()
                        .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>?", "queryOptions", param => param.WithDefaultValue("default"))
                        .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        .AddStatement("return await QueryInternal(x => true, queryOptions).CountAsync(cancellationToken);", s => s.SeparatedFromPrevious())
                    );
                    @class.AddMethod("bool", "Any", method => method
                        .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                        .AddStatement("return QueryInternal(filterExpression).Any();", s => s.SeparatedFromPrevious())
                    );
                    @class.AddMethod("Task<bool>", "AnyAsync", method => method
                    .Virtual()
                        .Async()
                        .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                        .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        .AddStatement("return await QueryInternal(filterExpression).AnyAsync(cancellationToken);", s => s.SeparatedFromPrevious())
                    );
                    @class.AddMethod("Task<bool>", "AnyAsync", method => method
                    .Virtual()
                        .Async()
                        .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>?", "queryOptions", param => param.WithDefaultValue("default"))
                        .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        .AddStatement("return await QueryInternal(x => true, queryOptions).AnyAsync(cancellationToken);", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"IQueryable<TDocument>", "QueryInternal", m => m
                        .Protected().Virtual()
                        .AddParameter("Expression<Func<TDocumentInterface, bool>>?", "filterExpression")
                        .AddStatement("var queryable = _collection.AsQueryable();")
                        .AddIfStatement("filterExpression != null", @if =>
                        {
                            @if.AddStatement("queryable = queryable.Where(AdaptFilterPredicate(filterExpression));");
                        })
                        .AddStatement("return queryable;", stmt => stmt.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"IQueryable<TDocument>", "QueryInternal", m => m
                        .Protected().Virtual()
                        .AddParameter("Expression<Func<TDocumentInterface, bool>>", "filterExpression")
                        .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "linq")
                        .AddStatement("var queryable = QueryInternal(filterExpression);")
                        .AddStatement("var castable = linq(queryable.Cast<TDocumentInterface>());")
                        .AddStatement("return castable.Cast<TDocument>();", stmt => stmt.SeparatedFromPrevious())
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
                        method.AddStatement("_objectIds.Add(document.Id);");

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

        //public override void AfterTemplateRegistration()
        //{
        //    base.AfterTemplateRegistration();

        //    this.ApplyAppSetting("RepositoryOptions", GetRepositoryOptions());

        //    ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.MongoDb.Name)
        //        .WithProperty(Infrastructure.MongoDb.Property.ConnectionStringSettingPath, "RepositoryOptions:MongoDbConnectionString"));
        //}

        //private object GetRepositoryOptions()
        //{
        //    if (DocumentTemplateHelpers.IsSeparateDatabaseMultiTenancy(ExecutionContext.Settings))
        //    {
        //        return new
        //        {
        //            DatabaseId = ExecutionContext.GetApplicationConfig().Name,
        //            ContainerId = "Container"
        //        };
        //    }
        //    else
        //    {
        //        return new
        //        {
        //            CosmosConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
        //            DatabaseId = ExecutionContext.GetApplicationConfig().Name,
        //            ContainerId = "Container"
        //        };
        //    }
        //}

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