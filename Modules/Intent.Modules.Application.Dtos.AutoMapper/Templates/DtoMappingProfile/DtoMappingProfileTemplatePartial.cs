using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.AutoMapper.Templates.MapFromInterface;
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

namespace Intent.Modules.Application.Dtos.AutoMapper.Templates.DtoMappingProfile
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DtoMappingProfileTemplate : CSharpTemplateBase<DTOModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Dtos.AutoMapper.DtoMappingProfile";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DtoMappingProfileTemplate(IOutputTarget outputTarget, DTOModel model) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Application.EntityDtoMappingExtensions);
            IsDiscoverable = ExecutionContext.GetSettings().GetAutoMapperSettings().ProfileLocation().IsProfileSeparate();

            CSharpFile = new CSharpFile(this.GetNamespace().Replace(".Mappings", ""), this.GetFolderPath())
                .AddUsing("AutoMapper")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddClass($"{Model.Name}Profile", @class =>
                {
                    string dtoModelName = GetDtoModelName();
                    var entityTemplate = MappingHelper.GetEntityTemplate(this, Model);
                    var dtoTemplate = this.GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Contracts.Dto, Model) as DtoModelTemplate;
                    @class.WithBaseType($"Profile");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddMethodChainStatement($"CreateMap<{this.GetTypeName(entityTemplate)}, {dtoModelName}>()", statement =>
                        {
                            MappingHelper.ImplementMapping(statement, dtoTemplate, Model);
                        });

                        if (MappingHelper.RequiresPersistenceMappings(ExecutionContext, (IntentTemplateBase)dtoTemplate, Model.Mapping?.Element as IElement, out var persistenceContractName))
                        {
                            ctor.AddMethodChainStatement($"CreateMap<{persistenceContractName}, {dtoModelName}>()", statement =>
                            {
                                MappingHelper.ImplementMapping(statement, dtoTemplate, Model);
                            });
                        }
                    });
                })
                .AddClass($"{Model.Name}MappingExtensions", @class => 
                {
                    @class.Static();
                    string dtoModelName = GetDtoModelName();
                    @class.AddMethod(dtoModelName, $"MapTo{dtoModelName}", method =>
                    {
                        method
                            .Static()
                            .AddParameter(GetEntityName(), "projectFrom", p => p.WithThisModifier())
                            .AddParameter("IMapper", "mapper")
                            .WithExpressionBody($"mapper.Map<{dtoModelName}>(projectFrom)");

                    });
                    @class.AddMethod($"List<{dtoModelName}>", $"MapTo{dtoModelName}List", method =>
                    {
                        method
                            .Static()
                            .AddParameter($"IEnumerable<{GetEntityName()}>", "projectFrom", p => p.WithThisModifier())
                            .AddParameter("IMapper", "mapper")
                            .WithExpressionBody($"projectFrom.Select(x => x.MapTo{dtoModelName}(mapper)).ToList()");

                    });
                });
        }



        private string GetDtoModelName()
        {
            return GetTypeName(TemplateRoles.Application.Contracts.Dto, Model);
        }

        private string GetEntityName()
        {
            return TryGetTypeName(TemplateRoles.Domain.Entity.Interface, Model.Mapping.ElementId, out var name)
                   ? name
                   : TryGetTypeName(TemplateRoles.Domain.ValueObject, Model.Mapping.ElementId, out name)
                       ? name
                       : TryGetTypeName(TemplateRoles.Domain.DataContract, Model.Mapping.ElementId, out name)
                           ? name
                           : throw new Exception($"Could not resolve mapped type '{Model.Mapping.Element.Name}'");
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetAutoMapperSettings().ProfileLocation().IsProfileSeparate();
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