using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.AspNetCore.Scalar.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Scalar.Templates.OpenApiConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class OpenApiConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Scalar.OpenApiConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public OpenApiConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var package = this.ExecutionContext.MetadataManager.GetDesigner(this.ExecutionContext.GetApplicationConfig().Id, Designers.Services).Packages.FirstOrDefault();
            if (package is null)
            {
                throw new Exception("No package found. Please create a package and uninstall and re-install the Intent.AspNetCore.IdentityService module.");
            }
            var versionCheck = OutputTarget.GetProject().GetMaxNetAppVersion();
            if (versionCheck.Major < 9)
            {
                throw new ElementException(package, "Microsoft.AspNetCore.OpenApi is only supported on .NET 9 or greater, please update your application.");
            }

            if (ExecutionContext.InstalledModules.Any(x => x.ModuleId == "Intent.AspNetCore.Swashbuckle"))
            {
                throw new ElementException(package, "Intent.AspNetCore.Swashbucle is also installed, only Intent.AspNetCore.Swashbuckle or Intent.AspNetCore.Scalar can be installed. Uninstall one or the other module");
            }

            AddNugetDependency(NugetPackages.ScalarAspNetCore(OutputTarget));
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreOpenApi(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.OpenApi.Models")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("System")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Generic")
                .AddClass($"OpenApiConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod("IServiceCollection", "ConfigureOpenApi", configureOpenApi =>
                    {
                        configureOpenApi.Static();
                        configureOpenApi.AddParameter("IServiceCollection", "services", c => c.WithThisModifier());


                        configureOpenApi.AddStatement(new CSharpInvocationStatement("services.AddOpenApi")
                            .AddArgument(new CSharpLambdaBlock("options"), argument =>
                            {
                                var lambdaBlock = (CSharpLambdaBlock)argument;
                                if (ExecutionContext.GetSettings().GetScalarSettings().UseFullyQualifiedSchemaIdentifiers())
                                {
                                    lambdaBlock.AddStatement(new CSharpInvocationStatement("options.AddSchemaTransformer")
                                        .AddArgument(new CSharpLambdaBlock("(schema, context, cancellationToken)"), configure =>
                                        {
                                            configure.AddIfStatement("context.JsonTypeInfo.Type.IsValueType || context.JsonTypeInfo.Type == typeof(String) || context.JsonTypeInfo.Type == typeof(string)", @if => @if.AddReturn("Task.CompletedTask"));
                                            configure.AddIfStatement("schema.Annotations == null || !schema.Annotations.TryGetValue(\"x-schema-id\", out object? _)", @if => @if.AddReturn("Task.CompletedTask"));

                                            configure.AddStatement("string? transformedTypeName = context.JsonTypeInfo.Type.FullName?.Replace(\"+\", \".\", StringComparison.Ordinal);");
                                            configure.AddStatement("schema.Annotations[\"x-schema-id\"] = transformedTypeName;");
                                            configure.AddStatement("schema.Title = transformedTypeName;");

                                            configure.AddStatement("return Task.CompletedTask;");
                                        }).WithArgumentsOnNewLines());
                                }

                                lambdaBlock.AddStatement(new CSharpInvocationStatement("options.AddDocumentTransformer")
                                        .AddArgument(new CSharpLambdaBlock("(document, context, cancellationToken)"), configure =>
                                        {
                                            configure.AddStatement("document.Components ??= new();");

                                            if (ExecutionContext.GetSettings().GetScalarSettings().Authentication().IsBearer())
                                            {
                                                configure.AddStatement(new CSharpObjectInitializerBlock("document.Components.SecuritySchemes[\"Bearer\"] = new()")
                                                    .AddInitStatement("Type", "SecuritySchemeType.Http")
                                                    .AddInitStatement("Scheme", "\"bearer\"")
                                                    .AddInitStatement("BearerFormat", "\"JWT\"")
                                                    .WithSemicolon());

                                                configure.AddStatement(new CSharpObjectInitializerBlock("var bearerSchemeReference = new OpenApiSecurityScheme")
                                                    .AddInitStatement("Reference", new CSharpObjectInitializerBlock("new OpenApiReference")
                                                        .AddInitStatement("Id", "\"Bearer\"")
                                                        .AddInitStatement("Type", "ReferenceType.SecurityScheme")
                                                    )
                                                    .WithSemicolon());

                                                configure.AddStatement(new CSharpObjectInitializerBlock("var securityStatement = new OpenApiSecurityRequirement")
                                                    .AddInitStatement("[bearerSchemeReference]", "new List<string>()").WithSemicolon());

                                                configure.AddStatement("document.SecurityRequirements.Add(securityStatement);");
                                            }
                                            else if (ExecutionContext.GetSettings().GetScalarSettings().Authentication().IsImplicit())
                                            {
                                                configure.AddStatement("var configuration = context.ApplicationServices.GetRequiredService<IConfiguration>();");

                                                configure.AddStatement("var oidcSection = configuration.GetSection(\"OpenApi:Oidc\");");
                                                configure.AddStatement("var authorizationUrl = oidcSection.GetValue<string>(\"AuthorizationUrl\");");
                                                configure.AddStatement("var scopes = oidcSection.GetSection(\"Scopes\").Get<string[]>() ?? Array.Empty<string>();");

                                                configure.AddIfStatement("string.IsNullOrEmpty(authorizationUrl)", @if => @if.AddStatement("throw new ArgumentException(\"You have not configured your AuthorizationUrl\", nameof(authorizationUrl));"));

                                                configure.AddStatement(new CSharpObjectInitializerBlock("document.Components.SecuritySchemes[\"OidcImplicit\"] = new()")
                                                   .AddInitStatement("Type", "SecuritySchemeType.OAuth2")
                                                   .AddInitStatement("Flows", new CSharpObjectInitializerBlock("new OpenApiOAuthFlows")
                                                        .AddInitStatement("Implicit", new CSharpObjectInitializerBlock("new OpenApiOAuthFlow")
                                                            .AddInitStatement("AuthorizationUrl", "new Uri(authorizationUrl)")
                                                            .AddInitStatement("Scopes", "scopes.ToDictionary(s => s, s => $\"Access to {s}\")")))
                                                   .WithSemicolon());

                                                configure.AddStatement(new CSharpObjectInitializerBlock("var oidcSchemeReference = new OpenApiSecurityScheme")
                                                    .AddInitStatement("Reference", new CSharpObjectInitializerBlock("new OpenApiReference")
                                                        .AddInitStatement("Id", "\"OidcImplicit\"")
                                                        .AddInitStatement("Type", "ReferenceType.SecurityScheme")
                                                    )
                                                    .WithSemicolon());

                                                configure.AddStatement(new CSharpObjectInitializerBlock("var securityStatement = new OpenApiSecurityRequirement")
                                                    .AddInitStatement("[oidcSchemeReference]", "scopes.ToList()").WithSemicolon());

                                                configure.AddStatement("document.SecurityRequirements.Add(securityStatement);");
                                            }

                                            configure.AddStatement("return Task.CompletedTask;");
                                        })
                                    );
                            })
                            .WithArgumentsOnNewLines()
                        );
                        configureOpenApi.AddReturn("services");
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