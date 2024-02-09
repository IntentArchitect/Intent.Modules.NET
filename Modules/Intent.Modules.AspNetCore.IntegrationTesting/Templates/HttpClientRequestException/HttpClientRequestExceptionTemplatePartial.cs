using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.HttpClientRequestException
{
    [IntentManaged(Mode.Ignore)]
    public partial class HttpClientRequestExceptionTemplate : HttpClientRequestExceptionTemplateBase
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.HttpClientRequestException";

        public HttpClientRequestExceptionTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile.OnBuild(file =>
            {
                file.AddUsing("System.Text.Json");
                var @class = file.Classes.First();
                var method = @class.FindMethod("GetMessage");
                if (method == null) return;

                var toReplace = method.FindStatement(s => s.GetText("") == "message += \" See content for more detail.\";");
                toReplace.InsertAbove(@"message = FormatJson(JsonSerializer.Deserialize<dynamic>(responseContent))
                    .Replace(""\\r\\n"", Environment.NewLine);");
                toReplace.Remove();

                @class.AddMethod("string", "FormatJson", method =>
                {
                    method
                        .Private()
                        .Static()
                        .AddParameter("object", "jsonObject")
                        .AddStatement("return JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions() { WriteIndented = true });");
                });
            }, 10);
        }
    }
}