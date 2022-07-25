using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureFunctionClassTemplate(IOutputTarget outputTarget, OperationModel model) : base(TemplateId,
            outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MicrosoftNETSdkFunctions);
            AddNugetDependency(NuGetPackages.MicrosoftExtensionsDependencyInjection);
            AddNugetDependency(NuGetPackages.MicrosoftExtensionsHttp);
            AddNugetDependency(NuGetPackages.MicrosoftAzureFunctionsExtensions);

            if (model.GetAzureFunction()?.Type().IsServiceBusTrigger() == true)
            {
                AddNugetDependency(NuGetPackages.MicrosoftAzureServiceBus);
                AddNugetDependency(NuGetPackages.MicrosoftAzureWebJobsExtensionsServiceBus);
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

            if (Model.GetAzureFunction()?.Type()?.IsHttpTrigger() == true)
            {
                var httpTriggersView = Model.GetAzureFunction().GetHttpTriggerView();
                var method = @$"""{httpTriggersView.Method().Value.ToLower()}""";
                var route = !string.IsNullOrWhiteSpace(httpTriggersView.Route())
                    ? $@"""{httpTriggersView.Route()}"""
                    : "null";
                paramList.Add(
                    @$"[HttpTrigger(AuthorizationLevel.{httpTriggersView.AuthorizationLevel().Value}, {method}, Route = {route})] HttpRequest req");
            }

            if (Model.GetAzureFunction()?.Type()?.IsServiceBusTrigger() == true)
            {
                var serviceBusTriggerView = Model.GetAzureFunction().GetServiceBusTriggerView();
                var attrParamList = new List<string>();
                attrParamList.Add($@"""{serviceBusTriggerView.QueueName()}""");
                if (!string.IsNullOrEmpty(serviceBusTriggerView.Connection()))
                {
                    attrParamList.Add($@"Connection = ""{serviceBusTriggerView.Connection()}""");
                }

                paramList.Add(
                    $@"[ServiceBusTrigger({string.Join(", ", attrParamList)})] {GetRequestDtoType()} {GetRequestDtoParamName()}");
            }

            foreach (var parameterModel in Model.Parameters.Where(IsParameterRoute))
            {
                paramList.Add($@"{GetTypeName(parameterModel.Type)} {parameterModel.Name.ToParameterName()}");
            }

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

            foreach (var param in GetQueryParams())
            {
                if (GetTypeName(param.Type) == "string")
                {
                    statementList.Add(
                        $@"string {param.Name.ToParameterName()} = req.Query[""{param.Name.ToCamelCase()}""];");
                    continue;
                }

                statementList.Add(
                    $@"{GetTypeName(param.Type)} {param.Name.ToParameterName()} = {this.GetAzureFunctionClassHelperName()}.{(param.Type.IsNullable ? "GetQueryParamNullable" : "GetQueryParam")}(""{param.Name.ToParameterName()}"", req.Query, (string val, out {GetTypeName(param.Type).Replace("?", string.Empty)} parsed) => {GetTypeName(param.Type).Replace("?", string.Empty)}.TryParse(val, out parsed));");
            }

            if (Model.GetAzureFunction()?.Type()?.IsHttpTrigger() == true
                && !string.IsNullOrWhiteSpace(GetRequestDtoType()))
            {
                statementList.Add($@"string requestBody = await new StreamReader(req.Body).ReadToEndAsync();");
                statementList.Add($@"var dto = JsonConvert.DeserializeObject<{GetRequestDtoType()}>(requestBody);");
            }

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
                   || GetQueryParams().Any();
        }

        private string GetExceptionCatchBlocks()
        {
            var blockLines = new List<string>();

            foreach (var block in GetDecorators().SelectMany(s => s.GetExceptionCatchBlocks()))
            {
                blockLines.Add($"catch ({block.ExceptionType})");
                blockLines.Add("{");
                blockLines.AddRange(block.StatementLines.Select(s => $"    {s}"));
                blockLines.Add("}");
            }

            if (GetQueryParams().Any())
            {
                blockLines.Add($"catch (FormatException exception)");
                blockLines.Add("{");
                blockLines.Add("    return new BadRequestObjectResult(new { Message = exception.Message });");
                blockLines.Add("}");
            }

            const string newLine = @"
            ";
            return string.Join(newLine, blockLines);
        }

        private DTOModel GetRequestDtoModel()
        {
            var dtoParams = Model.Parameters
                .Where(IsParameterBody)
                .ToArray();
            switch (dtoParams.Length)
            {
                case 0:
                    return null;
                case > 1:
                    throw new Exception($"Multiple DTOs not supported on {Model.Name} operation");
                default:
                {
                    var param = dtoParams.First();
                    return param.TypeReference.Element.AsDTOModel();
                }
            }
        }

        private string GetRequestDtoParamName()
        {
            var dtoParams = Model.Parameters
                .Where(IsParameterBody)
                .ToArray();
            switch (dtoParams.Length)
            {
                case 0:
                    return null;
                case > 1:
                    throw new Exception($"Multiple DTOs not supported on {Model.Name} operation");
                default:
                {
                    var param = dtoParams.First();
                    return param.Name.ToParameterName();
                }
            }
        }

        private bool IsParameterBody(ParameterModel parameterModel)
        {
            return parameterModel.GetParameterSetting()?.Source().IsFromBody() == true
                   || (parameterModel.GetParameterSetting()?.Source().IsDefault() == true &&
                       parameterModel.TypeReference.Element.IsDTOModel());
        }

        private bool IsParameterRoute(ParameterModel parameterModel)
        {
            return parameterModel.GetParameterSetting()?.Source().IsFromRoute() == true
                   || (parameterModel.GetParameterSetting()?.Source().IsDefault() == true &&
                       !parameterModel.TypeReference.Element.IsDTOModel());
        }

        public string GetRequestDtoType()
        {
            var model = GetRequestDtoModel();
            return model == null ? null : this.GetDtoModelName(model);
        }

        private IReadOnlyCollection<ParameterModel> GetQueryParams()
        {
            return Model.Parameters.Where(p => p.GetParameterSetting()?.Source().IsFromQuery() == true).ToArray();
        }
    }
}