using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.DomainEvents.Templates.DomainEvent;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.SharedKernel.Consumer.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.SharedKernel.Consumer.MockTemplateRegistrations
{
    public class DomainEventTemplateStubRegistration : DomainEventTemplateRegistration
    {

        public DomainEventTemplateStubRegistration(IMetadataManager metadataManager) : base(metadataManager)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, DomainEventModel model)
        {
            var sharedKernel = TemplateHelper.GetSharedKernel();
            var result = base.CreateTemplateInstance(outputTarget, model) as ICSharpFileBuilderTemplate;
            result.CanRun = false;
            result.CSharpFile.WithNamespace(result.CSharpFile.Namespace.Replace(outputTarget.ApplicationName(), sharedKernel.ApplicationName));
            return result;
        }

        public override IEnumerable<DomainEventModel> GetModels(IApplication application)
        {
            var sharedKernel = TemplateHelper.GetSharedKernel();
            return base.GetModels(new ApplicationStub(sharedKernel.ApplicationId));
        }
    }
}
