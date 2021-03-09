using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Application.Security.BearerToken.Interop.IdentityServer4.Decorators
{
    public class AuthenticationSchemesDecorator : ControllerDecorator, IDeclareUsings
    {
        public const string DecoratorId = "Application.Security.BearerToken.Interop.IdentityServer4.AuthenticationSchemesDecorator";

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
