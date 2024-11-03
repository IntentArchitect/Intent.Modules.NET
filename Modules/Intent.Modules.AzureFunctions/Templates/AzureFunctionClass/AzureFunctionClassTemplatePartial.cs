using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.AzureFunctions.Settings;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using ModelHasFolderTemplateExtensionsV2 = Intent.Modules.Common.ModelHasFolderTemplateExtensionsV2;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Ignore)]
    public partial class AzureFunctionClassTemplate : CSharpTemplateBase<IAzureFunctionModel, AzureFunctionClassDecorator>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.AzureFunctionClass";

        [IntentManaged(Mode.Ignore)]
        public AzureFunctionClassTemplate(IOutputTarget outputTarget, IAzureFunctionModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftExtensionsDependencyInjection(outputTarget));

            var triggerStrategyHandler = TriggerStrategyResolver.GetFunctionTriggerHandler(this, model);

            foreach (var dependency in triggerStrategyHandler.GetNugetDependencies())
            {
                AddNugetDependency(dependency);
            }

            foreach (var dependency in triggerStrategyHandler.GetNugetDependencies())
            {
                ExecutionContext.EventDispatcher.Publish(new RemoveNugetPackageEvent(dependency.Name, outputTarget));
            }

            AddTypeSource(TemplateRoles.Application.Contracts.Dto, "List<{0}>");
            AddTypeSource(TemplateRoles.Application.Command);
            AddTypeSource(TemplateRoles.Application.Query);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);

            CSharpFile = new CSharpFile(this.GetNamespace(), GetRelativeLocation())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.IO")
                .AddUsing("System.Threading.Tasks")
                .AddClass(Model.Name, @class =>
                {
                    @class.AddConstructor(ctor => { });

                    @class.AddMethod(GetRunMethodReturnType(), "Run", method =>
                    {
                        method.Async();

                        switch (AzureFunctionsHelper.GetAzureFunctionsProcessType(OutputTarget))
                        {
                            case AzureFunctionsHelper.AzureFunctionsProcessType.InProcess:
                                method.AddAttribute(UseType("Microsoft.Azure.WebJobs.FunctionName"), attr => attr.AddArgument(@$"""{GetFunctionName()}"""));
                                break;
                            default:
                            case AzureFunctionsHelper.AzureFunctionsProcessType.Isolated:
                                method.AddAttribute(UseType("Microsoft.Azure.Functions.Worker.Function"), attr => attr.AddArgument(@$"""{GetFunctionName()}"""));
                                break;
                        }

                        triggerStrategyHandler.ApplyMethodParameters(method);
                        triggerStrategyHandler.ApplyMethodStatements(method);
                    });
                })
                .AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("Run");
                    if (method?.Statements.Any() == false)
                    {
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());
                        method.AddNotImplementedException();
                    }
                });
        }

        private string GetFunctionName()
        {
            if (!ExecutionContext.Settings.GetAzureFunctionsSettings().SimpleFunctionNames())
            {
                var path = string.Join("_", Model.GetParentFolderNames());
                if (!string.IsNullOrWhiteSpace(path))
                {
                    return $"{path}_{Model.Name}";
                }
            }

            return Model.Name;
        }

        private string GetRelativeLocation()
        {
            var path = string.Join("/", Model.GetParentFolderNames());
            return path;
        }

        string GetNamespace(
            params string[] additionalFolders)
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

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "2.0"), new NewtonSoftRemovalMigration());
        }

        private class NewtonSoftRemovalMigration : ITemplateMigration
        {
            public string Execute(string currentText)
            {
                return currentText.Replace(@"using Newtonsoft.Json;\r\n", "")
                    .Replace(@"using Newtonsoft.Json;\n", "")
                    .Replace(@"using Newtonsoft.Json;\r", "")
                    .Replace(@"using Newtonsoft.Json;", "");
            }

            public TemplateMigrationCriteria Criteria => TemplateMigrationCriteria.Upgrade(1, 2);
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
                return $"Task<{UseType("Microsoft.AspNetCore.Mvc.IActionResult")}>";
            }
            else if (Model.TriggerType == TriggerType.QueueTrigger &&
                     AzureFunctionsHelper.GetAzureFunctionsProcessType(OutputTarget) == AzureFunctionsHelper.AzureFunctionsProcessType.InProcess)
            {
                return $"Task";
            }

            return Model.ReturnType?.Element != null
                ? $"Task<{GetTypeName(Model.ReturnType)}>"
                : "Task";
        }
    }
}