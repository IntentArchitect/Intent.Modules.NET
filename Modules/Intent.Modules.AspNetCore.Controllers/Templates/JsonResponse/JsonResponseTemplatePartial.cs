using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.JsonResponse
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class JsonResponseTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.AspNetCore.Controllers.JsonResponse";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public JsonResponseTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"JsonResponse",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private bool _isTemplateRequired = false;
        public void NotifyTemplateIsRequired()
        {
            _isTemplateRequired = true;
        }

        public override bool CanRunTemplate()
        {
            return _isTemplateRequired;
        }
    }
}