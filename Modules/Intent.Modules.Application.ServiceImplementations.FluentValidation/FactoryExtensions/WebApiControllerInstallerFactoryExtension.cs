using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.ServiceImplementations.FluentValidation.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WebApiControllerInstallerFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.ServiceImplementations.FluentValidation.WebApiControllerInstallerFactoryExtension";

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
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(
                TemplateDependency.OnTemplate(Roles.Distribution.WebApi)).ToList();
            
            foreach (var template in templates)
            {
                var validationProviderName = template.GetTypeName("Application.Common.ValidatonProvider");
                var @class = template.CSharpFile.Classes.First();
                @class.AddField(validationProviderName, "_validationProvider", x => x.PrivateReadOnly());
                
                var ctor = @class.Constructors.First();
                ctor.AddParameter(validationProviderName, "validationProvider");
                ctor.AddStatement($"_validationProvider = validationProvider;");

                foreach (var method in @class.Methods)
                {
                    var fromBodyParam = method.Parameters.FirstOrDefault(p => p.Attributes.Any(p => p.GetText("")?.Contains("FromBody") == true));
                    if (fromBodyParam != null)
                    {
                        method.InsertStatement(0, $"await _validationProvider.Handle({fromBodyParam.Name}, cancellationToken);");
                    }
                }
            }
        }
    }
}