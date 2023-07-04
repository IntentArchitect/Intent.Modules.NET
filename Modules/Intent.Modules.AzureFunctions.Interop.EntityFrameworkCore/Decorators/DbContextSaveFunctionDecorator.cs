using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Interop.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DbContextSaveFunctionDecorator : AzureFunctionClassDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AzureFunctions.Interop.EntityFrameworkCore.DbContextSaveFunctionDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly AzureFunctionClassTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DbContextSaveFunctionDecorator(AzureFunctionClassTemplate template, IApplication application)
        {
            _application = application;
            _template = template;
            
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
            
            if (HttpEndpointModelFactory.GetEndpoint(_template.Model.InternalElement)?.Verb == HttpVerb.Get)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                @class.Constructors.First().AddParameter(GetUnitOfWork(), "unitOfWork", param =>
                {
                    param.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                });

                var runMethod = @class.FindMethod("Run");
                
                runMethod.FindStatement(x => x.HasMetadata("service-dispatch-statement"))
                    ?.InsertBelow($"await _unitOfWork.SaveChangesAsync();");
            }, 10);
        }

        private string GetUnitOfWork()
        {
            if (_template.TryGetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, out var unitOfWork) ||
                _template.TryGetTypeName(TemplateFulfillingRoles.Application.Common.DbContextInterface, out unitOfWork) ||
                _template.TryGetTypeName(TemplateFulfillingRoles.Infrastructure.Data.DbContext, out unitOfWork))
            {
                return unitOfWork;
            }

            throw new Exception(
                $"A unit of work interface could not be resolved. Please ensure an interface with the role [{TemplateFulfillingRoles.Domain.UnitOfWork}] exists.");
        }
    }
}