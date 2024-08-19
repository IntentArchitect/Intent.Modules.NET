using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Templates.MultiTenantStoreDbContext
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MultiTenantStoreDbContextTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Modules.AspNetCore.MultiTenancy.MultiTenantStoreDbContext";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public MultiTenantStoreDbContextTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.FinbuckleMultiTenantEntityFrameworkCore(outputTarget));
            AddNugetDependency(NugetPackages.MicrosoftEntityFrameworkCoreInMemory(OutputTarget));
            AddNugetDependency(NugetPackages.FinbuckleMultiTenantAspNetCore(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Finbuckle.MultiTenant")
                .AddUsing("Finbuckle.MultiTenant.Stores")
                .AddUsing("Microsoft.EntityFrameworkCore")
                .AddClass("MultiTenantStoreDbContext", @class => @class
                    .WithBaseType("EFCoreStoreDbContext<TenantInfo>")
                    .AddConstructor(constructor => constructor
                        .AddParameter("DbContextOptions<MultiTenantStoreDbContext>", "options")
                        .CallsBase(c => c.AddArgument("options"))
                    )
                    .AddMethod("void", "OnConfiguring", method => method
                        .Protected()
                        .Override()
                        .AddParameter("DbContextOptionsBuilder", "optionsBuilder")
                        .AddStatement("// Use InMemory, but could be MsSql, Sqlite, MySql, etc...")
                        .AddStatement("optionsBuilder.UseInMemoryDatabase(\"MultiTenancyConnection\");")
                        .AddStatement("base.OnConfiguring(optionsBuilder);")
                    )
                );
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig() => CSharpFile.GetConfig();

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}