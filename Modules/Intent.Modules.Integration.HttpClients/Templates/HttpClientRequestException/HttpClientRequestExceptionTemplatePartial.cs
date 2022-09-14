using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates.HttpClientRequestException
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class HttpClientRequestExceptionTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Integration.HttpClients.HttpClientRequestException";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientRequestExceptionTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"HttpClientRequestException",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}