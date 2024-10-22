using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.DomainServices.Templates.DomainServiceInterface;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.SharedKernel.Consumer.MockTemplateRegistrations
{
    public class DomainServiceInterfaceTemplateStubRegistration : DomainServiceInterfaceTemplateRegistration
    {

        public DomainServiceInterfaceTemplateStubRegistration(IMetadataManager metadataManager) : base(metadataManager)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, DomainServiceModel model)
        {
            var sharedKernel = TemplateHelper.GetSharedKernel();
            var result = base.CreateTemplateInstance(outputTarget, model) as ICSharpFileBuilderTemplate;
            //Want the template to construct for CRUD inspection but not to actually run
            result.CSharpFile.AfterBuild(file =>
            {
                file.Template.CanRun = false;
            }, 100);
            result.CSharpFile.WithNamespace(result.CSharpFile.Namespace.Replace(outputTarget.ApplicationName(), sharedKernel.ApplicationName));
            return result;
        }

        public override IEnumerable<DomainServiceModel> GetModels(IApplication application)
        {
            var sharedKernel = TemplateHelper.GetSharedKernel();
            return base.GetModels(new ApplicationStub(sharedKernel.ApplicationId));
        }
    }
}
