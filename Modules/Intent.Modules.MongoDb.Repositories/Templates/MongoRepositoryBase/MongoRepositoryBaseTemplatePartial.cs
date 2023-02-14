using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.MongoDb.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Repositories.Templates.MongoRepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
    public partial class MongoRepositoryBaseTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.Repositories.MongoRepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoRepositoryBaseTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MongoDbDataUnitOfWork);
            
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("MongoDB.Driver")
                .AddUsing("MongoDB.Driver.Linq")
                .AddClass($"MongoRepositoryBase", @class =>
                {
                    @class.Abstract();
                    @class.AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TPersistence", out var tPersistence);
                    @class.WithBaseType($"MongoDbRepository<{tPersistence}>")
                        .ImplementsInterface($"IRepository<{tDomain} ,{tPersistence}>");
                    @class.AddGenericTypeConstraint(tPersistence, p => p.AddType("class").AddType(tDomain))
                        .AddGenericTypeConstraint(tDomain, p => p.AddType("class"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetApplicationMongoDbContextName(), "context")
                            .CallsBase(b => b.AddArgument("context"));
                        ctor.AddStatement("UnitOfWork = context;");
                    });

                    @class.AddProperty(this.GetUnitOfWorkInterfaceName(), "UnitOfWork", prop => prop.ReadOnly());

                    @class.AddMethod("void", "Add", method =>
                    {
                        method.Virtual();
                        method.AddParameter(tDomain, "entity");
                        method.AddStatement($"base.InsertOne(({tPersistence})entity);");
                    });
                    @class.AddMethod("void", "Remove", method =>
                    {
                        method.Abstract();
                        method.AddParameter(tDomain, "entity");
                    });
                    @class.AddMethod($"object", "Update", method =>
                    {
                        method.Virtual();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "predicate")
                            .AddParameter(tDomain, "entity");
                        method.AddStatement($"return base.UpdateOne(predicate, (TPersistence)entity, null);");
                    });

                    @class.AddMethod($"Task<{tDomain}>", "FindAsync", method =>
                    {
                        method.Virtual();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return FindAsync(filterExpression, null, cancellationToken);");
                    });
                    @class.AddMethod($"Task<{tDomain}>", "FindAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"IMongoQueryable<{tPersistence}> query = Context.GetCollection<{tPersistence}>().AsQueryable();")
                            .AddStatement(new CSharpStatementBlock("if (filterExpression != null)")
                                .AddStatement("query = query.Where(filterExpression);"))
                            .AddStatement(new CSharpStatementBlock("if (linq != null)")
                                .AddStatement($"query = (IMongoQueryable<{tPersistence}>)linq(query);"))
                            .AddStatement("return await query.FirstOrDefaultAsync(cancellationToken);");
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return FindAllAsync(null, cancellationToken);");
                    });
                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return FindAllAsync(filterExpression, null, cancellationToken);");
                    });
                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"IMongoQueryable<{tPersistence}> query = Context.GetCollection<{tPersistence}>().AsQueryable();")
                            .AddStatement(new CSharpStatementBlock("if (filterExpression != null)")
                                .AddStatement("query = query.Where(filterExpression);"))
                            .AddStatement(new CSharpStatementBlock("if (linq != null)")
                                .AddStatement($"query = (IMongoQueryable<{tPersistence}>)linq(query);"))
                            .AddStatement($"return (await query.ToListAsync(cancellationToken)).Cast<{tDomain}>().ToList();");
                    });

                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return FindAllAsync(null, pageNo, pageSize, cancellationToken);");
                    });
                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return FindAllAsync(filterExpression, pageNo, pageSize, null, cancellationToken);");
                    });
                    @class.AddMethod($"Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"IMongoQueryable<{tPersistence}> query = Context.GetCollection<{tPersistence}>().AsQueryable();")
                            .AddStatement(new CSharpStatementBlock("if (filterExpression != null)")
                                .AddStatement("query = query.Where(filterExpression);"))
                            .AddStatement(new CSharpStatementBlock("if (linq != null)")
                                .AddStatement($"query = (IMongoQueryable<{tPersistence}>)linq(query);"));
                        method.AddStatement(new CSharpInvocationStatement($"return await {this.GetMongoPagedListName()}<{tPersistence}>.CreateAsync")
                                .AddArgument("query")
                                .AddArgument("pageNo")
                                .AddArgument("pageSize")
                                .AddArgument("cancellationToken")
                                .WithArgumentsOnNewLines());
                    });
                });
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