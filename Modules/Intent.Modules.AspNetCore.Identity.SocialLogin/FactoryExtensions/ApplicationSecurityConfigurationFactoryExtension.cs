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
        private const string SocialSettingsGroupName = "SocialLogin";
        
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

        private void AddNugetDependencies(ICSharpTemplate template, ISet<SocialProvider> enabledSocialProviders)
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

                (template as IntentTemplateBase).ApplyAppSetting(SocialSettingsGroupName, new SocialSettings
                {
                    AllowSelfRegistration = false,
                    WhitelistedDomains = Array.Empty<string>(),
                });

                addJwtBearerInvocationStatement.WithoutSemicolon();
                AddSocialLogins(template, authStatement, enabledSocialProviders);
                authStatement.AddChainStatement(new CSharpInvocationStatement("AddIdentityCookies"));
            }, 10);
        }

        private ISet<SocialProvider> GetEnabledSocialProviders(IIntentTemplate template)
        {
            var socialProviders = new HashSet<SocialProvider>();
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

            return socialProviders;
        }

        private void AddSocialLogins(ICSharpTemplate template, CSharpMethodChainStatement authStatement,
            ISet<SocialProvider> socialProviders)
        {
            foreach (var socialProvider in socialProviders)
            {
                switch (socialProvider)
                {
                    case SocialProvider.Google:
                        AddGoogle(template, authStatement);
                        break;
                    case SocialProvider.Microsoft:
                        AddMicrosoft(template, authStatement);
                        break;
                    case SocialProvider.Twitter:
                        AddTwitter(template, authStatement);
                        break;
                    case SocialProvider.Facebook:
                        AddFacebook(template, authStatement);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void AddGoogle(ICSharpTemplate template, CSharpMethodChainStatement authStatement)
        {
            const string settingGroupName = SocialSettingsGroupName + ":Google";

            (template as IntentTemplateBase).ApplyAppSetting(settingGroupName, new SocialSettings.Google
            {
                ClientId = string.Empty,
                ClientSecret = string.Empty,
            });

            var invocationStatement = new CSharpInvocationStatement("AddGoogle")
                .AddArgument(new CSharpLambdaBlock("options")
                    .AddStatement(
                        GetStringOptionStatement(nameof(SocialSettings.Google.ClientId), settingGroupName))
                    .AddStatement(
                        GetStringOptionStatement(nameof(SocialSettings.Google.ClientSecret), settingGroupName))
                    .AddStatement("options.SignInScheme = IdentityConstants.ExternalScheme;")
                ).WithoutSemicolon();;

            authStatement.AddChainStatement(invocationStatement);
        }

        private static void AddMicrosoft(ICSharpTemplate template, CSharpMethodChainStatement authStatement)
        {
            const string settingGroupName = SocialSettingsGroupName + ":Microsoft";

            (template as IntentTemplateBase).ApplyAppSetting(settingGroupName, new SocialSettings.Microsoft
            {
                ClientId = string.Empty,
                ClientSecret = string.Empty,
            });

            var invocationStatement = new CSharpInvocationStatement("AddMicrosoftAccount")
                .AddArgument(new CSharpLambdaBlock("options")
                    .AddStatement(
                        GetStringOptionStatement(nameof(SocialSettings.Microsoft.ClientId), settingGroupName))
                    .AddStatement(
                        GetStringOptionStatement(nameof(SocialSettings.Microsoft.ClientSecret), settingGroupName))
                    .AddStatement("options.SignInScheme = IdentityConstants.ExternalScheme;")
                ).WithoutSemicolon();

            authStatement.AddChainStatement(invocationStatement);
        }

        private static void AddTwitter(ICSharpTemplate template, CSharpMethodChainStatement authStatement)
        {
            const string settingGroupName = SocialSettingsGroupName + ":Twitter";

            (template as IntentTemplateBase).ApplyAppSetting(settingGroupName, new SocialSettings.Twitter
            {
                ConsumerKey = string.Empty,
                ConsumerSecret = string.Empty,
            });

            var invocationStatement = new CSharpInvocationStatement("AddTwitter")
                .AddArgument(new CSharpLambdaBlock("options")
                    .AddStatement(
                        GetStringOptionStatement(nameof(SocialSettings.Twitter.ConsumerKey), settingGroupName))
                    .AddStatement(
                        GetStringOptionStatement(nameof(SocialSettings.Twitter.ConsumerSecret), settingGroupName))
                    .AddStatement("options.SignInScheme = IdentityConstants.ExternalScheme;")
                ).WithoutSemicolon();

            authStatement.AddChainStatement(invocationStatement);
        }

        private static void AddFacebook(ICSharpTemplate template, CSharpMethodChainStatement authStatement)
        {
            const string settingGroupName = SocialSettingsGroupName + ":Facebook";

            (template as IntentTemplateBase).ApplyAppSetting(settingGroupName, new SocialSettings.Facebook
            {
                AppId = string.Empty,
                AppSecret = string.Empty,
            });

            var invocationStatement = new CSharpInvocationStatement("AddFacebook")
                .AddArgument(new CSharpLambdaBlock("options")
                    .AddStatement(
                        GetStringOptionStatement(nameof(SocialSettings.Facebook.AppId), settingGroupName))
                    .AddStatement(
                        GetStringOptionStatement(nameof(SocialSettings.Facebook.AppSecret), settingGroupName))
                    .AddStatement("options.SignInScheme = IdentityConstants.ExternalScheme;")
                ).WithoutSemicolon();

            authStatement.AddChainStatement(invocationStatement);
        }

        private static string GetStringOptionStatement(string optionName, string groupName)
        {
            return
                $@"options.{optionName} = configuration.GetSection(""{groupName}:{optionName}"").Get<string>();";
        }

        private class SocialSettings
        {
            public bool AllowSelfRegistration { get; set; }
            public string[] WhitelistedDomains { get; set; }

            internal class Google
            {
                public string ClientId { get; set; } = string.Empty;
                public string ClientSecret { get; set; } = string.Empty;
            }

            internal class Microsoft
            {
                public string ClientId { get; set; } = string.Empty;
                public string ClientSecret { get; set; } = string.Empty;
            }

            internal class Twitter
            {
                public string ConsumerKey { get; set; } = string.Empty;
                public string ConsumerSecret { get; set; } = string.Empty;
            }

            internal class Facebook
            {
                public string AppId { get; set; } = string.Empty;
                public string AppSecret { get; set; } = string.Empty;
            }
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