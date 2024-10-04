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

namespace Intent.Modules.AspNetCore.Swashbuckle.Templates.TypeSchemaFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TypeSchemaFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Swashbuckle.TypeSchemaFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TypeSchemaFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Microsoft.OpenApi.Any")
                .AddUsing("Microsoft.OpenApi.Models")
                .AddUsing("Swashbuckle.AspNetCore.SwaggerGen")
                .AddClass($"TypeSchemaFilter", @class =>
                {
                    @class.ImplementsInterface("ISchemaFilter");
                    @class.AddMethod("void", "Apply", method =>
                    {
                        method
                            .AddParameter("OpenApiSchema", "schema")
                            .AddParameter("SchemaFilterContext", "context");
                        method.AddIfStatement("context.Type == typeof(TimeSpan) || context.Type == typeof(TimeSpan?)", stmt =>
                        {
                            stmt.AddStatement("schema.Example = new OpenApiString(\"00:00:00\"); // Set your desired format here");
                            stmt.AddStatement("schema.Type = \"string\"; // Override the default representation to be a string");
                        });
                        method.AddIfStatement("context.Type == typeof(DateOnly) || context.Type == typeof(DateOnly?)", stmt =>
                        {
                            stmt.AddStatement("schema.Example = new OpenApiString(DateTime.Today.ToString(\"yyyy-MM-dd\")); // Set your desired format here");
                            stmt.AddStatement("schema.Type = \"string\"; // Override the default representation to be a string");
                        });
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                   ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration")) != null;
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