using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.RepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
    public partial class RepositoryBaseTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
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
                .AddClass($"RepositoryBase")
                .OnBuild(file =>
                {
                    string nullableChar = this.OutputTarget.GetProject().NullableEnabled ? "?" : "";

                    var @class = file.Classes.First();
                    @class.AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TPersistence", out var tPersistence)
                        .AddGenericParameter("TDbContext", out var tDbContext);
                    @class.ImplementsInterface($"{this.GetEFRepositoryInterfaceName()}<{tDomain}, {tPersistence}>");
                    @class.AddGenericTypeConstraint(tDbContext, constr => constr
                        .AddType(UseType("Microsoft.EntityFrameworkCore.DbContext"))
                        .AddType(this.GetUnitOfWorkInterfaceName()));
                    @class.AddGenericTypeConstraint(tPersistence, constr => constr
                        .AddType("class")
                        .AddType(tDomain));
                    @class.AddGenericTypeConstraint(tDomain, constr => constr.AddType("class"));
                    @class.AddField(tDbContext, "_dbContext", field => field.ProtectedReadOnly());
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
                    @class.AddMethod("void", "Update", method =>
                    {
                        method.Virtual();
                        method.AddParameter(tDomain, "entity");
                        method.AddStatement($"GetSet().Update(({tPersistence})entity);");
                    });

                    @class.AddMethod($"Task<{tDomain}{nullableChar}>", "FindAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(filterExpression).SingleOrDefaultAsync<{tDomain}>(cancellationToken);");
                    });
                    @class.AddMethod($"Task<{tDomain}{nullableChar}>", "FindAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "queryOptions")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(filterExpression, queryOptions).SingleOrDefaultAsync<{tDomain}>(cancellationToken);");
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
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "queryOptions")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(filterExpression, queryOptions).ToListAsync<{tDomain}>(cancellationToken);");
                    });

                    @class.AddMethod($"Task<{PagedListInterfaceName}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"var query = QueryInternal(x => true);")
                            .AddStatement(new CSharpInvocationStatement($"return await ToPagedListAsync<TDomain>")
                                .AddArgument("query")
                                .AddArgument("pageNo")
                                .AddArgument("pageSize")
                                .AddArgument("cancellationToken")
                                .WithArgumentsOnNewLines());
                    });
                    @class.AddMethod($"Task<{PagedListInterfaceName}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"var query = QueryInternal(filterExpression);")
                            .AddStatement(new CSharpInvocationStatement($"return await ToPagedListAsync<TDomain>")
                                .AddArgument("query")
                                .AddArgument("pageNo")
                                .AddArgument("pageSize")
                                .AddArgument("cancellationToken")
                                .WithArgumentsOnNewLines());
                    });
                    @class.AddMethod($"Task<{PagedListInterfaceName}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "queryOptions")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"var query = QueryInternal(filterExpression, queryOptions);")
                            .AddStatement(new CSharpInvocationStatement($"return await ToPagedListAsync<TDomain>")
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

                    @class.AddMethod($"Task<{tDomain}{nullableChar}>", "FindAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>", "queryOptions")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(queryOptions).SingleOrDefaultAsync<{tDomain}>(cancellationToken);");
                    });
                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>", "queryOptions")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(queryOptions).ToListAsync<{tDomain}>(cancellationToken);");
                    });
                    @class.AddMethod($"Task<{PagedListInterfaceName}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>", "queryOptions")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"var query = QueryInternal(queryOptions);")
                            .AddStatement(new CSharpInvocationStatement($"return await ToPagedListAsync<TDomain>")
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
                        method.AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>{nullableChar}", "queryOptions", p => p.WithDefaultValue("default"))
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(queryOptions).CountAsync(cancellationToken);");
                    });
                    @class.AddMethod("Task<bool>", "AnyAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>{nullableChar}", "queryOptions", p => p.WithDefaultValue("default"))
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return await QueryInternal(queryOptions).AnyAsync(cancellationToken);");
                    });


                    if (ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                    {
                        @class.AddMethod($"{tDomain}{nullableChar}", "Find", method =>
                        {
                            method.Virtual();
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression");
                            method.AddStatement($"return QueryInternal(filterExpression).SingleOrDefault<{tDomain}>();");
                        });
                        @class.AddMethod($"{tDomain}{nullableChar}", "Find", method =>
                        {
                            method.Virtual();
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                                .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "queryOptions");
                            method.AddStatement($"return QueryInternal(filterExpression, queryOptions).SingleOrDefault<{tDomain}>();");
                        });

                        @class.AddMethod($"List<{tDomain}>", "FindAll", method =>
                        {
                            method.Virtual();
                            method.AddStatement($"return QueryInternal(x => true).ToList<{tDomain}>();");
                        });
                        @class.AddMethod($"List<{tDomain}>", "FindAll", method =>
                        {
                            method.Virtual();
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression");
                            method.AddStatement($"return QueryInternal(filterExpression).ToList<{tDomain}>();");
                        });
                        @class.AddMethod($"List<{tDomain}>", "FindAll", method =>
                        {
                            method.Virtual();
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                                .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "queryOptions");
                            method.AddStatement($"return QueryInternal(filterExpression, queryOptions).ToList<{tDomain}>();");
                        });

                        @class.AddMethod($"{PagedListInterfaceName}<{tDomain}>", "FindAll", method =>
                        {
                            method.Virtual();
                            method.AddParameter("int", "pageNo")
                                .AddParameter("int", "pageSize");
                            method.AddStatement($"var query = QueryInternal(x => true);")
                                .AddStatement(new CSharpInvocationStatement($"return ToPagedList<{tDomain}>")
                                    .AddArgument("query")
                                    .AddArgument("pageNo")
                                    .AddArgument("pageSize")
                                    .WithArgumentsOnNewLines());
                        });
                        @class.AddMethod($"{PagedListInterfaceName}<{tDomain}>", "FindAll", method =>
                        {
                            method.Virtual();
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                                .AddParameter("int", "pageNo")
                                .AddParameter("int", "pageSize");
                            method.AddStatement($"var query = QueryInternal(filterExpression);")
                                .AddStatement(new CSharpInvocationStatement($"return ToPagedList<{tDomain}>")
                                    .AddArgument("query")
                                    .AddArgument("pageNo")
                                    .AddArgument("pageSize")
                                    .WithArgumentsOnNewLines());
                        });

                        @class.AddMethod($"{PagedListInterfaceName}<{tDomain}>", "FindAll", method =>
                        {
                            method.Virtual();
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                                .AddParameter("int", "pageNo")
                                .AddParameter("int", "pageSize")
                                .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "queryOptions");
                            method.AddStatement($"var query = QueryInternal(filterExpression, queryOptions);")
                                .AddStatement(new CSharpInvocationStatement($"return ToPagedList<{tDomain}>")
                                    .AddArgument("query")
                                    .AddArgument("pageNo")
                                    .AddArgument("pageSize")
                                    .WithArgumentsOnNewLines());
                        });

                        @class.AddMethod("int", "Count", method =>
                        {
                            method.Virtual();
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression");
                            method.AddStatement($"return QueryInternal(filterExpression).Count();");
                        });

                        @class.AddMethod($"{tDomain}{nullableChar}", "Find", method =>
                        {
                            method.Virtual();
                            method.AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>", "queryOptions");
                            method.AddStatement($"return QueryInternal(queryOptions).SingleOrDefault<{tDomain}>();");
                        });
                        @class.AddMethod($"List<{tDomain}>", "FindAll", method =>
                        {
                            method.Virtual();
                            method.AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>", "queryOptions");
                            method.AddStatement($"return QueryInternal(queryOptions).ToList<{tDomain}>();");
                        });
                        @class.AddMethod($"{PagedListInterfaceName}<{tDomain}>", "FindAll", method =>
                        {
                            method.Virtual();
                            method
                                .AddParameter("int", "pageNo")
                                .AddParameter("int", "pageSize")
                                .AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>", "queryOptions")
                                ;
                            method.AddStatement($"var query = QueryInternal(queryOptions);")
                                .AddStatement(new CSharpInvocationStatement($"return ToPagedList<{tDomain}>")
                                    .AddArgument("query")
                                    .AddArgument("pageNo")
                                    .AddArgument("pageSize")
                                    .WithArgumentsOnNewLines());
                        });
                        @class.AddMethod("int", "Count", method =>
                        {
                            method.Virtual();
                            method.AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>{nullableChar}", "queryOptions", p => p.WithDefaultValue("default"));
                            method.AddStatement($"return QueryInternal(queryOptions).Count();");
                        });
                        @class.AddMethod("bool", "Any", method =>
                        {
                            method.Virtual();
                            method.AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>{nullableChar}", "queryOptions", p => p.WithDefaultValue("default"));
                            method.AddStatement($"return QueryInternal(queryOptions).Any();");
                        });

                        @class.AddMethod($"{PagedListInterfaceName}<T>", "ToPagedList<T>", method =>
                        {
                            method.Private().Static();
                            method.AddParameter($"IQueryable<T>", "queryable");
                            method.AddParameter($"int", "pageNo");
                            method.AddParameter($"int", "pageSize");
                            method.AddStatement("var count = queryable.Count();");
                            method.AddStatement("var skip = ((pageNo - 1) * pageSize);");
                            method.AddStatement(new CSharpMethodChainStatement("var results = queryable")
                                .AddChainStatement("Skip(skip)")
                                .AddChainStatement("Take(pageSize)")
                                .AddChainStatement("ToList()"));
                            method.AddStatement($"return new {PagedListClassName}<T>(count, pageNo, pageSize, results);");
                        });
                    }

                    @class.AddMethod($"IQueryable<{tPersistence}>", "QueryInternal", method =>
                    {
                        method.Protected();
                        method.Virtual();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>{nullableChar}", "filterExpression");
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
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tResult}>>", "queryOptions");
                        method.AddStatement("var queryable = CreateQuery();")
                            .AddStatement("queryable = queryable.Where(filterExpression);")
                            .AddStatement("var result = queryOptions(queryable);")
                            .AddStatement("return result;")
                            ;
                    });

                    @class.AddMethod($"IQueryable<{tPersistence}>", "QueryInternal", method =>
                    {
                        method.Protected();
                        method.Virtual();
                        method.AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>{nullableChar}", "queryOptions");
                        method.AddStatement("var queryable = CreateQuery();")
                            .AddStatement("if (queryOptions != null)")
                            .AddStatement(new CSharpStatementBlock()
                                .AddStatement("queryable = queryOptions(queryable);"))
                            .AddStatement("return queryable;")
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

                    @class.AddMethod($"{PagedListInterfaceName}<T>", "ToPagedListAsync<T>", method =>
                    {
                        method.Private().Static().Async();
                        method.AddParameter($"IQueryable<T>", "queryable");
                        method.AddParameter($"int", "pageNo");
                        method.AddParameter($"int", "pageSize");
                        method.AddParameter($"CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                        method.AddStatement("var count = await queryable.CountAsync(cancellationToken);");
                        method.AddStatement("var skip = ((pageNo - 1) * pageSize);");
                        method.AddStatement(new CSharpMethodChainStatement("var results = await queryable")
                            .AddChainStatement("Skip(skip)")
                            .AddChainStatement("Take(pageSize)")
                            .AddChainStatement("ToListAsync(cancellationToken)"));
                        method.AddStatement($"return new {PagedListClassName}<T>(count, pageNo, pageSize, results);");
                    });

                });
        }

        public string RepositoryInterfaceName => GetTypeName(RepositoryInterfaceTemplate.TemplateId);
        public string PagedListInterfaceName => TryGetTypeName(TemplateRoles.Repository.Interface.PagedList, out var name)
            ? name : GetTypeName(TemplateRoles.Repository.Interface.PagedResult); // for backward compatibility
        public string PagedListClassName => GetTypeName(TemplateRoles.Application.Common.PagedList);

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