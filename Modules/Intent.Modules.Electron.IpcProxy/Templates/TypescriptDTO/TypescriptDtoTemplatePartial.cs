using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.TypeScript;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.Templates;

namespace Intent.Modules.Electron.IpcProxy.Templates.TypescriptDTO
{
    partial class TypescriptDtoTemplate : TypeScriptTemplateBase<DTOModel>, ITemplate
    {
        public const string TemplateId = "Intent.Electron.IpcProxy.Contracts.TypeScriptDTO";

        public TypescriptDtoTemplate(string identifier, IOutputTarget project, DTOModel model)
            : base(identifier, project, model)
        {
            AddTypeSource(TypescriptTypeSource.Create(ExecutionContext, TypescriptDtoTemplate.TemplateId));
            // For reference purposes only:
            //Namespace = model.BoundedContextName == project.ApplicationName().Replace("_Client", "") ? "App.Contracts" : $"App.Contracts.{model.BoundedContextName}";
            //Location = model.BoundedContextName == project.ApplicationName().Replace("_Client", "") ? $"wwwroot/App/DTOs/Generated" : $@"wwwroot/App/DTOs/Generated/{model.BoundedContextName}";
        }
        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new ITemplateDependency[0]; // disable adding on imports when merged
        }

        public string ApplicationName => Model.Application.Name;

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: Model.Name,
                relativeLocation: "",
                className: $"{Model.Name}",
                @namespace: "App.Contracts");
        }
    }
}
