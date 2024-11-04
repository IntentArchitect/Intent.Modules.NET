using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.Configuration;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.LaunchSettings
{
    public class LaunchSettingsJsonTemplate : IntentFileTemplateBase<object>, ITemplate
    {
        private int _randomPort;
        private int _randomSslPort;
        private string _defaultLaunchUrlPath = string.Empty;
        private bool _launchProfileHttpPortRequired;
        private CSharpProjectNETModel _model;

        private readonly List<EnvironmentVariableRegistrationRequest> _environmentVariables = new();
        private bool _noDefaultLaunchSettingsFile;

        public const string Identifier = "Intent.VisualStudio.Projects.CoreWeb.LaunchSettings";

        public LaunchSettingsJsonTemplate(IOutputTarget outputTarget, IApplicationEventDispatcher applicationEventDispatcher)
            : base(Identifier, outputTarget, null)
        {
            _model = ExecutionContext.MetadataManager.VisualStudio(ExecutionContext.GetApplicationConfig().Id).GetCSharpProjectNETModels().First(m => m.Id == OutputTarget.Id);

            ExecutionContext.EventDispatcher.Subscribe<EnvironmentVariableRegistrationRequest>(HandleEnvironmentVariable);
            ExecutionContext.EventDispatcher.Subscribe<LaunchProfileRegistrationRequest>(HandleLaunchProfileRegistration);
#pragma warning disable CS0618 // Type or member is obsolete
            ExecutionContext.EventDispatcher.Subscribe(LaunchProfileRegistrationEvent.EventId, Handle);
#pragma warning restore CS0618 // Type or member is obsolete
            ExecutionContext.EventDispatcher.Subscribe<DefaultLaunchUrlPathRequest>(HandleDefaultLaunchUrlRequest);
            ExecutionContext.EventDispatcher.Subscribe(LaunchProfileHttpPortRequired.EventId, _ => _launchProfileHttpPortRequired = true);
            ExecutionContext.EventDispatcher.Subscribe<AddProjectPropertyEvent>(HandleNoDefaultLaunchSettingsFile);
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
            Profiles.Add(@event.GetValue(LaunchProfileRegistrationEvent.ProfileNameKey), new Profile
            {
                CommandName = Enum.Parse<CommandName>(@event.GetValue(LaunchProfileRegistrationEvent.CommandNameKey), true),
                LaunchBrowser = bool.TryParse(@event.GetValue(LaunchProfileRegistrationEvent.LaunchBrowserKey),
                    out var launchBrowser) && launchBrowser,
                LaunchUrl = @event.TryGetValue(LaunchProfileRegistrationEvent.LaunchUrlKey),
                ApplicationUrl = @event.TryGetValue(LaunchProfileRegistrationEvent.ApplicationUrl),
                PublishAllPorts = bool.TryParse(@event.TryGetValue(LaunchProfileRegistrationEvent.PublishAllPorts),
                    out var publishAllPorts) && publishAllPorts ? null : true,
                UseSsl = bool.TryParse(@event.TryGetValue(LaunchProfileRegistrationEvent.UseSSL), out var useSsl) && useSsl ? null : false,
            });
        }

        private void HandleNoDefaultLaunchSettingsFile(AddProjectPropertyEvent @event)
        {
            if (!OutputTarget.GetProject().Equals(@event.Target))
            {
                return;
            }

            if (@event.PropertyName != "NoDefaultLaunchSettingsFile")
            {
                return;
            }

            _noDefaultLaunchSettingsFile = @event.PropertyValue == "true";
        }
        
        public override bool CanRunTemplate()
        {
            return !_noDefaultLaunchSettingsFile;
        }

        private void HandleLaunchProfileRegistration(LaunchProfileRegistrationRequest request)
        {
            if (!request.IsApplicableTo(this))
            {
                return;
            }

            Profiles.Add(request.Name, new Profile
            {
                CommandName = Enum.Parse<CommandName>(request.CommandName, true),
                LaunchBrowser = request.LaunchBrowser,
                ApplicationUrl = request.ApplicationUrl,
                LaunchUrl = request.LaunchUrl,
                PublishAllPorts = request.PublishAllPorts ? null : false, // default value is true
                UseSsl = request.UseSsl ? null : false, // default value is true
                InspectUri = request.InspectUri,
                DotnetRunMessages = request.DotnetRunMessages,
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

            var iisExpress = LaunchSettings.FromJson(content).IisSettings?.IisExpress;
            if (iisExpress?.ApplicationUrl == null ||
                !iisExpress.SslPort.HasValue)
            {
                return;
            }

            _randomPort = iisExpress.ApplicationUrl.Port;
            _randomSslPort = iisExpress.SslPort.Value;

            ExecutionContext.EventDispatcher.Publish(new HostingSettingsCreatedEvent(
                applicationUrl: iisExpress.ApplicationUrl.ToString(),
                port: _randomPort,
                sslPort: _randomSslPort));
        }

        public override string TransformText()
        {
            var launchSettings = TryGetExistingFileContent(out var content)
                ? LaunchSettings.FromJson(content)
                : GetDefaultLaunchSettings();

            foreach (var (key, profile) in Profiles)
            {
                launchSettings.Profiles ??= new Dictionary<string, Profile>();
                launchSettings.Profiles.TryAdd(key, profile);
            }

            if (_launchProfileHttpPortRequired)
            {
                foreach (var profile in launchSettings.Profiles.Values)
                {
                    var split = profile.ApplicationUrl?.Split(';');
                    if (split?.Length != 1 ||
                        !split[0].StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    profile.ApplicationUrl = string.Join(";", split.Append($"http://localhost:{_randomPort}"));
                }
            }

            if (IsMicrosoftNETSdkWorker())
            {
                // In case has not been set through an event, sets this be default.
                _environmentVariables.Insert(0, new EnvironmentVariableRegistrationRequest(
                    "DOTNET_ENVIRONMENT", "Development", new[]{ Project.Name}));
            }
            else
            {
                // In case has not been set through an event, sets this be default.
                _environmentVariables.Insert(0, new EnvironmentVariableRegistrationRequest(
                    "ASPNETCORE_ENVIRONMENT", "Development",
                    new[]
                    {
                        "IIS Express",
                        Project.Name
                    }));
            }
            foreach (var environmentVariable in _environmentVariables)
            {
                var keys = environmentVariable.TargetProfiles ??= launchSettings.Profiles.Keys;
                foreach (var key in keys)
                {
                    if (!launchSettings.Profiles.TryGetValue(key, out var profile))
                    {
                        continue;
                    }

                    profile.EnvironmentVariables ??= new Dictionary<string, string>();
                    profile.EnvironmentVariables[environmentVariable.Key] = environmentVariable.Value as string;
                }
            }

            return launchSettings.ToJson();
        }

        private bool IsMicrosoftNETSdkWorker()
        {
            return _model.HasNETSettings() && _model.GetNETSettings().SDK().IsMicrosoftNETSdkWorker();
        }

        private LaunchSettings GetDefaultLaunchSettings()
        {

            if (IsMicrosoftNETSdkWorker())
            {
                return GetDefaultMicrosoftNETSdkWorkerLaunchSettings();
            }
            return GetDefaultWebLaunchSettings();
        }

        private LaunchSettings GetDefaultMicrosoftNETSdkWorkerLaunchSettings()
        {
            return new LaunchSettings
            {
                Profiles = new Dictionary<string, Profile>
                {
                    [Project.Name] = Profiles.ContainsKey(Project.Name) ?
                        ReplacePorts(Profiles[Project.Name])
                        : new()
                        {
                            CommandName = CommandName.Project,
                            DotnetRunMessages = true
                        }
                }
            };
        }

        private LaunchSettings GetDefaultWebLaunchSettings()
        {
            return new LaunchSettings
            {
                IisSettings = new IisSetting
                {
                    WindowsAuthentication = false,
                    AnonymousAuthentication = true,
                    IisExpress = new IisExpressClass
                    {
                        ApplicationUrl = new Uri($"http://localhost:{_randomPort}/"),
                        SslPort = _randomSslPort
                    }
                },
                Profiles = new Dictionary<string, Profile>
                {

                    [Project.Name] = Profiles.ContainsKey(Project.Name) ?
                        ReplacePorts(Profiles[Project.Name])
                        : new()
                        {
                            CommandName = CommandName.Project,
                            LaunchBrowser = true,
                            ApplicationUrl = $"https://localhost:{_randomSslPort}/",
                            LaunchUrl = _defaultLaunchUrlPath == null
                                ? null
                                : $"https://localhost:{_randomSslPort}/{_defaultLaunchUrlPath}"
                        },
                    ["IIS Express"] = Profiles.ContainsKey("IIS Express") ?
                        ReplacePorts(Profiles["IIS Express"])
                        : new()
                        {
                            CommandName = CommandName.IisExpress,
                            LaunchBrowser = true,
                            LaunchUrl = _defaultLaunchUrlPath == null
                                ? null
                                : $"https://localhost:{_randomSslPort}/{_defaultLaunchUrlPath}"
                        }
                }
            };
        }

        private Profile ReplacePorts(Profile profile)
        {
            if (profile.LaunchUrl != null)
            {
                profile.LaunchUrl = profile.LaunchUrl.Replace("{HttpPort}", _randomPort.ToString()).Replace("{HttpsPort}", _randomSslPort.ToString());
            }
            if (profile.ApplicationUrl != null)
            {
                profile.ApplicationUrl = profile.ApplicationUrl.Replace("{HttpPort}", _randomPort.ToString()).Replace("{HttpsPort}", _randomSslPort.ToString());
            }
            return profile;
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "launchSettings",
                fileExtension: "json",
                relativeLocation: "Properties"
            );
        }
    }
}