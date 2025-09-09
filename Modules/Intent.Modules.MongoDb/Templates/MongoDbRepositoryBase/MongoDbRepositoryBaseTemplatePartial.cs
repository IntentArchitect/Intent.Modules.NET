using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
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

            outputTarget.ExecutionContext.EventDispatcher.Publish(new RemoveNugetPackageEvent(
                    NugetPackages.MongoFrameworkPackageName, outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Threading")
                .AddUsing("System.Linq")
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
                        .AddGenericParameter("TDocumentInterface", out var tDocumentInterface)
                        .AddGenericParameter("TIdentifier", out var tIdentifier);

                    @class
                        .ImplementsInterface($"{this.GetMongoDbRepositoryInterfaceName()}<{tDomain}, {tDocumentInterface}, {tIdentifier}>")
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
                            .AddType("class")
                            .AddType($"{this.GetMongoDbDocumentOfTInterfaceName()}<{tDomain}{tDomainStateConstraint}, {tDocument}, {tIdentifier}>")
                            .AddType(tDocumentInterface)
                            .AddType("new()"))
                        ;

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
                                    .AddStatement("await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);")
                                    .AddStatement("document.ToEntity(entity);"));
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
                                    .AddStatement(
                                        "await _collection.ReplaceOneAsync(document.GetIdFilter(), document, cancellationToken: cancellationToken);")
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
                                    .AddStatement(
                                        "await _collection.DeleteOneAsync(document.GetIdFilter(), cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod($"Task<{tDomain}>", "FindByIdAsync", m => m
                        .Virtual().Async()
                        .AddParameter(tIdentifier, "id")
                        .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"))
                        .AddStatement($"var result = QueryInternalTDocument(TDocument.GetIdFilterPredicate(id)).SingleOrDefault();")
                        .AddIfStatement("result == null", @if => @if.AddReturn("null"))
                        .AddStatement($"return LoadAndTrackDocument(result);")
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindByIdsAsync", m => m
                        .Virtual().Async()
                        .AddParameter($"{tIdentifier}[]", "ids")
                        .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"))
                        .AddStatement($"var result = QueryInternalTDocument(TDocument.GetIdsFilterPredicate(ids));")
                        .AddStatement($"return LoadAndTrackDocuments(result).ToList();")
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
                        method.AddStatement("return await MongoPagedList<TDomain, TDocument, TIdentifier>.CreateAsync(query, pageNo, pageSize, cancellationToken);");
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
                        method.AddStatement("return await MongoPagedList<TDomain, TDocument, TIdentifier>.CreateAsync(query, pageNo, pageSize, cancellationToken);");
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
                        method.AddStatement("return await MongoPagedList<TDomain, TDocument, TIdentifier>.CreateAsync(query, pageNo, pageSize, cancellationToken);");
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
                            .AddStatement("return await MongoPagedList<TDomain, TDocument, TIdentifier>.CreateAsync(query, pageNo, pageSize, cancellationToken);");
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

                    @class.AddMethod($"IQueryable<{tDocument}>", "QueryInternal", m => m
                        .Protected().Virtual()
                        .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>?", "filterExpression")
                        .AddIfStatement("filterExpression != null", @if =>
                        {
                            @if.AddStatement("return QueryInternalTDocument(AdaptFilterPredicate(filterExpression));");
                        })
                        .AddStatement("return QueryInternalTDocument(null);", stmt => stmt.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"IQueryable<{tDocument}>", "QueryInternalTDocument", m => m
                        .Protected().Virtual()
                        .AddParameter($"Expression<Func<{tDocument}, bool>>?", "filterExpression")
                        .AddStatement("var queryable = _collection.AsQueryable();")
                        .AddIfStatement("filterExpression != null", @if =>
                        {
                            @if.AddStatement("queryable = queryable.Where(filterExpression);");
                        })
                        .AddStatement("return queryable;", stmt => stmt.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"IQueryable<TDocument>", "QueryInternal", m => m
                        .Protected().Virtual()
                        .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                        .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "linq")
                        .AddStatement("var queryable = QueryInternal(filterExpression);")
                        .AddStatement("var adaptedQueryFunction = QueryableAdapter.AdaptQueryFunction<TDocumentInterface, TDocument>(linq);")
                        .AddStatement("var result = adaptedQueryFunction(queryable);")
                        .AddStatement("return result;", stmt => stmt.SeparatedFromPrevious())
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
                }).AddClass("QueryableAdapter", @class =>
                {
                    @class.Static();

                    @class.AddMethod("Func<IQueryable<TDocument>, IQueryable<TDocument>>", "AdaptQueryFunction", method =>
                    {
                        method.Static();
                        method.AddGenericParameter("TDocumentInterface");
                        method.AddGenericParameter("TDocument");

                        method.AddParameter("Func<IQueryable<TDocumentInterface>, IQueryable<TDocumentInterface>>", "queryOptions");

                        method.AddGenericTypeConstraint("TDocument", tDocument => tDocument.AddType("class").AddType("TDocumentInterface"));

                        method.AddReturn(@$"sourceQueryable =>
                        {{
                            // Create a fake queryable of the interface type
                            var interfaceQueryable = new InterfaceQueryableAdapter<TDocumentInterface, TDocument>(sourceQueryable);

                            // Apply the user's query function
                            var resultQueryable = queryOptions(interfaceQueryable);

                            // Extract the adapted queryable
                            if (resultQueryable is InterfaceQueryableAdapter<TDocumentInterface, TDocument> adapter)
                            {{
                                return adapter.UnderlyingQueryable;
                            }}

                            throw new InvalidOperationException(""Query function returned an unexpected queryable type"");
                        }}");
                    });
                }).AddClass("InterfaceQueryableAdapter", @class =>
                {
                    @class.Internal();

                    @class.AddGenericParameter("TInterface", out var tDocument);
                    @class.AddGenericParameter("TDocument", out var tInterface);

                    @class.ImplementsInterface("IQueryable<TInterface>");

                    @class.AddGenericTypeConstraint(tInterface, c => c.AddType("class").AddType(tDocument));

                    @class.AddField("QueryableMethodAdapter", "_methodAdapter", f => f.PrivateReadOnly());

                    @class.AddConstructor(c =>
                    {
                        c.AddParameter("IQueryable<TDocument>", "underlyingQueryable", p => p.IntroduceReadonlyField());

                        c.AddStatement("_methodAdapter = new QueryableMethodAdapter(typeof(TInterface), typeof(TDocument));");
                    });

                    @class.AddProperty("IQueryable<TDocument>", "UnderlyingQueryable", p => p.ReadOnly().Getter.WithExpressionImplementation("_underlyingQueryable"));
                    @class.AddProperty("Type", "ElementType", p => p.ReadOnly().Getter.WithExpressionImplementation("typeof(TInterface)"));
                    @class.AddProperty("Expression", "Expression", p => p.ReadOnly().Getter.WithExpressionImplementation("_methodAdapter.AdaptExpression(_underlyingQueryable.Expression)"));
                    @class.AddProperty("IQueryProvider", "Provider", p => p.ReadOnly().Getter.WithExpressionImplementation("new InterfaceQueryProvider<TInterface, TDocument>(_underlyingQueryable.Provider, _methodAdapter)"));

                    @class.AddMethod("IEnumerator<TInterface>", "GetEnumerator", m => m.AddReturn("_underlyingQueryable.Cast<TInterface>().GetEnumerator()"));

                    @class.AddMethod("System.Collections.IEnumerator", "System.Collections.IEnumerable.GetEnumerator", m => m.AddReturn("GetEnumerator()").WithoutAccessModifier());
                }).AddClass("InterfaceQueryProvider", @class =>
                {
                    @class.Internal();

                    @class.AddGenericParameter("TInterface", out var tDocument);
                    @class.AddGenericParameter("TDocument", out var tInterface);

                    @class.ImplementsInterface("IQueryProvider");

                    @class.AddGenericTypeConstraint(tInterface, c => c.AddType("class").AddType(tDocument));

                    @class.AddConstructor(c =>
                    {
                        c.AddParameter("IQueryProvider", "underlyingProvider", p => p.IntroduceReadonlyField());
                        c.AddParameter("QueryableMethodAdapter", "methodAdapter", p => p.IntroduceReadonlyField());
                    });

                    @class.AddMethod("IQueryable", "CreateQuery", method =>
                    {
                        method.AddParameter("Expression", "expression");

                        method.AddStatement("var adaptedExpression = _methodAdapter.AdaptExpressionFromInterface(expression);");
                        method.AddStatement("var result = _underlyingProvider.CreateQuery(adaptedExpression);");

                        method.AddIfStatement("result is IQueryable<TDocument> typedResult", @if => @if.AddReturn("new InterfaceQueryableAdapter<TInterface, TDocument>(typedResult)"));

                        method.AddReturn("result");
                    });

                    @class.AddMethod("IQueryable<TElement>", "CreateQuery", method =>
                    {
                        method.AddGenericParameter("TElement");
                        method.AddParameter("Expression", "expression");

                        method.AddIfStatement("typeof(TElement) == typeof(TInterface)", @if =>
                        {
                            @if.AddStatement("var adaptedExpression = _methodAdapter.AdaptExpressionFromInterface(expression);");
                            @if.AddStatement("var result = _underlyingProvider.CreateQuery<TDocument>(adaptedExpression);");
                            @if.AddReturn("(IQueryable<TElement>)(object)new InterfaceQueryableAdapter<TInterface, TDocument>(result)");
                        });

                        method.AddStatement("var directAdaptedExpression = _methodAdapter.AdaptExpressionFromInterface(expression);");
                        method.AddReturn("_underlyingProvider.CreateQuery<TElement>(directAdaptedExpression)");
                    });

                    @class.AddMethod("object", "Execute", method =>
                    {
                        method.AddParameter("Expression", "expression");

                        method.AddStatement("var adaptedExpression = _methodAdapter.AdaptExpressionFromInterface(expression);");
                        method.AddReturn("_underlyingProvider.Execute(adaptedExpression)");
                    });

                    @class.AddMethod("TResult", "Execute", method =>
                    {
                        method.AddGenericParameter("TResult");
                        method.AddParameter("Expression", "expression");

                        method.AddStatement("var adaptedExpression = _methodAdapter.AdaptExpressionFromInterface(expression);");
                        method.AddReturn("_underlyingProvider.Execute<TResult>(adaptedExpression)");
                    });
                }).AddClass("QueryableMethodAdapter", @class =>
                {
                    @class.Internal();

                    @class.AddConstructor(c =>
                    {
                        c.AddParameter("Type", "interfaceType", p => p.IntroduceReadonlyField());
                        c.AddParameter("Type", "documentType", p => p.IntroduceReadonlyField());
                    });

                    @class.AddMethod("Expression", "AdaptExpression", method =>
                    {
                        method.AddParameter("Expression", "expression");

                        method.AddReturn("new TypeSubstitutionVisitor(_documentType, _interfaceType).Visit(expression)");
                    });

                    @class.AddMethod("Expression", "AdaptExpressionFromInterface", method =>
                    {
                        method.AddParameter("Expression", "expression");

                        method.AddReturn("new TypeSubstitutionVisitor(_interfaceType, _documentType).Visit(expression)");
                    });
                }).AddClass("TypeSubstitutionVisitor", @class =>
                {
                    @class.Internal();

                    @class.WithBaseType("ExpressionVisitor");

                    @class.AddConstructor(c =>
                    {
                        c.AddParameter("Type", "fromType", p => p.IntroduceReadonlyField());
                        c.AddParameter("Type", "toType", p => p.IntroduceReadonlyField());
                    });

                    @class.AddMethod("Expression", "VisitParameter", method =>
                    {
                        method.Protected().Override();
                        method.AddParameter("ParameterExpression", "node");

                        method.AddIfStatement("node.Type == _fromType", @if =>
                        {
                            @if.AddReturn("Expression.Parameter(_toType, node.Name)");
                        });

                        method.AddReturn("base.VisitParameter(node)");
                    });

                    @class.AddMethod("Expression", "VisitMethodCall", method =>
                    {
                        method.Protected().Override();
                        method.AddParameter("MethodCallExpression", "node");

                        method.AddIfStatement("node.Method.IsGenericMethod", @if =>
                        {
                            @if.AddStatements(@$"var genericArgs = node.Method.GetGenericArguments();
                                var newGenericArgs = new Type[genericArgs.Length];
                                bool hasChanges = false;

                                for (int i = 0; i < genericArgs.Length; i++)
                                {{
                                    if (genericArgs[i] == _fromType)
                                    {{
                                        newGenericArgs[i] = _toType;
                                        hasChanges = true;
                                    }}
                                    else
                                    {{
                                        newGenericArgs[i] = genericArgs[i];
                                    }}
                                }}

                                if (hasChanges)
                                {{
                                    var newMethod = node.Method.GetGenericMethodDefinition().MakeGenericMethod(newGenericArgs);
                                    var newObject = Visit(node.Object);
                                    var newArgs = node.Arguments.Select(Visit).ToArray();
                                    return Expression.Call(newObject, newMethod, newArgs);
                                }}".ConvertToStatements());
                            
                        });

                        method.AddReturn("base.VisitMethodCall(node)");
                    });

                    @class.AddMethod("Expression", "VisitLambda", method =>
                    {
                        method.Protected().Override();
                        method.AddGenericParameter("T");
                        method.AddParameter("Expression<T>", "node");

                        method.AddStatements($@"var newParameters = node.Parameters.Select(p =>
                            p.Type == _fromType ? Expression.Parameter(_toType, p.Name) : p).ToArray();

                        if (newParameters.SequenceEqual(node.Parameters))
                        {{
                            return base.VisitLambda(node);
                        }}

                        var parameterMap = node.Parameters.Zip(newParameters, (old, @new) => new {{ old, @new }})
                            .ToDictionary(x => x.old, x => x.@new);

                        var visitor = new ParameterReplacementVisitor(parameterMap);
                        var newBody = visitor.Visit(node.Body);

                        // Create new lambda with correct delegate type
                        var delegateType = typeof(T);
                        if (delegateType.IsGenericType)
                        {{
                            var genericArgs = delegateType.GetGenericArguments();
                            var newGenericArgs = genericArgs.Select(arg => arg == _fromType ? _toType : arg).ToArray();

                            if (!newGenericArgs.SequenceEqual(genericArgs))
                            {{
                                var genericDefinition = delegateType.GetGenericTypeDefinition();
                                delegateType = genericDefinition.MakeGenericType(newGenericArgs);
                            }}
                        }}

                        return Expression.Lambda(delegateType, newBody, newParameters);".ConvertToStatements());
                    });
                }).AddClass("ParameterReplacementVisitor", @class =>
                {
                    @class.Internal();

                    @class.WithBaseType("ExpressionVisitor");

                    @class.AddConstructor(c =>
                    {
                        c.AddParameter("Dictionary<ParameterExpression, ParameterExpression>", "parameterMap", p => p.IntroduceReadonlyField());
                    });

                    @class.AddMethod("Expression", "VisitParameter", method =>
                    {
                        method.Protected().Override();
                        method.AddParameter("ParameterExpression", "node");

                        method.AddReturn("_parameterMap.TryGetValue(node, out var replacement) ? replacement : node");
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