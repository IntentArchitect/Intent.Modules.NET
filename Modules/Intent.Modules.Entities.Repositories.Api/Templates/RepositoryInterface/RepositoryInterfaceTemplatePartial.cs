using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge)]
    public partial class RepositoryInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Entities.Repositories.Api.RepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryInterfaceTemplate(IOutputTarget outputTarget, object model = null)
            : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface("IRepository", @interface =>
                {
                    @interface.AddAttribute("[IntentManaged(Mode.Merge, Signature = Mode.Fully)]");
                    @interface.AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TPersistence", out var tPersistence)
                        .AddMethod("void", "Add", method =>
                        {
                            method.AddParameter(tDomain, "entity");
                        })
                        .AddMethod("void", "Remove", method =>
                        {
                            method.AddParameter(tDomain, "entity");
                        })
                        .AddMethod($"Task<{tDomain}>", "FindAsync", method =>
                        {
                            method.AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        })
                        .AddMethod($"Task<{tDomain}>", "FindAsync", method =>
                        {
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                                .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        })
                        .AddMethod($"Task<{tDomain}>", "FindAsync", method =>
                        {
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                                .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                                .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        })
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                        {
                            method.AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        })
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                        {
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                                .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        })
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                        {
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                                .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                                .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        })
                        .AddMethod($"Task<IPagedResult<{tDomain}>>", "FindAllAsync", method =>
                        {
                            method.AddParameter("int", "pageNo")
                                .AddParameter("int", "pageSize")
                                .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        })
                        .AddMethod($"Task<IPagedResult<{tDomain}>>", "FindAllAsync", method =>
                        {
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                                .AddParameter("int", "pageNo")
                                .AddParameter("int", "pageSize")
                                .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        })
                        .AddMethod($"Task<IPagedResult<{tDomain}>>", "FindAllAsync", method =>
                        {
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                                .AddParameter("int", "pageNo")
                                .AddParameter("int", "pageSize")
                                .AddParameter($"Func<IQueryable<{tPersistence}>, IQueryable<{tPersistence}>>", "linq")
                                .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        })
                        .AddMethod("Task<int>", "CountAsync", method =>
                        {
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                                .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        })
                        .AddMethod("Task<bool>", "AnyAsync", method =>
                        {
                            method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                                .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        })
                        .AddProperty(this.GetUnitOfWorkInterfaceName(), "UnitOfWork", prop =>
                        {
                            prop.ReadOnly();
                        });
                });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public CSharpFile CSharpFile { get; }
    }
}
