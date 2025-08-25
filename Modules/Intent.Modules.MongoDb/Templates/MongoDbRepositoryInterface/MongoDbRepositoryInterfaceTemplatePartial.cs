using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.MongoDb.Templates.MongoDbDocumentInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.MongoDbRepositoryInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbRepositoryInterfaceTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoDbRepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbRepositoryInterfaceTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Linq")
                .AddUsing("System.Collections.Generic")
                .AddInterface($"IMongoRepository", @interface =>
                {

                    var pagedListInterface = TryGetTypeName(TemplateRoles.Repository.Interface.PagedList, out var name)
                        ? name : GetTypeName(TemplateRoles.Repository.Interface.PagedResult); // for backward compatibility

                    @interface
                        .AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TDocumentInterface", out var tDocumentInterface)
                        .AddGenericParameter("TIdentifier", out var tIdentifier);

                    @interface
                        .ImplementsInterfaces($"{this.GetRepositoryInterfaceName()}<{tDomain}>")
                        .AddProperty(this.GetMongoDbUnitOfWorkInterfaceName(), "UnitOfWork", property => property
                            .WithoutSetter()
                        )
                        .AddMethod($"Task<{tDomain}?>", "FindAsync", method => method
                            .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<{tDomain}?>", "FindAsync", method => method
                            .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter("CancellationToken", "cancellationToken",
                                parameter => parameter.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<{pagedListInterface}<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"int", "pageNo")
                            .AddParameter($"int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken",
                                parameter => parameter.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<{pagedListInterface}<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter($"int", "pageNo")
                            .AddParameter($"int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken",
                                parameter => parameter.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<{pagedListInterface}<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter($"int", "pageNo")
                            .AddParameter($"int", "pageSize")
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken",
                                parameter => parameter.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<{tDomain}?>", "FindAsync", method => method
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "queryOptions")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "queryOptions")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<{pagedListInterface}<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>", "queryOptions")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod("Task<int>", "CountAsync", method => method
                            .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod("Task<int>", "CountAsync", method => method
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>?", "queryOptions", param => param.WithDefaultValue("default"))
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod("Task<bool>", "AnyAsync", method => method
                            .AddParameter($"Func<IQueryable<{tDocumentInterface}>, IQueryable<{tDocumentInterface}>>?", "queryOptions", param => param.WithDefaultValue("default"))
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod("Task<bool>", "AnyAsync", method => method
                            .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        ).AddMethod($"Task<{tDomain}>", "FindByIdAsync", method => method
                            .AddParameter($"{tIdentifier}", "id")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        ).AddMethod($"Task<List<{tDomain}>>", "FindByIdsAsync", method => method
                            .AddParameter($"{tIdentifier}[]", "ids")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        ).AddMethod($"List<{tDomain}>", "SearchText", method => method
                            .AddParameter($"string", "searchText")
                            .AddParameter($"Expression<Func<{tDocumentInterface}, bool>>?", "filterExpression", c => c.WithDefaultValue("null"))
                        );
                });
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            foreach (var model in Model)
            {
                if (TryGetTemplate<ICSharpFileBuilderTemplate>(EntityRepositoryInterfaceTemplate.TemplateId, model, out var template))
                {
                    if(model.GetPrimaryKeyAttribute() == null)
                    {
                        return;
                    }
                    template.CSharpFile.OnBuild(file => file.Metadata["entity-state-template-id"] = MongoDbDocumentInterfaceTemplate.TemplateId);
                    template.CSharpFile.AfterBuild(file =>
                    {
                        file.AddUsing("System.Linq.Expressions");
                        var @interface = file.Interfaces.Single();
                        @interface.Interfaces.Clear();
                        var genericTypeParameters = model.GenericTypes.Any()
                            ? $"<{string.Join(", ", model.GenericTypes)}>"
                            : string.Empty;
                        var tDomainGenericArgument = template.GetTypeName(TemplateRoles.Domain.Entity.Interface, model);
                        var tDocumentInterfaceGenericArgument = template.GetTypeName(MongoDbDocumentInterfaceTemplate.TemplateId, model);
                        @interface.ImplementsInterfaces($"{this.GetMongoDbRepositoryInterfaceName()}<{tDomainGenericArgument}{genericTypeParameters}, {tDocumentInterfaceGenericArgument}{genericTypeParameters}, {GetTypeName(model.GetPrimaryKeyAttribute())}>");
                        @interface.Methods.Clear();
                        //@interface.AddMethod($"List<{tDomainGenericArgument}>", "SearchText", method =>
                        //{
                        //    method.AddParameter($"string", "searchText");
                        //    method.AddParameter($"Expression<Func<{tDomainGenericArgument}, bool>>", "filterExpression", p => p.WithDefaultValue("null"));
                        //});
                    }, 1000);
                }
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