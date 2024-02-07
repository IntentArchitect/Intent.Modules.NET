using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClientRequestException;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IntegrationTesting.Templates.HttpClientRequestException
{
    [IntentManaged(Mode.Ignore)]
    public partial class HttpClientRequestExceptionTemplate : HttpClientRequestExceptionTemplateBase
    {
        public const string TemplateId = "Intent.IntegrationTesting.HttpClientRequestException";

        public HttpClientRequestExceptionTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }
    }
}