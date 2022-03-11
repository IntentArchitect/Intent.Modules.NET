using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Entities.Templates.DomainEnum
{
    partial class DomainEnumTemplate : CSharpTemplateBase<EnumModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Entities.DomainEnum";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public DomainEnumTemplate(IOutputTarget outputTarget, EnumModel model) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole("Domain.Enums");
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        private string GetEnumLiterals(IEnumerable<EnumLiteralModel> literals)
        {
            return string.Join(@",
            ", literals.Select(GetEnumLiteral));
        }

        private string GetEnumLiteral(EnumLiteralModel literal)
        {
            return $"{literal.Name}{(string.IsNullOrWhiteSpace(literal.Value) ? "" : $" = {literal.Value}")}";
        }
    }
}
