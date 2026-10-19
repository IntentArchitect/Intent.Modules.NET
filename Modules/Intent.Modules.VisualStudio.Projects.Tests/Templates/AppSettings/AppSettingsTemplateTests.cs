using System;
using System.Collections.Generic;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.AppSettings
{
    public class AppSettingsTemplateTests
    {
        // dotnet/eShop's src/OrderProcessor/appsettings.json: 2-space indent, CRLF.
        private static readonly string TwoSpaceFile = Crlf("""
            {
              "Logging": {
                "LogLevel": {
                  "Default": "Information",
                  "Microsoft.AspNetCore": "Warning"
                }
              },
              "ConnectionStrings": {
                "EventBus": "amqp://localhost"
              }
            }
            """);

        private static readonly string FourSpaceFile = Crlf("""
            {
                "Logging": {
                    "LogLevel": {
                        "Default": "Information"
                    }
                }
            }

            """);

        // ----- Existing file -----

        [Fact]
        public void WhenNothingIsAdded_ExistingContentIsReturnedByteForByte()
        {
            var result = AppSettingsTemplate.UpdateExistingFile(TwoSpaceFile, json =>
            {
                // Already present with a different value: allowReplacement is false, so nothing changes
                json["Logging"]!["LogLevel"]!["Default"] ??= "Warning";
            });

            result.ShouldBeSameAs(TwoSpaceFile);
        }

        [Fact]
        public void WhenAKeyIsAdded_ATwoSpaceFileKeepsItsIndentAndNewlines()
        {
            var result = AppSettingsTemplate.UpdateExistingFile(TwoSpaceFile, json => json["AllowedHosts"] = "*");

            result.ShouldBe(Crlf("""
                {
                  "Logging": {
                    "LogLevel": {
                      "Default": "Information",
                      "Microsoft.AspNetCore": "Warning"
                    }
                  },
                  "ConnectionStrings": {
                    "EventBus": "amqp://localhost"
                  },
                  "AllowedHosts": "*"
                }
                """));
        }

        [Fact]
        public void WhenAKeyIsAdded_AFourSpaceFileKeepsItsIndentAndTrailingNewline()
        {
            var result = AppSettingsTemplate.UpdateExistingFile(FourSpaceFile, json => json["AllowedHosts"] = "*");

            result.ShouldBe(Crlf("""
                {
                    "Logging": {
                        "LogLevel": {
                            "Default": "Information"
                        }
                    },
                    "AllowedHosts": "*"
                }

                """));
        }

        [Fact]
        public void CommentsAndDateLikeStringsSurviveAnUpdate()
        {
            var existing = Lf("""
                {
                  // Rotated monthly
                  "ExpiresOn": "2025-01-31T00:00:00Z",
                  "Price": 1.50,
                  "Feature": {
                    "Enabled": true // until GA
                  }
                }
                """);

            var result = AppSettingsTemplate.UpdateExistingFile(existing, json => json["AllowedHosts"] = "*");

            result.ShouldBe(Lf("""
                {
                  // Rotated monthly
                  "ExpiresOn": "2025-01-31T00:00:00Z",
                  "Price": 1.50,
                  "Feature": {
                    "Enabled": true // until GA
                  },
                  "AllowedHosts": "*"
                }
                """));
        }

        // ----- New file -----

        [Fact]
        public void ANewFileUsesTheGivenIndentation()
        {
            var result = AppSettingsTemplate.CreateNewFile("{}", json => json["AllowedHosts"] = "*", indentChar: ' ', indentation: 4);

            result.ShouldBe($"{{{Environment.NewLine}    \"AllowedHosts\": \"*\"{Environment.NewLine}}}");
        }

        // ----- .editorconfig -----

        // The tail of dotnet/eShop's .editorconfig: no JSON section, and a C#-only indent_size.
        private const string EShopEditorConfig = """
            root = true

            [*]
            indent_style = space

            [*.{csproj,vbproj,vcxproj,vcxproj.filters,proj,projitems,shproj}]
            indent_size = 2

            [*.{cs,csx,vb,vbx}]
            indent_size = 4
            """;

        [Fact]
        public void EditorConfig_ASectionForOtherFilesIsIgnored()
        {
            var settings = DefaultSettings();

            AppSettingsTemplate.ParseEditorConfigToSettings(EShopEditorConfig, @"D:\Dev\MSeShop\src\OrderProcessor\appsettings.json", settings);

            settings["indent_size"].ShouldBe("2");
            settings["indent_style"].ShouldBe("space");
            settings.ShouldNotContainKey("root");
        }

        [Fact]
        public void EditorConfig_AJsonSectionApplies()
        {
            var settings = DefaultSettings();
            const string editorConfig = """
                [*]
                indent_size = 2

                [*.{json,yml}]
                indent_size = 4
                indent_style = tab

                [*.cs]
                indent_size = 8
                """;

            AppSettingsTemplate.ParseEditorConfigToSettings(editorConfig, "/repo/src/Api/appsettings.json", settings);

            settings["indent_size"].ShouldBe("4");
            settings["indent_style"].ShouldBe("tab");
        }

        [Theory]
        [InlineData("*", true)]
        [InlineData("*.json", true)]
        [InlineData("*.{json,yml}", true)]
        [InlineData("{*.json,*.yml}", true)]
        [InlineData("appsettings*.json", true)]
        [InlineData("**.json", true)]
        [InlineData("**/*.json", true)]
        [InlineData("src/**/*.json", true)]
        [InlineData("*.js?n", true)]
        [InlineData("*.[jJ]son", true)]
        [InlineData("*.cs", false)]
        [InlineData("*.{cs,csx,vb,vbx}", false)]
        [InlineData("tests/**/*.json", false)]
        [InlineData("*.[!j]son", false)]
        public void EditorConfig_SectionGlobsMatchAsExpected(string glob, bool expected)
        {
            AppSettingsTemplate.EditorConfigSectionMatches(glob, @"C:\repo\src\Api\appsettings.json").ShouldBe(expected);
        }

        private static Dictionary<string, string> DefaultSettings() => new(StringComparer.OrdinalIgnoreCase)
        {
            ["indent_size"] = "2",
            ["indent_style"] = "space"
        };

        private static string Lf(string value) => value.ReplaceLineEndings("\n");

        private static string Crlf(string value) => value.ReplaceLineEndings("\r\n");
    }
}
