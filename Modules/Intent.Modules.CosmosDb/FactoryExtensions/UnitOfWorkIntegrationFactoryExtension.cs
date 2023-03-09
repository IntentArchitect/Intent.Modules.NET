using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.CosmosDb.Templates.ApplicationCosmosDbContext;
using Intent.Modules.CosmosDb.Templates.CosmosDbUnitOfWorkInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.CosmosDb.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UnitOfWorkIntegrationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.CosmosDb.UnitOfWorkIntegrationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            InstallCosmosDbUnitOfWorkForStandardIntegration(application);
        }

        private void InstallCosmosDbUnitOfWorkForStandardIntegration(IApplication application)
        {
            var controllerTemplates =
                application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Application.Services.Controllers));
            foreach (var template in controllerTemplates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var ctor = @class.Constructors.First();
                    ctor.AddParameter(template.GetTypeName(CosmosDbUnitOfWorkInterfaceTemplate.TemplateId), "cosmosDbUnitOfWork", p => { p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                    foreach (var method in @class.Methods)
                    {
                        if (method.TryGetMetadata<IHasStereotypes>("model", out var operation) &&
                            operation.HasStereotype("Http Settings") && operation.GetStereotype("Http Settings").GetProperty<string>("Verb") != "GET")
                        {
                            method.Statements.LastOrDefault(x => x.ToString().Contains("return "))
                                ?.InsertAbove($"await _cosmosDbUnitOfWork.SaveChangesAsync(cancellationToken);");
                        }
                    }
                }, -150);
            }
        }

        private string GetAppDbContext(ICSharpFileBuilderTemplate template)
        {
            return template.GetTypeName(ApplicationCosmosDbContextTemplate.TemplateId);
        }
    }
}