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

namespace Intent.Modules.NetTopologySuite.Templates.GeoJsonSchemaSwaggerFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GeoJsonSchemaSwaggerFilterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.NetTopologySuite.GeoJsonSchemaSwaggerFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GeoJsonSchemaSwaggerFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.NetTopologySuite);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.OpenApi.Any")
                .AddUsing("Microsoft.OpenApi.Models")
                .AddUsing("Swashbuckle.AspNetCore.SwaggerGen")
                .AddUsing("NetTopologySuite.Geometries")
                .AddClass($"GeoJsonSchemaFilter", @class =>
                {
                    @class.ImplementsInterface("ISchemaFilter");
                    @class.AddMethod("void", "Apply", method =>
                    {
                        method.AddParameter("OpenApiSchema", "schema");
                        method.AddParameter("SchemaFilterContext", "context");
                        method.AddIfStatement("context.Type == typeof(Point)", stmt => stmt
                            .AddStatement(@"schema.Format = ""geojson"";")
                            .AddStatement(new CSharpAssignmentStatement("schema.Example", new CSharpObjectInitializerBlock("new OpenApiObject")
                                .AddKeyAndValue(@"""type""", @"new OpenApiString(""Point"")")
                                .AddKeyAndValue(@"""coordinates""", "new OpenApiArray { new OpenApiDouble(1.0), new OpenApiDouble(2.0) }"))
                                .WithSemicolon())
                        );
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration")) != null;
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