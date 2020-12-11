using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Selfhost.Templates.IdentityConfig
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class IdentityConfigTemplate : CSharpTemplateBase<object, IdentityConfigDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "IdentityServer4.Selfhost.IdentityConfig";

        public IdentityConfigTemplate(IOutputTarget outputTarget, object model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.IdentityServer4);
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IdentityConfig",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public string GetClients(int tabSubIndents)
        {
            var indents = string.Join("    ", Enumerable.Range(0, tabSubIndents).Select(s => string.Empty));
            var sb = new StringBuilder();

            GetDecorators()
                .SelectMany(s => s.GetClients())
                .ToList()
                .ForEach(x => GetFormattedEntity(sb, indents, x));

            return sb.ToString();
        }

        private void GetFormattedEntity(StringBuilder sb, string indents, Entity entity)
        {
            sb.Append(indents).AppendLine($"new Client");
            sb.Append(indents).AppendLine("{");
            foreach (var field in entity)
            {
                sb.Append(indents).Append("    ").AppendLine($"{field.Key} = {field.Value},");
            }
            sb.Append(indents).AppendLine("},");
        }
    }
}