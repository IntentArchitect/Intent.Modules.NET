using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.IdentityServer4.SecureTokenServer.Events;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class IdentityServerConfigurationTemplate : CSharpTemplateBase<object, IdentityConfigurationDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IdentityServer4.SecureTokenServer.IdentityServerConfiguration";

        private bool _hasCertificateSpecified;

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public IdentityServerConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.IdentityServer4);
            base.Project.ExecutionContext.EventDispatcher.Subscribe<CertificateSpecifiedEvent>(evt =>
            {
                _hasCertificateSpecified = true;
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IdentityServerConfiguration",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public string GetServiceConfigurations(string baseIndent)
        {
            var methodElements = new List<(string Code, int Priority)>();
            methodElements.AddRange(GetDecorators().Select(s => (s.ConfigureServices(), s.Priority)));

            if (!_hasCertificateSpecified)
            {
                methodElements.Add(("idServerBuilder.AddDeveloperSigningCredential();", -8));
            }

            return GetCodeInNeatLines(methodElements, baseIndent);
        }

        private string GetCodeInNeatLines(List<(string Code, int Priority)> codeSections, string baseIndent)
        {
            codeSections.Sort(Comparer<(string Code, int Priority)>
                .Create((x, y) => x.Priority.CompareTo(y.Priority)));

            var sb = new StringBuilder();

            foreach (var element in codeSections)
            {
                var codeLines = element.Code
                    .Trim()
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in codeLines)
                {
                    sb.Append(baseIndent).AppendLine(line);
                }
            }

            return sb.ToString();
        }
    }
}