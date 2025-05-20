using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.AutoMapper.Templates.MapFromInterface;
using Intent.Modules.Application.Dtos.AutoMapper.Templates;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using DataContractGeneralizationModel = Intent.Modelers.Domain.Api.DataContractGeneralizationModel;
using GeneralizationModel = Intent.Modelers.Domain.Api.GeneralizationModel;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.AutoMapper.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AutoMapperDtoFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Dtos.AutoMapper.AutoMapperDtoFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.GetSettings().GetAutoMapperSettings().ProfileLocation().IsProfileInDto())
            {
                return;
            }
            var templates = application.FindTemplateInstances<DtoModelTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Application.Contracts.Dto));
            foreach (var template in templates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.TypeDeclarations.First();
                    var templateModel = template.Model;

                    if (!templateModel.HasMapFromDomainMapping())
                    {
                        return;
                    }

                    var entityTemplate = MappingHelper.GetEntityTemplate(template, templateModel);

                    file.AddUsing("AutoMapper");

                    @class.ImplementsInterface($"{template.GetTypeName(MapFromInterfaceTemplate.TemplateId)}<{template.GetTypeName(entityTemplate)}>");

                    @class.AddMethod("void", "Mapping", method =>
                    {
                        method.AddParameter("Profile", "profile");

                        method.AddMethodChainStatement($"profile.CreateMap<{template.GetTypeName(entityTemplate)}, {template.ClassName}>()", statement =>
                        {
                            MappingHelper.ImplementMapping(statement, template, templateModel);
                        });

                        if (MappingHelper.RequiresPersistenceMappings(application, template, templateModel.Mapping?.Element as IElement, out var persistenceContractName))
                        {
                            method.AddMethodChainStatement($"profile.CreateMap<{persistenceContractName}, {template.ClassName}>()", statement =>
                            {
                                MappingHelper.ImplementMapping(statement, template, templateModel);
                            });
                        }
                    });
                });
            }
        }


    }
}