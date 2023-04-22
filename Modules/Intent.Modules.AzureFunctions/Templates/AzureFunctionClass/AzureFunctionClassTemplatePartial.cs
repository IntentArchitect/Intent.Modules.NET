using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Ignore)]
    public partial class AzureFunctionClassTemplate : CSharpTemplateBase<IAzureFunctionModel, AzureFunctionClassDecorator>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.AzureFunctionClass";

        private readonly IFunctionTriggerHandler _triggerStrategyHandler;

        [IntentManaged(Mode.Ignore)]
        public AzureFunctionClassTemplate(IOutputTarget outputTarget, IAzureFunctionModel model) : base(TemplateId, outputTarget, model)
        {
            _triggerStrategyHandler = TriggerStrategyResolver.GetFunctionTriggerHandler(this, model);

            AddNugetDependency(NuGetPackages.MicrosoftNETSdkFunctions);
            AddNugetDependency(NuGetPackages.MicrosoftExtensionsDependencyInjection);
            AddNugetDependency(NuGetPackages.MicrosoftAzureFunctionsExtensions);

            foreach (var dependency in _triggerStrategyHandler.GetNugetDependencies())
            {
                AddNugetDependency(dependency);
            }

            AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.IO")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.Azure.WebJobs")
                .AddUsing("Microsoft.Azure.WebJobs.Extensions.Http")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("Newtonsoft.Json")
                .AddClass(Model.Name, @class =>
                {
                    @class.AddConstructor(ctor =>
                    {
                    });

                    @class.AddMethod(GetRunMethodReturnType(), "Run", method =>
                    {
                        method.Async();
                        method.AddAttribute("FunctionName", attr => attr.AddArgument(@$"""{Model.Name}"""));
                        _triggerStrategyHandler.ApplyMethodParameters(method);
                        _triggerStrategyHandler.ApplyMethodStatements(method);
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: this.GetFolderPath());
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private string GetRunMethodReturnType()
        {
            if (Model.TriggerType == TriggerType.HttpTrigger)
            {
                return "Task<IActionResult>";
            }

            return Model.TypeReference.Element != null
                ? "Task<IActionResult>"
                : "Task";
        }
    }
}