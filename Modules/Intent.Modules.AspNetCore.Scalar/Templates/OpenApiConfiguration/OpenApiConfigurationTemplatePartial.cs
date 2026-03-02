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
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.AspNetCore.Scalar.Settings.ScalarSettings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Scalar.Templates.OpenApiConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class OpenApiConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Scalar.OpenApiConfiguration";
        private const string OpenApiOidcAuthorizationUrlKey = "OpenApi:Oidc:AuthorizationUrl";
        private const string OpenApiOidcTokenUrlKey = "OpenApi:Oidc:TokenUrl";
        private const string OpenApiOidcScopesKey = "OpenApi:Oidc:Scopes";

        private const string DefaultAuthorizationUrl = "https://your-oauth-provider.com/connect/authorize";
        private const string DefaultTokenUrl = "https://your-oauth-provider.com/connect/token";

        private static readonly string[] DefaultOpenApiOidcScopes = ["api", "openid", "profile", "offline_access"];

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
                throw new ElementException(package, "Intent.AspNetCore.Swashbuckle is also installed, only Intent.AspNetCore.Swashbuckle or Intent.AspNetCore.Scalar can be installed. Uninstall one or the other module");
            }

            AddNugetDependency(NugetPackages.ScalarAspNetCore(OutputTarget));
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreOpenApi(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Generic")
                .AddClass($"OpenApiConfiguration", @class =>
                {
                    var usesMicrosoftOpenApiV2 = Project.GetMaxNetAppVersion().Major >= 10;
                    if (!usesMicrosoftOpenApiV2)
                    {
                        AddUsing("Microsoft.OpenApi.Models");
                    }
                    var schemaMetadataProperty = usesMicrosoftOpenApiV2
                        ? "Metadata"
                        : "Annotations";

                    @class.Static();

                    @class.AddMethod("IServiceCollection", "ConfigureOpenApi", configureOpenApi =>
                    {
                        configureOpenApi.Static();
                        configureOpenApi.AddParameter("IServiceCollection", "services", c => c.WithThisModifier());


                        configureOpenApi.AddStatement(new CSharpInvocationStatement("services.AddOpenApi")
                            .AddArgument(new CSharpLambdaBlock("options"), argument =>
                            {
                                var lambdaBlock = (CSharpLambdaBlock)argument;

                                lambdaBlock.AddStatement(new CSharpInvocationStatement("options.AddSchemaTransformer")
                                    .AddArgument(new CSharpLambdaBlock("(schema, context, cancellationToken)"), configure =>
                                    {
                                        configure.AddIfStatement("context.JsonTypeInfo.Type.IsValueType || context.JsonTypeInfo.Type == typeof(String) || context.JsonTypeInfo.Type == typeof(string)", @if => @if.AddReturn("Task.CompletedTask"));
                                        configure.AddIfStatement($"schema.{schemaMetadataProperty} == null || !schema.{schemaMetadataProperty}.TryGetValue(\"x-schema-id\", out object? _)", @if => @if.AddReturn("Task.CompletedTask"));

                                        configure.AddStatement("var schemaId = SchemaIdSelector(context.JsonTypeInfo.Type);", s => s.SeparatedFromPrevious());
                                        configure.AddStatement($"schema.{schemaMetadataProperty}[\"x-schema-id\"] = schemaId;");
                                        configure.AddStatement("schema.Title = schemaId;");

                                        configure.AddStatement("return Task.CompletedTask;", s => s.SeparatedFromPrevious());
                                    }).WithArgumentsOnNewLines());

                                var authenticationType = ExecutionContext.GetSettings().GetScalarSettings().Authentication().AsEnum();
                                if (authenticationType != AuthenticationOptionsEnum.None)
                                {
                                    lambdaBlock.AddStatement(new CSharpInvocationStatement("options.AddDocumentTransformer")
                                        .AddArgument(new CSharpLambdaBlock("(document, context, cancellationToken)"), configure =>
                                        {
                                            switch (authenticationType)
                                            {
                                                case AuthenticationOptionsEnum.Bearer:
                                                    if (!usesMicrosoftOpenApiV2)
                                                    {
                                                        configure.AddStatement("document.Components ??= new();");
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
                                                    else
                                                    {
                                                        AddUsing("Microsoft.OpenApi");
                                                        configure.AddStatement("document.Components ??= new OpenApiComponents();", s => s.SeparatedFromPrevious());
                                                        configure.AddStatement("document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();");
                                                        configure.AddInvocationStatement("document.Components.SecuritySchemes.Add", c =>
                                                        {
                                                            c.AddArgument("\"bearer\"");
                                                            c.AddArgument(new CSharpObjectInitializerBlock("new OpenApiSecurityScheme")
                                                                .AddInitStatement("Type", "SecuritySchemeType.Http")
                                                                .AddInitStatement("Scheme", "\"bearer\"")
                                                                .AddInitStatement("In", "ParameterLocation.Header")
                                                                .AddInitStatement("BearerFormat", "\"Json Web Token\""));
                                                        });

                                                        configure.AddForEachStatement("operation", "document.Paths.Values.SelectMany(path => path.Operations ?? [])", @foreach =>
                                                        {
                                                            @foreach.AddStatement("operation.Value.Security ??= [];");
                                                            @foreach.AddInvocationStatement("operation.Value.Security.Add", invocation =>
                                                            {
                                                                invocation.AddArgument(new CSharpObjectInitializerBlock("new OpenApiSecurityRequirement")
                                                                    .AddKeyAndValue("new OpenApiSecuritySchemeReference(\"Bearer\", document)", "[]"));
                                                            });
                                                        });
                                                    }
                                                    break;
                                                case AuthenticationOptionsEnum.Implicit:
                                                    configure.AddStatement("var configuration = context.ApplicationServices.GetRequiredService<IConfiguration>();");
                                                    configure.AddStatement("var oidcSection = configuration.GetSection(\"OpenApi:Oidc\");");
                                                    configure.AddStatement("var authorizationUrl = oidcSection.GetValue<string>(\"AuthorizationUrl\");");
                                                    configure.AddStatement("var scopes = oidcSection.GetSection(\"Scopes\").Get<string[]>() ?? Array.Empty<string>();");
                                                    configure.AddIfStatement("string.IsNullOrEmpty(authorizationUrl)", @if => @if.AddStatement("throw new ArgumentException(\"You have not configured your AuthorizationUrl\", nameof(authorizationUrl));"));

                                                    if (!usesMicrosoftOpenApiV2)
                                                    {
                                                        configure.AddStatement("document.Components ??= new();", s => s.SeparatedFromPrevious());
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
                                                    else
                                                    {
                                                        AddUsing("Microsoft.OpenApi");
                                                        configure.AddStatement("document.Components ??= new OpenApiComponents();", s => s.SeparatedFromPrevious());
                                                        configure.AddStatement("document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();");
                                                        configure.AddInvocationStatement("document.Components.SecuritySchemes.Add", c =>
                                                        {
                                                            c.AddArgument("\"oauth2\"");
                                                            c.AddArgument(new CSharpObjectInitializerBlock("new OpenApiSecurityScheme")
                                                                .AddInitStatement("Type", "SecuritySchemeType.OAuth2")
                                                                .AddInitStatement("Flows", new CSharpObjectInitializerBlock("new OpenApiOAuthFlows")
                                                                    .AddInitStatement("Implicit", new CSharpObjectInitializerBlock("new OpenApiOAuthFlow")
                                                                        .AddInitStatement("AuthorizationUrl", "new Uri(authorizationUrl)")
                                                                        .AddInitStatement("Scopes", "scopes.ToDictionary(s => s, s => $\"Access to {s}\")"))));
                                                        });

                                                        configure.AddStatement("document.Security ??= new List<OpenApiSecurityRequirement>();", s => s.SeparatedFromPrevious());
                                                        configure.AddInvocationStatement("document.Security.Add", c =>
                                                        {
                                                            c.AddArgument(new CSharpObjectInitializerBlock("new OpenApiSecurityRequirement")
                                                                .AddStatement(new CSharpObjectInitializerBlock(null)
                                                                    .AddStatement("new OpenApiSecuritySchemeReference(\"oauth2\")")
                                                                    .AddStatement("scopes.ToList()")));
                                                        });

                                                        configure.AddStatement("document.SetReferenceHostDocument();", s => s.SeparatedFromPrevious());
                                                    }
                                                    break;
                                                case AuthenticationOptionsEnum.AuthorizationCode:
                                                    configure.AddStatement("var configuration = context.ApplicationServices.GetRequiredService<IConfiguration>();");
                                                    configure.AddStatement("var oidcSection = configuration.GetSection(\"OpenApi:Oidc\");");
                                                    configure.AddStatement("var authorizationUrl = oidcSection.GetValue<string>(\"AuthorizationUrl\");");
                                                    configure.AddStatement("var tokenUrl = oidcSection.GetValue<string>(\"TokenUrl\");");
                                                    configure.AddStatement("var scopes = oidcSection.GetSection(\"Scopes\").Get<string[]>() ?? Array.Empty<string>();");
                                                    configure.AddIfStatement("string.IsNullOrEmpty(authorizationUrl)", @if => @if.AddStatement("throw new ArgumentException(\"You have not configured your AuthorizationUrl\", nameof(authorizationUrl));"));
                                                    configure.AddIfStatement("string.IsNullOrEmpty(tokenUrl)", @if => @if.AddStatement("throw new ArgumentException(\"You have not configured your TokenUrl\", nameof(tokenUrl));"));

                                                    if (!usesMicrosoftOpenApiV2)
                                                    {
                                                        configure.AddStatement("document.Components ??= new();", s => s.SeparatedFromPrevious());
                                                        configure.AddStatement(new CSharpObjectInitializerBlock("document.Components.SecuritySchemes[\"OidcAuthCode\"] = new()")
                                                            .AddInitStatement("Type", "SecuritySchemeType.OAuth2")
                                                            .AddInitStatement("Flows", new CSharpObjectInitializerBlock("new OpenApiOAuthFlows")
                                                                .AddInitStatement("AuthorizationCode", new CSharpObjectInitializerBlock("new OpenApiOAuthFlow")
                                                                    .AddInitStatement("AuthorizationUrl", "new Uri(authorizationUrl)")
                                                                    .AddInitStatement("TokenUrl", "new Uri(tokenUrl)")
                                                                    .AddInitStatement("Scopes", "scopes.ToDictionary(s => s, s => $\"Access to {s}\")")))
                                                            .WithSemicolon());

                                                        configure.AddStatement(new CSharpObjectInitializerBlock("var authCodeSchemeReference = new OpenApiSecurityScheme")
                                                            .AddInitStatement("Reference", new CSharpObjectInitializerBlock("new OpenApiReference")
                                                                .AddInitStatement("Id", "\"OidcAuthCode\"")
                                                                .AddInitStatement("Type", "ReferenceType.SecurityScheme")
                                                            )
                                                            .WithSemicolon());

                                                        configure.AddStatement(new CSharpObjectInitializerBlock("var securityStatement = new OpenApiSecurityRequirement")
                                                            .AddInitStatement("[authCodeSchemeReference]", "scopes.ToList()").WithSemicolon());

                                                        configure.AddStatement("document.SecurityRequirements.Add(securityStatement);");
                                                    }
                                                    else
                                                    {
                                                        AddUsing("Microsoft.OpenApi");
                                                        configure.AddStatement("document.Components ??= new OpenApiComponents();", s => s.SeparatedFromPrevious());
                                                        configure.AddStatement("document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();");
                                                        configure.AddInvocationStatement("document.Components.SecuritySchemes.Add", c =>
                                                        {
                                                            c.AddArgument("\"oauth2\"");
                                                            c.AddArgument(new CSharpObjectInitializerBlock("new OpenApiSecurityScheme")
                                                                .AddInitStatement("Type", "SecuritySchemeType.OAuth2")
                                                                .AddInitStatement("Flows", new CSharpObjectInitializerBlock("new OpenApiOAuthFlows")
                                                                    .AddInitStatement("AuthorizationCode", new CSharpObjectInitializerBlock("new OpenApiOAuthFlow")
                                                                        .AddInitStatement("AuthorizationUrl", "new Uri(authorizationUrl)")
                                                                        .AddInitStatement("TokenUrl", "new Uri(tokenUrl)")
                                                                        .AddInitStatement("Scopes", "scopes.ToDictionary(s => s, s => $\"Access to {s}\")"))));
                                                        });

                                                        configure.AddStatement("document.Security ??= new List<OpenApiSecurityRequirement>();", s => s.SeparatedFromPrevious());
                                                        configure.AddInvocationStatement("document.Security.Add", c =>
                                                        {
                                                            c.AddArgument(new CSharpObjectInitializerBlock("new OpenApiSecurityRequirement")
                                                                .AddStatement(new CSharpObjectInitializerBlock(null)
                                                                    .AddStatement("new OpenApiSecuritySchemeReference(\"oauth2\")")
                                                                    .AddStatement("scopes.ToList()")));
                                                        });

                                                        configure.AddStatement("document.SetReferenceHostDocument();", s => s.SeparatedFromPrevious());
                                                    }
                                                    break;
                                                case AuthenticationOptionsEnum.None:
                                                default:
                                                    throw new ArgumentOutOfRangeException($"Unsupported authentication type {authenticationType}");
                                            }

                                            configure.AddStatement("return Task.CompletedTask;");
                                        }));
                                }
                            })
                            .WithArgumentsOnNewLines()
                        );
                        configureOpenApi.AddReturn("services");
                    });

                    if (ExecutionContext.GetSettings().GetScalarSettings().UseFullyQualifiedSchemaIdentifiers())
                    {
                        @class.AddMethod("string", "SchemaIdSelector", method =>
                        {
                            method.Private().Static();
                            method.AddParameter(UseType("System.Type"), "modelType");

                            method.AddIfStatement("modelType.IsArray", @if =>
                            {
                                @if.AddStatement(@"var elementType = modelType.GetElementType()!;");
                                @if.AddStatement(@"return $""{SchemaIdSelector(elementType)}Array"";");
                            });

                            method.AddStatement(@"var typeName = modelType.FullName?.Replace(""+"", ""_"") ?? modelType.Name.Replace(""+"", ""_"");", s => s.SeparatedFromPrevious());

                            method.AddIfStatement("!modelType.IsConstructedGenericType", @if =>
                            {
                                @if.AddStatement("return typeName;");
                            });

                            method.AddStatement(@"var genericTypeDefName = modelType.GetGenericTypeDefinition().FullName;", s => s.SeparatedFromPrevious());
                            method.AddStatement(@"var baseName = (genericTypeDefName?.Split('`')[0] ?? modelType.Name.Split('`')[0]).Replace(""+"", ""_"");");

                            method.AddStatement(@"var genericArgs = modelType.GetGenericArguments()
                .Select(SchemaIdSelector)
                .ToArray();", s => s.SeparatedFromPrevious());

                            method.AddStatement(@"return $""{baseName}_Of_{string.Join(""_And_"", genericArgs)}"";", s => s.SeparatedFromPrevious());
                        });
                    }
                    else
                    {
                        @class.AddMethod("string", "SchemaIdSelector", method =>
                        {
                            method.Private().Static();
                            method.AddParameter(UseType("System.Type"), "modelType");

                            method.AddIfStatement("modelType.IsArray", @if =>
                            {
                                @if.AddStatement(@"var elementType = modelType.GetElementType()!;");
                                @if.AddStatement(@"return $""{SchemaIdSelector(elementType)}Array"";");
                            });

                            method.AddStatement(@"var modelName = modelType.Name.Replace(""+"", ""_"");", s => s.SeparatedFromPrevious());

                            method.AddIfStatement("!modelType.IsConstructedGenericType", @if =>
                            {
                                @if.AddStatement("return modelName;");
                            });

                            method.AddStatement(@"var baseName = modelName.Split('`').First();", s => s.SeparatedFromPrevious());

                            method.AddStatement(@"var genericArgs = modelType.GetGenericArguments()
                .Select(SchemaIdSelector)
                .ToArray();", s => s.SeparatedFromPrevious());

                            method.AddStatement(@"return $""{baseName}Of{string.Join(""And"", genericArgs)}"";", s => s.SeparatedFromPrevious());
                        });
                    }
                });
        }

        public override void BeforeTemplateExecution()
        {
            switch (ExecutionContext.GetSettings().GetScalarSettings().Authentication().AsEnum())
            {
                case AuthenticationOptionsEnum.AuthorizationCode:
                    this.ApplyAppSetting(OpenApiOidcAuthorizationUrlKey, DefaultAuthorizationUrl);
                    this.ApplyAppSetting(OpenApiOidcTokenUrlKey, DefaultTokenUrl);
                    this.ApplyAppSetting(OpenApiOidcScopesKey, DefaultOpenApiOidcScopes);
                    break;
                case AuthenticationOptionsEnum.Implicit:
                    this.ApplyAppSetting(OpenApiOidcAuthorizationUrlKey, DefaultAuthorizationUrl);
                    this.ApplyAppSetting(OpenApiOidcScopesKey, DefaultOpenApiOidcScopes);
                    break;
                case AuthenticationOptionsEnum.Bearer:
                case AuthenticationOptionsEnum.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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