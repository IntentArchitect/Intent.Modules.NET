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

namespace Intent.Modules.AspNetCore.OData.EntityFramework.Templates.ExcludeODataDocumentFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ExcludeODataDocumentFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.OData.EntityFramework.ExcludeODataDocumentFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ExcludeODataDocumentFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Swashbuckle.AspNetCore.SwaggerGen")
                .AddUsing("Microsoft.OpenApi.Models")
                .AddUsing("System.Linq")
                .AddUsing("System")
                .AddClass($"ExcludeODataDocumentFilter", @class =>
                {
                    @class.ImplementsInterface("IDocumentFilter");
                    @class.AddMethod("void", "Apply", method =>
                    {
                        method.AddParameter("OpenApiDocument", "swaggerDoc");
                        method.AddParameter("DocumentFilterContext ", "context");

                        method.AddStatement("var keepPaths = new[] { \"/odata\", \"/odata/$metadata\" };");

                        method.AddStatement("var keysToRemove = swaggerDoc.Paths.Keys")
                        .AddInvocationStatement(".Where", stmt => stmt.AddLambdaBlock("path", arg =>
                            arg.AddStatement("return path.StartsWith(\"/odata\", StringComparison.OrdinalIgnoreCase) " +
                            "&& !keepPaths.Contains(path, StringComparer.OrdinalIgnoreCase);")));

                        method.AddForEachStatement("key", "keysToRemove", forEach =>
                        {
                            forEach.AddStatement("swaggerDoc.Paths.Remove(key);");
                        });

                        method.AddForEachStatement("keep", "keepPaths", forEach =>
                        {
                            forEach.AddIfStatement("!swaggerDoc.Paths.ContainsKey(keep)", ifStatement =>
                            {
                                ifStatement.AddAssignmentStatement("var openApiPathItem", new CSharpObjectInitializerBlock("new OpenApiPathItem")
                                    .AddInitStatement("Description", "\"OData service endpoint\"").WithSemicolon());

                                ifStatement.AddStatement($"swaggerDoc.Paths.Add(keep, openApiPathItem);");
                            });
                        });
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