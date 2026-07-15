using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using System.Linq;

namespace Intent.Modules.Blazor.Authentication.DefaultContent
{
    /// <summary>
    /// Default (first-generation only) content for the shared StatusMessage component, seeded onto
    /// the modelled <c>Component</c>'s <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent.RazorComponentTemplate"/>
    /// and <see cref="Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind.RazorComponentCodeBehindTemplate"/>
    /// by <see cref="FactoryExtensions.AuthPageDefaultContentFactoryExtension"/>. Mechanically ported,
    /// unchanged, from the old static content/Components(MudBlazor)/Account/Shared/StatusMessage pair.
    /// Shared between the Identity and JWT authentication modes (no mode prefix on its page id).
    /// </summary>
    internal static class StatusMessageContent
    {
        public static string BuildRazorContent(RazorComponentTemplate template)
        {
            var isMudBlazor = template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Blazor.Components.MudBlazor");

            return isMudBlazor
                ? BuildMudBlazorContent()
                : BuildBootstrapContent();
        }

        private static string BuildMudBlazorContent()
        {
            return """
                @if (!string.IsNullOrEmpty(DisplayMessage))
                {
                    var severity = DisplayMessage.StartsWith("Error") ? Severity.Error : Severity.Success;
                    <MudAlert Severity="@severity" Class="mb-3">@DisplayMessage</MudAlert>
                }
                """;
        }

        private static string BuildBootstrapContent()
        {
            return """
                @if (!string.IsNullOrEmpty(DisplayMessage))
                {
                    var statusMessageClass = DisplayMessage.StartsWith("Error") ? "danger" : "success";
                    <div class="alert alert-@statusMessageClass" role="alert">
                        @DisplayMessage
                    </div>
                }
                """;
        }

        public static void BuildCodeBehind(IBuildsCSharpMembers code)
        {
            var redirectManager = code.Template.GetIdentityRedirectManagerTemplateName();

            code.AddField("string?", "messageFromCookie");

            code.AddProperty("string?", "Message", p => p.AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.ParameterAttribute").RemoveSuffix("Attribute")));
            code.AddProperty(code.Template.UseType("Microsoft.AspNetCore.Http.HttpContext"), "HttpContext", p => p.Private().WithInitialValue("default!").AddAttribute(code.Template.UseType("Microsoft.AspNetCore.Components.CascadingParameterAttribute").RemoveSuffix("Attribute")));
            code.AddProperty("string?", "DisplayMessage", p => p.Private().WithoutSetter().Getter.WithExpressionImplementation("Message ?? messageFromCookie"));

            code.AddMethod("void", "OnInitialized", onInitialized =>
            {
                onInitialized.Protected().Override();

                onInitialized.AddAssignmentStatement("messageFromCookie", new CSharpStatement($"HttpContext.Request.Cookies[{redirectManager}.StatusCookieName];"));
                onInitialized.AddIfStatement("messageFromCookie is not null", @if =>
                {
                    @if.AddStatement($"HttpContext.Response.Cookies.Delete({redirectManager}.StatusCookieName);");
                });
            });
        }
    }
}
