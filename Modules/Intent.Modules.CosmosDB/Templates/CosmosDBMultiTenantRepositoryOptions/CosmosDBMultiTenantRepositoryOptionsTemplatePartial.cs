using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBMultiTenantRepositoryOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBMultiTenantRepositoryOptionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBMultiTenantRepositoryOptions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBMultiTenantRepositoryOptionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpClass mainClass = null;
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Azure.CosmosRepository.Options")
                .AddUsing("Microsoft.Azure.CosmosRepository.Providers")
                .AddClass($"CosmosDBMultiTenantRepositoryOptions", @class =>
                {
                    mainClass = @class;
                    @class.WithBaseType("RepositoryOptions");
                    @class.AddField(this.GetCosmosDBMultiTenantClientProviderName(), "_clientProvider", f => f.PrivateReadOnly());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("ICosmosClientProvider", "clientProvider");
                        ctor.AddStatement($"_clientProvider = ({this.GetCosmosDBMultiTenantClientProviderName()})clientProvider;");
                    });
                    @class.AddProperty("string", "ContainerId", p =>
                    {
                        p.Override();
                        p.Getter.WithExpressionImplementation("_clientProvider.GetDefaultContainer()");
                        p.Setter.WithBodyImplementation("");
                    });
                    @class.AddProperty("string", "DatabaseId", p =>
                    {
                        p.Override();
                        p.Getter.WithExpressionImplementation("_clientProvider.GetDatabase()");
                        p.Setter.WithBodyImplementation("");
                    });
                });

            // CosmosConnectionString is added inside OnBuild (deferred to the Build phase, after all templates
            // across all modules are registered) because it needs GetTypeName to resolve
            // Intent.Modules.AspNetCore.MultiTenancy.TenantExtendedInfo (no ProjectReference to that module - cross-
            // module lookup by TemplateId). Base ITenantInfo/TenantInfo lost `ConnectionString` at Finbuckle 9.x, so
            // `_clientProvider.Tenant` (typed ITenantInfo?) must be cast to the app's concrete extended tenant type
            // to reach it. This template only runs for separate-database multi-tenancy (see CanRunTemplate);
            // `MultiTenancyFactoryExtension` registers CosmosDB as a connection-string requester so
            // TenantExtendedInfo always generates a named `CosmosDbConnection` property (not a bare
            // `ConnectionString`) regardless of which other separate-database modules are also installed.
            CSharpFile.OnBuild(file =>
            {
                var tenantInfoTypeName = this.GetTypeName("Intent.Modules.AspNetCore.MultiTenancy.TenantExtendedInfo");

                mainClass.AddProperty("string?", "CosmosConnectionString", p =>
                {
                    p.Override();
                    p.Getter.WithExpressionImplementation($"(({tenantInfoTypeName}?)_clientProvider.Tenant)?.CosmosDbConnection");
                    p.Setter.WithBodyImplementation("");
                });
            }, 1000);
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && DocumentTemplateHelpers.IsSeparateDatabaseMultiTenancy(ExecutionContext.Settings);
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