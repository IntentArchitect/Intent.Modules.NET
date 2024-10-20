using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.SharedKernel.Consumer.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DependencyInjectionExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.SharedKernel.Consumer.DependencyInjectionExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            base.OnAfterTemplateRegistrations(application);
            var sharedKernel = TemplateHelper.GetSharedKernel();

            UpdateApplicationDI(application, sharedKernel);
            UpdateInfrastructureDI(application, sharedKernel);
        }

        private void UpdateInfrastructureDI(IApplication application, SharedKernel sharedKernel)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Infrastructure.DependencyInjection.DependencyInjection");
            if (template is null)
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var method = @class.FindMethod("AddInfrastructure");
                var returnStatement = method.FindStatement(s => s.GetText("") == "return services;");

                AddBeforeReturn(method, returnStatement, $"{sharedKernel.ApplicationName}.Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);");
                AddBeforeReturn(method, returnStatement, $"services.AddScoped<{sharedKernel.ApplicationName}.Infrastructure.Persistence.ApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());");
                AddBeforeReturn(method, returnStatement, $"services.AddScoped<{sharedKernel.ApplicationName}.Application.Common.Interfaces.IDomainEventService>(provider => provider.GetRequiredService<IDomainEventService>());");
            }, 1000);
        }

        private static void AddBeforeReturn(CSharpClassMethod method, ICSharpStatement returnStatement, string statement)
        {
            if (returnStatement is null)
            {
                method.AddStatement(statement);
            }
            else
            {
                returnStatement.InsertAbove(statement);
            }
        }

        private void UpdateApplicationDI(IApplication application, SharedKernel sharedKernel)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Application.DependencyInjection);
            if (template is null)
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var method = @class.FindMethod("AddApplication");
                var statement = $"{sharedKernel.ApplicationName}.Application.DependencyInjection.AddApplication(services, configuration);";
                var returnStatement = method.FindStatement(s => s.GetText("") == "return services;");

                if (returnStatement is null)
                {
                    method.AddStatement(statement);
                }
                else
                {
                    returnStatement.InsertAbove(statement);
                }
            }, 1000);
        }
    }
}