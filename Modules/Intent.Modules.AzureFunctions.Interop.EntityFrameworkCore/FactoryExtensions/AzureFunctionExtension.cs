using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Interop.EntityFrameworkCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AzureFunctionExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AzureFunctions.Interop.EntityFrameworkCore.AzureFunctionExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<AzureFunctionClassTemplate>(TemplateDependency.OnTemplate(AzureFunctionClassTemplate.TemplateId));
            foreach (var template in templates)
            {
                template.AddTypeSource("Domain.NotFoundException");

                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var runMethod = @class.FindMethod("Run");

                    runMethod.FindStatement<CSharpTryBlock>(x => true)
                        ?.InsertBelow(new CSharpCatchBlock(template.GetTypeName("Domain.NotFoundException"),
                                "exception")
                            .AddStatement("return new NotFoundObjectResult(new { Message = exception.Message });"));
                });

                if (HttpEndpointModelFactory.GetEndpoint(template.Model.InternalElement)?.Verb == HttpVerb.Get)
                {
                    continue;
                }

                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    @class.Constructors.First().AddParameter(GetUnitOfWork(template), "unitOfWork", param =>
                    {
                        param.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                    });

                    var runMethod = @class.FindMethod("Run");

                    runMethod.FindStatement(x => x.HasMetadata("service-dispatch-statement"))
                        ?.InsertBelow($"await _unitOfWork.SaveChangesAsync(cancellationToken);");
                }, 10);
            }
        }
        
        private static string GetUnitOfWork(AzureFunctionClassTemplate template)
        {
            if (template.TryGetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, out var unitOfWork) ||
                template.TryGetTypeName(TemplateFulfillingRoles.Application.Common.DbContextInterface, out unitOfWork) ||
                template.TryGetTypeName(TemplateFulfillingRoles.Infrastructure.Data.DbContext, out unitOfWork))
            {
                return unitOfWork;
            }

            throw new Exception(
                $"A unit of work interface could not be resolved. Please ensure an interface with the role [{TemplateFulfillingRoles.Domain.UnitOfWork}] exists.");
        }
    }
}