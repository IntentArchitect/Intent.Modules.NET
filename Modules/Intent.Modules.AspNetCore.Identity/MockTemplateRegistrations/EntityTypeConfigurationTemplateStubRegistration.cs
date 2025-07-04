﻿using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.Identity.MockTemplateRegistrations
{
    public class EntityTypeConfigurationTemplateStubRegistration : EntityTypeConfigurationTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        private const string DomainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";
        public EntityTypeConfigurationTemplateStubRegistration(IMetadataManager metadataManager) : base(metadataManager)
        {
            _metadataManager = metadataManager;
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
        {
            var result = base.CreateTemplateInstance(outputTarget, model) as ICSharpFileBuilderTemplate;
            //Want the template to construct for CRUD inspection but not to actually run
            //result.CanRun = false;
            result.CSharpFile.AfterBuild(file =>
            {
                file.Template.CanRun = false;
            }, 100);
            return result;
        }

        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            var associations = _metadataManager.Domain(application).GetClassModels().Select(c => c.InternalElement).SelectMany(a => a.AssociatedElements);

            var models = associations.Where(a => a is not null).Where(e => e.Association.SourceEnd is not null).Select(s => s.Association.SourceEnd);

            return models.Select(p => p.ParentElement.AsClassModel()).Where(m => m.Name == "IdentityUserRole" || m.Name == "IdentityRole" ||
            m.Name == "IdentityUser" || m.Name == "IdentityRoleClaim" || m.Name == "IdentityUserToken" || m.Name == "IdentityUserClaim" ||
            m.Name == "IdentityUserLogin");
        }
    }
}
