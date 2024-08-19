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
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            if (!_template.OutputTarget.HasRole("Distribution"))
            {
                return;
            }

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


        private class WriteToStratergy
        {
            private JToken _token;
            private JArray _array;
            private JObject _object;
            private Func<HashSet<string>> _getSinkNames;
            private Action<IEnumerable<string>> _removeSinksByName;
            private Action<JObject> _addSink;

            public WriteToStratergy(JToken token)
            {
                _token = token;
                if (token is JArray jarray)
                {
                    _array = jarray;
                    _getSinkNames = ArrayGetSinkNames;
                    _addSink = ArrayAddSink;
                    _removeSinksByName = ArrayRemoveSinksByName;
                }
                else if (token is JObject jobject)
                {
                    _object = jobject;
                    _getSinkNames = ObjectGetSinkNames;
                    _addSink = ObjectAddSink;
                    _removeSinksByName = ObjectRemoveSinksByName;
                }
            }

            public JToken Token => _token;

            private HashSet<string> ArrayGetSinkNames()
            {
                return _array.Cast<JObject>().Select(x => x.GetValue("Name")?.Value<string>()).ToHashSet();
            }
            private HashSet<string> ObjectGetSinkNames()
            {
                return _object.Properties().Select(p => ((JObject)p.Value).GetValue("Name")?.Value<string>()).ToHashSet();
            }

            private void ArrayRemoveSinksByName(IEnumerable<string> sinksToRemove)
            {
                foreach (var sink in sinksToRemove.ToArray())
                {
                    var sinkToRemove = _array.FirstOrDefault(x => x["Name"]?.ToString() == sink);
                    if (sinkToRemove is not null)
                    {
                        _array.Remove(sinkToRemove);
                    }
                }
            }

            private void ObjectRemoveSinksByName(IEnumerable<string> sinksToRemove)
            {
                var toRemoveName = sinksToRemove.ToArray();
                var toRemove = new List<string>();
                foreach (var p in _object.Properties())
                {
                    JObject value = p.Value as JObject;
                    if (toRemoveName.Contains(value["Name"]?.ToString()))
                    {
                        toRemove.Add(p.Name);
                    }
                }
                foreach (var propertyName in toRemove)
                {
                    _object.Remove(propertyName);
                }
            }

            private void ArrayAddSink(JObject sink)
            {
                _array.Add(sink);
            }

            private void ObjectAddSink(JObject sink)
            {
                _object.Add(_object.Properties().Count().ToString(), sink);
            }


            public HashSet<string> GetSinkNames() => _getSinkNames();

            public void RemoveSinksByName(IEnumerable<string> names) => _removeSinksByName(names);

            public void AddSink(JObject sink) => _addSink(sink);
        }

        private void PopulateWriteToSinks(JObject serilog)
        {
            var writeTo = new WriteToStratergy(serilog.TryGetValue("WriteTo", out var sinkEntry) ? sinkEntry : new JArray());

            serilog.TryAdd("WriteTo", writeTo.Token);

            var currentSinks = writeTo.GetSinkNames();
            var selectedSinks = _application.Settings.GetSerilogSettings().Sinks().Select(sink => SerilogOptionToSectionName(sink.AsEnum())).ToHashSet();
            var managedSinks = Enum.GetValues<SerilogSettings.SinksOptionsEnum>().Select(SerilogOptionToSectionName).ToHashSet();

            writeTo.RemoveSinksByName((currentSinks.Except(selectedSinks)).Intersect(managedSinks));

            // Add new sinks
            foreach (var serilogSink in _application.Settings.GetSerilogSettings().Sinks())
            {
                var sinkName = SerilogOptionToSectionName(serilogSink.AsEnum());
                if (currentSinks.Contains(sinkName))
                {
                    continue;
                }

                var sinkToAdd = new JObject { ["Name"] = sinkName };
                var args = new JObject();
                switch (serilogSink.AsEnum())
                {
                    case SerilogSettings.SinksOptionsEnum.Console:
                        args["outputTemplate"] = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] {Category:0} - {Message}{NewLine:1}{Exception}";
                        args["restrictedToMinimumLevel"] = "Information";
                        break;
                    case SerilogSettings.SinksOptionsEnum.File:
                        args["path"] = @"Logs\.log";
                        args["rollingInterval"] = "Day";
                        args["restrictedToMinimumLevel"] = "Information";
                        break;
                    case SerilogSettings.SinksOptionsEnum.Graylog:
                        args["hostnameOrAddress"] = "localhost";
                        args["port"] = "12201";
                        args["transportType"] = "Udp";
                        break;
                    case SerilogSettings.SinksOptionsEnum.ApplicationInsights:
                        if (_template.Model.RuntimeEnvironment is not null && _template.Model.RuntimeEnvironment.Name != "Development")
                        {
                            args["connectionString"] = "[your connection string here]";
                        }
                        else
                        {
                            args["connectionString"] = "";
                        }
                        args["telemetryConverter"] = "Serilog.Sinks.ApplicationInsights.TelemetryConverters.TraceTelemetryConverter, Serilog.Sinks.ApplicationInsights";
                        break;
                    default:
                        continue; // If the sink is not recognized, skip adding it
                }

                sinkToAdd["Args"] = args;
                writeTo.AddSink(sinkToAdd);
            }

            return;

            static string SerilogOptionToSectionName(SerilogSettings.SinksOptionsEnum option)
            {
                return option switch
                {
                    SerilogSettings.SinksOptionsEnum.Console => "Console",
                    SerilogSettings.SinksOptionsEnum.File => "File",
                    SerilogSettings.SinksOptionsEnum.Graylog => "Graylog",
                    SerilogSettings.SinksOptionsEnum.ApplicationInsights => "ApplicationInsights",
                    _ => null
                };
            }
        }

        private void PopulateUsings(JObject serilog)
        {
            var usingArr = serilog.TryGetValue("Using", out var usingSink) ? (JArray)usingSink! : new JArray();
            if (!serilog.ContainsKey("Using"))
            {
                serilog.AddFirst(new JProperty("Using", usingArr));
            }

            // Add "Using" only if it doesn't exist
            var existingUsings = new HashSet<string>(usingArr.Select(u => u.ToString()));
            var validUsings = _application.Settings.GetSerilogSettings().Sinks()
                .Select(sink => SerilogOptionToType(sink.AsEnum()))
                .ToHashSet();
            var managedSinks = Enum.GetValues<SerilogSettings.SinksOptionsEnum>().Select(SerilogOptionToType).ToHashSet();

            // Remove invalid usings
            foreach (var usingToRemove in existingUsings.Except(validUsings).ToList())
            {
                var itemToRemove = usingArr.FirstOrDefault(u => u.ToString() == usingToRemove && managedSinks.Contains(u.ToString()));
                if (itemToRemove != null)
                {
                    usingArr.Remove(itemToRemove);
                }
            }

            // Add new usings
            foreach (var serilogSink in _application.Settings.GetSerilogSettings().Sinks())
            {
                var sinkType = SerilogOptionToType(serilogSink.AsEnum());
                if (!existingUsings.Contains(sinkType))
                {
                    usingArr.Add(sinkType);
                }
            }

            return;

            static string SerilogOptionToType(SerilogSettings.SinksOptionsEnum option)
            {
                return option switch
                {
                    SerilogSettings.SinksOptionsEnum.Console => "Serilog.Sinks.Console",
                    SerilogSettings.SinksOptionsEnum.File => "Serilog.Sinks.File",
                    SerilogSettings.SinksOptionsEnum.Graylog => "Serilog.Sinks.Graylog",
                    SerilogSettings.SinksOptionsEnum.ApplicationInsights => "Serilog.Sinks.ApplicationInsights",
                    _ => null // Handle default case gracefully
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
                serilog.Add(new JProperty("Enrich", enrichArr));
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