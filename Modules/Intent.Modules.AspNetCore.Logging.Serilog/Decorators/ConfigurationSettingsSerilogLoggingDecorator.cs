using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Logging.Serilog.Settings;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;
using Newtonsoft.Json.Linq;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Logging.Serilog.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ConfigurationSettingsSerilogLoggingDecorator : AppSettingsDecorator
    {
        [IntentManaged(Mode.Fully)] public const string DecoratorId = "Intent.Modules.AspNetCore.Logging.Serilog.ConfigurationSettingsSerilogLoggingDecorator";

        [IntentManaged(Mode.Fully)] private readonly AppSettingsTemplate _template;
        [IntentManaged(Mode.Fully)] private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public ConfigurationSettingsSerilogLoggingDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            var serilog = appSettings.GetProperty<JObject>("Serilog");
            serilog = CreateDefaultSerilogSettingsObj(serilog);

            PopulateUsings(serilog);
            PopulateEnriches(serilog);
            PopulateWriteToSinks(serilog);

            appSettings.SetProperty("Serilog", serilog);
        }

        private static JObject CreateDefaultSerilogSettingsObj(JObject serilog)
        {
            serilog = serilog ?? new JObject()
            {
                {
                    "MinimumLevel", new JObject
                    {
                        { "Default", "Information" },
                        {
                            "Override", new JObject()
                            {
                                { "Microsoft", "Warning" },
                                { "System", "Warning" }
                            }
                        }
                    }
                }
            };
            return serilog;
        }

        private void PopulateWriteToSinks(JObject serilog)
        {
            JArray writeTo;
            if (serilog.ContainsKey("WriteTo"))
            {
                writeTo = (JArray)serilog.GetValue("WriteTo")!;
            }
            else
            {
                writeTo = new JArray();
                serilog.TryAdd("WriteTo", writeTo);
            }

            foreach (var serilogSink in _application.Settings.GetSerilogSettings().Sinks())
            {
                if (writeTo.Cast<JObject>().Any(sink => string.Equals(sink.GetValue("Name")?.ToString(), serilogSink.Value, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                writeTo.Add(serilogSink.AsEnum() switch
                {
                    SerilogSettings.SinksOptionsEnum.Console =>
                        new JObject
                        {
                            { "Name", "Console" },
                            {
                                "Args", new JObject
                                {
                                    { "outputTemplate", "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] {Category:0} - {Message}{NewLine:1}" },
                                    { "restrictedToMinimumLevel", "Information" }
                                }
                            }
                        },
                    SerilogSettings.SinksOptionsEnum.File =>
                        new JObject
                        {
                            { "Name", "File" },
                            {
                                "Args", new JObject
                                {
                                    { "path", @"Logs\.log" },
                                    { "rollingInterval", "Day" },
                                    { "restrictedToMinimumLevel", "Information" }
                                }
                            }
                        },
                    SerilogSettings.SinksOptionsEnum.Graylog =>
                        new JObject
                        {
                            { "Name", "Graylog" },
                            {
                                "Args", new JObject
                                {
                                    { "hostnameOrAddress", "localhost" },
                                    { "port", "12201" },
                                    { "transportType", "Udp" }
                                }
                            }
                        },
                    _ => new JObject()
                });
            }
        }

        private void PopulateUsings(JObject serilog)
        {
            JArray usingArr;
            if (serilog.ContainsKey("Using"))
            {
                usingArr = (JArray)serilog.GetValue("Using")!;
            }
            else
            {
                usingArr = new JArray();
                serilog.AddFirst(new JProperty("Using", usingArr));
            }

            var existing = new HashSet<string>(usingArr.Cast<JValue>().Select(s => s.Value?.ToString()));
            foreach (var serilogSink in _application.Settings.GetSerilogSettings().Sinks())
            {
                if (existing.Contains(SerilogOptionToType(serilogSink.AsEnum())))
                {
                    continue;
                }

                usingArr.Add(SerilogOptionToType(serilogSink.AsEnum()));
            }

            return;

            static string SerilogOptionToType(SerilogSettings.SinksOptionsEnum option)
            {
                return option switch
                {
                    SerilogSettings.SinksOptionsEnum.Console => "Serilog.Sinks.Console",
                    SerilogSettings.SinksOptionsEnum.File => "Serilog.Sinks.File",
                    SerilogSettings.SinksOptionsEnum.Graylog => "Serilog.Sinks.Graylog",
                    _ => null
                };
            }
        }

        private void PopulateEnriches(JObject serilog)
        {
            JArray enrichArr;
            if (serilog.ContainsKey("Enrich"))
            {
                enrichArr = (JArray)serilog.GetValue("Enrich")!;
            }
            else
            {
                enrichArr = new JArray();
                serilog.AddFirst(new JProperty("Enrich", enrichArr));
            }

            var expectedEnriches = new List<string> { "FromLogContext" };
            if (_application.Settings.GetSerilogSettings().Sinks().Any(x => x.IsGraylog()))
            {
                expectedEnriches.Add("WithSpan");
            }

            var existing = new HashSet<string>(enrichArr.Cast<JValue>().Select(s => s.Value?.ToString()));
            foreach (var enrich in expectedEnriches)
            {
                if (existing.Contains(enrich))
                {
                    continue;
                }

                enrichArr.Add(enrich);
            }
        }
    }
}