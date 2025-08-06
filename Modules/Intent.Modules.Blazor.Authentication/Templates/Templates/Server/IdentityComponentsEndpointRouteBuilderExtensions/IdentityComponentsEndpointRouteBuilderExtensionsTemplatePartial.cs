using Intent.Engine;
using Intent.Modules.Blazor.Authentication.FactoryExtensions;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityComponentsEndpointRouteBuilderExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IdentityComponentsEndpointRouteBuilderExtensionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.IdentityComponentsEndpointRouteBuilderExtensionsTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdentityComponentsEndpointRouteBuilderExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Security.Claims")
                .AddUsing("System.Text.Json")
                .AddUsing("Microsoft.AspNetCore.Authentication")
                .AddUsing("Microsoft.AspNetCore.Components.Authorization")
                .AddUsing("Microsoft.AspNetCore.Http.Extensions")
                .AddUsing("Microsoft.AspNetCore.Identity")
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .AddUsing("Microsoft.Extensions.Primitives")
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddUsing("Microsoft.AspNetCore.Routing")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing($"{outputTarget.GetNamespace()}.Pages.Manage")
                .AddUsing($"{outputTarget.GetNamespace()}.Pages")
                .AddClass($"IdentityComponentsEndpointRouteBuilderExtensions", @class =>
                {
                    @class.Internal().Static();
                    var identityUserName = string.Empty;
                    if (ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity"))
                    {
                        identityUserName = IdentityHelperExtensions.GetIdentityUserClass(this);
                    }
                    else
                    {
                        identityUserName = GetTypeName(ApplicationUserTemplate.TemplateId);
                    }
                    @class.AddMethod("IEndpointConventionBuilder", "MapAdditionalIdentityEndpoints", mapAdditionalIdentityEndpoints =>
                    {
                        mapAdditionalIdentityEndpoints.Static();
                        mapAdditionalIdentityEndpoints.AddParameter("IEndpointRouteBuilder", "endpoints", endpoints =>
                        {
                            endpoints.WithThisModifier();
                        });

                        mapAdditionalIdentityEndpoints.AddStatement("ArgumentNullException.ThrowIfNull(endpoints);");
                        mapAdditionalIdentityEndpoints.AddStatement("var accountGroup = endpoints.MapGroup(\"/Account\");");

                        mapAdditionalIdentityEndpoints.AddStatements(@$"accountGroup.MapPost(""/PerformExternalLogin"", (
                            HttpContext context,
                            [FromServices] SignInManager<{identityUserName}> signInManager,
                            [FromForm] string provider,
                            [FromForm] string returnUrl) =>
                        {{
                            IEnumerable<KeyValuePair<string, StringValues>> query = [
                                new(""ReturnUrl"", returnUrl),
                                new(""Action"", ExternalLogin.LoginCallbackAction)];

                            var redirectUrl = UriHelper.BuildRelative(
                                context.Request.PathBase,
                                ""/Account/ExternalLogin"",
                                QueryString.Create(query));

                            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                            return TypedResults.Challenge(properties, [provider]);
                        }});".ConvertToStatements());

                        mapAdditionalIdentityEndpoints.AddStatements(@$"accountGroup.MapPost(""/Logout"", async (
                            ClaimsPrincipal user,
                            SignInManager<{identityUserName}> signInManager,
                            [FromForm] string returnUrl) =>
                            {{
                                await signInManager.SignOutAsync();
                                return TypedResults.LocalRedirect($""~/{{returnUrl}}"");
                            }});".ConvertToStatements());

                        mapAdditionalIdentityEndpoints.AddStatement("var manageGroup = accountGroup.MapGroup(\"/Manage\").RequireAuthorization();");

                        mapAdditionalIdentityEndpoints.AddStatements(@$"manageGroup.MapPost(""/LinkExternalLogin"", async (
                            HttpContext context,
                            [FromServices] SignInManager<{identityUserName}> signInManager,
                            [FromForm] string provider) =>
                            {{
                                // Clear the existing external cookie to ensure a clean login process
                                await context.SignOutAsync(IdentityConstants.ExternalScheme);

                                var redirectUrl = UriHelper.BuildRelative(
                                    context.Request.PathBase,
                                    ""/Account/Manage/ExternalLogins"",
                                    QueryString.Create(""Action"", ExternalLogins.LinkLoginCallbackAction));

                                var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, signInManager.UserManager.GetUserId(context.User));
                                return TypedResults.Challenge(properties, [provider]);
                            }});".ConvertToStatements());

                        mapAdditionalIdentityEndpoints.AddStatement("var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();");
                        mapAdditionalIdentityEndpoints.AddStatement("var downloadLogger = loggerFactory.CreateLogger(\"DownloadPersonalData\");");

                        mapAdditionalIdentityEndpoints.AddStatements(@$"manageGroup.MapPost(""/DownloadPersonalData"", async (
                            HttpContext context,
                            [FromServices] UserManager<{identityUserName}> userManager,
                            [FromServices] AuthenticationStateProvider authenticationStateProvider) =>
                            {{
                                var user = await userManager.GetUserAsync(context.User);
                                if (user is null)
                                {{
                                    return Results.NotFound($""Unable to load user with ID '{{userManager.GetUserId(context.User)}}'."");
                                }}

                                var userId = await userManager.GetUserIdAsync(user);
                                downloadLogger.LogInformation(""User with ID '{{UserId}}' asked for their personal data."", userId);

                                // Only include personal data for download
                                var personalData = new Dictionary<string, string>();
                                var personalDataProps = typeof({identityUserName}).GetProperties().Where(
                                    prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                                foreach (var p in personalDataProps)
                                {{
                                    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? ""null"");
                                }}

                                var logins = await userManager.GetLoginsAsync(user);
                                foreach (var l in logins)
                                {{
                                    personalData.Add($""{{l.LoginProvider}} external login provider key"", l.ProviderKey);
                                }}

                                personalData.Add(""Authenticator Key"", (await userManager.GetAuthenticatorKeyAsync(user))!);
                                var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

                                context.Response.Headers.TryAdd(""Content-Disposition"", ""attachment; filename=PersonalData.json"");
                                return TypedResults.File(fileBytes, contentType: ""application/json"", fileDownloadName: ""PersonalData.json"");
                            }});".ConvertToStatements());

                        mapAdditionalIdentityEndpoints.AddStatement("return accountGroup;");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity();
        }
    }
}