using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.Templates.DynamoDBRepositoryBase
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DynamoDBRepositoryBaseTemplateRegistration : SingleFileListModelTemplateRegistration<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DynamoDBRepositoryBaseTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DynamoDBRepositoryBaseTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<ClassModel> model)
        {
            return new DynamoDBRepositoryBaseTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<ClassModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels().ToList();
        }
    }
}