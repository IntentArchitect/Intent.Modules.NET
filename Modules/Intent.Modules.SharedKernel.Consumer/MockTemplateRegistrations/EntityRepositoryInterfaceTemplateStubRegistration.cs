using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
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
    public class EntityRepositoryInterfaceTemplateStubRegistration : EntityRepositoryInterfaceTemplateRegistration
    {
        private readonly SharedKernel _sharedKernel;

        public EntityRepositoryInterfaceTemplateStubRegistration(IMetadataManager metadataManager) : base(metadataManager)
        {
            _sharedKernel = TemplateHelper.GetSharedKernel();
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
        {
            var result = base.CreateTemplateInstance(outputTarget, model) as ICSharpFileBuilderTemplate;
            result.CSharpFile.AfterBuild(file =>
            {
                file.Template.CanRun = false;
            }, 100);
            result.CSharpFile.WithNamespace(result.CSharpFile.Namespace.Replace(outputTarget.ApplicationName(), _sharedKernel.ApplicationName));
            return result;
        }

        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            var models =  base.GetModels(new ApplicationStub(_sharedKernel.ApplicationId));
            return models;
        }
    }
}
