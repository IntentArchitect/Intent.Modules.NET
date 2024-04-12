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

namespace Intent.Modules.CosmosDB.Templates.CosmosDBMultiTenantOptionsMonitor
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBMultiTenantOptionsMonitorTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBMultiTenantOptionsMonitor";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBMultiTenantOptionsMonitorTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("Microsoft.Azure.CosmosRepository.Options")
                .AddClass($"CosmosDBMultiTenantOptionsMonitor", @class =>
                {
                    @class.ImplementsInterface("IOptionsMonitor<RepositoryOptions>");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("RepositoryOptions", "options", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });
                    @class.AddProperty("RepositoryOptions", "CurrentValue", p => p.WithoutSetter().Getter.WithExpressionImplementation("_options"));
                    @class.AddMethod("RepositoryOptions", "Get", method =>
                    {
                        method.AddParameter("string?", "name");
                        method.AddStatement("return _options;");
                    });
                    @class.AddMethod("IDisposable?", "OnChange", method =>
                    {
                        method.AddParameter("Action<RepositoryOptions, string?>", "listener");
                        method.AddStatement("return null;");
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