using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.AutoMapper;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.AutoMapper.Templates.MappingExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class MappingExtensionsTemplate : CSharpTemplateBase<DTOModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Dtos.AutoMapper.MappingExtensions";

        public MappingExtensionsTemplate(IOutputTarget outputTarget, DTOModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AutoMapper);
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}MappingExtensions",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: this.GetFolderPath());
        }

        private string GetDtoModelName()
        {
            return GetTypeName(DtoModelTemplate.TemplateId, Model);
        }

        private string GetEntityName()
        {
            return GetTypeName(DomainEntityInterfaceTemplate.Identifier, Model.Mapping.ElementId);
        }
    }
}