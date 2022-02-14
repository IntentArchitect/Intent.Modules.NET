using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.Configuration;
using Intent.Templates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.LaunchSettings
{
    public class LaunchSettingsJsonTemplate : IntentFileTemplateBase<object>, ITemplate
    {
        private int _randomPort;
        private int _randomSslPort;
        public const string Identifier = "Intent.VisualStudio.Projects.CoreWeb.LaunchSettings";

        public LaunchSettingsJsonTemplate(IProject project, IApplicationEventDispatcher applicationEventDispatcher)
            : base(Identifier, project, null)
        {
            applicationEventDispatcher.Subscribe(LaunchProfileRegistrationEvent.EventId, Handle);
        }

        public override string GetCorrelationId()
        {
            return $"{Identifier}#{OutputTarget.Id}";
        }

        public IDictionary<string, Profile> Profiles { get; } = new Dictionary<string, Profile>();

        private void Handle(ApplicationEvent @event)
        {
            Profiles.Add(@event.GetValue(LaunchProfileRegistrationEvent.ProfileNameKey), new Profile
            {
                commandName = @event.GetValue(LaunchProfileRegistrationEvent.CommandNameKey),
                launchBrowser = bool.TryParse(@event.GetValue(LaunchProfileRegistrationEvent.LaunchBrowserKey), out var launchBrowser) && launchBrowser,
                launchUrl = @event.TryGetValue(LaunchProfileRegistrationEvent.LaunchUrlKey),
                applicationUrl = @event.TryGetValue(LaunchProfileRegistrationEvent.ApplicationUrl),
                publishAllPorts = bool.TryParse(@event.TryGetValue(LaunchProfileRegistrationEvent.PublishAllPorts), out var publishAllPorts) && publishAllPorts,
                useSSL = bool.TryParse(@event.TryGetValue(LaunchProfileRegistrationEvent.UseSSL), out var useSSL) && useSSL,
            });
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            if (!File.Exists(GetMetadata().GetFilePath()))
            {
                _randomPort = new Random().Next(40000, 65535);
                _randomSslPort = new Random().Next(44300, 44399);
                ExecutionContext.EventDispatcher.Publish(new HostingSettingsCreatedEvent($"http://localhost:{_randomPort}/", _randomPort, _randomSslPort));
            }
            else
            {
                var appSettings = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(GetMetadata().GetFilePath()));
                if (int.TryParse(appSettings["iisSettings"]?["iisExpress"]?["sslPort"]?.ToString(), out _randomSslPort) &&
                    appSettings["iisSettings"]?["iisExpress"]?["applicationUrl"]?.ToString().Split(new[] { ':', '/' }, StringSplitOptions.RemoveEmptyEntries).Any(x => int.TryParse(x, out _randomPort)) == true)
                {
                    ExecutionContext.EventDispatcher.Publish(new HostingSettingsCreatedEvent(
                        applicationUrl: appSettings["iisSettings"]["iisExpress"]["applicationUrl"].ToString(), 
                        port: _randomPort, 
                        sslPort: _randomSslPort));
                }
            }
        }

        public override string TransformText()
        {
            dynamic config;
            if (!File.Exists(GetMetadata().GetFilePath()))
            {
                config = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new LaunchSettingsJson()
                {
                    iisSettings = new IISSettings()
                    {
                        windowsAuthentication = false,
                        anonymousAuthentication = true,
                        iisExpress = new IISExpress()
                        {
                            applicationUrl = $"http://localhost:{_randomPort}/",
                            sslPort = _randomSslPort
                        }
                    },
                    profiles = new Dictionary<string, Profile>()
                    {
                        { "IIS Express", new Profile()
                            {
                                commandName = "IISExpress",
                                launchBrowser = true,
                                environmentVariables = new EnvironmentVariables()
                                {
                                    ASPNETCORE_ENVIRONMENT = "Development"
                                }
                            }
                        },
                        { Project.Name, new Profile()
                            {
                                commandName = "Project",
                                launchBrowser = true,
                                environmentVariables = new EnvironmentVariables()
                                {
                                    ASPNETCORE_ENVIRONMENT = "Development"
                                },
                                applicationUrl = $"http://localhost:{_randomPort}/"
                            }
                        }
                    }
                }, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                }));
            }
            else
            {
                var existingFileContent = File.ReadAllText(GetMetadata().GetFilePath());
                config = JsonConvert.DeserializeObject(existingFileContent, new JsonSerializerSettings());
            }

            if (config.profiles == null)
            {
                config.profiles = JObject.FromObject(new Dictionary<string, string>());
            }

            foreach (var profile in Profiles)
            {
                if (config.profiles[profile.Key] == null)
                {
                    config.profiles[profile.Key] = JObject.FromObject(profile.Value, JsonSerializer.Create(new JsonSerializerSettings()
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore
                    }));
                }
            }

            return JsonConvert.SerializeObject(config, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "launchsettings",
                fileExtension: "json",
                relativeLocation: "Properties"
                );
        }
    }

    public class LaunchSettingsJson
    {
        public IISSettings iisSettings { get; set; }
        public Dictionary<string, Profile> profiles { get; set; }
    }

    public class IISSettings
    {
        public bool windowsAuthentication { get; set; }
        public bool anonymousAuthentication { get; set; }
        public IISExpress iisExpress { get; set; }
    }

    public class IISExpress
    {
        public string applicationUrl { get; set; }
        public int sslPort { get; set; }
    }

    public class Profile
    {
        public string commandName { get; set; }
        public bool launchBrowser { get; set; }
        public EnvironmentVariables environmentVariables { get; set; }
        public string applicationUrl { get; set; }
        public string launchUrl { get; set; }
        public bool publishAllPorts { get; set; }
        public bool useSSL { get; set; }
    }

    public class EnvironmentVariables
    {
        public string ASPNETCORE_ENVIRONMENT { get; set; }
    }
}