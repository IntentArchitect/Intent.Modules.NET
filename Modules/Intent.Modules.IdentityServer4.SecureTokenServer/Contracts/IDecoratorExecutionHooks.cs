using System;
using System.Collections.Generic;

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Contracts
{
    public interface IDecoratorExecutionHooks
    {
        void BeforeTemplateExecution();
    }
}
