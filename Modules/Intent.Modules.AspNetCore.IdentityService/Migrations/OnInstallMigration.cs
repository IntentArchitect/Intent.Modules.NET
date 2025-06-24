using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Services.Api;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using static Intent.AspNetCore.IdentityService.Api.ServiceModelStereotypeExtensions;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityService.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;
        private const string DomainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";


        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.AspNetCore.IdentityService";

        public void OnInstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var serviceDesignerId = "81104ae6-2bc5-4bae-b05a-f987b0372d81";
            var packages = app.GetDesigner(serviceDesignerId).GetPackages();
            foreach (var pkg in packages)
            {
                var services = pkg.GetElementsOfType("b16578a5-27b1-4047-a8df-f0b783d706bd");
                foreach (var service in services)
                {
                    if (service.Stereotypes.Any(s => s.DefinitionId == IdentityServiceHandler.DefinitionId))
                    {
                        return;
                    }
                }
            }

            var package = packages.FirstOrDefault(p => p.SpecializationTypeId == "df45eaf6-9202-4c25-8dd5-677e9ba1e906");

            if (package is null)
            {
                return;
            }
            // Add Identity Folder
            var identityFolderId = Guid.NewGuid().ToString();
            var identityServiceId = Guid.NewGuid().ToString();
            var confirmEmailEndpointId = Guid.NewGuid().ToString();
            var forgotPasswordEndpointId = Guid.NewGuid().ToString();
            var getInfoEndpointId = Guid.NewGuid().ToString();
            var loginEndpointId = Guid.NewGuid().ToString();
            var refreshTokenEndpointId = Guid.NewGuid().ToString();
            var registerEndpointId = Guid.NewGuid().ToString();
            var resendConfirmationEmailEndpointId = Guid.NewGuid().ToString();
            var resetPasswordEndpointId = Guid.NewGuid().ToString();
            var updateInfoEndpointId = Guid.NewGuid().ToString();
            var updateTwoFactorEndpointId = Guid.NewGuid().ToString();
            var accessTokenResponseDtoId = Guid.NewGuid().ToString();

            package.Classes.Add(new ElementPersistable
            {
                Id = identityFolderId,
                SpecializationType = "Folder",
                SpecializationTypeId = "4d95d53a-8855-4f35-aa82-e312643f5c5f",
                Name = "Identity",
                Display = "Identity",
                IsAbstract = false,
                SortChildren = SortChildrenOptions.SortByTypeAndName,
                IsMapped = false,
                ParentFolderId = package.Id,
                PackageId = package.Id,
                PackageName = package.Name,
                Stereotypes = new List<StereotypePersistable>
                    {
                        new StereotypePersistable
                        {
                            DefinitionId = Guid.Parse("66fd9e66-42c7-4ef9-a778-b68e009272b9").ToString(),
                            Name = "Folder Options",
                            AddedByDefault = true,
                            DefinitionPackageName = "Intent.Common.CSharp",
                            DefinitionPackageId = Guid.Parse("730e1275-0c32-44f7-991a-9619d07ca68d").ToString(),
                            Properties = new List<StereotypePropertyPersistable>
                            {
                                new StereotypePropertyPersistable
                                {
                                    Name = "Namespace Provider",
                                    Value = "true",
                                    IsActive = true
                                }
                            }
                        }
                    }
            });

            CreateDto(package, accessTokenResponseDtoId, "AccessTokenResponseDto", "AccessTokenResponseDto", identityFolderId, new List<ElementPersistable>
                {
                    CreateDtoField("TokenType", "TokenType: string", accessTokenResponseDtoId, package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(false, false)
                                        }),
                    CreateDtoField("AccessToken", "AccessToken: string", accessTokenResponseDtoId, package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(false, false)
                                        }),
                    CreateDtoField("ExpiresIn", "ExpiresIn: long", accessTokenResponseDtoId, package.Id, package.Name, DateTimeTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(false, false)
                                        }),
                    CreateDtoField("RefreshToken", "RefreshToken: string", accessTokenResponseDtoId, package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(false, false)
                                        })
                });

            var identityFolder = package.Classes.First(f => f.Id == identityFolderId);

            package.ChildElements.Add(
                    new ElementPersistable
                    {
                        Id = identityServiceId,
                        Name = "IdentityService",
                        Display = "IdentityService",
                        IsAbstract = false,
                        SortChildren = SortChildrenOptions.SortByTypeThenManually,
                        IsMapped = false,
                        ParentFolderId = package.Id,
                        PackageId = package.Id,
                        PackageName = package.Name,
                        SpecializationType = "Service",
                        SpecializationTypeId = "b16578a5-27b1-4047-a8df-f0b783d706bd",
                        Stereotypes = new List<StereotypePersistable>
                        {
                                new StereotypePersistable
                                {
                                    DefinitionId = "c29224ec-d473-4b95-ad4a-ec55c676c4fd",
                                    Name = "Http Service Settings",
                                    AddedByDefault = false,
                                    DefinitionPackageId = "0011387a-b122-45d7-9cdb-8e21b315ab9f",
                                    DefinitionPackageName = "Intent.Metadata.WebApi",
                                    Properties = new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "1e223bd0-7a72-435a-8741-a612d88e4a12", Name = "Route", Value="", IsActive = true}
                                    }
                                },
                                new StereotypePersistable
                                {
                                    DefinitionId = "8ef84001-167a-4cbb-8950-e519937e7981",
                                    AddedByDefault = false,
                                    DefinitionPackageName = "Intent.AspNetCore.IdentityService",
                                    DefinitionPackageId = "a1a75470-3437-43b1-be57-f2187693929b",
                                    Name = "Identity Service"
                                }
                        },
                        ChildElements = new List<ElementPersistable>
                        {
                                CreateEndpoint(confirmEmailEndpointId, "ConfirmEmail", "ConfirmEmail(userId: string, code: string, changedEmail: string?): string",
                                identityServiceId, package.Id, package.Name, StringTypeReference(), new List<StereotypePropertyPersistable>
                                {
                                    new StereotypePropertyPersistable { DefinitionId = "801c3e61-4431-4d81-93fa-7e57d33cb3fa", Name = "Verb",  Value="GET", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "5dd3e07d-76eb-45d4-9956-4325fb068acc", Name = "Route", Value="confirmEmail", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8", Name = "Return Type Mediatype", Value="Default",IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "e3870725-34b3-4684-85f2-ec4a667207fb", Name = "Success Response Code", IsActive = true }
                                },
                                new List<ElementPersistable>
                                {
                                    CreateInParameter(Guid.NewGuid().ToString(), "userId", "in : userId: string", confirmEmailEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Query", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = true }
                                    }, StringTypeReference()),
                                    CreateInParameter(Guid.NewGuid().ToString(), "code", "in : code: string", confirmEmailEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Query", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = true }
                                    }, StringTypeReference()),
                                    CreateInParameter(Guid.NewGuid().ToString(), "changedEmail", "in : changedEmail: string", confirmEmailEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Query", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = true }
                                    }, StringTypeReference(true))
                                }),
                                CreateEndpoint(forgotPasswordEndpointId, "ForgotPassword", "ForgotPassword(resetRequest: ForgotPasswordRequestDto): void",
                                identityServiceId, package.Id, package.Name, VoidTypeReference(), new List<StereotypePropertyPersistable>
                                {
                                    new StereotypePropertyPersistable { DefinitionId = "801c3e61-4431-4d81-93fa-7e57d33cb3fa", Name = "Verb",  Value="POST", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "5dd3e07d-76eb-45d4-9956-4325fb068acc", Name = "Route", Value="forgotPassword", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8", Name = "Return Type Mediatype", Value="Default",IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "e3870725-34b3-4684-85f2-ec4a667207fb", Name = "Success Response Code", Value="200 (Ok)", IsActive = true }
                                },
                                new List<ElementPersistable>
                                {
                                    CreateInParameter(Guid.NewGuid().ToString(), "resetRequest", "in : resetRequest: ForgotPasswordRequestDto", forgotPasswordEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Body", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = false }
                                    }, CustomTypeReference(package.Name, package.Id, CreateDto(package, "0fb483ce-b561-46e8-b9b5-6d07082792e6", "ForgotPasswordRequestDto", "ForgotPasswordRequestDto", identityFolderId, new List<ElementPersistable>
                                    {
                                        CreateDtoField("Email", "Email: string", "0fb483ce-b561-46e8-b9b5-6d07082792e6", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(true, true)
                                        })
                                    }
                                    )))
                                }),
                                CreateEndpoint(getInfoEndpointId, "GetInfo", "GetInfo(): InfoResponseDto",
                                identityServiceId, package.Id, package.Name,
                                CustomTypeReference(package.Name, package.Id, CreateDto(package, "8b6cbf88-5100-4505-b6ce-4a880dd55e2c", "InfoResponseDto", "InfoResponseDto", identityFolderId, new List<ElementPersistable>
                                {
                                    CreateDtoField("Email", "Email: string", "8b6cbf88-5100-4505-b6ce-4a880dd55e2c", package.Id, package.Name, StringTypeReference(), new List<StereotypePersistable>
                                    {
                                        CreateValidationsStereotype(true, true)
                                    }),
                                    CreateDtoField("IsEmailConfirmed", "IsEmailConfirmed: bool", "8b6cbf88-5100-4505-b6ce-4a880dd55e2c", package.Id, package.Name, BoolTypeReference(), new List<StereotypePersistable>
                                    {
                                        CreateValidationsStereotype(true, false)
                                    })
                                })), new List<StereotypePropertyPersistable>
                                {
                                    new StereotypePropertyPersistable { DefinitionId = "801c3e61-4431-4d81-93fa-7e57d33cb3fa", Name = "Verb",  Value="GET", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "5dd3e07d-76eb-45d4-9956-4325fb068acc", Name = "Route", Value="manage/info", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8", Name = "Return Type Mediatype", Value="Default",IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "e3870725-34b3-4684-85f2-ec4a667207fb", Name = "Success Response Code", IsActive = true }
                                },
                                new List<ElementPersistable>
                                {

                                }, true),
                                CreateEndpoint(loginEndpointId, "Login", "Login(login: LoginRequestDto, useCookies: bool?, useSessionCookies: bool?): void",
                                identityServiceId, package.Id, package.Name, CustomTypeReference(package.Name, package.Id,accessTokenResponseDtoId), new List<StereotypePropertyPersistable>
                                {
                                    new StereotypePropertyPersistable { DefinitionId = "801c3e61-4431-4d81-93fa-7e57d33cb3fa", Name = "Verb",  Value="POST", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "5dd3e07d-76eb-45d4-9956-4325fb068acc", Name = "Route", Value="login", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8", Name = "Return Type Mediatype", Value="Default",IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "e3870725-34b3-4684-85f2-ec4a667207fb", Name = "Success Response Code", Value="200 (Ok)", IsActive = true }
                                },
                                new List<ElementPersistable>
                                {
                                    CreateInParameter(Guid.NewGuid().ToString(), "login", "in : login: LoginRequestDto", loginEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Body", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = false }
                                    }, CustomTypeReference(package.Name, package.Id, CreateDto(package, "3775e275-50c9-4fc9-b450-03d0386cdf91", "LoginRequestDto", "LoginRequestDto", identityFolderId, new List<ElementPersistable>
                                    {
                                        CreateDtoField("Email", "Email: string", "3775e275-50c9-4fc9-b450-03d0386cdf91", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(true, true)
                                        }),
                                        CreateDtoField("Password", "Password: string", "3775e275-50c9-4fc9-b450-03d0386cdf91", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(true, false)
                                        }),
                                        CreateDtoField("TwoFactorCode", "TwoFactorCode: string?", "3775e275-50c9-4fc9-b450-03d0386cdf91", package.Id, package.Name, StringTypeReference(true),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(false, false)
                                        })
                                        ,
                                        CreateDtoField("TwoFactorRecoveryCode", "TwoFactorRecoveryCode: string?", "3775e275-50c9-4fc9-b450-03d0386cdf91", package.Id, package.Name, StringTypeReference(true),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(false, false)
                                        })
                                    }
                                    ))),
                                    CreateInParameter(Guid.NewGuid().ToString(), "useCookies", "in : useCookies: bool?", loginEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Query", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = true }
                                    }, BoolTypeReference(true)),
                                    CreateInParameter(Guid.NewGuid().ToString(), "useSessionCookies", "in : useSessionCookies: bool?", loginEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Query", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = true }
                                    }, BoolTypeReference(true))
                                }),
                                CreateEndpoint(refreshTokenEndpointId, "Refresh", "Refresh(refreshRequest: RefreshRequestDto): AccessTokenResponseDto",
                                identityServiceId, package.Id, package.Name, CustomTypeReference(package.Name, package.Id,accessTokenResponseDtoId), new List<StereotypePropertyPersistable>
                                {
                                    new StereotypePropertyPersistable { DefinitionId = "801c3e61-4431-4d81-93fa-7e57d33cb3fa", Name = "Verb",  Value="POST", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "5dd3e07d-76eb-45d4-9956-4325fb068acc", Name = "Route", Value="refresh", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8", Name = "Return Type Mediatype", Value="Default",IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "e3870725-34b3-4684-85f2-ec4a667207fb", Name = "Success Response Code", Value="200 (Ok)", IsActive = true }
                                },
                                new List<ElementPersistable>
                                {
                                    CreateInParameter(Guid.NewGuid().ToString(), "refreshRequest", "in : refreshRequest: RefreshRequestDto", refreshTokenEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Body", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = false }
                                    }, CustomTypeReference(package.Name, package.Id, CreateDto(package, "e68ed718-58c2-4fb9-953a-aa250d459321", "RefreshRequestDto", "RefreshRequestDto", identityFolderId, new List<ElementPersistable>
                                    {
                                        CreateDtoField("RefreshToken", "RefreshToken: string", "e68ed718-58c2-4fb9-953a-aa250d459321", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(true, false)
                                        })
                                    }
                                    )))
                                }),
                                CreateEndpoint(registerEndpointId, "Register", "Register(registration: RegisterRequestDto): void",
                                identityServiceId, package.Id, package.Name, VoidTypeReference(), new List<StereotypePropertyPersistable>
                                {
                                    new StereotypePropertyPersistable { DefinitionId = "801c3e61-4431-4d81-93fa-7e57d33cb3fa", Name = "Verb",  Value="POST", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "5dd3e07d-76eb-45d4-9956-4325fb068acc", Name = "Route", Value="register", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8", Name = "Return Type Mediatype", Value="Default",IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "e3870725-34b3-4684-85f2-ec4a667207fb", Name = "Success Response Code", Value="200 (Ok)", IsActive = true }
                                },
                                new List<ElementPersistable>
                                {
                                    CreateInParameter(Guid.NewGuid().ToString(), "register", "in : register: RegisterRequestDto", registerEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Body", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = false }
                                    }, CustomTypeReference(package.Name, package.Id, CreateDto(package, "34c256c7-379a-4d09-abe4-cced74dc1b83", "RegisterRequestDto", "RegisterRequestDto", identityFolderId, new List<ElementPersistable>
                                    {
                                        CreateDtoField("Email", "Email: string", "34c256c7-379a-4d09-abe4-cced74dc1b83", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(true, true)
                                        }),
                                        CreateDtoField("Password", "Password: string", "34c256c7-379a-4d09-abe4-cced74dc1b83", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(true, false)
                                        })
                                    }
                                    )))
                                }),
                                CreateEndpoint(resendConfirmationEmailEndpointId, "ResendConfirmationEmail", "ResendConfirmationEmail(resendRequest: ResendConfirmationEmailRequestDto): void",
                                identityServiceId, package.Id, package.Name, VoidTypeReference(), new List<StereotypePropertyPersistable>
                                {
                                    new StereotypePropertyPersistable { DefinitionId = "801c3e61-4431-4d81-93fa-7e57d33cb3fa", Name = "Verb",  Value="POST", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "5dd3e07d-76eb-45d4-9956-4325fb068acc", Name = "Route", Value="resendConfirmationEmail", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8", Name = "Return Type Mediatype", Value="Default",IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "e3870725-34b3-4684-85f2-ec4a667207fb", Name = "Success Response Code", Value="200 (Ok)", IsActive = true }
                                },
                                new List<ElementPersistable>
                                {
                                    CreateInParameter(Guid.NewGuid().ToString(), "resendRequest", "in : resendRequest: ResendConfirmationEmailRequestDto", resendConfirmationEmailEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Body", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = false }
                                    }, CustomTypeReference(package.Name, package.Id, CreateDto(package, "83019f86-e9da-4a3b-8cf3-8489cfd88a9e", "ResendConfirmationEmailRequestDto", "ResendConfirmationEmailRequestDto", identityFolderId, new List<ElementPersistable>
                                    {
                                        CreateDtoField("Email", "Email: string", "83019f86-e9da-4a3b-8cf3-8489cfd88a9e", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(true, true)
                                        })
                                    }
                                    )))
                                }),
                                CreateEndpoint(resetPasswordEndpointId, "ResetPassword", "ResetPassword(resetRequest: ResetPasswordRequestDto): void",
                                identityServiceId, package.Id, package.Name, VoidTypeReference(), new List<StereotypePropertyPersistable>
                                {
                                    new StereotypePropertyPersistable { DefinitionId = "801c3e61-4431-4d81-93fa-7e57d33cb3fa", Name = "Verb",  Value="POST", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "5dd3e07d-76eb-45d4-9956-4325fb068acc", Name = "Route", Value="resetPassword", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8", Name = "Return Type Mediatype", Value="Default",IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "e3870725-34b3-4684-85f2-ec4a667207fb", Name = "Success Response Code", Value="200 (Ok)", IsActive = true }
                                },
                                new List<ElementPersistable>
                                {
                                    CreateInParameter(Guid.NewGuid().ToString(), "resetRequest", "in : resetRequest: ResetPasswordRequestDto", resetPasswordEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Body", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = false }
                                    }, CustomTypeReference(package.Name, package.Id, CreateDto(package, "379bada3-e556-4918-983e-b0d67a68260f", "ResetPasswordRequestDto", "ResetPasswordRequestDto", identityFolderId, new List<ElementPersistable>
                                    {
                                        CreateDtoField("Email", "Email: string", "379bada3-e556-4918-983e-b0d67a68260f", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(true, true)
                                        }),
                                        CreateDtoField("ResetCode", "ResetCode: string", "379bada3-e556-4918-983e-b0d67a68260f", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(true, false)
                                        }),
                                        CreateDtoField("NewPassword", "NewPassword: string", "379bada3-e556-4918-983e-b0d67a68260f", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(true, false)
                                        })
                                    }
                                    )))
                                }),
                                CreateEndpoint(updateInfoEndpointId, "UpdateInfo", "UpdateInfo(infoRequest: InfoRequestDto): InfoResponseDto",
                                identityServiceId, package.Id, package.Name, CustomTypeReference(package.Name, package.Id, "8b6cbf88-5100-4505-b6ce-4a880dd55e2c"), new List<StereotypePropertyPersistable>
                                {
                                    new StereotypePropertyPersistable { DefinitionId = "801c3e61-4431-4d81-93fa-7e57d33cb3fa", Name = "Verb",  Value="POST", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "5dd3e07d-76eb-45d4-9956-4325fb068acc", Name = "Route", Value="manage/info", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8", Name = "Return Type Mediatype", Value="Default",IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "e3870725-34b3-4684-85f2-ec4a667207fb", Name = "Success Response Code", Value="200 (Ok)", IsActive = true }
                                },
                                new List<ElementPersistable>
                                {
                                    CreateInParameter(Guid.NewGuid().ToString(), "infoRequest", "in : infoRequest: InfoRequestDto", updateInfoEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Body", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = false }
                                    }, CustomTypeReference(package.Name, package.Id, CreateDto(package, "d57d393c-507b-4c50-8eaf-6a5005787c08", "InfoRequestDto", "InfoRequestDto", identityFolderId, new List<ElementPersistable>
                                    {
                                        CreateDtoField("NewEmail", "NewEmail: string?", "d57d393c-507b-4c50-8eaf-6a5005787c08", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(false, true)
                                        }),
                                        CreateDtoField("NewPassword", "NewPassword: string?", "d57d393c-507b-4c50-8eaf-6a5005787c08", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(false, false)
                                        }),
                                        CreateDtoField("OldPassword", "OldPassword: string", "d57d393c-507b-4c50-8eaf-6a5005787c08", package.Id, package.Name, StringTypeReference(),
                                        new List<StereotypePersistable>
                                        {
                                           CreateValidationsStereotype(false, false)
                                        })
                                    }
                                    )))
                                }, true),
                                CreateEndpoint(updateTwoFactorEndpointId, "UpdateTwoFactor", "UpdateTwoFactor(tfaRequest: TwoFactorRequestDto): TwoFactorResponseDto",
                                identityServiceId, package.Id, package.Name, CustomTypeReference(package.Name, package.Id, CreateDto(package, "d26e5694-d249-43c8-95c9-2621a85b6ca4", "TwoFactorResponseDto",
                                "TwoFactorResponseDto", identityFolderId, new List<ElementPersistable>
                                {
                                    CreateDtoField("SharedKey", "SharedKey: string", "d26e5694-d249-43c8-95c9-2621a85b6ca4", package.Id, package.Name, StringTypeReference(), new List<StereotypePersistable>
                                    {
                                        CreateValidationsStereotype(true, false)
                                    }),
                                    CreateDtoField("RecoveryCodesLeft", "RecoveryCodesLeft: int", "d26e5694-d249-43c8-95c9-2621a85b6ca4", package.Id, package.Name, IntTypeReference(), new List<StereotypePersistable>
                                    {
                                        CreateValidationsStereotype(true, false)
                                    }),
                                    CreateDtoField("RecoveryCodes", "RecoveryCodes: string[]?", "d26e5694-d249-43c8-95c9-2621a85b6ca4", package.Id, package.Name, StringArrayTypeReference(true), new List<StereotypePersistable>
                                    {
                                        CreateValidationsStereotype(false, false)
                                    }),
                                    CreateDtoField("IsTwoFactorEnabled", "IsTwoFactorEnabled: bool", "d26e5694-d249-43c8-95c9-2621a85b6ca4", package.Id, package.Name, BoolTypeReference(), new List<StereotypePersistable>
                                    {
                                        CreateValidationsStereotype(true, false)
                                    }),
                                    CreateDtoField("IsMachineRemembered", "IsMachineRemembered: bool", "d26e5694-d249-43c8-95c9-2621a85b6ca4", package.Id, package.Name, BoolTypeReference(), new List<StereotypePersistable>
                                    {
                                        CreateValidationsStereotype(true, false)
                                    })
                                })), new List<StereotypePropertyPersistable>
                                {
                                    new StereotypePropertyPersistable { DefinitionId = "801c3e61-4431-4d81-93fa-7e57d33cb3fa", Name = "Verb",  Value="POST", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "5dd3e07d-76eb-45d4-9956-4325fb068acc", Name = "Route", Value="manage/2fa", IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "4490e212-1e99-43ce-b3dd-048ed2a6bae8", Name = "Return Type Mediatype", Value="Default",IsActive = true },
                                    new StereotypePropertyPersistable { DefinitionId = "e3870725-34b3-4684-85f2-ec4a667207fb", Name = "Success Response Code", Value="200 (Ok)", IsActive = true }
                                },
                                new List<ElementPersistable>
                                {
                                    CreateInParameter(Guid.NewGuid().ToString(), "tfaRequest", "in : tfaRequest: TwoFactorRequestDto", updateTwoFactorEndpointId, package.Id,
                                    package.Name, new List<StereotypePropertyPersistable>
                                    {
                                        new StereotypePropertyPersistable { DefinitionId = "d2630e0f-f930-404f-b453-1e8052a712f5", Name = "Source",  Value="From Body", IsActive = true },
                                        new StereotypePropertyPersistable { DefinitionId = "7a331e9b-f13c-4b33-9013-bd59b4a4999c", Name = "Header Name", IsActive = false },
                                        new StereotypePropertyPersistable { DefinitionId = "c8caa58e-972a-42f2-983e-652ceee762b2", Name = "Query String Name", IsActive = false }
                                    }, CustomTypeReference(package.Name, package.Id, CreateDto(package, "4bc3ad74-244d-405e-9aa0-3d1c6b9b7e94", "TwoFactorRequestDto",
                                        "TwoFactorRequestDto", identityFolderId, new List<ElementPersistable>
                                        {
                                            CreateDtoField("Enable", "Enable: bool?", "4bc3ad74-244d-405e-9aa0-3d1c6b9b7e94", package.Id, package.Name, BoolTypeReference(true), new List<StereotypePersistable>
                                            {
                                                CreateValidationsStereotype(false, false)
                                            }),
                                            CreateDtoField("TwoFactorCode", "TwoFactorCode: string?", "4bc3ad74-244d-405e-9aa0-3d1c6b9b7e94", package.Id, package.Name, StringTypeReference(true), new List<StereotypePersistable>
                                            {
                                                CreateValidationsStereotype(false, false)
                                            }),
                                            CreateDtoField("ResetSharedKey", "ResetSharedKey: bool", "4bc3ad74-244d-405e-9aa0-3d1c6b9b7e94", package.Id, package.Name, BoolTypeReference(), new List<StereotypePersistable>
                                            {
                                                CreateValidationsStereotype(true, false)
                                            }),
                                            CreateDtoField("ResetRecoveryCodes", "ResetRecoveryCodes: bool", "4bc3ad74-244d-405e-9aa0-3d1c6b9b7e94", package.Id, package.Name, BoolTypeReference(), new List<StereotypePersistable>
                                            {
                                                CreateValidationsStereotype(true, false)
                                            }),
                                            CreateDtoField("ForgetMachine", "ForgetMachine: bool", "4bc3ad74-244d-405e-9aa0-3d1c6b9b7e94", package.Id, package.Name, BoolTypeReference(), new List<StereotypePersistable>
                                            {
                                                CreateValidationsStereotype(true, false)
                                            })
                                        })))
                                }, true)
                        }
                    });



            package.Save();

            app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);


            if (app.Modules.Any(m => m.ModuleId == "Intent.ModularMonolith.Module"))
            {
                app = null;
                var solution = SolutionPersistable.Load(Path.Combine(_configurationProvider.GetSolutionConfig().SolutionRootLocation, _configurationProvider.GetSolutionConfig().SolutionName + ".isln"));
                foreach (var current in solution.GetApplications())
                {
                    if (current.Modules.Any(m => m.ModuleId == "Intent.ModularMonolith.Host"))
                    {
                        app = current;
                        break;
                    }
                }
                if (app is null)
                {
                    throw new System.Exception($"Unable to find Modular Monolith Host for : {app.Name} (Application)");
                }
            }

            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == "2c1a1918-97eb-4b21-8a15-021dd72db73c");
            if (group == null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = "2c1a1918-97eb-4b21-8a15-021dd72db73c",
                    Module = "Intent.Security.JWT",
                    Title = "JWT Security Settings"
                };

                app.ModuleSettingGroups.Add(group);
            }

            var bearerAuthenticationType = group.Settings.FirstOrDefault(s => s.Id == "c2fba79a-6285-4ec8-ad76-585f089d8aa5");

            if (bearerAuthenticationType == null)
            {
                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "c2fba79a-6285-4ec8-ad76-585f089d8aa5",
                    Title = "JWT Bearer Authentication Type",
                    Module = "Intent.Security.JWT",
                    Hint = "Configure how you JWT Token is authenticated",
                    ControlType = ModuleSettingControlType.Select,
                    Value = "manual",
                    Options =
                    [
                        new SettingFieldOptionPersistable { Description = "OIDC JWT Auth", Value = "oidc" },
                        new SettingFieldOptionPersistable { Description = "Manual JWT Auth", Value = "manual" }
                    ]
                });
            }
            else
            {
                bearerAuthenticationType.Value = "manual";
            }

            app.SaveAllChanges();

            var designer = app.GetDesigner(DomainDesignerId);
            package = designer.GetPackages().FirstOrDefault();
            var diagrams = package.GetElementsOfType("4d66fecd-e9b8-436f-aa50-c59040ad0879");

            if (!diagrams.Any(d => d.Name == "Identity Diagram"))
            {
                return;
            }

            var identityDiagram = diagrams.First(d => d.Name == "Identity Diagram");

            var applicationIdentityUserId = Guid.NewGuid().ToString();
            package.Classes.Add(new ElementPersistable
            {
                Id = applicationIdentityUserId,
                SpecializationType = "Class",
                SpecializationTypeId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10",
                Name = "ApplicationIdentityUser",
                Display = "ApplicationIdentityUser: IdentityUser<string>",
                IsAbstract = false,
                IsMapped = false,
                ParentFolderId = package.Id,
                PackageId = package.Id,
                PackageName = package.Name,
                ChildElements = new System.Collections.Generic.List<ElementPersistable>
                {
                    new ElementPersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        SpecializationType = "Attribute",
                        SpecializationTypeId = "0090fb93-483e-41af-a11d-5ad2dc796adf",
                        ParentFolderId = applicationIdentityUserId,
                        PackageId= package.Id,
                        PackageName = package.Name,
                        Name = "RefreshToken",
                        Display = "RefreshToken: string",
                        TypeReference = new TypeReferencePersistable
                        {
                            Id = Guid.NewGuid().ToString(),
                            TypeId = "d384db9c-a279-45e1-801e-e4e8099625f2",
                            TypePackageName = "Intent.Common.Types",
                            TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67",
                            IsRequired = true,
                            IsNavigable = true,
                            IsNullable = true,
                            IsCollection = false
                        },
                        Stereotypes = new System.Collections.Generic.List<IArchitect.Agent.Persistence.Model.Common.StereotypePersistable>
                        {
                            new IArchitect.Agent.Persistence.Model.Common.StereotypePersistable
                            {
                                DefinitionId = "6347286E-A637-44D6-A5D7-D9BE5789CA7A",
                                Name = "Text CConstraints",
                                DefinitionPackageId = "AF8F3810-745C-42A2-93C8-798860DC45B1",
                                DefinitionPackageName = "Intent.Metadata.RDBMS",
                                Properties = new System.Collections.Generic.List<IArchitect.Agent.Persistence.Model.Common.StereotypePropertyPersistable>
                                {
                                    new IArchitect.Agent.Persistence.Model.Common.StereotypePropertyPersistable { DefinitionId = "1288cfcd-ee51-437e-9713-73b80118f026", Name = "SQL Data Type", Value = "DEFAULT", IsActive = true },
                                    new IArchitect.Agent.Persistence.Model.Common.StereotypePropertyPersistable { DefinitionId = "A04CC24D-81FB-4EA2-A34A-B3C58E04DCFD", Name = "MaxLength", IsActive = true },
                                    new IArchitect.Agent.Persistence.Model.Common.StereotypePropertyPersistable { DefinitionId = "67EC4CF4-7706-4B39-BC7C-DF539EE2B0AF", Name = "IsUnicode", Value = "false", IsActive = true }
                                }
                            }
                        }
                    },
                    new ElementPersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        SpecializationType = "Attribute",
                        SpecializationTypeId = "0090fb93-483e-41af-a11d-5ad2dc796adf",
                        ParentFolderId = applicationIdentityUserId,
                        PackageId= package.Id,
                        PackageName = package.Name,
                        Name = "RefreshTokenExpired",
                        Display = "RefreshTokenExpired: datetime",
                        TypeReference = new TypeReferencePersistable
                        {
                            Id = Guid.NewGuid().ToString(),
                            TypeId = "a4107c29-7851-4121-9416-cf1236908f1e",
                            TypePackageName = "Intent.Common.Types",
                            TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67",
                            IsRequired = true,
                            IsNavigable = true,
                            IsNullable = true,
                            IsCollection = false
                        }
                    }
                }
            });

            var associationVisualId = Guid.NewGuid().ToString();

            package.Associations.Add(new AssociationPersistable
            {
                AssociationType = "Generalization",
                AssociationTypeId = "5de35973-3ac7-4e65-b48c-385605aec561",
                Id = associationVisualId,
                SourceEnd = new AssociationEndPersistable
                {
                    Id = Guid.NewGuid().ToString(),
                    SpecializationType = "Generalization Source End",
                    SpecializationTypeId = "8190bf43-222c-4b53-8a44-14626efe3574",
                    Display = ": ApplicationIdentityUser",
                    Order = 0,
                    TypeReference = new TypeReferencePersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        TypeId = applicationIdentityUserId,
                        TypePackageName = package.Name,
                        TypePackageId = package.Id,
                        IsRequired = true,
                        IsNavigable = false,
                        IsNullable = false,
                        IsCollection = false
                    }
                },
                TargetEnd = new AssociationEndPersistable
                {
                    Id = Guid.NewGuid().ToString(),
                    SpecializationType = "Generalization Target End",
                    SpecializationTypeId = "4686cc1d-b4d8-4b99-b45b-f77bd5496946",
                    Display = "extends IdentityUser<string>",
                    Order = 0,
                    Name = "base",
                    TypeReference = new TypeReferencePersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        TypeId = "f6505b15-dced-4beb-9a58-e7b5447a7c73",
                        TypePackageName = "Intent.AspNetCore.Identity.Domain",
                        TypePackageId = "d1f3cf7b-cd9a-431f-af26-a86aec1ace6f",
                        IsRequired = true,
                        IsNavigable = false,
                        IsNullable = false,
                        IsCollection = false,
                        GenericTypeParameters = new System.Collections.Generic.List<TypeReferencePersistable>
                        {
                            new TypeReferencePersistable
                            {
                                Id = Guid.NewGuid().ToString(),
                                TypeId = "d384db9c-a279-45e1-801e-e4e8099625f2",
                                TypePackageName = "Intent.Common.Types",
                                TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67",
                                IsRequired = true,
                                IsNavigable = true,
                                IsNullable = false,
                                IsCollection = false,
                                GenericTypeId = "d618eff4-adab-4f6d-a758-6ecad2eb8429"
                            }
                        }
                    }
                }
            });

            identityDiagram.Diagram.ClassVisuals.Add(new IArchitect.Agent.Persistence.Model.Visual.ElementVisualPersistable
            {
                Id = applicationIdentityUserId,
                ZIndex = 13,
                AutoResizeEnabled = true,
                Position = new IArchitect.Agent.Persistence.Model.Visual.Point { X = 400, Y = 900 },
                Size = new IArchitect.Agent.Persistence.Model.Visual.Size { Height = 30, Width = 200 }
            });

            identityDiagram.Diagram.AssociationVisuals.Add(new IArchitect.Agent.Persistence.Model.Visual.AssociationVisualPersistable
            {
                Id = associationVisualId,
                SourceId = applicationIdentityUserId,
                TargetId = "f6505b15-dced-4beb-9a58-e7b5447a7c73",
                TargetPrefPoint = new IArchitect.Agent.Persistence.Model.Visual.Point { X = 100, Y = 310 },
                ZIndex = 14
            });

            package.Save();

            app.SaveAllChanges();
        }

        private StereotypePersistable CreateValidationsStereotype(bool notEmpty = false, bool isEmail = false)
        {
            return new StereotypePersistable
            {
                DefinitionId = "4b54a612-2664-4493-a1f7-dc0623aa03da",
                Name = "Validations",
                AddedByDefault = true,
                DefinitionPackageId = "55856e5d-8217-4c17-8fbb-082ac75baaf5",
                DefinitionPackageName = "Intent.Application.FluentValidation",
                Properties = new List<StereotypePropertyPersistable>
                {
                    new StereotypePropertyPersistable { DefinitionId = "d144e1aa-b506-407a-b60b-716ce38b70bb", Name = "Not Empty", Value = notEmpty.ToString().ToLower(), IsActive = true },
                    new StereotypePropertyPersistable { DefinitionId = "8d844d65-bf88-4023-85e1-4d3d7f8784cd", Name = "Equal", IsActive = true },
                    new StereotypePropertyPersistable { DefinitionId = "490e08cd-5fb4-4bfd-80cf-fda3b88f745e", Name = "Not Equal", IsActive = true },
                    new StereotypePropertyPersistable { DefinitionId = "c94dc001-4ec7-42a0-a1e0-5b75502e7bec", Name = "Min Length", IsActive = true },
                    new StereotypePropertyPersistable { DefinitionId = "cbd52384-0f1e-4f41-9e98-314231b731e1", Name = "Max Length", IsActive = true },
                    new StereotypePropertyPersistable { DefinitionId = "f9802e6b-b033-4280-ad83-e4b064943d87", Name = "Min", IsActive = false },
                    new StereotypePropertyPersistable { DefinitionId = "aca206a8-cb7e-4537-b3f5-c5c301863c14", Name = "Max", IsActive = false },
                    new StereotypePropertyPersistable { DefinitionId = "ad0c1113-ee22-4275-8177-576494ecfa45", Name = "Regular Expression", IsActive = true },
                    new StereotypePropertyPersistable { DefinitionId = "a4fdee2f-36d9-463c-bb2e-54579a16083e", Name = "Regular Expression Message", IsActive = false },
                    new StereotypePropertyPersistable { DefinitionId = "25d15e9b-7dbd-403e-837d-2fcfc786d3f2", Name = "Regular Expression Timeout", Value = "1", IsActive = false },
                    new StereotypePropertyPersistable { DefinitionId = "9023c463-1cce-4f11-baa5-96561697fc7f", Name = "Predicate", IsActive = true },
                    new StereotypePropertyPersistable { DefinitionId = "13d364d0-975e-47e9-ae0c-a3137326ee60", Name = "Predicate Message", IsActive = false },
                    new StereotypePropertyPersistable { DefinitionId = "85282cb5-5e12-4eb0-9568-7ccee0685e12", Name = "Email Address", Value = isEmail.ToString().ToLower(), IsActive = true },
                    new StereotypePropertyPersistable { DefinitionId = "e441d802-9aea-4d74-b5f9-1cd2527f3719", Name = "Has Custom Validation", Value = "false", IsActive = false },
                    new StereotypePropertyPersistable { DefinitionId = "e94008f2-3d5b-4157-ba72-69b3d6812617", Name = "Custom", Value = "false", IsActive = true },
                    new StereotypePropertyPersistable { DefinitionId = "60e7953f-29b8-4e56-a2c9-c02fae7de91e", Name = "Must", Value = "false", IsActive = true },
                    new StereotypePropertyPersistable { DefinitionId = "f43fcab6-5f8f-42e4-96d2-83d79cedfab9", Name = "Must Message", IsActive = false },
                    new StereotypePropertyPersistable { DefinitionId = "9ce77ae5-9ad5-4436-8ae0-c6c296de8492", Name = "CascadeMode", IsActive = true }
                }
            };
        }

        private ElementPersistable CreateDtoField(string name, string display, string parentFolderId, string packageId, string packageName,
            TypeReferencePersistable typeReference, List<StereotypePersistable> stereotypes)
        {
            return new ElementPersistable
            {
                Id = Guid.NewGuid().ToString(),
                SpecializationType = "DTO-Field",
                SpecializationTypeId = "7baed1fd-469b-4980-8fd9-4cefb8331eb2",
                Name = name,
                Display = display,
                ParentFolderId = parentFolderId,
                PackageId = packageId,
                PackageName = packageName,
                TypeReference = typeReference,
                Stereotypes = stereotypes
            };
        }

        private string CreateDto(PackageModelPersistable package, string id, string name, string display, string parentFolderId, List<ElementPersistable> dtoFields)
        {
            var folder = package.ChildElements.First(f => f.Id == parentFolderId);
            folder.ChildElements.Add(new ElementPersistable
            {
                Id = id,
                Name = name,
                Display = display,
                ParentFolderId = parentFolderId,
                PackageId = package.Id,
                PackageName = package.Name,
                SpecializationType = "DTO",
                SpecializationTypeId = "fee0edca-4aa0-4f77-a524-6bbd84e78734",
                IsAbstract = false,
                SortChildren = SortChildrenOptions.SortByTypeThenManually,
                ChildElements = dtoFields
            });

            return id;
        }

        private ElementPersistable CreateEndpoint(string id, string name, string display, string parentFolderId, string packageId, string packageName,
            TypeReferencePersistable typeReference, List<StereotypePropertyPersistable> httpSettingsProperties, List<ElementPersistable> inParameters,
            bool isSecure = false)
        {
            var endpoint = new ElementPersistable
            {
                Id = id,
                SpecializationType = "Operation",
                SpecializationTypeId = "e030c97a-e066-40a7-8188-808c275df3cb",
                Name = name,
                Display = display,
                IsAbstract = false,
                SortChildren = SortChildrenOptions.SortByTypeThenManually,
                TypeReference = typeReference,
                IsMapped = false,
                ParentFolderId = parentFolderId,
                PackageId = packageId,
                PackageName = packageName,
                Stereotypes = new List<StereotypePersistable>
                {
                    new StereotypePersistable
                    {
                        DefinitionId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6",
                        AddedByDefault = false,
                        DefinitionPackageName = "Intent.Metadata.WebApi",
                        DefinitionPackageId = "0011387a-b122-45d7-9cdb-8e21b315ab9f",
                        Name = "Http Settings",
                        Properties = httpSettingsProperties
                    },
                    new StereotypePersistable
                    {
                        DefinitionId = "8ef84001-167a-4cbb-8950-e519937e7981",
                        AddedByDefault = false,
                        DefinitionPackageName = "Intent.AspNetCore.IdentityService",
                        DefinitionPackageId = "a1a75470-3437-43b1-be57-f2187693929b",
                        Name = "Identity Service"
                    }
                },
                ChildElements = inParameters
            };

            if (isSecure)
            {
                endpoint.Stereotypes.Add(new StereotypePersistable
                {
                    DefinitionId = "a9eade71-1d56-4be7-a80c-81046c0c978b",
                    DefinitionPackageId = "a6fa1088-0064-43e3-a7fc-36c97b2b9285",
                    DefinitionPackageName = "Intent.Metadata.Security",
                    Name = "Secured",
                    Properties = new List<StereotypePropertyPersistable>
                    {
                        new StereotypePropertyPersistable { DefinitionId = "2b39acef-6079-48c9-b85e-2b0981f9de70", Name = "Roles", IsActive = true},
                        new StereotypePropertyPersistable { DefinitionId = "ae5251ff-40a1-4e46-be66-6b176f329f98", Name = "Policy", IsActive = true},
                        new StereotypePropertyPersistable { DefinitionId = "28bbe8bb-8d31-44c7-b642-ff0e279ab85f", Name = "Security Roles", IsActive = false},
                        new StereotypePropertyPersistable { DefinitionId = "68cbcd05-cd5c-49f3-a982-8ee9caf554bb", Name = "Security Policies", IsActive = false}
                    }
                });
            }

            return endpoint;
        }

        private ElementPersistable CreateInParameter(string id, string name, string display, string parentFolderId, string packageId, string packageName,
            List<StereotypePropertyPersistable> parameterSettingsProperties, TypeReferencePersistable typeReferencePersistable, bool isNullable = false)
        {
            return new ElementPersistable
            {
                Id = id,
                SpecializationType = "Parameter",
                SpecializationTypeId = "00208d20-469d-41cb-8501-768fd5eb796b",
                Name = name,
                Display = display,
                IsAbstract = false,
                TypeReference = typeReferencePersistable,
                IsMapped = false,
                ParentFolderId = parentFolderId,
                PackageId = packageId,
                PackageName = packageName,
                Stereotypes = new List<StereotypePersistable>
                {
                    new StereotypePersistable
                    {
                        DefinitionId = "d01df110-1208-4af8-a913-92a49d219552",
                        AddedByDefault = false,
                        DefinitionPackageName = "Intent.Metadata.WebApi",
                        DefinitionPackageId = "0011387a-b122-45d7-9cdb-8e21b315ab9f",
                        Name = "Parameter Settings",
                        Properties = parameterSettingsProperties
                    }
                }
            };
        }

        private static TypeReferencePersistable IntTypeReference(bool isNullable = false) => new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74",
            IsNavigable = true,
            IsNullable = isNullable,
            IsCollection = false,
            IsRequired = true,
            TypePackageName = "Intent.Common.Types",
            TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67"
        };

        private static TypeReferencePersistable StringArrayTypeReference(bool isNullable = false) => new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = "d384db9c-a279-45e1-801e-e4e8099625f2",
            IsNavigable = true,
            IsNullable = isNullable,
            IsCollection = true,
            IsRequired = true,
            TypePackageName = "Intent.Common.Types",
            TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67"
        };

        private static TypeReferencePersistable StringTypeReference(bool isNullable = false) => new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = "d384db9c-a279-45e1-801e-e4e8099625f2",
            IsNavigable = true,
            IsNullable = isNullable,
            IsCollection = false,
            IsRequired = true,
            TypePackageName = "Intent.Common.Types",
            TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67"
        };

        private static TypeReferencePersistable DateTimeTypeReference(bool isNullable = false) => new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = "a4107c29-7851-4121-9416-cf1236908f1e",
            IsNavigable = true,
            IsNullable = isNullable,
            IsCollection = false,
            IsRequired = true,
            TypePackageName = "Intent.Common.Types",
            TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67"
        };

        private static TypeReferencePersistable BoolTypeReference(bool isNullable = false) => new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = "e6f92b09-b2c5-4536-8270-a4d9e5bbd930",
            IsNavigable = true,
            IsNullable = isNullable,
            IsCollection = false,
            IsRequired = true,
            TypePackageName = "Intent.Common.Types",
            TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67"
        };

        private static TypeReferencePersistable VoidTypeReference() => new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            IsNavigable = true,
            IsNullable = false,
            IsCollection = false,
            IsRequired = true,
        };

        private static TypeReferencePersistable CustomTypeReference(string typePackageName, string typePackageId, string typeId) => new TypeReferencePersistable
        {
            Id = Guid.NewGuid().ToString(),
            TypeId = typeId,
            IsNavigable = true,
            IsNullable = false,
            IsCollection = false,
            IsRequired = true,
            TypePackageName = typePackageName,
            TypePackageId = typePackageId
        };
    }
}