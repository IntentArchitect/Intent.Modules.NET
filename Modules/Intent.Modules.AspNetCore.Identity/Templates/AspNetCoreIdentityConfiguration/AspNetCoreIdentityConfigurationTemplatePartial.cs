using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.Templates.AspNetCoreIdentityConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AspNetCoreIdentityConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Identity.AspNetCoreIdentityConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AspNetCoreIdentityConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreIdentityEntityFrameworkCore(outputTarget.GetProject()));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.AspNetCore.Identity")
                .AddUsing("System")
                .AddClass("AspNetCoreIdentityConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("void", "ConfigureIdentity", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param =>
                        {
                            param.WithThisModifier();
                        });
                        var dbContextName = GetDbContextName();
                        method.AddStatement(new CSharpMethodChainStatement($"services.AddIdentityWithoutCookieAuth<{this.GetIdentityUserClass()}, {this.GetIdentityRoleClass()}>()")
                            .AddChainStatement($@"AddEntityFrameworkStores<{dbContextName}>()")
                            .AddChainStatement($@"AddDefaultTokenProviders()"));
                        method.AddStatement(new CSharpInvocationStatement("services.Configure<IdentityOptions>")
                            .AddArgument(new CSharpLambdaBlock("options")
                                .AddStatement("// Password settings.")
                                .AddStatement("options.Password.RequireDigit = true;")
                                .AddStatement("options.Password.RequireLowercase = true;")
                                .AddStatement("options.Password.RequireNonAlphanumeric = true;")
                                .AddStatement("options.Password.RequireUppercase = true;")
                                .AddStatement("options.Password.RequiredLength = 6;")
                                .AddStatement("options.Password.RequiredUniqueChars = 1;")
                                .AddStatement(string.Empty)
                                .AddStatement("// Lockout settings.")
                                .AddStatement("options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);")
                                .AddStatement("options.Lockout.MaxFailedAccessAttempts = 5;")
                                .AddStatement("options.Lockout.AllowedForNewUsers = true;")
                                .AddStatement(string.Empty)
                                .AddStatement("// User settings.")
                                .AddStatement(@"options.User.AllowedUserNameCharacters = ""abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"";")
                                .AddStatement("options.User.RequireUniqueEmail = false;")
                            )
                            .WithArgumentsOnNewLines());
                    });
                });
        }

        private string GetDbContextName()
        {
            if (!this.TryGetTypeName("Infrastructure.Data.DbContext", out var result))
            {
                if (!this.TryGetTypeName("Intent.EntityFrameworkCore.DbContext", out result))
                {
                    var domainDesigner = ExecutionContext.MetadataManager.GetDesigner(ExecutionContext.GetApplicationConfig().Id, Designers.Domain);
                    DomainPackageModel? domainModel = domainDesigner?.GetDomainPackageModels()?.FirstOrDefault();

                    if (domainModel is not null)
                    {
                        throw new ElementException(domainModel.UnderlyingPackage, "Unable to find DB Context template. The 'Intent.AspNetCore.Identity' modules require the 'Intent.EntityFrameworkCore' module to be installed, along with a properly configured Domain package.");
                    }

                    throw new Exception("Unable to find DB Context template. The 'Intent.AspNetCore.Identity' modules require the 'Intent.EntityFrameworkCore' module to be installed, along with a properly configured Domain package.");

                }
            }
            return result;
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest.ToRegister(extensionMethodName: "ConfigureIdentity").HasDependency(this));
            base.BeforeTemplateExecution();
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
    }
}