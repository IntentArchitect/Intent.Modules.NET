using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
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
                        method.AddStatement(new CSharpMethodChainStatement($"services.AddIdentityWithoutCookieAuth<{this.GetIdentityUserClass()}, {this.GetIdentityRoleClass()}>()")
                            .AddChainStatement($@"AddEntityFrameworkStores<{this.GetTypeName("Infrastructure.Data.DbContext")}>()")
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