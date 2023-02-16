using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.MongoDb.Templates.ApplicationMongoDbContext;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UnitOfWorkIntegrationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.UnitOfWorkIntegrationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!UnitOfWorkHandler.ShouldInstallStandardIntegration(application))
            {
                return;
            }
            
            var controllerTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.AspNetCore.Controllers.Controller"));
            foreach (var template in controllerTemplates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var ctor = @class.Constructors.First();
                    ctor.AddParameter(GetUnitOfWork(template), "mongoDbUnitOfWork", p =>
                    {
                        p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                    });

                    foreach (var method in @class.Methods)
                    {
                        if (method.TryGetMetadata<IHasStereotypes>("model", out var operation) &&
                            operation.HasStereotype("Http Settings") && operation.GetStereotype("Http Settings").GetProperty<string>("Verb") != "GET")
                        {
                            method.Statements.LastOrDefault(x => x.ToString().Contains("return "))
                                ?.InsertAbove($"await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);");
                        }
                    }
                });
            }
        }

        private string GetUnitOfWork(ICSharpFileBuilderTemplate template)
        {
            return template.GetTypeName(ApplicationMongoDbContextTemplate.TemplateId);
        }
    }
}