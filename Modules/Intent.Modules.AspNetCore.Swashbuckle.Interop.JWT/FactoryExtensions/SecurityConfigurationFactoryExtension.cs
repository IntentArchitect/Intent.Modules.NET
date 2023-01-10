using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Swashbuckle.Interop.JWT.Events;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Interop.JWT.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SecurityConfigurationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Swashbuckle.Interop.JWT.SecurityConfigurationFactoryExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        private readonly List<SwaggerOAuth2SchemeEvent> _swaggerSchemes = new();
        private string _stsPort = SchemeEventConstants.STS_Port_Tag;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            application.EventDispatcher.Subscribe<SwaggerOAuth2SchemeEvent>(Handle);
            application.EventDispatcher.Subscribe<SecureTokenServiceHostedEvent>(Handle);
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

            configureSwaggerOptionsBlock.AddStatement(new CSharpInvocationStatement("options.AddSecurityDefinition")
                .AddArgument(@"""Bearer""")
                .AddArgument(new CSharpClassInitStatementBlock("new OpenApiSecurityScheme()")
                    .AddInitAssignment("Name", @"""Authorization""")
                    .AddInitAssignment("Description", @"""Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`""")
                    .AddInitAssignment("In", "ParameterLocation.Header")
                    .AddInitAssignment("Type", "SecuritySchemeType.ApiKey")
                    .AddInitAssignment("Scheme", @"""Bearer""")));

            if (_swaggerSchemes.Any())
            {
                var flowsBlock = new CSharpClassInitStatementBlock("new OpenApiOAuthFlows()");
                configureSwaggerOptionsBlock.AddStatement(new CSharpInvocationStatement("options.AddSecurityDefinition")
                    .AddArgument(@"""Bearer""")
                    .AddArgument(new CSharpClassInitStatementBlock("new OpenApiSecurityScheme()")
                        .AddInitAssignment("Type", "SecuritySchemeType.OAuth2")
                        .AddInitAssignment("Flows", flowsBlock))
                    .WithArgumentsOnNewLines());

                var castTemplate = ((IntentTemplateBase)template);
                foreach (var scheme in _swaggerSchemes)
                {
                    castTemplate.ApplyAppSetting($@"""Swashbuckle:Security:Bearer:{scheme.SchemeName}:AuthorizationUrl""", scheme.AuthorizationUrl);
                    castTemplate.ApplyAppSetting($@"""Swashbuckle:Security:Bearer:{scheme.SchemeName}:TokenUrl""", scheme.TokenUrl);
                    castTemplate.ApplyAppSetting($@"""Swashbuckle:Security:Bearer:{scheme.SchemeName}:Scope""", scheme.Scopes);

                    flowsBlock.AddInitAssignment(scheme.SchemeName, new CSharpClassInitStatementBlock("new OpenApiOAuthFlow()")
                        .AddInitAssignment("AuthorizationUrl", $@"configuration.GetValue<Uri>(""Swashbuckle:Security:Bearer:{scheme.SchemeName}:AuthorizationUrl"")")
                        .AddInitAssignment("TokenUrl", $@"configuration.GetValue<Uri>(""Swashbuckle:Security:Bearer:{scheme.SchemeName}:TokenUrl"")")
                        .AddInitAssignment("Scopes",
                            $@"configuration.GetSection(""Swashbuckle:Security:Bearer:{scheme.SchemeName}:Scope"").Get<Dictionary<string, string>>()!.ToDictionary(x => x.Value, x=> x.Key)"));
                }
            }

            var swaggerUiOptionsBlock = GetUseSwaggerUiOptionsBlock(@class);
            if (swaggerUiOptionsBlock == null)
            {
                return;
            }

            swaggerUiOptionsBlock.AddStatement($@"options.OAuthScopeSeparator("" "");")
                .AddStatement($@"options.OAuthUsePkce();");
        }

        private void Handle(SwaggerOAuth2SchemeEvent @event)
        {
            _swaggerSchemes.Add(@event);
        }

        private void Handle(SecureTokenServiceHostedEvent @event)
        {
            _stsPort = @event.Port;
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