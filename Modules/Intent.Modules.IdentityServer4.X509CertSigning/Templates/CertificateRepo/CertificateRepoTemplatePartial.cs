using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.X509CertSigning.Templates.CertificateRepo
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class CertificateRepoTemplate : CSharpTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IdentityServer4.X509CertSigning.CertificateRepo";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CertificateRepoTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"CertificateRepo",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public override void BeforeTemplateExecution()
        {
            this.ApplyAppSetting("CertificateOptions", new
            {
                Store = new
                {
                    FindType = "FindBySubjectName",
                    FindValue = "localhost",
                    StoreName = "My",
                    StoreLocation = "LocalMachine"
                }
            });
        }
    }
}