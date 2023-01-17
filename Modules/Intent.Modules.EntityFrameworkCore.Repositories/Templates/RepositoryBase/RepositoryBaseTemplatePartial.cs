using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.PagedList;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.RepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class RepositoryBaseTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.Repositories.RepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryBaseTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.EntityFrameworkCore")
                .AddClass($"RepositoryBase", @class =>
                {
                    @class.AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TPersistence", out var tPersistence)
                        .AddGenericParameter("TDbContext", out var tDbContext);
                    @class.ImplementsInterface($"IRepository<{tDomain}, {tPersistence}>");
                    @class.AddGenericTypeConstraint(tDbContext, constr => constr
                        .AddType(UseType("Microsoft.EntityFrameworkCore.DbContext"))
                        .AddType(this.GetUnitOfWorkInterfaceName()));
                    @class.AddGenericTypeConstraint(tPersistence, constr => constr
                        .AddType("class")
                        .AddType(tDomain));
                    @class.AddGenericTypeConstraint(tDomain, constr => constr.AddType("class"));
                    @class.AddField(tDbContext, "_dbContext", field => field.PrivateReadOnly());
                    @class.AddConstructor(ctor => ctor
                        .AddParameter(tDbContext, "dbContext")
                        .AddStatement($"_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));"));
                    @class.AddProperty(this.GetUnitOfWorkInterfaceName(), "UnitOfWork", prop =>
                    {
                        prop.ReadOnly();
                        prop.Getter.WithExpressionImplementation("_dbContext");
                    });
                    @class.AddMethod("void", "Remove", method =>
                    {
                        method.Virtual();
                        method.AddParameter(tDomain, "entity");
                        method.AddStatement($"GetSet().Remove(({tPersistence})entity);");
                    });
                    @class.AddMethod("void", "Add", method =>
                    {
                        method.Virtual();
                        method.AddParameter(tDomain, "entity");
                        method.AddStatement($"GetSet().Add(({tPersistence})entity);");
                    });

                    @class.AddMethod($"Task<{tDomain}>", "FindAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(filterExpression).SingleOrDefaultAsync<{tDomain}>(cancellationToken);");
                    });
                    @class.AddMethod($"Task<{tDomain}>", "FindAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(filterExpression, linq).SingleOrDefaultAsync<{tDomain}>(cancellationToken);");
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(x => true).ToListAsync<{tDomain}>(cancellationToken);");
                    });
                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(filterExpression).ToListAsync<{tDomain}>(cancellationToken);");
                    });
                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(filterExpression, linq).ToListAsync<{tDomain}>(cancellationToken);");
                    });

                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"var query = QueryInternal(x => true);")
                            .AddStatement(new CSharpInvocationStatement($"return await {this.GetPagedListName()}<{tDomain}>.CreateAsync")
                                .AddArgument("query")
                                .AddArgument("pageNo")
                                .AddArgument("pageSize")
                                .AddArgument("cancellationToken")
                                .WithArgumentsOnNewLines());
                    });
                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>","filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"var query = QueryInternal(filterExpression);")
                            .AddStatement(new CSharpInvocationStatement($"return await {this.GetPagedListName()}<{tDomain}>.CreateAsync")
                                .AddArgument("query")
                                .AddArgument("pageNo")
                                .AddArgument("pageSize")
                                .AddArgument("cancellationToken")
                                .WithArgumentsOnNewLines());
                    });
                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>","filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"var query = QueryInternal(filterExpression, linq);")
                            .AddStatement(new CSharpInvocationStatement($"return await {this.GetPagedListName()}<{tDomain}>.CreateAsync")
                                .AddArgument("query")
                                .AddArgument("pageNo")
                                .AddArgument("pageSize")
                                .AddArgument("cancellationToken")
                                .WithArgumentsOnNewLines());
                    });

                    @class.AddMethod("Task<int>", "CountAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(filterExpression).CountAsync(cancellationToken);");
                    });
                    @class.AddMethod("bool", "Any", method =>
                    {
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression");
                        method.AddStatement($"return QueryInternal(filterExpression).Any();");
                    });
                    @class.AddMethod("Task<bool>", "AnyAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(filterExpression).AnyAsync(cancellationToken);");
                    });

                    @class.AddMethod($"IQueryable<{tPersistence}>", "QueryInternal", method =>
                    {
                        method.Protected();
                        method.Virtual();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression");
                        method.AddStatement("var queryable = CreateQuery();")
                            .AddStatement("if (filterExpression != null)")
                            .AddStatement(new CSharpStatementBlock()
                                .AddStatement("queryable = queryable.Where(filterExpression);"))
                            .AddStatement("return queryable;")
                            ;
                    });
                    @class.AddMethod($"IQueryable<TResult>", "QueryInternal", method =>
                    {
                        method.Protected();
                        method.Virtual();
                        method.AddGenericParameter("TResult", out var tResult);
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tResult}>>", "linq");
                        method.AddStatement("var queryable = CreateQuery();")
                            .AddStatement("queryable = queryable.Where(filterExpression);")
                            .AddStatement("var result = linq(queryable);")
                            .AddStatement("return result;")
                            ;
                    });

                    @class.AddMethod($"IQueryable<{tPersistence}>", "CreateQuery", method =>
                    {
                        method.Protected();
                        method.Virtual();
                        method.AddStatement("return GetSet();");
                    });
                    @class.AddMethod($"DbSet<{tPersistence}>", "GetSet", method =>
                    {
                        method.Protected();
                        method.Virtual();
                        method.AddStatement($"return _dbContext.Set<{tPersistence}>();");
                    });

                    @class.AddMethod($"Task<int>", "SaveChangesAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement("return await _dbContext.SaveChangesAsync(cancellationToken);");
                    });
                });
        }

        public string RepositoryInterfaceName => GetTypeName(RepositoryInterfaceTemplate.TemplateId);
        public string PagedListClassName => GetTypeName(PagedListTemplate.TemplateId);

        [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }

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