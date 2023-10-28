using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.EFRepositoryInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EFRepositoryInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EFRepositoryInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"IEFRepository", @interface =>
                {
                    @interface
                        .AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TPersistence", out var tPersistence)
                        .ExtendsInterface($"{this.GetRepositoryInterfaceName()}<{tDomain}>")
                        .AddMethod($"Task<{tDomain}?>", "FindAsync", method => method
                            .AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<{tDomain}?>", "FindAsync", method => method
                            .AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<IPagedResult<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<IPagedResult<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task<IPagedResult<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("int", "pageNo")
                            .AddParameter("int", "pageSize")
                            .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod("Task<int>", "CountAsync", method => method
                            .AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod("Task<bool>", "AnyAsync", method => method
                            .AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod("bool", "Any", method => method
                            .AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                        )
                        .AddProperty(this.GetUnitOfWorkInterfaceName(), "UnitOfWork", prop => prop
                            .ReadOnly()
                        )
                        ;
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