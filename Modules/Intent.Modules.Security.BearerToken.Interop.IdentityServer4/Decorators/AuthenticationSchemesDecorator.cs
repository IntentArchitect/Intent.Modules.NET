using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Security.BearerToken.Interop.IdentityServer4.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class AuthenticationSchemesDecorator : ControllerDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Security.BearerToken.Interop.IdentityServer4.AuthenticationSchemesDecorator";

        private readonly ControllerTemplate _template;
        private readonly IApplication _application;

        public AuthenticationSchemesDecorator(ControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[] { "Microsoft.AspNetCore.Authentication.JwtBearer" };
        }

        public override void UpdateOperationAuhtorization(AuthorizationModel authorizationModel, OperationSecureModel secureModel)
        {
            authorizationModel.AuthenticationSchemesExpression = "JwtBearerDefaults.AuthenticationScheme";
        }

        public override void UpdateServiceAuhtorization(AuthorizationModel authorizationModel, ServiceSecureModel secureModel)
        {
            authorizationModel.AuthenticationSchemesExpression = "JwtBearerDefaults.AuthenticationScheme";
        }
    }
}
