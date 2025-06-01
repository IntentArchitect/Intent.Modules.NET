using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModularMonolith.Host.Templates.ModuleInstallerInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Host.Templates.ModuleConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ModuleConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.ModularMonolith.Host.ModuleConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"ModuleInstallerFactory", @class =>
                {
                    @class.AddAttribute("IntentMerge");
                    @class.AddAttribute("DefaultIntentManaged(Mode.Merge, Targets = Targets.Methods)");

                    @class.Static();
                    @class.AddMethod($"IEnumerable<{this.GetModuleInstallerInterfaceName()}>", "GetModuleInstallers", method =>
                    {
                        method.Static();
                        method.AddStatement($"yield break;");
                        //method.AddStatement($"var result = new List<{this.GetModuleInstallerInterfaceName()}>();");
                        //method.AddStatement($"return result;");
                    });
                });
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