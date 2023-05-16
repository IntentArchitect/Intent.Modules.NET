using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.AuthorizeCheckOperationFilter;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Templates.SwashbuckleConfiguration;

[IntentManaged(Mode.Merge, Signature = Mode.Merge)]
partial class SwashbuckleConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.AspNetCore.Swashbuckle.SwashbuckleConfiguration";

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public SwashbuckleConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.SwashbuckleAspNetCore);
        AddUsing("System");
        AddUsing("System.Collections.Generic");
        AddUsing("Microsoft.AspNetCore.Builder");
        AddUsing("Microsoft.Extensions.Configuration");
        AddUsing("Microsoft.Extensions.DependencyInjection");
        AddUsing("Microsoft.OpenApi.Models");
        AddUsing("Swashbuckle.AspNetCore.SwaggerUI");

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddClass("SwashbuckleConfiguration", @class =>
            {
                @class.Static();
                @class.AddMethod("IServiceCollection", "ConfigureSwagger",
                    method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => { param.WithThisModifier(); });
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddStatement(new CSharpInvocationStatement("services.AddSwaggerGen")
                            .AddArgument(new CSharpLambdaBlock("options")
                                .AddStatement(new CSharpInvocationStatement("options.SwaggerDoc")
                                    .AddArgument(@"""v1""")
                                    .AddArgument(new CSharpObjectInitializerBlock("new OpenApiInfo")
                                        .AddInitStatement("Version", @"""v1""")
                                        .AddInitStatement("Title", $@"""{OutputTarget.ApplicationName()} API""")
                                    )
                                    .WithArgumentsOnNewLines()
                                )
                                .AddStatement(
                                    $@"options.OperationFilter<{GetTypeName(AuthorizeCheckOperationFilterTemplate.TemplateId)}>();")
                                .AddStatement(
                                    $@"options.CustomSchemaIds(x => x.FullName);")
                            )
                            .WithArgumentsOnNewLines()
                            .AddMetadata("AddSwaggerGen", true)
                        );
                        method.AddStatement($@"return services;");
                    });

                @class.AddMethod("void", "UseSwashbuckle", method =>
                {
                    method.Static();
                    method.AddParameter("IApplicationBuilder", "app", conf => conf.WithThisModifier());
                    method.AddStatement(new CSharpInvocationStatement("app.UseSwagger")
                        .AddMetadata("UseSwagger", true));

                    method.AddStatement(new CSharpInvocationStatement("app.UseSwaggerUI")
                        .AddArgument(new CSharpLambdaBlock("options")
                            .AddStatement($@"options.RoutePrefix = ""swagger"";")
                            .AddStatement(
                                $@"options.SwaggerEndpoint(""/swagger/v1/swagger.json"", ""{OutputTarget.ApplicationName()} API V1"");")
                            .AddStatement($@"options.OAuthAppName(""{OutputTarget.ApplicationName()} API"");")
                            .AddStatement($@"options.EnableDeepLinking();")
                            .AddStatement($@"options.DisplayOperationId();")
                            .AddStatement($@"options.DefaultModelsExpandDepth(2);")
                            .AddStatement($@"options.DefaultModelRendering(ModelRendering.Model);")
                            .AddStatement($@"options.DocExpansion(DocExpansion.List);")
                            .AddStatement($@"options.ShowExtensions();")
                            .AddStatement($@"options.EnableFilter(string.Empty);"))
                        .WithArgumentsOnNewLines()
                        .AddMetadata("UseSwaggerUI", true));
                });
            });
    }

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    protected override CSharpFileConfig DefineFileConfig()
    {
        return CSharpFile.GetConfig();
    }

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public override string TransformText()
    {
        return CSharpFile.ToString();
    }

    public CSharpFile CSharpFile { get; }
}