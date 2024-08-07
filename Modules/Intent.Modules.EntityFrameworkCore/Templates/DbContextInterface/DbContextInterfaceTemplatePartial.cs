using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContextInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DbContextInterfaceTemplate : CSharpTemplateBase<DbContextInstance>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.DbContextInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DbContextInterfaceTemplate(IOutputTarget outputTarget, DbContextInstance model) : base(TemplateId, outputTarget, model)
        {
            if (IsEnabled)
            {
                AddNugetDependency(NugetPackages.MicrosoftEntityFrameworkCore(OutputTarget));
            }

            if (Model.IsApplicationDbContext)
            {
                FulfillsRole(TemplateRoles.Application.Common.DbContextInterface);
            }

            FulfillsRole(TemplateRoles.Application.Common.ConnectionStringDbContextInterface);

            // NOTE: This interface will get its DbSet fields injected into it from the DbContextTemplate
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.EntityFrameworkCore")
                .AddInterface($"I{Model.DbContextName}", @interface =>
                {
                    @interface.AddMethod("Task<int>", "SaveChangesAsync", method =>
                    {
                        method.AddOptionalCancellationTokenParameter(this);
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && IsEnabled;
        }

        public bool IsEnabled => ExecutionContext.Settings.GetDatabaseSettings().GenerateDbContextInterface()
                                 || !TryGetTemplate<ITemplate>(TemplateRoles.Domain.UnitOfWork, out _);

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