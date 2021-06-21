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
    public class ControllerAuthenticationSchemesDecorator : ControllerDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Security.BearerToken.Interop.IdentityServer4.ControllerAuthenticationSchemesDecorator";

        private readonly ControllerTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public ControllerAuthenticationSchemesDecorator(ControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[] { "Microsoft.AspNetCore.Authentication.JwtBearer" };
        }

        public override void UpdateOperationAuthorization(AuthorizationModel authorizationModel, OperationSecureModel secureModel)
        {
            authorizationModel.AuthenticationSchemesExpression = "JwtBearerDefaults.AuthenticationScheme";
        }

        public override void UpdateServiceAuthorization(AuthorizationModel authorizationModel, ServiceSecureModel secureModel)
        {
            authorizationModel.AuthenticationSchemesExpression = "JwtBearerDefaults.AuthenticationScheme";
        }
    }
}
