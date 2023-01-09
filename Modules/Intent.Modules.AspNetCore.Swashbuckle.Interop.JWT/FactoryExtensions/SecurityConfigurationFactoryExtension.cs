using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Swashbuckle.Interop.JWT.Events;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.SwashbuckleConfiguration;
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
            if (!_swaggerSchemes.Any())
            {
                return;
            }

            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(SwashbuckleConfigurationTemplate.TemplateId));
            if (template != null)
            {
                var @class = template.CSharpFile.Classes.First();
                var method = @class?.FindMethod("ConfigureSwagger");
                var addSwaggerGen = method
                    ?.FindStatement(p => p.HasMetadata("AddSwaggerGen")) as CSharpInvocationStatement;
                var optionsArg = addSwaggerGen?.Statements.First() as CSharpLambdaBlock;

                if (optionsArg == null)
                {
                    return;
                }

                var flowsBlock = new CSharpClassInitStatementBlock("new OpenApiOAuthFlows()");
                optionsArg.AddStatement(new CSharpInvocationStatement("options.AddSecurityDefinition")
                    .AddArgument($@"""Bearer""")
                    .AddArgument(new CSharpClassInitStatementBlock("new OpenApiSecurityScheme()")
                        .AddInitAssignment("Type", "SecuritySchemeType.OAuth2")
                        .AddInitAssignment("Flows", flowsBlock))
                    .WithArgumentsOnNewLines());
                foreach (var scheme in _swaggerSchemes)
                {
                    flowsBlock.AddInitAssignment(scheme.SchemeName, new CSharpClassInitStatementBlock("new OpenApiOAuthFlow()")
                        .AddInitAssignment("AuthorizationUrl", $@"configuration.GetValue<Uri>(""Swashbuckle:Security:Bearer:{scheme.SchemeName}:AuthorizationUrl"")")
                        .AddInitAssignment("TokenUrl", $@"configuration.GetValue<Uri>(""Swashbuckle:Security:Bearer:{scheme.SchemeName}:TokenUrl"")")
                        .AddInitAssignment("Scopes", $@"configuration.GetSection(""Swashbuckle:Security:Bearer:{scheme.SchemeName}:Scope"").Get<Dictionary<string, string>>()!.ToDictionary(x => x.Value, x=> x.Key)"));
                }
            }
        }

        private void Handle(SwaggerOAuth2SchemeEvent @event)
        {
            _swaggerSchemes.Add(@event);
        }

        private void Handle(SecureTokenServiceHostedEvent @event)
        {
            _stsPort = @event.Port;
        }
    }
}