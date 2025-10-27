using Intent.Engine;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.AzureFunctions.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.AzureQueueStorage.Templates.AzureFunctionConsumer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AzureFunctionConsumerTemplate : CSharpTemplateBase<IntegrationEventHandlerModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.AzureQueueStorage.AzureFunctionConsumer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureFunctionConsumerTemplate(IOutputTarget outputTarget, IntegrationEventHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetRelativeLocation())
                .AddClass(GetFunctionName(), @class =>
                {
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "exampleParam", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });
                });
        }

        private string GetFunctionName()
        {
            var functionName = $"{Model.Name.RemoveSuffix("Handler")}Consumer";

            if (!ExecutionContext.Settings.GetAzureFunctionsSettings().SimpleFunctionNames())
            {
                var path = string.Join("_", Model.GetParentFolderNames());
                if (!string.IsNullOrWhiteSpace(path))
                {
                    return $"{path}_{functionName}";
                }
            }

            return functionName;
        }

        private string GetRelativeLocation()
        {
            var path = string.Join("/", Model.GetParentFolderNames());
            return path;
        }

        string GetNamespace(params string[] additionalFolders)
        {
            return string.Join(".", new[]
                {
                    OutputTarget.GetNamespace()
                }
                .Concat(Model.GetParentFolders()
                    .Where(x =>
                    {
                        if (string.IsNullOrWhiteSpace(x.Name))
                            return false;

                        if (x is FolderModel fm)
                        {
                            return !fm.HasFolderOptions() || fm.GetFolderOptions().NamespaceProvider();
                        }

                        return true;
                    })
                    .Select(x => x.Name))
                .Concat(additionalFolders));
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