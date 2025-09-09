using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.CosmosDB.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBRepositoryOptionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBRepositoryOptions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBRepositoryOptionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"CosmosRepositoryOptions", @class =>
                {
                    @class.AddProperty("string?", "AuthenticationMethod");
                    @class.AddProperty("string?", "ManagedIdentityClientId");

                    @class.WithBaseType(UseType("Microsoft.Azure.CosmosRepository.Options.RepositoryOptions"));
                });
        }

        public override bool CanRunTemplate()
        {
            var authenticationMethods = ExecutionContext.Settings.GetCosmosDBSettings().AuthenticationMethods();
            var outputTemplate = authenticationMethods != null && authenticationMethods.Any(a => !a.IsKeyBased());

            return base.CanRunTemplate() && outputTemplate;
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