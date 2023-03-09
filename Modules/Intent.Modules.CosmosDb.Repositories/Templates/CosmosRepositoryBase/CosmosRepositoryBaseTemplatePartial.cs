using System.Collections.Generic;
using Intent.CosmosDb;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.CosmosDb.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDb.Repositories.Templates.CosmosRepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
    public partial class CosmosRepositoryBaseTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDb.Repositories.CosmosRepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosRepositoryBaseTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftAzureCosmos);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Azure.Cosmos")
                .AddUsing("Intent.Modules.CosmosDb.Repository")
                .AddClass($"CosmosRepositoryBase", @class =>
                {
                    @class.Abstract();
                    @class.AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TPersistence", out var tPersistence);
                    @class.WithBaseType($"CosmosDbRepository<{tPersistence}>")
                        .ImplementsInterface($"IRepository<{tDomain} ,{tPersistence}>");
                    @class.AddGenericTypeConstraint(tPersistence, p => p.AddType("class").AddType(tDomain))
                        .AddGenericTypeConstraint(tDomain, p => p.AddType("class"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetApplicationCosmosDbContextName(), "context")
                            .CallsBase(b => b.AddArgument("context"));
                        ctor.AddStatement("UnitOfWork = context;");
                    });

                    @class.AddProperty(this.GetUnitOfWorkInterfaceName(), "UnitOfWork", prop => prop.ReadOnly());

                    @class.AddMethod("Task", "Add", method =>
                    {
                        method.Virtual();
                        method.AddParameter(tDomain, "entity");
                        method.AddStatement($@"
                            var container = Context.GetContainer<{tPersistence}>();
                            var response = await container.CreateItemAsync(({tPersistence})entity);
                            if (response.StatusCode != HttpStatusCode.Created)
                                throw new Exception($""Failed to insert document. StatusCode={{response.StatusCode}}"");
                        ");
                    });

                    @class.AddMethod("void", "Remove", method =>
                    {
                        method.Abstract();
                        method.AddParameter(tDomain, "entity");
                    });
                    @class.AddMethod($"Task<{tDomain}>", "Update", method =>
                    {
                        method.Virtual();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "predicate")
                            .AddParameter(tDomain, "entity");
                        method.AddStatement($@"var id = ((dynamic)entity).Id.ToString();
                            var existing = await GetAsync(id);
                            if(existing == null)
                                throw new KeyNotFoundException($""Could not find {tDomain} with ID {{id}}"");
                            var result = await Context.ReplaceItemAsync<{tPersistence}>(id, ({tPersistence})entity);
                            return ({tDomain})result.Resource;");
                    });


                    @class.AddMethod($"Task<{tDomain}>", "FindAsync", method =>
                    {
                        method.Virtual();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return Context.GetContainer<{tPersistence}>().GetItemLinqQueryable<{tPersistence}>().Where(filterExpression).ToFeedIterator().ReadNextAsync(cancellationToken).ContinueWith(task => task.Result.FirstOrDefault()).Unwrap();");
                    });

                    @class.AddMethod($"Task<{tDomain}>", "FindAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddStatement($"var container = Context.CosmosDatabase.GetContainer(\"{tPersistence}\");")
                            .AddStatement($"var query = container.GetItemLinqQueryable<{tPersistence}>();")
                            .AddStatement(new CSharpStatementBlock("if (filterExpression != null)")
                                .AddStatement("query = query.Where(filterExpression);"))
                            .AddStatement(new CSharpStatementBlock("if (linq != null)")
                                .AddStatement($"query = linq(query);"))
                            .AddStatement("return await query.FirstOrDefaultAsync(cancellationToken);");
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddStatement($"var container = Context.CosmosDatabase.GetContainer(\"{tPersistence}\");")
                            .AddStatement($"var query = container.GetItemLinqQueryable<{tPersistence}>();")
                            .AddStatement($"return await query.ToListAsync(cancellationToken);");
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddStatement($"var container = Context.CosmosDatabase.GetContainer(\"{tPersistence}\");")
                            .AddStatement($"var query = container.GetItemLinqQueryable<{tPersistence}>();")
                            .AddStatement(new CSharpStatementBlock("if (filterExpression != null)")
                                .AddStatement("query = query.Where(filterExpression);"))
                            .AddStatement($"return await query.ToListAsync(cancellationToken);");
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddStatement($"var container = Context.CosmosDatabase.GetContainer(\"{tPersistence}\");")
                            .AddStatement($"var query = container.GetItemLinqQueryable<{tPersistence}>();")
                            .AddStatement(new CSharpStatementBlock("if (filterExpression != null)")
                                .AddStatement("query = query.Where(filterExpression);"))
                            .AddStatement(new CSharpStatementBlock("if (linq != null)")
                                .AddStatement($"query = linq(query);"))
                            .AddStatement($"var results = await query.ToListAsync(cancellationToken);")
                            .AddStatement($"return results.Select(x => ({tDomain})x).ToList();");
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
                    @class.AddMethod($"async Task<{this.GetPagedResultInterfaceName()}<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method.AddStatement($"var container = Context.CosmosDatabase.GetContainer(\"{tPersistence}\");")
                            .AddStatement($"var query = container.GetItemLinqQueryable<{tPersistence}>();")
                            .AddStatement(new CSharpStatementBlock("if (filterExpression != null)")
                                .AddStatement("query = query.Where(filterExpression);"))
                            .AddStatement(new CSharpStatementBlock("if (linq != null)")
                                .AddStatement($"query = linq(query);"))
                            .AddStatement($"return await {this.GetTypeName(TemplateFulfillingRoles.Repository.PagedList)}<{tDomain}>.CreateAsync(query, pageNo, pageSize, cancellationToken);");
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