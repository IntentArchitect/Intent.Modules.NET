using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayout;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutCodeBehind;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SnackbarInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Blazor.Components.MudBlazor.SnackbarInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var components = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(RazorComponentCodeBehindTemplate.TemplateId);
            AddErrorHandling(application, components);
            var layouts = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(RazorLayoutCodeBehindTemplate.TemplateId);
            AddErrorHandling(application, layouts);
        }

        private static void AddErrorHandling(IApplication application, IEnumerable<ICSharpFileBuilderTemplate> components)
        {
            foreach (var component in components)
            {
                component.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var methods = @class.Methods.Where(m => m.HasMetadata("model") && m.GetMetadata<ComponentOperationModel>("model") is not null);
                    foreach (var method in methods)
                    {
                        if (method.Statements.Any(x => x.ToString().Contains("await ")))
                        {
                            method.AddTryBlock(tryBlock =>
                            {
                                foreach (var statement in method.Statements.Where(x => x != tryBlock).ToList())
                                {
                                    statement.Remove();
                                    tryBlock.AddStatement(statement);
                                }
                            });

                            method.AddCatchBlock(catchBlock =>
                            {
                                catchBlock.WithExceptionType(component.UseType("System.Exception")).WithParameterName("e");

                                InjectServiceProperty(component, @class, "MudBlazor.ISnackbar", "Snackbar");
                                catchBlock.AddStatement($"Snackbar.Add(e.Message, {component.UseType("MudBlazor.Severity")}.Error);");
                            });
                        }
                    }

                }, 100);
            }
        }

        public static void InjectServiceProperty(ICSharpFileBuilderTemplate template, CSharpClass @class, string fullyQualifiedTypeName, string? propertyName = null)
        {
            var type = template.UseType(fullyQualifiedTypeName);
            propertyName ??= type.Length > 2 && type[0] == 'I' && char.IsUpper(type[1]) ? type[1..] : type; // remove 'I' prefix if necessary.

            if (@class.Properties.Any(x => x.Type == type))
            {
                return;
            }
            @class.AddProperty(
                type: type,
                name: propertyName ?? type,
                configure: property =>
                {
                    property.AddAttribute(template.UseType("Microsoft.AspNetCore.Components.Inject"));
                    property.WithInitialValue("default!");
                });
        }
    }
}