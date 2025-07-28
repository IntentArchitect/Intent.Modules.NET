using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.Templates.DynamoDBDocumentOfTInterface
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DynamoDBDocumentOfTInterfaceTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => DynamoDBDocumentOfTInterfaceTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new DynamoDBDocumentOfTInterfaceTemplate(outputTarget);
        }
    }
}