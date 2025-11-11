using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Templates.Controllers.AccountController
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AccountControllerTemplate : CSharpTemplateBase<object, AccountAuthProviderDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IdentityServer4.UI.Controllers.AccountController";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AccountControllerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"AccountController",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string _authProviderVariableName;
        public string AuthProviderVariableName
        {
            get
            {
                if (_authProviderVariableName == null)
                {
                    _authProviderVariableName = GetDecorators().Select(s => s.GetAuthProviderVariableName()).First(p => !string.IsNullOrEmpty(p));
                }
                return _authProviderVariableName;
            }
        }

        private string _authProviderType;
        public string AuthProviderType
        {
            get
            {
                if (_authProviderType == null)
                {
                    _authProviderType = GetDecorators().Select(s => s.GetAuthProviderType()).First(p => !string.IsNullOrEmpty(p));
                }
                return _authProviderType;
            }
        }

        public string GetPreAuthenticationCode()
        {
            return GetDecorators()
                .Select(s => s.GetPreAuthenticationCode())
                .FirstOrDefault(p => !string.IsNullOrEmpty(p))
                ?? string.Empty;
        }

        public string GetAuthenticationCheckCodeExpression()
        {
            return GetDecorators()
                .Select(s => s.GetAuthenticationCheckCodeExpression())
                .First(p => !string.IsNullOrEmpty(p));
        }

        public string GetUserMappingCode()
        {
            return GetDecorators()
                .Select(s => s.GetUserMappingCode())
                .First(p => !string.IsNullOrEmpty(p));
        }
    }
}