using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IntegrationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.ServiceImplementations.Conventions.CRUD.IntegrationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (application.InstalledModules.All(p => p.ModuleId != "Intent.MongoDb.Repositories"))
            {
                return;
            }

            var templates = application.FindTemplateInstances<ServiceImplementationTemplate>(TemplateDependency.OnTemplate(ServiceImplementationTemplate.TemplateId));
            foreach (var template in templates)
            {
                var strategy = new UpdateImplementationStrategy(template, application);
                template.CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First();
                    foreach (var method in priClass.Methods)
                    {
                        if (!method.TryGetMetadata<OperationModel>("model", out var operationModel) ||
                            !strategy.IsMatch(operationModel))
                        {
                            continue;
                        }

                        var (_, domainModel) = operationModel.GetUpdateModelPair();
                        if (domainModel.InternalElement.Package.SpecializationType != "Mongo Domain Package")
                        {
                            continue;
                        }

                        method.Statements.Last()
                            .InsertAbove(
                                $"{(domainModel.Name.ToCamelCase() + "Repository").ToPrivateMemberName()}.Update(existing{domainModel.Name.ToPascalCase()});");
                    }
                }, 100);
            }
        }
    }
}