using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Swashbuckle.Security.Events;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

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

            AddDefaultSecurityScheme(configureSwaggerOptionsBlock);

            var swaggerUiOptionsBlock = GetUseSwaggerUiOptionsBlock(@class);
            if (swaggerUiOptionsBlock == null)
            {
                return;
            }

            swaggerUiOptionsBlock.AddStatement($@"options.OAuthScopeSeparator("" "");");
            AddAdditionSecurityScheme(configureSwaggerOptionsBlock, swaggerUiOptionsBlock, template, application);
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

        private void AddDefaultSecurityScheme(CSharpLambdaBlock configureSwaggerOptionsBlock)
        {
            configureSwaggerOptionsBlock.AddStatement(new CSharpInvocationStatement("options.AddSecurityDefinition")
                .AddArgument(@"""ApiToken""")
                .AddArgument(new CSharpObjectInitializerBlock("new OpenApiSecurityScheme()")
                    .AddInitStatement("Name", @"""Authorization""")
                    .AddInitStatement("Description", @"""Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`""")
                    .AddInitStatement("In", "ParameterLocation.Header")
                    .AddInitStatement("Type", "SecuritySchemeType.ApiKey")
                    .AddInitStatement("Scheme", @"""Bearer""")));
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