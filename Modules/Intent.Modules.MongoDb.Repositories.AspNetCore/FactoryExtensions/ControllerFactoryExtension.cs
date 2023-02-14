using System;
using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.Repositories.AspNetCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ControllerFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.Repositories.AspNetCore.ControllerFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(ControllerTemplate.TemplateId));
            foreach (var template in templates)
            {
                
            }
        }
        
        private string GetUnitOfWork(ICSharpFileBuilderTemplate template)
        {
            if (template.TryGetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, out var unitOfWork) ||
                template.TryGetTypeName(TemplateFulfillingRoles.Application.Common.DbContextInterface, out unitOfWork) ||
                template.TryGetTypeName(TemplateFulfillingRoles.Infrastructure.Data.DbContext, out unitOfWork))
            {
                return unitOfWork;
            }
            throw new Exception($"A unit of work interface could not be resolved. Please ensure an interface with the role [{TemplateFulfillingRoles.Domain.UnitOfWork}] exists.");
        }
    }
}