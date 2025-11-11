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

namespace Intent.Modules.IdentityServer4.UI.Templates.Controllers.ExternalController
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ExternalControllerTemplate : CSharpTemplateBase<object, ExternalAuthProviderDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IdentityServer4.UI.Controllers.ExternalController";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ExternalControllerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ExternalController",
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

        private string _authProviderUserType;
        public string AuthProviderUserType
        {
            get
            {
                if (_authProviderUserType == null)
                {
                    _authProviderUserType = GetDecorators().Select(s => s.GetAuthProviderUserType()).First(p => !string.IsNullOrEmpty(p));
                }
                return _authProviderUserType;
            }
        }

        public string GetAutoProvisionUserMethodCode()
        {
            return GetDecorators()
                .Select(s => s.GetAutoProvisionUserMethodCode())
                .First(p => !string.IsNullOrEmpty(p));
        }

        public string GetUserLookupCodeExpression()
        {
            return GetDecorators()
                .Select(s => s.GetUserLookupCodeExpression())
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