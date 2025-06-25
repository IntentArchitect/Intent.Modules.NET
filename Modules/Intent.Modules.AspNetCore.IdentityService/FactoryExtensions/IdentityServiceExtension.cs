using System.Linq;
using Intent.AspNetCore.IdentityService.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
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
                foreach (var service in application.MetadataManager.Services(application).GetServiceModels())
                {
                    if (service.Operations.Any(o => o.HasIdentityServiceHandler()))
                    {
                        identityService = service;
                        break;
                    }
                }

                if (identityService is null)
                {
                    return;
                }
            }

            CreateService(application, identityService);
            ModifyIdentityServiceController(application, identityService);

            foreach (var @class in application.MetadataManager.Domain(application).GetClassModels())
            {
                if (@class.ParentClass is not null)
                {
                    UpdateEntityConfiguration(application, @class.ParentClass.Name switch
                    {
                        "IdentityUser" or "IdentityRole" or "IdentityUserRole" or
                            "IdentityRoleClaim" or "IdentityUserClaim" or "IdentityUserToken" or "IdentityUserLogin" => @class,
                        _ => null
                    });
                }
            }
        }

        private void UpdateEntityConfiguration(IApplication application, ClassModel? userIdentityEntity)
        {
            if (userIdentityEntity is null || userIdentityEntity.ParentClass is null)
            {
                return;
            }

            var entityTypeConfigTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.EntityTypeConfiguration", userIdentityEntity.Id);
            if (entityTypeConfigTemplate is not null)
            {
                entityTypeConfigTemplate.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var configMethod = @class.FindMethod("Configure");
                    configMethod.FindAndReplaceStatement(s => s.Text.Contains("builder.HasBaseType"), new Common.CSharp.Builder.CSharpStatement($"builder.HasBaseType<{userIdentityEntity.ParentClass.Name}<string>>();"));
                });
            }

            var entityTypeTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Entities.DomainEntity", userIdentityEntity.Id);
            if (entityTypeTemplate is not null)
            {
                entityTypeTemplate.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    @class.AddProperty("string", "Id", c => c.Override().WithInitialValue("Guid.NewGuid().ToString()"));
                });
            }
        }

        private void ModifyIdentityServiceController(IApplication application, ServiceModel identityService)
        {
            var serviceImplementationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.Controllers.Controller", identityService.Id);

            serviceImplementationTemplate.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("Microsoft.AspNetCore.Routing");
                var @class = file.Classes.First();
                var confirmEmailMethod = @class.Methods.First(x => x.Name == "ConfirmEmail");
                confirmEmailMethod.AddAttribute("EndpointName(\"ConfirmEmail\")");

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
                        case "ConfirmEmail":
                            {
                                var method = @class.FindMethod("ConfirmEmail");
                                method.Attributes.Remove(method.Attributes.FirstOrDefault(a => a.Name == "IntentManaged"));
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                                method.Statements.Clear();
                                method.AddStatement("return await _identityServiceManager.ConfirmEmail(userId, code, changedEmail);");
                                break;
                            }
                        case "ForgotPassword":
                            {
                                var method = @class.FindMethod("ForgotPassword");
                                method.Attributes.Remove(method.Attributes.FirstOrDefault(a => a.Name == "IntentManaged"));
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                                method.Statements.Clear();
                                method.AddStatement("await _identityServiceManager.ForgotPassword(resetRequest);");
                                break;
                            }
                        case "GetInfo":
                            {
                                var method = @class.FindMethod("GetInfo");
                                method.Attributes.Remove(method.Attributes.FirstOrDefault(a => a.Name == "IntentManaged"));
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                                method.Statements.Clear();
                                method.AddStatement("return await _identityServiceManager.GetInfo();");
                                break;
                            }
                        case "Login":
                            {
                                var method = @class.FindMethod("Login");
                                method.Attributes.Remove(method.Attributes.FirstOrDefault(a => a.Name == "IntentManaged"));
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                                method.Statements.Clear();
                                method.AddStatement("return await _identityServiceManager.Login(login, useCookies, useSessionCookies);");
                                break;
                            }
                        case "Refresh":
                            {
                                var method = @class.FindMethod("Refresh");
                                method.Attributes.Remove(method.Attributes.FirstOrDefault(a => a.Name == "IntentManaged"));
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                                method.Statements.Clear();
                                method.AddStatement("return await _identityServiceManager.Refresh(refreshRequest);");
                                break;
                            }
                        case "Register":
                            {
                                var method = @class.FindMethod("Register");
                                method.Attributes.Remove(method.Attributes.FirstOrDefault(a => a.Name == "IntentManaged"));
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                                method.Statements.Clear();
                                method.AddStatement("await _identityServiceManager.Register(register);");
                                break;
                            }
                        case "ResendConfirmationEmail":
                            {
                                var method = @class.FindMethod("ResendConfirmationEmail");
                                method.Attributes.Remove(method.Attributes.FirstOrDefault(a => a.Name == "IntentManaged"));
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                                method.Statements.Clear();
                                method.AddStatement("await _identityServiceManager.ResendConfirmationEmail(resendRequest);");
                                break;
                            }
                        case "ResetPassword":
                            {
                                var method = @class.FindMethod("ResetPassword");
                                method.Attributes.Remove(method.Attributes.FirstOrDefault(a => a.Name == "IntentManaged"));
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                                method.Statements.Clear();
                                method.AddStatement("await _identityServiceManager.ResetPassword(resetRequest);");
                                break;
                            }
                        case "UpdateInfo":
                            {
                                var method = @class.FindMethod("UpdateInfo");
                                method.Attributes.Remove(method.Attributes.FirstOrDefault(a => a.Name == "IntentManaged"));
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                                method.Statements.Clear();
                                method.AddStatement("return await _identityServiceManager.UpdateInfo(infoRequest);");
                                break;
                            }
                        case "UpdateTwoFactor":
                            {
                                var method = @class.FindMethod("UpdateTwoFactor");
                                method.Attributes.Remove(method.Attributes.FirstOrDefault(a => a.Name == "IntentManaged"));
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                                method.Statements.Clear();
                                method.AddStatement("return await _identityServiceManager.UpdateTwoFactor(tfaRequest);");
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