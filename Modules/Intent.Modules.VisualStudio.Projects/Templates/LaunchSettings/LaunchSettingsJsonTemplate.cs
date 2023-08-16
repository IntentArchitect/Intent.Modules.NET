using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.Configuration;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intent.Modules.VisualStudio.Projects.Templates.LaunchSettings
{
    public class LaunchSettingsJsonTemplate : IntentFileTemplateBase<object>, ITemplate
    {
        private int _randomPort;
        private int _randomSslPort;
        private string _defaultLaunchUrlPath = string.Empty;

        private readonly ICollection<EnvironmentVariableRegistrationRequest> _environmentVariables = new List<EnvironmentVariableRegistrationRequest>();

        public const string Identifier = "Intent.VisualStudio.Projects.CoreWeb.LaunchSettings";

        public LaunchSettingsJsonTemplate(IOutputTarget outputTarget, IApplicationEventDispatcher applicationEventDispatcher)
            : base(Identifier, outputTarget, null)
        {
            ExecutionContext.EventDispatcher.Subscribe<EnvironmentVariableRegistrationRequest>(HandleEnvironmentVariable);
            ExecutionContext.EventDispatcher.Subscribe<LaunchProfileRegistrationRequest>(HandleLaunchProfileRegistration);
#pragma warning disable CS0618 // Type or member is obsolete
            applicationEventDispatcher.Subscribe(LaunchProfileRegistrationEvent.EventId, Handle);
#pragma warning restore CS0618 // Type or member is obsolete
            ExecutionContext.EventDispatcher.Subscribe<DefaultLaunchUrlPathRequest>(HandleDefaultLaunchUrlRequest);
        }

        public override string GetCorrelationId()
        {
            return $"{Identifier}#{OutputTarget.Id}";
        }

        public IDictionary<string, Profile> Profiles { get; } = new Dictionary<string, Profile>();

        /// <summary>
        /// Will be retired in favour of <see cref="HandleLaunchProfileRegistration"/>.
        /// </summary>
        [FixFor_Version4(WillBeRemovedIn.Version4)]
        [Obsolete($"As per {nameof(LaunchProfileRegistrationEvent)}")]
        private void Handle(ApplicationEvent @event)
        {
            // TODO: Align this file with the schema: http://json.schemastore.org/launchsettings.json
            Profiles.Add(@event.GetValue(LaunchProfileRegistrationEvent.ProfileNameKey), new Profile
            {
                commandName = @event.GetValue(LaunchProfileRegistrationEvent.CommandNameKey),
                launchBrowser = bool.TryParse(@event.GetValue(LaunchProfileRegistrationEvent.LaunchBrowserKey),
                    out var launchBrowser) && launchBrowser,
                launchUrl = @event.TryGetValue(LaunchProfileRegistrationEvent.LaunchUrlKey),
                applicationUrl = @event.TryGetValue(LaunchProfileRegistrationEvent.ApplicationUrl),
                publishAllPorts = bool.TryParse(@event.TryGetValue(LaunchProfileRegistrationEvent.PublishAllPorts),
                    out var publishAllPorts) && publishAllPorts,
                useSSL = bool.TryParse(@event.TryGetValue(LaunchProfileRegistrationEvent.UseSSL), out var useSsl) &&
                         useSsl,
            });
        }

        private void HandleLaunchProfileRegistration(LaunchProfileRegistrationRequest request)
        {
            if (!request.IsApplicableTo(this))
            {
                return;
            }

            Profiles.Add(request.Name, new Profile
            {
                commandName = request.CommandName,
                launchBrowser = request.LaunchBrowser,
                applicationUrl = request.ApplicationUrl,
                launchUrl = request.LaunchUrl,
                publishAllPorts = request.PublishAllPorts,
                //Schema for json says default value is true
                useSSL = request.UseSsl ? null : false,
            });

            if (request.EnvironmentVariables?.Count > 0)
            {
                var targetProfiles = new[] { request.Name };

                foreach (var (key, value) in request.EnvironmentVariables)
                {
                    _environmentVariables.Add(new EnvironmentVariableRegistrationRequest(key, value, targetProfiles));
                }
            }
        }

        private void HandleEnvironmentVariable(EnvironmentVariableRegistrationRequest request)
        {
            if (!request.IsApplicableTo(this))
            {
                return;
            }

            _environmentVariables.Add(request);
        }

        private void HandleDefaultLaunchUrlRequest(DefaultLaunchUrlPathRequest request)
        {
            if (!request.IsApplicableTo(this))
            {
                return;
            }

            if (!string.IsNullOrEmpty(_defaultLaunchUrlPath))
            {
                throw new InvalidOperationException("Default Launch Url Path was already set and is being set again.");
            }

            if (string.IsNullOrEmpty(request.UrlPath))
            {
                throw new ArgumentNullException(nameof(request.UrlPath));
            }

            _defaultLaunchUrlPath = request.UrlPath;
            if (_defaultLaunchUrlPath.StartsWith("/"))
            {
                _defaultLaunchUrlPath = _defaultLaunchUrlPath.Remove(0, 1);
            }
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            if (!TryGetExistingFileContent(out var content))
            {
                _randomPort = new Random().Next(56600, 65535);
                _randomSslPort = new Random().Next(44300, 44399);
                ExecutionContext.EventDispatcher.Publish(new HostingSettingsCreatedEvent($"http://localhost:{_randomPort}/", _randomPort, _randomSslPort));
                return;
            }

            var appSettings = JsonConvert.DeserializeObject<JObject>(content);
            if (int.TryParse(appSettings["iisSettings"]?["iisExpress"]?["sslPort"]?.ToString(), out _randomSslPort) &&
                appSettings["iisSettings"]?["iisExpress"]?["applicationUrl"]?.ToString()
                    .Split(new[] { ':', '/' }, StringSplitOptions.RemoveEmptyEntries)
                    .Any(x => int.TryParse(x, out _randomPort)) == true)
            {
                ExecutionContext.EventDispatcher.Publish(new HostingSettingsCreatedEvent(
                    applicationUrl: appSettings["iisSettings"]["iisExpress"]["applicationUrl"].ToString(),
                    port: _randomPort,
                    sslPort: _randomSslPort));
            }
        }

        public override string TransformText()
        {
            if (!TryGetExistingFileContent(out var content))
            {
                content = JsonConvert.SerializeObject(new LaunchSettingsJson
                {
                    iisSettings = new IISSettings
                    {
                        windowsAuthentication = false,
                        anonymousAuthentication = true,
                        iisExpress = new IISExpress
                        {
                            applicationUrl = $"http://localhost:{_randomPort}/",
                            sslPort = _randomSslPort
                        }
                    },
                    profiles = new Dictionary<string, Profile>
                    {
                        {
                            Project.Name, new Profile
                            {
                                commandName = "Project",
                                launchBrowser = true,
                                applicationUrl = $"https://localhost:{_randomSslPort}/",
                                launchUrl = _defaultLaunchUrlPath == null
                                    ? null
                                    : $"https://localhost:{_randomSslPort}/{_defaultLaunchUrlPath}" 
                            }
                        },
                        {
                            "IIS Express", new Profile
                            {
                                commandName = "IISExpress",
                                launchBrowser = true,
                                launchUrl = _defaultLaunchUrlPath == null
                                    ? null
                                    : $"https://localhost:{_randomSslPort}/{_defaultLaunchUrlPath}"
                            }
                        }
                    }
                }, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
            }

            dynamic config = JsonConvert.DeserializeObject(content, new JsonSerializerSettings())!;

            if (config.profiles == null)
            {
                config.profiles = JObject.FromObject(new Dictionary<string, string>());
            }

            foreach (var profile in Profiles)
            {
                if (config.profiles[profile.Key] == null)
                {
                    config.profiles[profile.Key] = JObject.FromObject(profile.Value, JsonSerializer.Create(
                        new JsonSerializerSettings
                        {
                            Formatting = Formatting.Indented,
                            NullValueHandling = NullValueHandling.Ignore
                        }));
                }
            }

            // In case has not been set through an event, sets this be default.
            _environmentVariables.Add(new EnvironmentVariableRegistrationRequest("ASPNETCORE_ENVIRONMENT",
                "Development", new[] { "IIS Express", Project.Name }));

            foreach (var environmentVariable in _environmentVariables)
            {
                foreach (var profile in ((IEnumerable<dynamic>)config.profiles)
                         .Where(x => environmentVariable.TargetProfiles == null ||
                                     environmentVariable.TargetProfiles.Any(p => p == ((JProperty)x).Name))
                         .Select(x => x.Value))
                {
                    profile.environmentVariables ??= JObject.FromObject(new EnvironmentVariables());
                    profile.environmentVariables[environmentVariable.Key] ??= environmentVariable.Value;
                }
            }

            return JsonConvert.SerializeObject(config, new JsonSerializerSettings
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

        // ReSharper disable InconsistentNaming
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
        public object environmentVariables { get; set; }
        public string applicationUrl { get; set; }
        public string launchUrl { get; set; }
        public bool publishAllPorts { get; set; }
        public bool? useSSL { get; set; }
    }
    // ReSharper restore InconsistentNaming

    public class EnvironmentVariables
    {
    }
}