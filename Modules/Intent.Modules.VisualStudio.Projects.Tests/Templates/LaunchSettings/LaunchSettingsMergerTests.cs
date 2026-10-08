using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.VisualStudio.Projects.Templates.LaunchSettings;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;
using LaunchSettingsModel = Intent.Modules.VisualStudio.Projects.Templates.LaunchSettings.LaunchSettings;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.LaunchSettings
{
    public class LaunchSettingsMergerTests
    {
        private const string ProjectName = "MyApp.Api";

        // The shape Visual Studio / "dotnet new" writes: $schema, dotnetRunMessages ahead of
        // applicationUrl, and a trailing newline - none of which the typed classes round-trip.
        private static readonly string VisualStudioStyleFile = Lf("""
            {
              "$schema": "https://json.schemastore.org/launchsettings.json",
              "profiles": {
                "MyApp.Api": {
                  "commandName": "Project",
                  "dotnetRunMessages": true,
                  "launchBrowser": true,
                  "launchUrl": "swagger",
                  "applicationUrl": "https://localhost:7001;http://localhost:5001",
                  "environmentVariables": {
                    "ASPNETCORE_ENVIRONMENT": "Development"
                  }
                },
                "IIS Express": {
                  "commandName": "IISExpress",
                  "launchBrowser": true,
                  "launchUrl": "swagger",
                  "environmentVariables": {
                    "ASPNETCORE_ENVIRONMENT": "Development"
                  }
                }
              },
              "iisSettings": {
                "windowsAuthentication": false,
                "anonymousAuthentication": true,
                "iisExpress": {
                  "applicationUrl": "http://localhost:5002",
                  "sslPort": 44300
                }
              }
            }

            """);

        // ----- Nothing to add -----

        [Fact]
        public void WhenNothingIsMissing_ShouldReturnExistingContentByteForByte()
        {
            var result = MergeExisting(VisualStudioStyleFile, Registrations(
                environmentVariables: [DefaultAspNetCoreEnvironment()],
                httpPortRequired: true,
                defaultLaunchUrlPath: "swagger"));

            result.ShouldBeSameAs(VisualStudioStyleFile);
        }

        [Fact]
        public void WhenNothingIsMissing_WithCrlfAndNoTrailingNewLine_ShouldReturnExistingContentByteForByte()
        {
            var existing = Crlf(VisualStudioStyleFile).TrimEnd();

            var result = MergeExisting(existing, Registrations(environmentVariables: [DefaultAspNetCoreEnvironment()]));

            result.ShouldBeSameAs(existing);
        }

        [Fact]
        public void WhenRegisteredProfileHasSameNameAsExistingProfile_ShouldLeaveExistingProfileUntouched()
        {
            var registered = new Profile
            {
                CommandName = CommandName.Project,
                DotnetRunMessages = true,
                InspectUri = "{wsProtocol}://{url.hostname}:{url.port}/_framework/debug/ws-proxy?browser={browserInspectUri}"
            };

            var result = MergeExisting(VisualStudioStyleFile, Registrations(profiles: [new(ProjectName, registered)]));

            result.ShouldBeSameAs(VisualStudioStyleFile);
        }

        [Fact]
        public void WhenProfilesIsMissing_AndNothingToAdd_ShouldNotCreateIt()
        {
            var existing = Lf("""
                {
                  "iisSettings": {
                    "windowsAuthentication": false
                  }
                }
                """);

            var result = MergeExisting(existing, Registrations(
                environmentVariables: [DefaultAspNetCoreEnvironment()],
                httpPortRequired: true,
                defaultLaunchUrlPath: "swagger"));

            result.ShouldBeSameAs(existing);
        }

        [Fact]
        public void WhenLaunchUrlIsNull_ShouldNotThrow()
        {
            var existing = Lf("""
                {
                  "profiles": {
                    "MyApp.Api": {
                      "commandName": "Project",
                      "launchUrl": null
                    }
                  }
                }
                """);

            var result = MergeExisting(existing, Registrations(defaultLaunchUrlPath: "scalar/v1"));

            result.ShouldBeSameAs(existing);
        }

        // ----- Adding what's missing -----

        [Fact]
        public void WhenRegisteredProfileIsMissing_ShouldAppendItAndLeaveEverythingElseAsIs()
        {
            var result = MergeExisting(VisualStudioStyleFile, Registrations(
                profiles: [new("Docker", new Profile { CommandName = CommandName.Docker, LaunchBrowser = true })],
                environmentVariables:
                [
                    DefaultAspNetCoreEnvironment(),
                    new EnvironmentVariableRegistrationRequest("ASPNETCORE_HTTP_PORTS", "8080", ["Docker"])
                ],
                defaultLaunchUrlPath: "swagger"));

            result.ShouldBe(Lf("""
                {
                  "$schema": "https://json.schemastore.org/launchsettings.json",
                  "profiles": {
                    "MyApp.Api": {
                      "commandName": "Project",
                      "dotnetRunMessages": true,
                      "launchBrowser": true,
                      "launchUrl": "swagger",
                      "applicationUrl": "https://localhost:7001;http://localhost:5001",
                      "environmentVariables": {
                        "ASPNETCORE_ENVIRONMENT": "Development"
                      }
                    },
                    "IIS Express": {
                      "commandName": "IISExpress",
                      "launchBrowser": true,
                      "launchUrl": "swagger",
                      "environmentVariables": {
                        "ASPNETCORE_ENVIRONMENT": "Development"
                      }
                    },
                    "Docker": {
                      "commandName": "Docker",
                      "launchBrowser": true,
                      "environmentVariables": {
                        "ASPNETCORE_HTTP_PORTS": "8080"
                      }
                    }
                  },
                  "iisSettings": {
                    "windowsAuthentication": false,
                    "anonymousAuthentication": true,
                    "iisExpress": {
                      "applicationUrl": "http://localhost:5002",
                      "sslPort": 44300
                    }
                  }
                }

                """));
        }

        [Fact]
        public void WhenProfilesIsMissing_AndProfileIsRegistered_ShouldCreateProfiles()
        {
            var existing = Lf("""
                {
                  "iisSettings": {
                    "windowsAuthentication": false
                  }
                }
                """);

            var result = MergeExisting(existing, Registrations(
                profiles: [new(ProjectName, new Profile { CommandName = CommandName.Project })]));

            result.ShouldBe(Lf("""
                {
                  "iisSettings": {
                    "windowsAuthentication": false
                  },
                  "profiles": {
                    "MyApp.Api": {
                      "commandName": "Project"
                    }
                  }
                }
                """));
        }

        [Fact]
        public void WhenEnvironmentVariableIsMissing_ShouldAppendItAndKeepExistingValues()
        {
            var existing = Lf("""
                {
                  "profiles": {
                    "MyApp.Api": {
                      "commandName": "Project",
                      "environmentVariables": {
                        "ASPNETCORE_ENVIRONMENT": "Staging"
                      }
                    },
                    "Worker": {
                      "commandName": "Project"
                    }
                  }
                }
                """);

            var result = MergeExisting(existing, Registrations(environmentVariables:
            [
                DefaultAspNetCoreEnvironment(),
                new EnvironmentVariableRegistrationRequest("FEATURE_FLAG", "on", null)
            ]));

            result.ShouldBe(Lf("""
                {
                  "profiles": {
                    "MyApp.Api": {
                      "commandName": "Project",
                      "environmentVariables": {
                        "ASPNETCORE_ENVIRONMENT": "Staging",
                        "FEATURE_FLAG": "on"
                      }
                    },
                    "Worker": {
                      "commandName": "Project",
                      "environmentVariables": {
                        "FEATURE_FLAG": "on"
                      }
                    }
                  }
                }
                """));
        }

        // ----- Formatting is taken from the existing file -----

        [Fact]
        public void WhenChanged_ShouldKeepCrlfAndFourSpaceIndentation()
        {
            var existing = Crlf("""
                {
                    "profiles": {
                        "MyApp.Api": {
                            "commandName": "Project"
                        }
                    }
                }
                """);

            var result = MergeExisting(existing, Registrations(environmentVariables: [DefaultAspNetCoreEnvironment()]));

            result.ShouldBe(Crlf("""
                {
                    "profiles": {
                        "MyApp.Api": {
                            "commandName": "Project",
                            "environmentVariables": {
                                "ASPNETCORE_ENVIRONMENT": "Development"
                            }
                        }
                    }
                }
                """));
        }

        [Fact]
        public void WhenChanged_ShouldKeepTabIndentationAndTrailingNewLine()
        {
            var existing = "{\n\t\"profiles\": {\n\t\t\"MyApp.Api\": {\n\t\t\t\"commandName\": \"Project\"\n\t\t}\n\t}\n}\n";

            var result = MergeExisting(existing, Registrations(environmentVariables: [DefaultAspNetCoreEnvironment()]));

            result.ShouldBe("{\n\t\"profiles\": {\n\t\t\"MyApp.Api\": {\n\t\t\t\"commandName\": \"Project\",\n\t\t\t\"environmentVariables\": {\n\t\t\t\t\"ASPNETCORE_ENVIRONMENT\": \"Development\"\n\t\t\t}\n\t\t}\n\t}\n}\n");
        }

        [Fact]
        public void WhenChanged_ShouldKeepUnknownCommandNamesPropertiesCasingAndValues()
        {
            var existing = Lf("""
                {
                  "x-custom": {
                    "createdOn": "2026-10-08T00:00:00Z",
                    "ratio": 1.50
                  },
                  "profiles": {
                    "Compose": {
                      "commandName": "SomeFutureCommand",
                      "hotReloadProfile": "aspnetcore"
                    },
                    "IIS Express": {
                      "commandName": "iisexpress"
                    }
                  }
                }
                """);

            var result = MergeExisting(existing, Registrations(
                environmentVariables: [new EnvironmentVariableRegistrationRequest("FEATURE_FLAG", "on", ["Compose"])]));

            result.ShouldBe(Lf("""
                {
                  "x-custom": {
                    "createdOn": "2026-10-08T00:00:00Z",
                    "ratio": 1.50
                  },
                  "profiles": {
                    "Compose": {
                      "commandName": "SomeFutureCommand",
                      "hotReloadProfile": "aspnetcore",
                      "environmentVariables": {
                        "FEATURE_FLAG": "on"
                      }
                    },
                    "IIS Express": {
                      "commandName": "iisexpress"
                    }
                  }
                }
                """));
        }

        [Fact]
        public void WhenMergedOutputIsMergedAgain_ShouldReturnItByteForByte()
        {
            var registrations = Registrations(
                profiles: [new("Docker", new Profile { CommandName = CommandName.Docker })],
                environmentVariables: [DefaultAspNetCoreEnvironment()],
                httpPortRequired: true,
                defaultLaunchUrlPath: "scalar/v1");

            var first = MergeExisting(VisualStudioStyleFile, registrations);
            var second = MergeExisting(first, registrations);

            first.ShouldNotBe(VisualStudioStyleFile);
            second.ShouldBeSameAs(first);
        }

        // ----- LaunchProfileHttpPortRequired -----

        [Fact]
        public void WhenHttpPortRequired_ShouldOnlyAddHttpUrlToHttpsOnlyApplicationUrls()
        {
            var existing = Lf("""
                {
                  "profiles": {
                    "HttpsOnly": {
                      "commandName": "Project",
                      "applicationUrl": "https://localhost:7001"
                    },
                    "AlreadyHasHttp": {
                      "commandName": "Project",
                      "applicationUrl": "https://localhost:7002;http://localhost:5002"
                    },
                    "NoApplicationUrl": {
                      "commandName": "IISExpress"
                    }
                  }
                }
                """);

            var result = MergeExisting(existing, Registrations(httpPortRequired: true, httpPort: 5050));

            var profiles = (JObject)LaunchSettingsMerger.Parse(result)["profiles"];
            profiles["HttpsOnly"]["applicationUrl"].Value<string>().ShouldBe("https://localhost:7001;http://localhost:5050");
            profiles["AlreadyHasHttp"]["applicationUrl"].Value<string>().ShouldBe("https://localhost:7002;http://localhost:5002");
            ((JObject)profiles["NoApplicationUrl"]).ContainsKey("applicationUrl").ShouldBeFalse();
        }

        // ----- launchUrl on "*.Api" profiles -----

        [Theory]
        [InlineData("MyApp.Api", "https://localhost:7001/swagger", "scalar/v1", "https://localhost:7001/scalar/v1")]
        [InlineData("MyApp.Api", "https://localhost:7001/scalar/v1", "swagger", "https://localhost:7001/swagger")]
        [InlineData("MyApp.Api", "swagger", "scalar/v1", "scalar/v1")]
        [InlineData("MyApp.Api", "/swagger", "scalar/v1", "/scalar/v1")]
        [InlineData("MyApp.Api", "https://localhost:7001/health", "scalar/v1", "https://localhost:7001/health")]
        [InlineData("MyApp.Api", "https://localhost:7001", "swagger", "https://localhost:7001")]
        [InlineData("MyApp.Api", "https://localhost:7001/swagger?urls.primaryName=v2", "scalar/v1", "https://localhost:7001/swagger?urls.primaryName=v2")]
        [InlineData("MyApp.Web", "https://localhost:7001/swagger", "scalar/v1", "https://localhost:7001/swagger")]
        public void WhenDefaultLaunchUrlPathIsSet_ShouldOnlyUpdateIntentManagedLaunchUrls(
            string profileName,
            string launchUrl,
            string defaultLaunchUrlPath,
            string expectedLaunchUrl)
        {
            var existing = new JObject
            {
                ["profiles"] = new JObject
                {
                    [profileName] = new JObject
                    {
                        ["commandName"] = "Project",
                        ["launchUrl"] = launchUrl
                    }
                }
            }.ToString();

            var result = MergeExisting(existing, Registrations(defaultLaunchUrlPath: defaultLaunchUrlPath));

            LaunchSettingsMerger.Parse(result)["profiles"][profileName]["launchUrl"].Value<string>().ShouldBe(expectedLaunchUrl);
        }

        // ----- No existing file -----

        [Fact]
        public void WhenNoExistingFile_ShouldMatchTypedSerializationOfDefaultsPlusRegistrations()
        {
            var registrations = Registrations(
                profiles:
                [
                    new("Mock Lambda Test Tool", new Profile
                    {
                        CommandName = CommandName.Executable,
                        CommandLineArgs = "--port 5050",
                        WorkingDirectory = "$(ProjectDir)$(OutputPath)"
                    })
                ],
                environmentVariables:
                [
                    DefaultAspNetCoreEnvironment(),
                    new EnvironmentVariableRegistrationRequest("FEATURE_FLAG", "on", null)
                ],
                httpPortRequired: true,
                httpPort: 5050,
                defaultLaunchUrlPath: "swagger");

            var result = LaunchSettingsMerger.Merge(existingContent: null, CreateWebDefaults, registrations);

            // What the typed LaunchSettings model serializes to once the same registrations are applied:
            // environmentVariables keeps its declared position (ahead of workingDirectory).
            var expected = CreateWebDefaults();
            expected.Profiles[ProjectName].ApplicationUrl = "https://localhost:44300/;http://localhost:5050";
            expected.Profiles[ProjectName].EnvironmentVariables = new() { ["ASPNETCORE_ENVIRONMENT"] = "Development", ["FEATURE_FLAG"] = "on" };
            expected.Profiles["IIS Express"].EnvironmentVariables = new() { ["ASPNETCORE_ENVIRONMENT"] = "Development", ["FEATURE_FLAG"] = "on" };
            expected.Profiles["Mock Lambda Test Tool"] = new Profile
            {
                CommandName = CommandName.Executable,
                CommandLineArgs = "--port 5050",
                EnvironmentVariables = new() { ["FEATURE_FLAG"] = "on" },
                WorkingDirectory = "$(ProjectDir)$(OutputPath)"
            };

            result.ShouldBe(expected.ToJson());
            result.IndexOf("\"environmentVariables\"", result.IndexOf("Mock Lambda Test Tool", StringComparison.Ordinal), StringComparison.Ordinal)
                .ShouldBeLessThan(result.IndexOf("\"workingDirectory\"", StringComparison.Ordinal));
        }

        [Fact]
        public void WhenNoExistingFile_AndNoProfiles_ShouldGenerateEmptyObject()
        {
            var result = LaunchSettingsMerger.Merge(
                existingContent: null,
                () => new LaunchSettingsModel(),
                Registrations(environmentVariables: [DefaultAspNetCoreEnvironment()]));

            result.ShouldBe(new LaunchSettingsModel().ToJson());
        }

        [Fact]
        public void WhenExistingFileIsWhitespace_ShouldGenerateNewFile()
        {
            var result = LaunchSettingsMerger.Merge(existingContent: "  \r\n", CreateWebDefaults, Registrations());

            result.ShouldBe(CreateWebDefaults().ToJson());
        }

        [Fact]
        public void WhenNewFileIsMergedAgain_ShouldReturnItByteForByte()
        {
            var registrations = Registrations(
                environmentVariables: [DefaultAspNetCoreEnvironment()],
                httpPortRequired: true,
                defaultLaunchUrlPath: "swagger");

            var created = LaunchSettingsMerger.Merge(existingContent: null, CreateWebDefaults, registrations);
            var merged = MergeExisting(created, registrations);

            merged.ShouldBeSameAs(created);
        }

        private static LaunchSettingsModel CreateWebDefaults()
        {
            return new LaunchSettingsModel
            {
                IisSettings = new IisSetting
                {
                    WindowsAuthentication = false,
                    AnonymousAuthentication = true,
                    IisExpress = new IisExpressClass
                    {
                        ApplicationUrl = new Uri("http://localhost:5002/"),
                        SslPort = 44300
                    }
                },
                Profiles = new Dictionary<string, Profile>
                {
                    [ProjectName] = new()
                    {
                        CommandName = CommandName.Project,
                        LaunchBrowser = true,
                        ApplicationUrl = "https://localhost:44300/",
                        LaunchUrl = "https://localhost:44300/swagger"
                    },
                    ["IIS Express"] = new()
                    {
                        CommandName = CommandName.IisExpress,
                        LaunchBrowser = true,
                        LaunchUrl = "https://localhost:44300/swagger"
                    }
                }
            };
        }

        private static EnvironmentVariableRegistrationRequest DefaultAspNetCoreEnvironment()
        {
            return new EnvironmentVariableRegistrationRequest("ASPNETCORE_ENVIRONMENT", "Development", ["IIS Express", ProjectName]);
        }

        private static LaunchSettingsRegistrations Registrations(
            IEnumerable<KeyValuePair<string, Profile>> profiles = null,
            IEnumerable<EnvironmentVariableRegistrationRequest> environmentVariables = null,
            bool httpPortRequired = false,
            int httpPort = 5050,
            string defaultLaunchUrlPath = null)
        {
            return new LaunchSettingsRegistrations(
                Profiles: profiles ?? [],
                EnvironmentVariables: (environmentVariables ?? []).ToList(),
                HttpPortRequired: httpPortRequired,
                HttpPort: httpPort,
                DefaultLaunchUrlPath: defaultLaunchUrlPath);
        }

        private static string MergeExisting(string existing, LaunchSettingsRegistrations registrations)
        {
            return LaunchSettingsMerger.Merge(
                existing,
                () => throw new InvalidOperationException("Defaults should only be created when there is no existing file."),
                registrations);
        }

        private static string Lf(string value) => value.Replace("\r\n", "\n");

        private static string Crlf(string value) => Lf(value).Replace("\n", "\r\n");
    }
}
