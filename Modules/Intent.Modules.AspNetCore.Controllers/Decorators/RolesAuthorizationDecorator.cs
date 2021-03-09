using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class RolesAuthorizationDecorator : ControllerDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Controllers.RolesAuthorizationDecorator";

        private readonly ControllerTemplate _template;

        public RolesAuthorizationDecorator(ControllerTemplate template)
        {
            _template = template;
        }

        public override void UpdateServiceAuhtorization(AuthorizationModel authorizationModel, ServiceSecureModel secureModel)
        {
            UpdateRoles(authorizationModel, secureModel.Stereotype.Roles() ?? string.Empty);
        }

        public override void UpdateOperationAuhtorization(AuthorizationModel authorizationModel, OperationSecureModel secureModel)
        {
            UpdateRoles(authorizationModel, secureModel.Stereotype.Roles() ?? string.Empty);
        }

        private static void UpdateRoles(AuthorizationModel authorizationModel, string roles)
        {
            var interimExpression = string.Join(",", roles
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()));
            if (!string.IsNullOrWhiteSpace(interimExpression))
            {
                authorizationModel.RolesExpression = $@"""{interimExpression}""";
            }
        }
    }
}