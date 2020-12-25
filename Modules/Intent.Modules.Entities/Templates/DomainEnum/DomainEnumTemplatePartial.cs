using System;
using System.Collections;
using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Templates.DomainEnum
{
    partial class DomainEnumTemplate : CSharpTemplateBase<EnumModel>
    {
        private readonly IMetadataManager _metadataManager;
        public const string TemplateId = "Intent.Entities.DomainEnum";


        public DomainEnumTemplate(EnumModel model, IOutputTarget project, IMetadataManager metadataManager)
            : base(TemplateId, project, model)
        {
            _metadataManager = metadataManager;
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
