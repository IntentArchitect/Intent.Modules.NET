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

namespace Intent.Modules.AspNetCore.OData.EntityFramework.Templates.ODataJsonSchemaSwaggerFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ODataJsonSchemaSwaggerFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.OData.EntityFramework.ODataJsonSchemaSwaggerFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ODataJsonSchemaSwaggerFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Swashbuckle.AspNetCore.SwaggerGen")
                .AddUsing("Microsoft.OpenApi.Models")
                .AddClass($"ODataJsonSchemaSwaggerFilter", @class =>
                {
                    @class.ImplementsInterface("ISchemaFilter");
                    @class.AddMethod("void", "Apply", method =>
                    {
                        method.AddParameter("OpenApiSchema", "schema");
                        method.AddParameter("SchemaFilterContext ", "context");
                        method.AddIfStatement("schema?.Properties == null", ifStatement =>
                        {
                            ifStatement.AddStatement("return;");
                            ifStatement.SeparatedFromNext();
                        });
                        
                        method.AddStatement("""var hasDomainEvents = schema.Properties.ContainsKey("domainEvents");""");
                        method.AddIfStatement("hasDomainEvents", ifStatement =>
                        {
                            ifStatement.SeparatedFromPrevious();
                            ifStatement.AddIfStatement("typeof(IHasDomainEvent).IsAssignableFrom(context.Type)", nestedIf =>
                            {
                                nestedIf.AddStatement("""schema.Properties.Remove("domainEvents");""");
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