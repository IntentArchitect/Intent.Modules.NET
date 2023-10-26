using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#pragma warning disable IDE0010 // Add missing switch cases

namespace Intent.Modules.VisualStudio.Projects.Templates.LaunchSettings
{
    // Generated using https://app.quicktype.io/?l=csharp with http://json.schemastore.org/launchsettings.json
    // ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
    public partial class LaunchSettings
    {
        /// <summary>
        /// IIS and IIS Express settings
        /// </summary>
        [JsonProperty("iisSettings", NullValueHandling = NullValueHandling.Ignore)]
        public IisSetting IisSettings { get; set; }

        /// <summary>
        /// A list of debug profiles
        /// </summary>
        [JsonProperty("profiles", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Profile> Profiles { get; set; }
    }

    /// <summary>
    /// IIS and IIS Express settings
    /// </summary>
    public partial class IisSetting
    {
        /// <summary>
        /// Set to true to enable windows authentication for your site in IIS and IIS Express.
        /// </summary>
        [JsonProperty("windowsAuthentication", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WindowsAuthentication { get; set; }

        /// <summary>
        /// Set to true to enable anonymous authentication for your site in IIS and IIS Express.
        /// </summary>
        [JsonProperty("anonymousAuthentication", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AnonymousAuthentication { get; set; }

        /// <summary>
        /// Site settings to use with IIS profiles.
        /// </summary>
        [JsonProperty("iis", NullValueHandling = NullValueHandling.Ignore)]
        public Iis Iis { get; set; }

        /// <summary>
        /// Site settings to use with IISExpress profiles.
        /// </summary>
        [JsonProperty("iisExpress", NullValueHandling = NullValueHandling.Ignore)]
        public IisExpressClass IisExpress { get; set; }
    }

    /// <summary>
    /// Site settings to use with IIS profiles.
    /// </summary>
    public partial class Iis
    {
        /// <summary>
        /// The URL of the web site.
        /// </summary>
        [JsonProperty("applicationUrl", NullValueHandling = NullValueHandling.Ignore)]
        public Uri ApplicationUrl { get; set; }

        /// <summary>
        /// The SSL port to use for the web site.
        /// </summary>
        [JsonProperty("sslPort", NullValueHandling = NullValueHandling.Ignore)]
        public int? SslPort { get; set; }
    }

    /// <summary>
    /// Site settings to use with IISExpress profiles.
    /// </summary>
    public partial class IisExpressClass
    {
        /// <summary>
        /// The URL of the web site.
        /// </summary>
        [JsonProperty("applicationUrl", NullValueHandling = NullValueHandling.Ignore)]
        public Uri ApplicationUrl { get; set; }

        /// <summary>
        /// The SSL port to use for the web site.
        /// </summary>
        [JsonProperty("sslPort", NullValueHandling = NullValueHandling.Ignore)]
        public int? SslPort { get; set; }
    }

    public partial class Profile
    {
        /// <summary>
        /// Identifies the debug target to run.
        /// </summary>
        [JsonProperty("commandName")]
        public CommandName CommandName { get; set; }

        /// <summary>
        /// Set to true if the browser should be launched.
        /// </summary>
        [JsonProperty("launchBrowser", NullValueHandling = NullValueHandling.Ignore)]
        public bool? LaunchBrowser { get; set; }

        /// <summary>
        /// Specifies the hosting model to use when running ASP.NET core projects in IIS and IIS
        /// Express.
        /// </summary>
        [JsonProperty("ancmHostingModel", NullValueHandling = NullValueHandling.Ignore)]
        public AncmHostingModel? AncmHostingModel { get; set; }

        /// <summary>
        /// A semi-colon delimited list of URL(s) to configure for the web server.
        /// </summary>
        [JsonProperty("applicationUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplicationUrl { get; set; }

        /// <summary>
        /// The authentication scheme to use when connecting to the remote computer.
        /// </summary>
        [JsonProperty("authenticationMode", NullValueHandling = NullValueHandling.Ignore)]
        public AuthenticationMode? AuthenticationMode { get; set; }

        /// <summary>
        /// The arguments to pass to the target being run.
        /// </summary>
        [JsonProperty("commandLineArgs", NullValueHandling = NullValueHandling.Ignore)]
        public string CommandLineArgs { get; set; }

        /// <summary>
        /// Set to true to display a message when the project is building.
        /// </summary>
        [JsonProperty("dotnetRunMessages", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DotnetRunMessages { get; set; }

        /// <summary>
        /// An absolute or relative path to the executable.
        /// </summary>
        [JsonProperty("executablePath", NullValueHandling = NullValueHandling.Ignore)]
        public string ExecutablePath { get; set; }

        /// <summary>
        /// Set to true to disable configuration of the site when running the Asp.Net Core Project
        /// profile.
        /// </summary>
        [JsonProperty("externalUrlConfiguration", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ExternalUrlConfiguration { get; set; }

        /// <summary>
        /// Set to true to enable applying code changes to the running application.
        /// </summary>
        [JsonProperty("hotReloadEnabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HotReloadEnabled { get; set; }

        /// <summary>
        /// The HTTP port to use for the web site.
        /// </summary>
        [JsonProperty("httpPort", NullValueHandling = NullValueHandling.Ignore)]
        public int? HttpPort { get; set; }

        /// <summary>
        /// The url to enable debugging on a Blazor WebAssembly application.
        /// </summary>
        [JsonProperty("inspectUri", NullValueHandling = NullValueHandling.Ignore)]
        public string InspectUri { get; set; }

        /// <summary>
        /// Set to true to enable the JavaScript debugger for Microsoft Edge (Chromium) based
        /// WebView2.
        /// </summary>
        [JsonProperty("jsWebView2Debugging", NullValueHandling = NullValueHandling.Ignore)]
        public bool? JsWebView2Debugging { get; set; }

        /// <summary>
        /// The relative URL to launch in the browser.
        /// </summary>
        [JsonProperty("launchUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string LaunchUrl { get; set; }

        /// <summary>
        /// Set to true to leave the IIS application pool running when the project is closed.
        /// </summary>
        [JsonProperty("leaveRunningOnClose", NullValueHandling = NullValueHandling.Ignore)]
        public bool? LeaveRunningOnClose { get; set; }

        /// <summary>
        /// Set to true to enable native code debugging.
        /// </summary>
        [JsonProperty("nativeDebugging", NullValueHandling = NullValueHandling.Ignore)]
        public bool? NativeDebugging { get; set; }

        /// <summary>
        /// Publish all exposed ports to random ports in Docker (-P).
        /// </summary>
        [JsonProperty("publishAllPorts", NullValueHandling = NullValueHandling.Ignore)]
        public bool? PublishAllPorts { get; set; }

        /// <summary>
        /// Set to true to have the debugger attach to a process on a remote computer.
        /// </summary>
        [JsonProperty("remoteDebugEnabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? RemoteDebugEnabled { get; set; }

        /// <summary>
        /// The name and port number of the remote machine in name:port format.
        /// </summary>
        [JsonProperty("remoteDebugMachine", NullValueHandling = NullValueHandling.Ignore)]
        public string RemoteDebugMachine { get; set; }

        /// <summary>
        /// Set to true to enable debugging of SQL scripts and stored procedures.
        /// </summary>
        [JsonProperty("sqlDebugging", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SqlDebugging { get; set; }

        /// <summary>
        /// The SSL port to use for the web site.
        /// </summary>
        [JsonProperty("sslPort", NullValueHandling = NullValueHandling.Ignore)]
        public int? SslPort { get; set; }

        /// <summary>
        /// A relative ot absolute path to the .NET project file on which Roslyn component should be
        /// executed. Relative to the current project's folder.
        /// </summary>
        [JsonProperty("targetProject", NullValueHandling = NullValueHandling.Ignore)]
        public string TargetProject { get; set; }

        /// <summary>
        /// Set to true to run the 64 bit version of IIS Express, false to run the x86 version.
        /// </summary>
        [JsonProperty("use64Bit", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Use64Bit { get; set; }

        /// <summary>
        /// Set to true to bind the SSL port.
        /// </summary>
        [JsonProperty("useSSL", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UseSsl { get; set; }

        /// <summary>
        /// Set the environment variables as key/value pairs.
        /// </summary>
        [JsonProperty("environmentVariables", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> EnvironmentVariables { get; set; }

        /// <summary>
        /// Sets the working directory of the command.
        /// </summary>
        [JsonProperty("workingDirectory", NullValueHandling = NullValueHandling.Ignore)]
        public string WorkingDirectory { get; set; }
    }

    /// <summary>
    /// Specifies the hosting model to use when running ASP.NET core projects in IIS and IIS
    /// Express.
    /// </summary>
    public enum AncmHostingModel { InProcess, OutOfProcess };

    /// <summary>
    /// The authentication scheme to use when connecting to the remote computer.
    /// </summary>
    public enum AuthenticationMode { None, Windows };

    /// <summary>
    /// Identifies the debug target to run.
    /// </summary>
    public enum CommandName { DebugRoslynComponent, Docker, DockerCompose, Executable, Iis, IisExpress, MsixPackage, Project, WSL2 };

    public partial class LaunchSettings
    {
        public static LaunchSettings FromJson(string json) => JsonConvert.DeserializeObject<LaunchSettings>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this LaunchSettings self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                AncmHostingModelConverter.Singleton,
                AuthenticationModeConverter.Singleton,
                CommandNameConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            Formatting = Formatting.Indented
        };
    }

    internal class AncmHostingModelConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(AncmHostingModel) || t == typeof(AncmHostingModel?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "InProcess":
                    return AncmHostingModel.InProcess;
                case "OutOfProcess":
                    return AncmHostingModel.OutOfProcess;
            }
            throw new Exception("Cannot unmarshal type AncmHostingModel");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (AncmHostingModel)untypedValue;
            switch (value)
            {
                case AncmHostingModel.InProcess:
                    serializer.Serialize(writer, "InProcess");
                    return;
                case AncmHostingModel.OutOfProcess:
                    serializer.Serialize(writer, "OutOfProcess");
                    return;
            }
            throw new Exception("Cannot marshal type AncmHostingModel");
        }

        public static readonly AncmHostingModelConverter Singleton = new AncmHostingModelConverter();
    }

    internal class AuthenticationModeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(AuthenticationMode) || t == typeof(AuthenticationMode?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "None":
                    return AuthenticationMode.None;
                case "Windows":
                    return AuthenticationMode.Windows;
            }
            throw new Exception("Cannot unmarshal type AuthenticationMode");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (AuthenticationMode)untypedValue;
            switch (value)
            {
                case AuthenticationMode.None:
                    serializer.Serialize(writer, "None");
                    return;
                case AuthenticationMode.Windows:
                    serializer.Serialize(writer, "Windows");
                    return;
            }
            throw new Exception("Cannot marshal type AuthenticationMode");
        }

        public static readonly AuthenticationModeConverter Singleton = new AuthenticationModeConverter();
    }

    internal class CommandNameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(CommandName) || t == typeof(CommandName?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value.ToLower())
            {
                case "debugroslyncomponent":
                    return CommandName.DebugRoslynComponent;
                case "docker":
                    return CommandName.Docker;
                case "dockercompose":
                    return CommandName.DockerCompose;
                case "executable":
                    return CommandName.Executable;
                case "iis":
                    return CommandName.Iis;
                case "iisexpress":
                    return CommandName.IisExpress;
                case "msixpackage":
                    return CommandName.MsixPackage;
                case "project":
                    return CommandName.Project;
                case "wsl2":
                    return CommandName.WSL2;
            }
            throw new Exception("Cannot unmarshal type CommandName");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (CommandName)untypedValue;
            switch (value)
            {
                case CommandName.DebugRoslynComponent:
                    serializer.Serialize(writer, "DebugRoslynComponent");
                    return;
                case CommandName.Docker:
                    serializer.Serialize(writer, "Docker");
                    return;
                case CommandName.DockerCompose:
                    serializer.Serialize(writer, "DockerCompose");
                    return;
                case CommandName.Executable:
                    serializer.Serialize(writer, "Executable");
                    return;
                case CommandName.Iis:
                    serializer.Serialize(writer, "IIS");
                    return;
                case CommandName.IisExpress:
                    serializer.Serialize(writer, "IISExpress");
                    return;
                case CommandName.MsixPackage:
                    serializer.Serialize(writer, "MsixPackage");
                    return;
                case CommandName.Project:
                    serializer.Serialize(writer, "Project");
                    return;
                case CommandName.WSL2:
                    serializer.Serialize(writer, "WSL2");
                    return;
            }
            throw new Exception("Cannot marshal type CommandName");
        }

        public static readonly CommandNameConverter Singleton = new CommandNameConverter();
    }
}
