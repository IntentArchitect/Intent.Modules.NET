using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Pagination.Templates.PagedResultMappingExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class PagedResultMappingExtensionsTemplate : CSharpTemplateBase<IList<DTOModel>>
    {
        public const string TemplateId = "Intent.Application.Dtos.Pagination.PagedResultMappingExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PagedResultMappingExtensionsTemplate(IOutputTarget outputTarget, IList<DTOModel> model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"PagedResultMappingExtensions",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetDtoModelName(DTOModel dto)
        {
            return GetTypeName(Roles.Application.ContractDto, dto);
        }

        private string GetEntityName(DTOModel dto)
        {
            return GetTypeName(Roles.Domain.EntityInterface, dto.Mapping.ElementId);
        }

        private string GetPagedResultInterfaceName()
        {
            return GetTypeName(Roles.Repository.InterfacePagedResult);
        }
    }
}