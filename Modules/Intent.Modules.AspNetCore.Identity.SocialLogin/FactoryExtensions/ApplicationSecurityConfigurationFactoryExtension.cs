using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Identity.SocialLogin.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.SocialLogin.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ApplicationSecurityConfigurationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Identity.SocialLogin.ApplicationSecurityConfigurationFactoryExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Security.Configuration"));

            if (template is null)
            {
                return;
            }
            
            var enabledSocialProviders = GetEnabledSocialProviders(template);
            if (enabledSocialProviders.Count == 0)
            {
                return;
            }
            
            AddNugetDependencies(template, enabledSocialProviders);
        }

        private void AddNugetDependencies(ICSharpTemplate template, List<SocialProvider> enabledSocialProviders)
        {
            foreach (var socialProvider in enabledSocialProviders)
            {
                switch (socialProvider)
                {
                    case SocialProvider.Google:
                        template.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationGoogle(template.OutputTarget));
                        break;
                    case SocialProvider.Microsoft:
                        template.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationMicrosoft(template.OutputTarget));
                        break;
                    case SocialProvider.Twitter:
                        template.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationTwitter(template.OutputTarget));
                        break;
                    case SocialProvider.Facebook:
                        template.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationFacebook(template.OutputTarget));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

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
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Security.Configuration"));

            if (template is null)
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var priClass = file.Classes.First();
                var securityMethod = priClass.FindMethod("ConfigureApplicationSecurity");
                var authStatement =
                    securityMethod.FindStatement(stmt => stmt.HasMetadata("add-authentication")) as
                        CSharpMethodChainStatement;
                var addJwtBearerInvocationStatement =
                    authStatement?.Statements.SingleOrDefault(x => x.ToString().StartsWith("AddJwtBearer(")) as
                        CSharpInvocationStatement;
                if (addJwtBearerInvocationStatement == null)
                {
                    return;
                }

                var enabledSocialProviders = GetEnabledSocialProviders(template);
                
                if (enabledSocialProviders.Count == 0)
                {
                    return;
                }

                addJwtBearerInvocationStatement.WithoutSemicolon();
                AddSocialLogins(template, authStatement, enabledSocialProviders);
            }, 10);
        }

        private List<SocialProvider> GetEnabledSocialProviders(IIntentTemplate template)
        {
            var socialProviders = new List<SocialProvider>();
            var settings = template.ExecutionContext.Settings.GetSocialLoginProviders();
            if (settings.Google())
            {
                socialProviders.Add(SocialProvider.Google);
            }

            if (settings.Microsoft())
            {
                socialProviders.Add(SocialProvider.Microsoft);
            }

            if (settings.Twitter())
            {
                socialProviders.Add(SocialProvider.Twitter);
            }

            if (settings.Facebook())
            {
                socialProviders.Add(SocialProvider.Facebook);
            }

            return socialProviders.Distinct().ToList();
        }

        private void AddSocialLogins(ICSharpTemplate template, CSharpMethodChainStatement authStatement,
            IReadOnlyList<SocialProvider> socialProviders)
        {
            for (var i = 0; i < socialProviders.Count; i++)
            {
                var socialProvider = socialProviders[i];
                var isFinalItem = i == socialProviders.Count - 1;
                switch (socialProvider)
                {
                    case SocialProvider.Google:
                        AddGoogle(template, authStatement, isFinalItem);
                        break;
                    case SocialProvider.Microsoft:
                        AddMicrosoft(template, authStatement, isFinalItem);
                        break;
                    case SocialProvider.Twitter:
                        AddTwitter(template, authStatement, isFinalItem);
                        break;
                    case SocialProvider.Facebook:
                        AddFacebook(template, authStatement, isFinalItem);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private const string SocialSettingsGroup = "SocialLogin";
        private const string GoogleSettingsGroup = "Google";
        private const string MicrosoftSettingsGroup = "Microsoft";
        private const string FacebookSettingsGroup = "Facebook";
        private const string TwitterSettingsGroup = "Twitter";

        private static void AddGoogle(ICSharpTemplate template, CSharpMethodChainStatement authStatement,
            bool isFinalItem)
        {
            (template as IntentTemplateBase).ApplyAppSetting("SocialLogin:Google", new
            {
                ClientId = string.Empty,
                ClientSecret = string.Empty,
            });

            var invocationStatement = new CSharpInvocationStatement("AddGoogle")
                .AddArgument(new CSharpLambdaBlock("options")
                    .AddStatement(
                        @"options.ClientId = configuration.GetSection(""SocialLogin:Google:ClientId"").Get<string>();")
                    .AddStatement(
                        @"options.ClientSecret = configuration.GetSection(""SocialLogin:Google:ClientSecret"").Get<string>();")
                );

            if (!isFinalItem)
            {
                invocationStatement.WithoutSemicolon();
            }

            authStatement.AddChainStatement(invocationStatement);
        }

        private static void AddMicrosoft(ICSharpTemplate template, CSharpMethodChainStatement authStatement,
            bool isFinalItem)
        {
            (template as IntentTemplateBase).ApplyAppSetting("SocialLogin:Microsoft", new
            {
                ClientId = string.Empty,
                ClientSecret = string.Empty,
            });

            var invocationStatement = new CSharpInvocationStatement("AddMicrosoftAccount")
                .AddArgument(new CSharpLambdaBlock("options")
                    .AddStatement(
                        @"options.ClientId = configuration.GetSection(""SocialLogin:Microsoft:ClientId"").Get<string>();")
                    .AddStatement(
                        @"options.ClientSecret = configuration.GetSection(""SocialLogin:Microsoft:ClientSecret"").Get<string>();")
                );

            if (!isFinalItem)
            {
                invocationStatement.WithoutSemicolon();
            }

            authStatement.AddChainStatement(invocationStatement);
        }

        private static void AddTwitter(ICSharpTemplate template, CSharpMethodChainStatement authStatement,
            bool isFinalItem)
        {
            (template as IntentTemplateBase).ApplyAppSetting("SocialLogin:Twitter", new
            {
                ConsumerKey = string.Empty,
                ConsumerSecret = string.Empty,
            });

            var invocationStatement = new CSharpInvocationStatement("AddTwitter")
                .AddArgument(new CSharpLambdaBlock("options")
                    .AddStatement(
                        @"options.ConsumerKey = configuration.GetSection(""SocialLogin:Twitter:ConsumerKey"").Get<string>();")
                    .AddStatement(
                        @"options.ConsumerSecret = configuration.GetSection(""SocialLogin:Twitter:ConsumerSecret"").Get<string>();")
                );

            if (!isFinalItem)
            {
                invocationStatement.WithoutSemicolon();
            }

            authStatement.AddChainStatement(invocationStatement);
        }

        private static void AddFacebook(ICSharpTemplate template, CSharpMethodChainStatement authStatement,
            bool isFinalItem)
        {
            (template as IntentTemplateBase).ApplyAppSetting("SocialLogin:Facebook", new
            {
                AppId = string.Empty,
                AppSecret = string.Empty,
            });

            var invocationStatement = new CSharpInvocationStatement("AddFacebook")
                .AddArgument(new CSharpLambdaBlock("options")
                    .AddStatement(
                        @"options.AppId = configuration.GetSection(""SocialLogin:Facebook:AppId"").Get<string>();")
                    .AddStatement(
                        @"options.AppSecret = configuration.GetSection(""SocialLogin:Facebook:AppSecret"").Get<string>();")
                );

            if (!isFinalItem)
            {
                invocationStatement.WithoutSemicolon();
            }

            authStatement.AddChainStatement(invocationStatement);
        }

        private enum SocialProvider
        {
            Google,
            Microsoft,
            Twitter,
            Facebook,
        }
    }
}