using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Templates.TenantExtendedInfo
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class TenantExtendedInfoTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Modules.AspNetCore.MultiTenancy.TenantExtendedInfo";
        private bool _canRun = false;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TenantExtendedInfoTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public override bool CanRunTemplate() => _canRun;

        public void SetCanRun(bool value) => _canRun = value;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"TenantExtendedInfo",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}