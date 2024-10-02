using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AspNetCore.Swashbuckle.Settings;
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
public partial class SwashbuckleConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
{
    [IntentManaged(Mode.Fully)]
    public const string TemplateId = "Intent.AspNetCore.Swashbuckle.SwashbuckleConfiguration";

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public SwashbuckleConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
    {
        var useSimpleSchemaIdentifiers = ExecutionContext.Settings.GetSwaggerSettings().UseSimpleSchemaIdentifiers();
        var markNonNullableFieldsAsRequired = ExecutionContext.Settings.GetSwaggerSettings().MarkNonNullableFieldsAsRequired();

        AddNugetDependency(NugetPackages.SwashbuckleAspNetCore(OutputTarget));
        AddUsing("System");
        AddUsing("System.Collections.Generic");
        AddUsing("System.IO");
        AddUsing("System.Reflection");
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
                            .AddArgument(new CSharpLambdaBlock("options"), argument =>
                            {
                                var lambdaBlock = (CSharpLambdaBlock)argument;

                                lambdaBlock.AddStatement(new CSharpInvocationStatement("options.SwaggerDoc")
                                        .AddArgument(@"""v1""")
                                        .AddArgument(new CSharpObjectInitializerBlock("new OpenApiInfo")
                                            .AddInitStatement("Version", @"""v1""")
                                            .AddInitStatement("Title", $@"""{OutputTarget.ApplicationName()} API""")
                                        )
                                        .WithArgumentsOnNewLines()
                                    );

                                if (markNonNullableFieldsAsRequired)
                                {
                                    lambdaBlock.AddStatement("options.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();");
                                    lambdaBlock.AddStatement("options.SupportNonNullableReferenceTypes();");
                                }

                                lambdaBlock.AddStatement(useSimpleSchemaIdentifiers
                                    ? "options.CustomSchemaIds(GetFriendlyName);"
                                    : "options.CustomSchemaIds(x => x.FullName?.Replace(\"+\", \"_\"));");

                                lambdaBlock.AddStatement(
                                    "var apiXmlFile = Path.Combine(AppContext.BaseDirectory, $\"{Assembly.GetExecutingAssembly().GetName().Name}.xml\");",
                                    s => s.SeparatedFromPrevious());
                                lambdaBlock.AddIfStatement("File.Exists(apiXmlFile)", @if =>
                                {
                                    @if
                                        .AddStatement("options.IncludeXmlComments(apiXmlFile);")
                                        .SeparatedFromPrevious(false);
                                });

                                if (TryGetTemplate<ICSharpTemplate>("Intent.Application.DependencyInjection.DependencyInjection", out _))
                                {
                                    lambdaBlock.AddStatement(
                                        $"var applicationXmlFile = Path.Combine(AppContext.BaseDirectory," +
                                        $" $\"{{typeof({GetTypeName("Intent.Application.DependencyInjection.DependencyInjection")}).Assembly.GetName().Name}}.xml\");",
                                        s => s.SeparatedFromPrevious());
                                    lambdaBlock.AddIfStatement("File.Exists(applicationXmlFile)", @if =>
                                    {
                                        @if
                                            .AddStatement("options.IncludeXmlComments(applicationXmlFile);")
                                            .SeparatedFromPrevious(false);
                                    });
                                }
                            })
                            .WithArgumentsOnNewLines()
                            .AddMetadata("AddSwaggerGen", true)
                        );
                        method.AddStatement($@"return services;");
                    });

                @class.AddMethod("void", "UseSwashbuckle", method =>
                {
                    method.Static();
                    method.AddParameter("IApplicationBuilder", "app", conf => conf.WithThisModifier());
                    method.AddParameter("IConfiguration", "configuration");

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
                            .AddStatement($@"options.DefaultModelRendering(ModelRendering.Example);")
                            .AddStatement($@"options.DocExpansion(DocExpansion.List);")
                            .AddStatement($@"options.ShowExtensions();")
                            .AddStatement($@"options.EnableFilter(string.Empty);"))
                        .WithArgumentsOnNewLines()
                        .AddMetadata("UseSwaggerUI", true));
                });

                if (useSimpleSchemaIdentifiers)
                {
                    @class.AddMethod("string", "GetFriendlyName", method =>
                    {
                        method.Private().Static();
                        method.AddParameter(UseType("System.Type"), "modelType");

                        method.AddIfStatement("!modelType.IsConstructedGenericType", @if =>
                        {
                            @if.AddStatement("return modelType.Name.Replace(\"[]\", \"Array\");");
                        });

                        method.AddStatement(@"var genericTypeArguments = modelType.GetGenericArguments()
                .Select(GetFriendlyName);", s => s.SeparatedFromPrevious());

                        method.AddStatement(
                            "return $\"{modelType.Name.Split('`')[0]}Of{string.Join(\"And\", genericTypeArguments)}\";",
                            s => s.SeparatedFromPrevious());
                    });
                }
            });

        if (markNonNullableFieldsAsRequired)
        {
            CreateRequireNonNullablePropertiesSchemaFilter(CSharpFile);
        }
    }

    private void CreateRequireNonNullablePropertiesSchemaFilter(CSharpFile cSharpFile)
    {
        cSharpFile.AddUsing("System.Linq");
        cSharpFile
            .AddClass("RequireNonNullablePropertiesSchemaFilter", @class =>
            {
                @class.Internal();
                @class.ExtendsClass(UseType("Swashbuckle.AspNetCore.SwaggerGen.ISchemaFilter"));
                @class.AddMethod("void", "Apply", method =>
                {
                    method.AddParameter("OpenApiSchema", "model");
                    method.AddParameter("SchemaFilterContext", "context");

                    method.AddMethodChainStatement("var additionalRequiredProps = model.Properties", chainStatement =>
                    {
                        chainStatement.AddChainStatement("Where(x => !x.Value.Nullable && !model.Required.Contains(x.Key))");
                        chainStatement.AddChainStatement("Select(x => x.Key)");
                    });

                    method.AddForEachStatement("propKey", "additionalRequiredProps", @foreach =>
                    {
                        @foreach.AddStatement("model.Required.Add(propKey);");
                    });
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