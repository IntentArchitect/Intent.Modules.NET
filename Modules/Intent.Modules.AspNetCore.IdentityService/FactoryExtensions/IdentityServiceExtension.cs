using System.Linq;
using Intent.AspNetCore.IdentityService.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
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

namespace Intent.Modules.AspNetCore.IdentityService.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class IdentityServiceExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.IdentityService.IdentityServiceExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var identityService = application.MetadataManager.Services(application).GetServiceModels().FirstOrDefault(s => s.HasIdentityServiceHandler());
            if (identityService is null)
            {
                return;
            }

            CreateService(application, identityService);
            ModifyIdentityServiceController(application, identityService);
        }

        private void ModifyIdentityServiceController(IApplication application, ServiceModel identityService)
        {
            var serviceImplementationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.Controllers.Controller", identityService.Id);

            serviceImplementationTemplate.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();

                var confirmEmailMethod = @class.Methods.First(x => x.Name == "ConfirmEmailAsync");
                confirmEmailMethod.AddAttribute("EndpointName(\"ConfirmEmailAsync\")");

                var loginMethod = @class.Methods.First(x => x.Name == "LoginAsync");
                loginMethod.Attributes.Remove(loginMethod.Attributes.First(x => x.Name == "ProducesResponseType(StatusCodes.Status204NoContent)"));
                loginMethod.AddAttribute("ProducesResponseType(typeof(AccessTokenResponseDto), StatusCodes.Status200OK)");

                loginMethod.Statements.Remove(loginMethod.Statements.First(s => s.Text == "return NoContent();"));
                loginMethod.Statements.Add(new CSharpStatement("return new EmptyResult();"));

                var refreshMethod = @class.Methods.First(x => x.Name == "RefreshAsync");
                refreshMethod.Attributes.Remove(refreshMethod.Attributes.First(x => x.Name == "ProducesResponseType(StatusCodes.Status204NoContent)"));
                refreshMethod.AddAttribute("ProducesResponseType(typeof(AccessTokenResponseDto), StatusCodes.Status200OK)");

                refreshMethod.Statements.Remove(refreshMethod.Statements.First(s => s.Text == "return NoContent();"));
                refreshMethod.Statements.Add(new CSharpStatement("return new EmptyResult();"));

            }, 99);
        }



        private void CreateService(IApplication application, ServiceModel identityService)
        {
            var serviceImplementationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.ServiceImplementations.ServiceImplementation", identityService.Id);

            serviceImplementationTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                @class.Constructors.FirstOrDefault().AddParameter(serviceImplementationTemplate.GetTypeName("Intent.AspNetCore.IdentityService.IdentityServiceManagerInterface"), "identityServiceManager",
                    c => c.IntroduceReadonlyField());

                foreach (var operation in identityService.Operations)
                {
                    if (!operation.HasIdentityServiceHandler())
                    {
                        continue;
                    }

                    switch (operation.Name)
                    {
                        case "ConfirmEmailAsync":
                            {
                                var method = @class.FindMethod("ConfirmEmailAsync");
                                method.Statements.Clear();
                                method.AddStatement("return await _identityServiceManager.ConfirmEmailAsync(userId, code, changedEmail);");
                                break;
                            }
                        case "ForgotPasswordAsync":
                            {
                                var method = @class.FindMethod("ForgotPasswordAsync");
                                method.Statements.Clear();
                                method.AddStatement("await _identityServiceManager.ForgotPasswordAsync(resetRequest);");
                                break;
                            }
                        case "GetInfoAsync":
                            {
                                var method = @class.FindMethod("GetInfoAsync");
                                method.Statements.Clear();
                                method.AddStatement("return await _identityServiceManager.GetInfoAsync();");
                                break;
                            }
                        case "LoginAsync":
                            {
                                var method = @class.FindMethod("LoginAsync");
                                method.Statements.Clear();
                                method.AddStatement("await _identityServiceManager.LoginAsync(login, useCookies, useSessionCookies);");
                                break;
                            }
                        case "RefreshAsync":
                            {
                                var method = @class.FindMethod("RefreshAsync");
                                method.Statements.Clear();
                                method.AddStatement("await _identityServiceManager.RefreshAsync(refreshRequest);");
                                break;
                            }
                        case "RegisterAsync":
                            {
                                var method = @class.FindMethod("RegisterAsync");
                                method.Statements.Clear();
                                method.AddStatement("await _identityServiceManager.RegisterAsync(register);");
                                break;
                            }
                        case "ResendConfirmationEmailAsync":
                            {
                                var method = @class.FindMethod("ResendConfirmationEmailAsync");
                                method.Statements.Clear();
                                method.AddStatement("await _identityServiceManager.ResendConfirmationEmailAsync(resendRequest);");
                                break;
                            }
                        case "ResetPasswordAsync":
                            {
                                var method = @class.FindMethod("ResetPasswordAsync");
                                method.Statements.Clear();
                                method.AddStatement("await _identityServiceManager.ResetPasswordAsync(resetRequest);");
                                break;
                            }
                        case "UpdateInfoAsync":
                            {
                                var method = @class.FindMethod("UpdateInfoAsync");
                                method.Statements.Clear();
                                method.AddStatement("return await _identityServiceManager.UpdateInfoAsync(infoRequest);");
                                break;
                            }
                        case "UpdateTwoFactorAsync":
                            {
                                var method = @class.FindMethod("UpdateTwoFactorAsync");
                                method.Statements.Clear();
                                method.AddStatement("return await _identityServiceManager.UpdateTwoFactorAsync(tfaRequest);");
                                break;
                            }
                        default:
                            break;
                    }
                }
            });

        }
    }
}