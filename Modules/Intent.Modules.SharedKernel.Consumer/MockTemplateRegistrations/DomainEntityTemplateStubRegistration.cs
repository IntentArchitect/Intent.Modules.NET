using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Templates;
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
    public class DomainEntityTemplateStubRegistration : DomainEntityTemplateRegistration
    {

        public DomainEntityTemplateStubRegistration(IMetadataManager metadataManager) : base(metadataManager)
        {
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
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

        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            var sharedKernel = TemplateHelper.GetSharedKernel();
            var models = base.GetModels(new ApplicationStub(sharedKernel.ApplicationId));
            return models;
        }
    }
}