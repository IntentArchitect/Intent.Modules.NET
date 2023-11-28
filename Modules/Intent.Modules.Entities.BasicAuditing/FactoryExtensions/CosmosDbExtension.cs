using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Identity.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryBase;
using Intent.Modules.Entities.BasicAuditing.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Entities.BasicAuditing.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CosmosDbExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Entities.BasicAuditing.CosmosDbExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            const string cosmosDbRepositoryBaseTemplateId = "Intent.CosmosDB.CosmosDBRepositoryBase";
            const string cosmosDbRepositoryTemplateId = "Intent.CosmosDB.CosmosDBRepository";

            if (!application.TemplateExists(cosmosDbRepositoryBaseTemplateId))
            {
                return;
            }

            var repositoryBaseTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(cosmosDbRepositoryBaseTemplateId);
            UpdateRepositoryBase(repositoryBaseTemplate);

            var repositoryTemplates = application.FindTemplateInstances(cosmosDbRepositoryTemplateId, _ => true);
            foreach (var template in repositoryTemplates)
            {
                UpdateRepository((ICSharpFileBuilderTemplate)template);
            }
        }

        private static void UpdateRepositoryBase(ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("System");

                var @class = file.Classes.First();
                @class.AddField("Lazy<(string UserName, DateTimeOffset TimeStamp)>", "_auditDetails", f => f.PrivateReadOnly());

                var constructor = @class.Constructors.First();

                constructor.AddParameter(
                    template.GetCurrentUserServiceInterfaceName(), "currentUserService",
                    param => param.IntroduceReadonlyField());
                constructor.AddStatement("_auditDetails = new Lazy<(string UserName, DateTimeOffset TimeStamp)>(GetAuditDetails);");

                @class.AddMethod("(string UserName, DateTimeOffset TimeStamp)", "GetAuditDetails", method =>
                {
                    method.Private();
                    method.AddStatements(new[]
                    {
                        "var userName = _currentUserService.UserId ?? throw new InvalidOperationException(\"UserId is null\");",
                        "var timestamp = DateTimeOffset.UtcNow;",
                        "",
                        "return (userName, timestamp);"
                    });
                });

                // Add
                {
                    var method = @class.FindMethod("Add");
                    var enqueueStatement = method.Statements.OfType<CSharpInvocationStatement>().First(x => x.HasMetadata(MetadataNames.EnqueueStatement));
                    var invocationArgument = (CSharpLambdaBlock)enqueueStatement.Statements[0];

                    invocationArgument.InsertStatement(
                        index: 0,
                        statement: $"(entity as {template.GetAuditableInterfaceName()})?.SetCreated(_auditDetails.Value.UserName, _auditDetails.Value.TimeStamp);");
                }

                // Update
                {
                    var method = @class.FindMethod("Update");
                    var enqueueStatement = method.Statements.OfType<CSharpInvocationStatement>().First(x => x.HasMetadata(MetadataNames.EnqueueStatement));
                    var invocationArgument = (CSharpLambdaBlock)enqueueStatement.Statements[0];

                    invocationArgument.InsertStatement(
                        index: 0,
                        statement: $"(entity as {template.GetAuditableInterfaceName()})?.SetUpdated(_auditDetails.Value.UserName, _auditDetails.Value.TimeStamp);");
                }
            }, 1000);
        }

        private static void UpdateRepository(ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                var constructor = @class.Constructors.First();
                constructor.AddParameter(template.GetCurrentUserServiceInterfaceName(), "currentUserService");
                constructor.ConstructorCall.AddArgument("currentUserService");
            }, 1000);
        }
    }
}