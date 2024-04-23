using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.Templates;
using System.Linq;
using Intent.Modules.Metadata.WebApi.Models;
using System.Net.Http.Headers;
using Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.ProblemDetailsWithErrors
{
    public abstract class ProblemDetailsWithErrorsBase : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProblemDetailsWithErrorsBase(
            string templateId,
            IOutputTarget outputTarget,
            object model) : base(templateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddClass($"ProblemDetailsWithErrors", @class =>
                {
                    @class.AddConstructor(ctor => {
                        ctor.AddStatement($"Type = null!;");
                        ctor.AddStatement($"Title = null!;");
                        ctor.AddStatement($"TraceId = null!;");
                        ctor.AddStatement($"Errors = null!;");
                        ctor.AddStatement($"ExtensionData = null!;");
                    });

                    @class.AddProperty("string", "Type", property => property.AddAttribute(UseType("System.Text.Json.Serialization.JsonPropertyName"), attribute =>
                    {
                        attribute.AddArgument("\"type\"");
                    }));
                    @class.AddProperty("string", "Title", property => property.AddAttribute(UseType("System.Text.Json.Serialization.JsonPropertyName"), attribute =>
                    {
                        attribute.AddArgument("\"title\"");
                    }));
                    @class.AddProperty("int", "Status", property => property.AddAttribute(UseType("System.Text.Json.Serialization.JsonPropertyName"), attribute =>
                    {
                        attribute.AddArgument("\"status\"");
                    }));
                    @class.AddProperty("string", "TraceId", property => property.AddAttribute(UseType("System.Text.Json.Serialization.JsonPropertyName"), attribute =>
                    {
                        attribute.AddArgument("\"traceId\"");
                    }));
                    @class.AddProperty("Dictionary<string, string[]>", "Errors", property => property.AddAttribute(UseType("System.Text.Json.Serialization.JsonPropertyName"), attribute =>
                    {
                        attribute.AddArgument("\"errors\"");
                    }));
                    @class.AddProperty("Dictionary<string, object>", "ExtensionData", property => property.AddAttribute(UseType("System.Text.Json.Serialization.JsonExtensionData")));
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
