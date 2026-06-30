using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
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

            CSharpFile = new CSharpFile(this.GetNamespace().Replace(".Mappings", ""), this.GetFolderPath())
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
            return base.CanRunTemplate()
                && Model.Mapping != null
                && !ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Application.Dtos.AutoMapper");
        }

        private string GetEntityTypeName()
        {
            return TryGetTypeName(TemplateRoles.Domain.Entity.Primary, Model.Mapping.ElementId, out var name)
                || TryGetTypeName(TemplateRoles.Domain.Entity.Interface, Model.Mapping.ElementId, out name)
                || TryGetTypeName(TemplateRoles.Domain.ValueObject, Model.Mapping.ElementId, out name)
                || TryGetTypeName(TemplateRoles.Domain.DataContract, Model.Mapping.ElementId, out name)
                ? name
                : throw new System.Exception($"Could not resolve mapped domain type '{Model.Mapping.Element.Name}'");
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
