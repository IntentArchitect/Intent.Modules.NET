using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.ProblemDetailsWithErrors;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.ProblemDetailsWithErrors
{
    [IntentIgnore]
    public partial class ProblemDetailsWithErrorsTemplate : ProblemDetailsWithErrorsBase
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.ProblemDetailsWithErrors";

        public ProblemDetailsWithErrorsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }
    }
}