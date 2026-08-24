using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Templates;
using System.Collections.Generic;

namespace Intent.Modules.AspNetCore.Identity.MockTemplateRegistrations
{
    public class DomainEntityStateTemplateStubRegistration : DomainEntityStateTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public DomainEntityStateTemplateStubRegistration(IMetadataManager metadataManager) : base(metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
        {
            var result = base.CreateTemplateInstance(outputTarget, model) as ICSharpFileBuilderTemplate;
            //Want the template to construct for CRUD inspection but not to actually run
            result.CSharpFile.AfterBuild(file =>
            {
                file.Template.CanRun = false;
            }, 100);
            result.CSharpFile.WithNamespace("Microsoft.AspNetCore.Identity");
            return result;
        }

        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            if (!application.Settings.GetDomainSettings().SeparateStateFromBehaviour())
            {
                return System.Array.Empty<ClassModel>();
            }

            return _metadataManager.GetIdentityClassModels(application.Id);
        }
    }
}
