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
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class AzureFunctionClassTemplate : CSharpTemplateBase<OperationModel, AzureFunctionClassDecorator>
    {
        public const string TemplateId = "Intent.AzureFunctions.AzureFunctionClass";

        private readonly bool _hasMultipleServices;
        private readonly IFunctionTriggerHandler _triggerStrategyHandler;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureFunctionClassTemplate(IOutputTarget outputTarget, OperationModel model) : base(TemplateId, outputTarget, model)
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
        }

        public AzureFunctionClassTemplate(IOutputTarget outputTarget, OperationModel model, bool hasMultipleServices)
            : this(outputTarget, model)
        {
            _hasMultipleServices = hasMultipleServices;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            var additionalFolders = _hasMultipleServices
                ? new[] { Model.ParentService.Name.ToPascalCase() }
                : Array.Empty<string>();

            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace(additionalFolders)}",
                relativeLocation: this.GetFolderPath(additionalFolders));
        }

        public override void BeforeTemplateExecution()
        {
            foreach (var block in GetDecorators().SelectMany(s => s.GetExceptionCatchBlocks()))
            {
                foreach (var @namespace in block.RequiredNamespaces)
                {
                    AddUsing(@namespace);
                }
            }
        }

        private string GetFunctionName()
        {
            return $"{Model.ParentService.Name.ToPascalCase()}-{Model.Name.ToPascalCase()}";
        }

        private string GetClassEntryDefinitionList()
        {
            var definitionList = new List<string>();

            definitionList.AddRange(GetDecorators()
                .SelectMany(s => s.GetClassEntryDefinitionList()));

            return string.Join(@"
        ", definitionList);
        }

        private string GetConstructorParameterDefinitionList()
        {
            var paramList = new List<string>();

            paramList.AddRange(GetDecorators()
                .SelectMany(s => s.GetConstructorParameterDefinitionList()));

            const string newLine = @"
            ";
            return newLine + string.Join("," + newLine, paramList);
        }

        private string GetConstructorBodyStatementList()
        {
            var statementList = new List<string>();

            statementList.AddRange(GetDecorators()
                .SelectMany(s => s.GetConstructorBodyStatementList()));

            const string newLine = @"
            ";
            return string.Join(newLine, statementList);
        }

        private string GetRunMethodReturnType()
        {
            if (Model.GetAzureFunction()?.Type().IsHttpTrigger() == true)
            {
                return "Task<IActionResult>";
            }

            return Model.ReturnType != null
                ? "Task<IActionResult>"
                : "Task";
        }

        private string GetRunMethodParameterDefinitionList()
        {
            var paramList = new List<string>();

            paramList.AddRange(_triggerStrategyHandler.GetMethodParameterDefinitionList());

            paramList.Add("ILogger log");

            paramList.AddRange(GetDecorators()
                .SelectMany(s => s.GetRunMethodParameterDefinitionList()));

            if (paramList.Count == 0)
            {
                throw new Exception($"Operation {Model.Name} has no Azure Function Trigger specified");
            }

            const string newLine = @"
            ";
            return newLine + string.Join("," + newLine, paramList);
        }

        private string GetRunMethodEntryStatementList()
        {
            var statementList = new List<string>();

            statementList.AddRange(_triggerStrategyHandler.GetRunMethodEntryStatementList());

            statementList.AddRange(GetDecorators()
                .SelectMany(s => s.GetRunMethodEntryStatementList()));

            const string newLine = @"
            ";
            return string.Join(newLine, statementList);
        }

        private string GetRunMethodBodyStatementList()
        {
            var statementList = new List<string>();

            statementList.AddRange(GetDecorators()
                .SelectMany(s => s.GetRunMethodBodyStatementList()));

            const string newLine = @"
            ";
            return string.Join(newLine, statementList);
        }

        private string GetRunMethodExitStatementList()
        {
            var statementList = new List<string>();

            statementList.AddRange(GetDecorators()
                .SelectMany(s => s.GetRunMethodExitStatementList()));

            const string newLine = @"
            ";
            return string.Join(newLine, statementList);
        }

        private bool HasExceptionCatchBlocks()
        {
            return GetDecorators().SelectMany(p => p.GetExceptionCatchBlocks()).Any()
                   || _triggerStrategyHandler.GetExceptionCatchBlocks().Any();
        }

        private string GetExceptionCatchBlocks()
        {
            var blockLines = new List<string>();

            foreach (var block in GetDecorators()
                         .SelectMany(s => s.GetExceptionCatchBlocks())
                         .Union(_triggerStrategyHandler.GetExceptionCatchBlocks()))
            {
                blockLines.Add($"catch ({block.ExceptionType})");
                blockLines.Add("{");
                blockLines.AddRange(block.StatementLines.Select(s => $"    {s}"));
                blockLines.Add("}");
            }

            const string newLine = @"
            ";
            return string.Join(newLine, blockLines);
        }
    }
}