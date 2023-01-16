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
                    
                });
        }

        public string RepositoryInterfaceName => GetTypeName(RepositoryInterfaceTemplate.TemplateId);
        public string PagedListClassName => GetTypeName(PagedListTemplate.TemplateId);

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
