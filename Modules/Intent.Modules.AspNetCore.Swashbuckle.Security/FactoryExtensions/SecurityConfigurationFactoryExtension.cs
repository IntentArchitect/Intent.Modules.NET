using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Swashbuckle.Security.Events;
using Intent.Modules.AspNetCore.Swashbuckle.Security.Settings;
using Intent.Modules.AspNetCore.Swashbuckle.Security.Templates.AuthorizeCheckOperationFilter;
using Intent.Modules.AspNetCore.Swashbuckle.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static System.Formats.Asn1.AsnWriter;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Security.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SecurityConfigurationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Swashbuckle.Security.SecurityConfigurationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        private string _stsPort = SchemeEventConstants.STS_Port_Tag;
        private readonly List<SwaggerOAuth2SchemeEvent> _swaggerSchemes = new();

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            application.EventDispatcher.Subscribe<SecureTokenServiceHostedEvent>(Handle);
            application.EventDispatcher.Subscribe<SwaggerOAuth2SchemeEvent>(Handle);
        }

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration"));
            if (template == null)
            {
                return;
            }

            var @class = template.CSharpFile.Classes.First();

            var configureSwaggerOptionsBlock = GetConfigureSwaggerOptionsBlock(@class);
            if (configureSwaggerOptionsBlock == null)
            {
                return;
            }
            var swaggerUiOptionsBlock = GetUseSwaggerUiOptionsBlock(@class);
            if (swaggerUiOptionsBlock == null)
            {
                return;
            }

            configureSwaggerOptionsBlock.AddStatement($@"options.OperationFilter<{template.GetTypeName(AuthorizeCheckOperationFilterTemplate.TemplateId)}>();");

            switch (application.Settings.GetSwaggerSettings().Authentication().AsEnum())
            {
                case SwaggerSettingsExtensions.AuthenticationOptionsEnum.Implicit:
                    AddOAuth2ImplicitSecurityScheme(application.Settings.GetSwaggerSettings().Authentication().Value, template, configureSwaggerOptionsBlock, swaggerUiOptionsBlock);
                    break;
                case SwaggerSettingsExtensions.AuthenticationOptionsEnum.Bearer:
                default:
                    AddBearerSecurityScheme(application.Settings.GetSwaggerSettings().Authentication().Value, template, configureSwaggerOptionsBlock);
                    break;
            }

            AddAdditionSecurityScheme(configureSwaggerOptionsBlock, swaggerUiOptionsBlock, template, application);
        }

        private void AddBearerSecurityScheme(string schemeName, ICSharpFileBuilderTemplate template, CSharpLambdaBlock configureSwaggerOptionsBlock)
        {
            //Bearer
            template.CSharpFile.AddUsing("Microsoft.AspNetCore.Authentication.JwtBearer");
            configureSwaggerOptionsBlock.AddStatement(new CSharpObjectInitializerBlock("var securityScheme = new OpenApiSecurityScheme()")
                .AddInitStatement("Name", @"""Authorization""")
                    .AddInitStatement("Description", @"""Enter a Bearer Token into the `Value` field to have it automatically prefixed with `Bearer ` and used as an `Authorization` header value for requests.""")
                .AddInitStatement("In", "ParameterLocation.Header")
                .AddInitStatement("Type", "SecuritySchemeType.Http")
                .AddInitStatement("Scheme", @"""bearer""")
                .AddInitStatement("BearerFormat", @"""JWT""")
                .AddInitStatement("Reference", new CSharpObjectInitializerBlock("new OpenApiReference")
                    .AddInitStatement("Id", "JwtBearerDefaults.AuthenticationScheme")
                    .AddInitStatement("Type", "ReferenceType.SecurityScheme"))
                .WithSemicolon());
            configureSwaggerOptionsBlock.AddStatement(new CSharpInvocationStatement("options.AddSecurityDefinition")
                .AddArgument($"\"{schemeName}\"")
                .AddArgument("securityScheme"));
            configureSwaggerOptionsBlock.AddStatement(new CSharpInvocationStatement("options.AddSecurityRequirement")
                .AddArgument(new CSharpObjectInitializerBlock("new OpenApiSecurityRequirement")
                    .AddKeyAndValue("securityScheme", "Array.Empty<string>()"))
                .WithArgumentsOnNewLines());
        }

        private void AddOAuth2ImplicitSecurityScheme(string schemeName, ICSharpFileBuilderTemplate template, CSharpLambdaBlock configureSwaggerOptionsBlock, CSharpLambdaBlock swaggerUiOptionsBlock)
        {
            //"Implicit";
            string configPrefix = $"Swashbuckle:Security:OAuth2:{schemeName}:";

            var castTemplate = ((IntentTemplateBase)template);
            castTemplate.ApplyAppSetting($@"{configPrefix}AuthorizationUrl", "https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/authorize");
            castTemplate.ApplyAppSetting($@"{configPrefix}TokenUrl", "https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token");
            castTemplate.ApplyAppSetting($@"{configPrefix}Scope", new Dictionary<string, string>() { { "{Scope Description}", "api://{ClientId}/Scope" } });
            castTemplate.ApplyAppSetting($@"{configPrefix}ClientId", "{ClientId}");

            configureSwaggerOptionsBlock.AddStatement($"var authorizationUrl = configuration.GetValue<Uri>(\"{configPrefix}AuthorizationUrl\");");
            configureSwaggerOptionsBlock.AddStatement($"var tokenUrl = configuration.GetValue<Uri>(\"{configPrefix}TokenUrl\");");
            configureSwaggerOptionsBlock.AddStatement($"var clientId = configuration.GetValue<string>(\"{configPrefix}ClientId\");");
            configureSwaggerOptionsBlock.AddStatement($"var scopes = configuration.GetSection(\"{configPrefix}Scope\").Get<Dictionary<string, string>>()!.ToDictionary(x => x.Value, x => x.Key);");
            configureSwaggerOptionsBlock.AddStatement(new CSharpObjectInitializerBlock("var securityScheme = new OpenApiSecurityScheme()")
                .AddInitStatement("Type", "SecuritySchemeType.OAuth2")
                .AddInitStatement("Flows", new CSharpObjectInitializerBlock("new OpenApiOAuthFlows")
                    .AddInitStatement("Implicit ", new CSharpObjectInitializerBlock("new OpenApiOAuthFlow")
                        .AddInitStatement("Scopes", "scopes")
                        .AddInitStatement("AuthorizationUrl", $"authorizationUrl")
                        .AddInitStatement("TokenUrl", $"tokenUrl"))
                    )
                .WithSemicolon());
            configureSwaggerOptionsBlock.AddStatement(new CSharpInvocationStatement("options.AddSecurityDefinition")
                .AddArgument($"\"{schemeName}\"")
                .AddArgument("securityScheme"));
            configureSwaggerOptionsBlock.AddStatement(new CSharpInvocationStatement("options.AddSecurityRequirement")
                .AddArgument(new CSharpObjectInitializerBlock("new OpenApiSecurityRequirement")
                    .AddKeyAndValue("securityScheme", "scopes.Select(s => s.Key).ToArray()"))
                .WithArgumentsOnNewLines());

            //SwaggerUI Block
            swaggerUiOptionsBlock.AddStatement($"var clientId = configuration.GetValue<string>(\"{configPrefix}ClientId\");");
            swaggerUiOptionsBlock.AddStatement($"var clientSecret = configuration.GetValue<string>(\"{configPrefix}ClientSecret\");");
            swaggerUiOptionsBlock.AddStatement($"options.OAuthClientId(clientId);");
            swaggerUiOptionsBlock.AddStatement($"if (!string.IsNullOrWhiteSpace(clientSecret))");
            swaggerUiOptionsBlock.AddStatement($"{{");
            swaggerUiOptionsBlock.AddStatement($"   options.OAuthClientSecret(clientSecret);");
            swaggerUiOptionsBlock.AddStatement($"}}");
        }


        private void AddAdditionSecurityScheme(CSharpLambdaBlock configureSwaggerOptionsBlock, CSharpLambdaBlock swaggerUiOptionsBlock,
            ICSharpFileBuilderTemplate template, IApplication application)
        {

            swaggerUiOptionsBlock.AddStatement($@"options.OAuthScopeSeparator("" "");");

            DetectKnownSwaggerSchemes(application);

            if (_swaggerSchemes.Any())
            {

                var flowsBlock = new CSharpObjectInitializerBlock("new OpenApiOAuthFlows()");
                configureSwaggerOptionsBlock.AddStatement(new CSharpInvocationStatement("options.AddSecurityDefinition")
                    .AddArgument(@"""OAuth2""")
                    .AddArgument(new CSharpObjectInitializerBlock("new OpenApiSecurityScheme()")
                        .AddInitStatement("Type", "SecuritySchemeType.OAuth2")
                        .AddInitStatement("Flows", flowsBlock))
                    .WithArgumentsOnNewLines());

                template.CSharpFile.AddUsing("System.Linq");
                var castTemplate = ((IntentTemplateBase)template);

                var scheme = _swaggerSchemes.MaxBy(x => x.Priority);

                if (scheme.SchemeName == "AuthorizationCode")
                {
                    swaggerUiOptionsBlock.AddStatement($@"options.OAuthUsePkce();");
                }

                castTemplate.ApplyAppSetting($@"Swashbuckle:Security:Bearer:{scheme.SchemeName}:AuthorizationUrl", scheme.AuthorizationUrl.Replace(SchemeEventConstants.STS_Port_Tag, _stsPort));
                castTemplate.ApplyAppSetting($@"Swashbuckle:Security:Bearer:{scheme.SchemeName}:TokenUrl", scheme.TokenUrl.Replace(SchemeEventConstants.STS_Port_Tag, _stsPort));
                castTemplate.ApplyAppSetting($@"Swashbuckle:Security:Bearer:{scheme.SchemeName}:Scope", scheme.Scopes);

                flowsBlock.AddInitStatement(scheme.SchemeName, new CSharpObjectInitializerBlock("new OpenApiOAuthFlow()")
                    .AddInitStatement("AuthorizationUrl", $@"configuration.GetValue<Uri>(""Swashbuckle:Security:Bearer:{scheme.SchemeName}:AuthorizationUrl"")")
                    .AddInitStatement("TokenUrl", $@"configuration.GetValue<Uri>(""Swashbuckle:Security:Bearer:{scheme.SchemeName}:TokenUrl"")")
                    .AddInitStatement("Scopes",
                        $@"configuration.GetSection(""Swashbuckle:Security:Bearer:{scheme.SchemeName}:Scope"").Get<Dictionary<string, string>>()!.ToDictionary(x => x.Value, x=> x.Key)"));
            }
        }

        private void DetectKnownSwaggerSchemes(IApplication application)
        {
            if (application.InstalledModules.Any(p => p.ModuleId == "Intent.IdentityServer4.SecureTokenServer"))
            {
                _swaggerSchemes.Add(new SwaggerOAuth2SchemeEvent(
                    schemeName: "ClientCredentials",
                    priority: 10,
                    clientId: "ClientCredentials_Client",
                    authUrl: $"https://localhost:{SchemeEventConstants.STS_Port_Tag}/connect/authorize",
                    tokenUrl: $"https://localhost:{SchemeEventConstants.STS_Port_Tag}/connect/token",
                    refreshUrl: null,
                    scopes: new Dictionary<string, string>() { { "API Scope", "api" } }));
            }

            if (application.InstalledModules.Any(p => p.ModuleId == "Intent.IdentityServer4.UI"))
            {
                _swaggerSchemes.Add(new SwaggerOAuth2SchemeEvent(
                    schemeName: "AuthorizationCode",
                    priority: 20,
                    clientId: "Auth_Code_Client",
                    authUrl: $"https://localhost:{SchemeEventConstants.STS_Port_Tag}/connect/authorize",
                    tokenUrl: $"https://localhost:{SchemeEventConstants.STS_Port_Tag}/connect/token",
                    refreshUrl: null,
                    scopes: new Dictionary<string, string>() { { "API Scope", "api" } }));
            }
        }

        private void Handle(SecureTokenServiceHostedEvent @event)
        {
            _stsPort = @event.Port;
        }

        private void Handle(SwaggerOAuth2SchemeEvent scheme)
        {
            _swaggerSchemes.Add(scheme);
        }

        private static CSharpLambdaBlock GetConfigureSwaggerOptionsBlock(CSharpClass @class)
        {
            var configureSwaggerMethod = @class.FindMethod("ConfigureSwagger");
            var addSwaggerGen = configureSwaggerMethod?.FindStatement(p => p.HasMetadata("AddSwaggerGen")) as CSharpInvocationStatement;
            var cSharpLambdaBlock = addSwaggerGen?.Statements.First() as CSharpLambdaBlock;
            return cSharpLambdaBlock;
        }

        private static CSharpLambdaBlock GetUseSwaggerUiOptionsBlock(CSharpClass @class)
        {
            var useSwashbuckleMethod = @class.FindMethod("UseSwashbuckle");
            var useSwaggerUI = useSwashbuckleMethod?.FindStatement(p => p.HasMetadata("UseSwaggerUI")) as CSharpInvocationStatement;
            var usesSwaggerUiOptionsArg = useSwaggerUI?.Statements.First() as CSharpLambdaBlock;
            return usesSwaggerUiOptionsArg;
        }
    }
}