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
            var razorComponents = application.FindTemplateInstances<IRazorComponentTemplate>(RazorComponentTemplate.TemplateId);
            AddErrorHandling(application, razorComponents);
            var razorLayouts = application.FindTemplateInstances<IRazorComponentTemplate>(RazorLayoutTemplate.TemplateId);
            AddErrorHandling(application, razorLayouts);
        }

        private static void AddErrorHandling(IApplication application, IEnumerable<IRazorComponentTemplate> components)
        {
            foreach (var component in components)
            {
                component.RazorFile.OnBuild(file =>
                {
                    var block = component.GetCodeBehind(); 

                    var methods = block.Declarations.OfType< CSharpClassMethod>().Where(m => m.HasMetadata("model") && m.GetMetadata<ComponentOperationModel>("model") is not null);
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
                                catchBlock.WithExceptionType(block.Template.UseType("System.Exception")).WithParameterName("e");

                                InjectServiceProperty(block, "MudBlazor.ISnackbar", "Snackbar");
                                catchBlock.AddStatement($"Snackbar.Add(e.Message, {block.Template.UseType("MudBlazor.Severity")}.Error);");
                            });
                        }
                    }

                }, 10);
            }
        }
        public static string InjectServiceProperty(IBuildsCSharpMembers block, string fullyQualifiedTypeName, string? propertyName = null)
        {
            var type = block.Template.UseType(fullyQualifiedTypeName);
            propertyName ??= type.Length > 2 && type[0] == 'I' && char.IsUpper(type[1]) ? type[1..] : type; // remove 'I' prefix if necessary.

            if (block is IRazorCodeBlock razorCodeBlock)
            {
                razorCodeBlock.RazorFile.AddInjectDirective(fullyQualifiedTypeName, propertyName);
            }
            else if (block is ICSharpClass @class)
            {
                if (@class.Properties.Any(x => x.Type == type))
                {
                    return propertyName;
                }
                @class.AddProperty(
                    type: type,
                    name: propertyName ?? type,
                    configure: property =>
                    {
                        property.AddAttribute(block.Template.UseType("Microsoft.AspNetCore.Components.Inject"));
                        property.WithInitialValue("default!");
                    });
            }
            return propertyName;
        }
    }
}