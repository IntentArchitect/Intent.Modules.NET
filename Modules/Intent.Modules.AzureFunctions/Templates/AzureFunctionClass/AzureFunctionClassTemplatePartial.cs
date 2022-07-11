using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class AzureFunctionClassTemplate : CSharpTemplateBase<OperationModel>
    {
        private static readonly ISet<string> ParentServices = new HashSet<string>();

        public const string TemplateId = "Intent.AzureFunctions.AzureFunctionClass";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureFunctionClassTemplate(IOutputTarget outputTarget, OperationModel model) : base(TemplateId,
            outputTarget, model)
        {
            ParentServices.Add(model.ParentService.Name);
            
            AddNugetDependency(NugetPackages.MicrosoftNETSdkFunctions);
            AddNugetDependency(NugetPackages.MicrosoftExtensionsDependencyInjection);
            AddNugetDependency(NugetPackages.MicrosoftExtensionsHttp);
            AddNugetDependency(NugetPackages.MicrosoftAzureFunctionsExtensions);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: GetRelativeLocation());
        }

        private string GetRelativeLocation()
        {
            return ParentServices.Count > 1
                ? Path.Join(this.GetFolderPath(), Model.ParentService.Name)
                : this.GetFolderPath();
        }

        private string GetRunParameterDefinition()
        {
            var paramList = new List<string>();

            if (Model.HasHttpTrigger())
            {
                paramList.Add(
                    @$"[HttpTrigger(AuthorizationLevel.{Model.GetHttpTrigger().AuthorizationLevel().Value}{GetVerbs()}, Route = {GetRoute()})] HttpRequest req");
            }

            paramList.Add("ILogger log");

            if (paramList.Count == 0)
            {
                throw new Exception($"Operation {Model.Name} has no Azure Function Trigger specified");
            }

            var newLine = $"{Environment.NewLine}            ";
            return newLine + string.Join("," + newLine, paramList);
        }

        private string GetVerbs()
        {
            var list = new List<string>();
            list.Add(string.Empty);
            list.AddRange(Model.GetHttpTrigger().Methods().Select(s => @$"""{s.Value.ToLower()}"""));
            return string.Join($", ", list);
        }

        private string GetRoute()
        {
            if (!string.IsNullOrWhiteSpace(Model.GetHttpTrigger().Route()))
            {
                return Model.GetHttpTrigger().Route();
            }

            return "null";
        }
    }
}