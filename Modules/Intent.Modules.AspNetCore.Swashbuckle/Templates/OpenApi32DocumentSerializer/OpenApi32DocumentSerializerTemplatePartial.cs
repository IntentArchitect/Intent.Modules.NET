using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Templates.OpenApi32DocumentSerializer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class OpenApi32DocumentSerializerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Swashbuckle.OpenApi32DocumentSerializer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public OpenApi32DocumentSerializerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.IO")
                .AddUsing("System.Text.Json.Nodes")
                .AddUsing("Microsoft.OpenApi")
                .AddUsing("Swashbuckle.AspNetCore.Swagger")
                .AddClass("OpenApi32DocumentSerializer", @class =>
                {
                    @class.Sealed();
                    @class.ImplementsInterface("ISwaggerDocumentSerializer");
                    @class.WithComments(
                    [
                        "/// <summary>",
                        "/// Writes the document as OpenAPI 3.1 and labels it 3.2.0, the first version with a native <c>query</c> operation.",
                        "/// <para>",
                        "/// OpenAPI 3.2 only adds to 3.1, so a 3.1 document plus <c>query</c> operations is a valid 3.2 document. Swashbuckle 10",
                        "/// (Microsoft.OpenApi 2.x) has no 3.2 writer; drop this once Swashbuckle writes OpenAPI 3.2 itself.",
                        "/// </para>",
                        "/// </summary>"
                    ]);
                    @class.AddField("string", "Version", field => field.PrivateConstant("\"3.2.0\""));
                    @class.AddMethod("void", "SerializeDocument", method =>
                    {
                        method.AddParameter("OpenApiDocument", "document");
                        method.AddParameter("IOpenApiWriter", "writer");
                        method.AddParameter("OpenApiSpecVersion", "specVersion");
                        method.AddStatement("using var buffer = new StringWriter();");
                        method.AddStatement("document.SerializeAsV31(new OpenApiJsonWriter(buffer));");
                        method.AddStatement("var json = JsonNode.Parse(buffer.ToString())!.AsObject();", s => s.SeparatedFromPrevious());
                        method.AddStatement("json[\"openapi\"] = Version;");
                        method.AddStatement("writer.WriteRaw(json.ToJsonString());");
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && HttpQueryHelper.NeedsHttpQueryOpenApi(OutputTarget);
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