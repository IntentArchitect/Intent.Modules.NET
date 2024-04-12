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
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Azure.CosmosRepository.Options")
                .AddUsing("Microsoft.Azure.CosmosRepository.Providers")
                .AddClass($"CosmosDBMultiTenantRepositoryOptions", @class =>
                {
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
                    @class.AddProperty("string?", "CosmosConnectionString", p =>
                    {
                        p.Override();
                        p.Getter.WithExpressionImplementation("_clientProvider.Tenant?.ConnectionString");
                        p.Setter.WithBodyImplementation("");
                    });
                });
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