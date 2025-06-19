using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.Identity.MockTemplateRegistrations
{
    public class DomainEntityTemplateStubRegistration : DomainEntityTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        private const string DomainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";
        public DomainEntityTemplateStubRegistration(IMetadataManager metadataManager) : base(metadataManager)
        {
            _metadataManager = metadataManager;
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
        {
            var result = base.CreateTemplateInstance(outputTarget, model) as ICSharpFileBuilderTemplate;
            //Want the template to construct for CRUD inspection but not to actually run
            result.CSharpFile.AfterBuild(file =>
            {
                file.Template.CanRun = false;
            }, 100);
            result.CSharpFile.WithNamespace("Microsoft.AspNet.Identity.EntityFramework");
            return result;
        }

        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            //var models = base.GetModels(new ApplicationStub(sharedKernel.ApplicationId));
            var associations = _metadataManager.Domain(application).GetClassModels().Select(c => c.InternalElement).SelectMany(a => a.AssociatedElements);

            var models = associations.Where(a => a is not null).Where(e => e.Association.SourceEnd is not null).Select(s => s.Association.SourceEnd);

            return models.Select(p => p.ParentElement.AsClassModel());
            //return models;
        }
    }
}
