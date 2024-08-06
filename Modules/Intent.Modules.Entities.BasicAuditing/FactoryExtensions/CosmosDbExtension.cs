using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Identity.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryBase;
using Intent.Modules.Entities.BasicAuditing.Settings;
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
                string userIdType = TemplateHelper.GetUserIdentifierType(template.ExecutionContext);

                var @class = file.Classes.First();
                @class.AddField($"Lazy<({template.UseType(userIdType)} UserIdentifier, DateTimeOffset TimeStamp)>", "_auditDetails", f => f.PrivateReadOnly());

                var constructor = @class.Constructors.First();

                constructor.AddParameter(
                    template.GetCurrentUserServiceInterfaceName(), "currentUserService",
                    param => param.IntroduceReadonlyField());
                constructor.AddStatement($"_auditDetails = new Lazy<({template.UseType(userIdType)} UserIdentifier, DateTimeOffset TimeStamp)>(GetAuditDetails);");

                @class.AddMethod($"({template.UseType(userIdType)} UserIdentifier, DateTimeOffset TimeStamp)", "GetAuditDetails", method =>
                {
                    string userIdentityProperty;
                    switch (template.ExecutionContext.Settings.GetBasicAuditing().UserIdentityToAudit().AsEnum())
                    {
                        case Settings.BasicAuditing.UserIdentityToAuditOptionsEnum.UserName:
                            userIdentityProperty = "UserName";
                            break;
                        case Settings.BasicAuditing.UserIdentityToAuditOptionsEnum.UserId:
                        default:
                            userIdentityProperty = "UserId";
                            break;
                    }

                    method.Private();
                    method.AddStatements(new[]
                    {
                        $"var userIdentifier = _currentUserService.{userIdentityProperty} ?? throw new InvalidOperationException(\"{userIdentityProperty} is null\");",
                        "var timestamp = DateTimeOffset.UtcNow;",
                        "",
                        "return (userIdentifier, timestamp);"
                    });
                });

                // Add
                {
                    var method = @class.FindMethod("Add");
                    var enqueueStatement = method.Statements.OfType<CSharpInvocationStatement>().First(x => x.HasMetadata(MetadataNames.EnqueueStatement));
                    var invocationArgument = (CSharpLambdaBlock)enqueueStatement.Statements[0];

                    invocationArgument.InsertStatement(
                        index: 0,
                        statement: $"(entity as {template.GetAuditableInterfaceName()})?.SetCreated(_auditDetails.Value.UserIdentifier, _auditDetails.Value.TimeStamp);");
                }

                // Update
                {
                    var method = @class.FindMethod("Update");
                    var enqueueStatement = method.Statements.OfType<CSharpInvocationStatement>().First(x => x.HasMetadata(MetadataNames.EnqueueStatement));
                    var invocationArgument = (CSharpLambdaBlock)enqueueStatement.Statements[0];

                    invocationArgument.InsertStatement(
                        index: 0,
                        statement: $"(entity as {template.GetAuditableInterfaceName()})?.SetUpdated(_auditDetails.Value.UserIdentifier, _auditDetails.Value.TimeStamp);");
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