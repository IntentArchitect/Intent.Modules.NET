using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.ObjectMapping.Templates;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.ObjectMapping.Templates.MappingExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MappingExtensionsTemplate : CSharpTemplateBase<DTOModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Dtos.ObjectMapping.MappingExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MappingExtensionsTemplate(IOutputTarget outputTarget, DTOModel model) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Application.EntityDtoMappingExtensions);
            AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            AddTypeSource(TemplateRoles.Domain.Entity.Interface);
            AddTypeSource(TemplateRoles.Domain.ValueObject);
            AddTypeSource(TemplateRoles.Domain.DataContract);
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateId);

            CSharpFile = new CSharpFile(GetDtoNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq")
                .AddClass($"{Model.Name}MappingExtensions", cls =>
                {
                    cls.Static();
                    var entityTypeName = GetEntityTypeName();
                    var dtoTypeName = GetTypeName(DtoModelTemplate.TemplateId, Model);

                    cls.AddMethod(dtoTypeName, $"MapTo{Model.Name}", method =>
                    {
                        method.Static();
                        method.AddParameter(entityTypeName, "projectFrom", p => p.WithThisModifier());

                        var initBlock = new CSharpObjectInitializerBlock($"return new {dtoTypeName}")
                            .WithSemicolon();
                        MappingHelper.AddInitializerEntries(initBlock, this, Model);
                        method.AddStatement(initBlock);
                    });

                    cls.AddMethod($"List<{dtoTypeName}>", $"MapTo{Model.Name}List", method =>
                    {
                        method.Static();
                        method.AddParameter($"IEnumerable<{entityTypeName}>", "projectFrom", p => p.WithThisModifier());
                        method.WithExpressionBody($"projectFrom.Select(x => x.MapTo{Model.Name}()).ToList()");
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            // Model.Mapping != null is the "no mapping, no class" rule — not a stand-down against
            // another Mapping Provider. Generation is unconditional with respect to other providers.
            return base.CanRunTemplate()
                && Model.Mapping != null;
        }

        /// <summary>
        /// The extension class is emitted into the DTO's own namespace so that it resolves from the
        /// DTO without an explicit using. Derived from the DTO template's output rather than by
        /// stripping ".Mappings" out of this template's namespace, which corrupts any application
        /// whose output path legitimately contains a "Mappings" segment.
        /// </summary>
        private string GetDtoNamespace()
        {
            return TryGetTemplate<ICSharpFileBuilderTemplate>(DtoModelTemplate.TemplateId, Model.Id, out var dtoTemplate)
                ? dtoTemplate.CSharpFile.Namespace
                : this.GetNamespace();
        }

        private string GetEntityTypeName()
        {
            if (TryGetTypeName(TemplateRoles.Domain.Entity.Primary, Model.Mapping.ElementId, out var name)
                || TryGetTypeName(TemplateRoles.Domain.Entity.Interface, Model.Mapping.ElementId, out name)
                || TryGetTypeName(TemplateRoles.Domain.ValueObject, Model.Mapping.ElementId, out name)
                || TryGetTypeName(TemplateRoles.Domain.DataContract, Model.Mapping.ElementId, out name))
            {
                return name;
            }

            throw new ElementException(Model.InternalElement,
                $"The DTO '{Model.Name}' is mapped from '{Model.Mapping.Element.Name}', but no domain entity, " +
                "value object or data contract type could be resolved for it, so no mapping extension class can be generated. " +
                "Check that the mapped element still exists in the Domain designer and that the module which generates its " +
                "C# type is installed, then re-apply the mapping on the DTO.");
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
