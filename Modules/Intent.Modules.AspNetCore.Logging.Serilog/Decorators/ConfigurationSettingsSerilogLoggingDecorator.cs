using System;
using System.Collections.Generic;
using System.Text;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.AspNetCore.Logging.Serilog.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ConfigurationSettingsSerilogLoggingDecorator : AppSettingsDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Modules.AspNetCore.Logging.Serilog.ConfigurationSettingsSerilogLoggingDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly AppSettingsTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public ConfigurationSettingsSerilogLoggingDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            _template.ApplyAppSetting("Serilog", new
            {
                MinimumLevel = new { 
                    Default = "Warning",
                    Override = new
                    {
                        Microsoft = "Warning",
                        System = "Warning"
                    }
                }                
            });
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            
        }
    }
}