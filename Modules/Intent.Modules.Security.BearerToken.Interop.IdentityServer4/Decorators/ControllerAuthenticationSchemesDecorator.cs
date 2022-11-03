using System;
using System.Collections.Generic;
using System.Text;
using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Security.BearerToken.Interop.IdentityServer4.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ControllerAuthenticationSchemesDecorator : ControllerDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Security.BearerToken.Interop.IdentityServer4.ControllerAuthenticationSchemesDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly ControllerTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public ControllerAuthenticationSchemesDecorator(ControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            // TODO: GCB - this isn't a great pattern at all for solving this problem. Consider converting to CSharpFile builder paradigm.
            // TODO: Could probably even get rid of this module completely.
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[] { "Microsoft.AspNetCore.Authentication.JwtBearer" };
        }

        public override void UpdateOperationAuthorization(AuthorizationModel authorizationModel, OperationSecureModel secureModel)
        {
            // Specifying this scheme explicitly is required when running IdentityServer4 alongside JWT auth as IdentityServer4 seems to override the default auth scheme (probably changing it to be cookie based).
            authorizationModel.AuthenticationSchemesExpression = "JwtBearerDefaults.AuthenticationScheme";
        }

        public override void UpdateServiceAuthorization(AuthorizationModel authorizationModel, ServiceSecureModel secureModel)
        {
            // Specifying this scheme explicitly is required when running IdentityServer4 alongside JWT auth as IdentityServer4 seems to override the default auth scheme (probably changing it to be cookie based).
            authorizationModel.AuthenticationSchemesExpression = "JwtBearerDefaults.AuthenticationScheme";
        }
    }
}
