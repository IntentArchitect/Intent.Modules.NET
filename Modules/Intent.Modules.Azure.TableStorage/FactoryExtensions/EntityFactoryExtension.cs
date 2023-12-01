using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.DocumentDB.Api.Extensions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Azure.TableStorage.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Azure.TableStorage.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Azure.TableStorage.EntityFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            RegisterServices(application);

        }

        private static void RegisterServices(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (template is null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("Microsoft.Extensions.DependencyInjection");
                var method = file.Classes.First().FindMethod("AddInfrastructure");
                method.AddStatement($"services.AddScoped<{template.UseType("Azure.Data.Tables.TableServiceClient")}>(provider => new TableServiceClient(configuration[\"TableStorageConnectionString\"]));");
            });
        }
    }
}