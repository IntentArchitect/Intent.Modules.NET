using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
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
            
            var isMicrosoftOpenApi_2_4_1 = OutputTarget.GetMaxNetAppVersion().Major >= 8;
            
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Swashbuckle.AspNetCore.SwaggerGen")
                .AddUsing("NetTopologySuite.Geometries")
                .AddClass($"GeoJsonSchemaFilter", @class =>
                {
                    @class.ImplementsInterface("ISchemaFilter");

                    if (isMicrosoftOpenApi_2_4_1)
                    {
                        @class.AddMethod("void", "Apply", method =>
                        {
                            method.AddParameter("IOpenApiSchema", "schema");
                            method.AddParameter("SchemaFilterContext", "context");
                            method.AddIfStatement("schema is OpenApiSchema concreteSchema && typeof(Geometry).IsAssignableFrom(context.Type)", stmt => stmt
                                .AddStatement(@"concreteSchema.Format = ""geojson"";")
                                .AddStatement(@"concreteSchema.Properties?.Clear();")
                                .AddStatement(@"concreteSchema.Required?.Clear();")
                                .AddStatement(@"concreteSchema.Description = ""GeoJSON geometry — shape of 'coordinates' depends on the geometry type."";")
                                .AddIfStatement("context.Type == typeof(Point)", pointStmt => pointStmt
                                    .AddStatement(new CSharpAssignmentStatement("concreteSchema.Example", new CSharpObjectInitializerBlock("new JsonObject")
                                            .AddKeyAndValue(@"""type""", @"""Point""")
                                            .AddKeyAndValue(@"""coordinates""", "new JsonArray { 1.0, 2.0 }"))
                                        .WithSemicolon())
                                )
                            );
                        });
                    }
                    else
                    {
                        @class.AddMethod("void", "Apply", method =>
                        {
                            method.AddParameter("OpenApiSchema", "schema");
                            method.AddParameter("SchemaFilterContext", "context");
                            method.AddIfStatement("typeof(Geometry).IsAssignableFrom(context.Type)", stmt => stmt
                                .AddStatement(@"schema.Format = ""geojson"";")
                                .AddStatement(@"schema.Properties?.Clear();")
                                .AddStatement(@"schema.Required?.Clear();")
                                .AddStatement(@"schema.Description = ""GeoJSON geometry — shape of 'coordinates' depends on the geometry type."";")
                                .AddIfStatement("context.Type == typeof(Point)", pointStmt => pointStmt
                                    .AddStatement(new CSharpAssignmentStatement("schema.Example", new CSharpObjectInitializerBlock("new OpenApiObject")
                                            .AddKeyAndValue(@"""type""", @"new OpenApiString(""Point"")")
                                            .AddKeyAndValue(@"""coordinates""", "new OpenApiArray { new OpenApiDouble(1.0), new OpenApiDouble(2.0) }"))
                                        .WithSemicolon())
                                )
                            );
                        });
                    }
                });
            
            if (isMicrosoftOpenApi_2_4_1)
            {
                CSharpFile.AddUsing("Microsoft.OpenApi");
                CSharpFile.AddUsing("System.Text.Json.Nodes");
            }
            else
            {
                CSharpFile.AddUsing("Microsoft.OpenApi.Any");
                CSharpFile.AddUsing("Microsoft.OpenApi.Models");
            }
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