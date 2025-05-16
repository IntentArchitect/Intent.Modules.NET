using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

namespace Intent.Modules.ModularMonolith.Module.Templates.ModuleConfiguration
{
    // This file is manual other wise IA tries to change the TemplateId in .modspec and I don't want to ignore it
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ModuleConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentIgnore]
        public const string TemplateId = "Intent.ModularMonolith.Host.ModuleConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"ModuleInstallerFactory", @class =>
                {
                    @class.AddAttribute("IntentMerge");
                    @class.AddAttribute("DefaultIntentManaged(Mode.Merge, Targets = Targets.Methods)");
                    string moduleInstallerTypeName = GetFullTypeName(ModuleInstaller.ModuleInstallerTemplate.TemplateId);
                    
                    @class.Static();
                    @class.AddMethod($"IEnumerable<{this.GetModuleInstallerInterfaceName()}>", "GetModuleInstallers", method =>
                    {
                        method.Static();
                        method.AddStatement($"var result = new List<{this.GetModuleInstallerInterfaceName()}>();");
                        method.AddStatement($"result.Add(new {moduleInstallerTypeName}());");
                        method.AddStatement($"return result;");
                    });
                });
            
        }

        private string GetFullTypeName(string templateId)
        {
            var template = this.GetTemplate<ICSharpFileBuilderTemplate>(templateId);
            return template.Namespace + "." + template.ClassName;
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