using Intent.Engine;
using Intent.Modules.Application.Identity.Settings;
using Intent.Modules.Entities.BasicAuditing.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Intent.Modules.Entities.BasicAuditing.Templates
{
    internal class TemplateHelper
    {

        public static string GetUserIdentifierType(ISoftwareFactoryExecutionContext executionContext)
        {
            string userIdType;
            switch (executionContext.Settings.GetBasicAuditing().UserIdentityToAudit().AsEnum())
            {
                case Settings.BasicAuditing.UserIdentityToAuditOptionsEnum.UserName:
                    userIdType = "string";
                    break;
                case Settings.BasicAuditing.UserIdentityToAuditOptionsEnum.UserId:
                default:
                    userIdType = executionContext.Settings.GetIdentitySettings().UserIdType().ToCSharpType();
                    break;
            }
            return userIdType;
        }
    }
}
