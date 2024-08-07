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

namespace Intent.Modules.AspNetCore.Versioning.Templates.ApiVersionSwaggerGenOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ApiVersionSwaggerGenOptionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Versioning.ApiVersionSwaggerGenOptions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ApiVersionSwaggerGenOptionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AspVersioningMvc(outputTarget));
            AddNugetDependency(NugetPackages.AspVersioningMvcApiExplorer(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Asp.Versioning")
                .AddUsing("Asp.Versioning.ApiExplorer")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("Swashbuckle.AspNetCore.SwaggerGen")
                .AddUsing("Microsoft.OpenApi.Models")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("System.Linq")
                .AddClass($"ApiVersionSwaggerGenOptions", @class =>
                {
                    @class.ImplementsInterface("IConfigureOptions<SwaggerGenOptions>");
                    @class.AddConstructor(ctor => ctor.AddParameter("IApiVersionDescriptionProvider", "provider",
                        parm => parm.IntroduceReadonlyField()));
                    @class.AddMethod("void", "Configure", method => method
                        .AddParameter("SwaggerGenOptions", "options")
                        .AddForEachStatement("description", "_provider.ApiVersionDescriptions.OrderByDescending(o => o.ApiVersion)", stmt => stmt
                            .AddStatement(
                                @"options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));")));
                    @class.AddMethod("OpenApiInfo", "CreateInfoForApiVersion", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("ApiVersionDescription", "description");
                        method.AddObjectInitializerBlock("var info = new OpenApiInfo()", block => block
                            .AddInitStatement("Title", $@"""{OutputTarget.ApplicationName()} API""")
                            .AddInitStatement("Version", "description.ApiVersion.ToString()")
                            .WithSemicolon());
                        method.AddIfStatement("description.IsDeprecated", stmt => stmt
                            .AddStatement(@"info.Description = ""This API version has been deprecated."";"));
                        method.AddStatement("return info;");
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