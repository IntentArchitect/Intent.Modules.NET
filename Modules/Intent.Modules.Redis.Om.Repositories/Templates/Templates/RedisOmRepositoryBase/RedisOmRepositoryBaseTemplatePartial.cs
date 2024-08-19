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

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmRepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RedisOmRepositoryBaseTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.RedisOmRepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RedisOmRepositoryBaseTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.RedisOM(OutputTarget));

            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Reflection")
                .AddClass("RedisOmRepositoryBase", @class =>
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

                    var selectGenericTypeArgument = createEntityInterfaces
                        ? $"<{tDocument}, {tDomain}>"
                        : string.Empty;

                    @class
                        .ImplementsInterface($"{this.GetRedisOmRepositoryInterfaceName()}<{tDomain}, {tDocumentInterface}>")
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
                            .AddType($"{this.GetRedisOmDocumentOfTInterfaceName()}<{tDomain}{tDomainStateConstraint}, {tDocument}>")
                            .AddType(tDocumentInterface)
                            .AddType("new()"))
                        ;

                    @class.AddConstructor(ctor =>
                    {
                        ctor.Protected();
                        ctor.AddParameter(this.GetRedisOmUnitOfWorkName(), "unitOfWork", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(UseType($"Redis.OM.RedisConnectionProvider"), "connectionProvider", param => param.IntroduceReadonlyField());
                        ctor.AddStatement($@"_collection = ({UseType($"Redis.OM.Searching.RedisCollection<{tDocument}>")})connectionProvider.RedisCollection<{tDocument}>(500);");
                        ctor.AddStatement($@"_collectionName = _collection.StateManager.DocumentAttribute.Prefixes.FirstOrDefault()
                              ?? throw new Exception($""{{typeof({tDocument}).FullName}} does not have a Document Prefix assigned."");");
                    });

                    @class.AddField(UseType($"Redis.OM.Searching.RedisCollection<{tDocument}>"), "_collection", field => field.PrivateReadOnly());
                    @class.AddField("string" + GetNullablePostfix(true), "_collectionName", field => field.PrivateReadOnly());

                    @class.AddProperty(this.GetRedisOmUnitOfWorkInterfaceName(), "UnitOfWork", p => p
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
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);",
                                        c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement("await _collection.InsertAsync(document);")
                                    .AddStatement("SetIdValue(entity, document);")
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
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);",
                                        c => c.AddMetadata(MetadataNames.DocumentDeclarationStatement, true))
                                    .AddStatement("await _collection.UpdateAsync(document);")
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
                                    .AddStatement($@"await _connectionProvider.Connection.UnlinkAsync($""{{_collectionName}}:{{GetIdValue(entity)}}"");")
                                );
                        })
                    );

                    @class.AddMethod($"Task<{tDomain}{GetNullablePostfix(true)}>", "FindAsync", m => m
                        .Async()
                        .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                        .AddOptionalCancellationTokenParameter(this)
                        .AddStatement("var document = await _collection.Where(AdaptFilterPredicate(filterExpression)).FirstOrDefaultAsync();")
                        .AddIfStatement("document is null", stmt => { stmt.AddStatement("return null;"); })
                        .AddStatement("var entity = document.ToEntity();")
                        .AddStatement("Track(entity);")
                        .AddStatement("return entity;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", m => m
                        .Async()
                        .AddOptionalCancellationTokenParameter(this)
                        .AddStatement("var documents = await _collection.ToListAsync();",
                            c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                        .AddStatement($"var results = documents.Select{selectGenericTypeArgument}(document => document.ToEntity()).ToList();")
                        .AddStatement("Track(results);")
                        .AddStatement("return results;", s => s.SeparatedFromPrevious())
                    );


                    @class.AddMethod($"Task<{tDomain}{GetNullablePostfix(true)}>", "FindByIdAsync", m => m
                        .Async()
                        .AddParameter("string", "id")
                        .AddOptionalCancellationTokenParameter(this)
                        .AddStatement("var document = await _collection.FindByIdAsync(id);")
                        .AddIfStatement("document is null", stmt => { stmt.AddStatement("return null;"); })
                        .AddStatement("var entity = document.ToEntity();")
                        .AddStatement("Track(entity);")
                        .AddStatement("return entity;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddOptionalCancellationTokenParameter(this);

                        method
                            .AddStatement(
                                "var documents = await _collection.Where(AdaptFilterPredicate(filterExpression)).ToListAsync();"
                                , c => c.AddMetadata(MetadataNames.DocumentsDeclarationStatement, true))
                            .AddStatement($"var results = documents.Select{selectGenericTypeArgument}(document => document.ToEntity()).ToList();")
                            .AddStatement("Track(results);")
                            .AddStatement("return results;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<{this.GetPagedListInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddOptionalCancellationTokenParameter(this);

                        var tDomainStateGenericTypeArgument = createEntityInterfaces
                            ? $", {tDomainState}"
                            : string.Empty;

                        method.AddStatements(
                            """
                            var query = _collection;
                            var count = await query.CountAsync().ConfigureAwait(false);
                            var skip = ((pageNo - 1) * pageSize);
                            """);

                        method.AddMethodChainStatement("var pagedDocuments = await query", chain => chain
                            .AddChainStatement("Skip(skip)")
                            .AddChainStatement("Take(pageSize)")
                            .AddChainStatement("ToListAsync()"));

                        method.AddStatements(
                            $$"""
                              var results = pagedDocuments.Select(document => document.ToEntity()).ToList();
                              Track(results);

                              return new {{this.GetRedisOmPagedListName()}}<{{tDomain}}{{tDomainStateGenericTypeArgument}}, {{tDocument}}>(count, pageNo, pageSize, results);
                              """);
                    });

                    @class.AddMethod($"Task<{this.GetPagedListInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddOptionalCancellationTokenParameter(this);

                        var tDomainStateGenericTypeArgument = createEntityInterfaces
                            ? $", {tDomainState}"
                            : string.Empty;

                        method.AddStatements(
                            """
                            var query = _collection.Where(AdaptFilterPredicate(filterExpression));
                            var count = await query.CountAsync().ConfigureAwait(false);
                            var skip = ((pageNo - 1) * pageSize);
                            """);

                        method.AddMethodChainStatement("var pagedDocuments = await query", chain => chain
                            .AddChainStatement("Skip(skip)")
                            .AddChainStatement("Take(pageSize)")
                            .AddChainStatement("ToListAsync()"));

                        method.AddStatements(
                            $$"""
                              var results = pagedDocuments.Select(document => document.ToEntity()).ToList();
                              Track(results);

                              return new {{this.GetRedisOmPagedListName()}}<{{tDomain}}{{tDomainStateGenericTypeArgument}}, {{tDocument}}>(count, pageNo, pageSize, results);
                              """);
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindByIdsAsync", m => m
                        .AddParameter("IEnumerable<string>", "ids")
                        .Async()
                        .AddOptionalCancellationTokenParameter(this)
                        .AddStatement("var documents = await _collection.FindByIdsAsync(ids);")
                        .AddMethodChainStatement("var results = documents", chain => chain
                            .AddChainStatement("Where(p => p.Value is not null)")
                            .AddChainStatement("Select(document => document.Value!.ToEntity())")
                            .AddChainStatement("ToList()"))
                        .AddStatement("return results;")
                    );

                    @class.AddMethod("string", "GetIdValue", method => method
                        .Protected().Abstract()
                        .AddParameter(tDomain, "entity"));

                    @class.AddMethod("void", "SetIdValue", method => method
                        .Protected().Abstract()
                        .AddParameter(tDomain, "domainEntity")
                        .AddParameter(tDocument, "document"));

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

                            method.AddStatement(
                                """
                                if (node == _before)
                                {
                                    return _after;
                                }

                                if (node?.NodeType == ExpressionType.MemberAccess &&
                                    node is MemberExpression mem &&
                                    mem.Member.ReflectedType == _before.Type)
                                {
                                    var newExpression = Visit(mem.Expression);
                                    return Expression.MakeMemberAccess(newExpression, _after.Type.GetMember(mem.Member.Name, BindingFlags.Instance | BindingFlags.Public).First());
                                }

                                return base.Visit(node);
                                """);
                        });
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            this.ApplyConnectionString(Constants.RedisConnectionStringName, "localhost:6379");

            ExecutionContext.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.Redis.Name)
                .WithProperty(Infrastructure.Redis.Property.ConnectionStringName, Constants.RedisConnectionStringName));
        }

        private string GetNullablePostfix(bool isNullable)
        {
            return isNullable && OutputTarget.GetProject().NullableEnabled ? "?" : "";
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