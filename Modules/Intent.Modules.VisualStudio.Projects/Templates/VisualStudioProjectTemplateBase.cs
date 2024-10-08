using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions;
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.NuGet;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates;

public abstract class VisualStudioProjectTemplateBase<TModel> : IntentFileTemplateBase<TModel>, IVisualStudioProjectTemplate, IHasGlobalUsings
    where TModel : IVisualStudioProject
{
    private string _fileContent;
    private readonly Dictionary<string, string> _moduleRequestedProperties = [];

    protected VisualStudioProjectTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
    {
        ExecutionContext.EventDispatcher.Subscribe<AddProjectPropertyEvent>(HandleAddProjectPropertyEvent);
        ExecutionContext.EventDispatcher.Subscribe<AddUserSecretsEvent>(HandleAddUserSecretsEvent);
    }

    public string ProjectId => Model.Id;
    public string Name => Model.Name;
    public string FilePath => FileMetadata.GetFilePath();
    IVisualStudioProject IVisualStudioProjectTemplate.Project => Model;

    public string LoadContent()
    {
        var change = ExecutionContext.ChangeManager.FindChange(FilePath);
        if (change != null)
        {
            return change.Content;
        }

        if (_fileContent == null)
        {
            TryGetExistingFileContent(out _fileContent);
        }

        return _fileContent;
    }

    public void UpdateContent(string content, ISoftwareFactoryEventDispatcher sfEventDispatcher)
    {
        // Normalize the content of both by parsing with no whitespace and calling .ToString()
        var targetContent = XDocument.Parse(content).ToString().ReplaceLineEndings();
        var existingContent = LoadContent().ReplaceLineEndings();

        if (existingContent == targetContent)
        {
            return;
        }

        var change = ExecutionContext.ChangeManager.FindChange(FilePath);
        if (change != null)
        {
            change.ChangeContent(content, content);
            return;
        }

        sfEventDispatcher.Publish(new SoftwareFactoryEvent(SoftwareFactoryEvents.OverwriteFileCommand, new Dictionary<string, string>
        {
            ["FullFileName"] = FilePath,
            ["Context"] = ToString(),
            ["Content"] = content
        }));
    }

    public virtual IEnumerable<INugetPackageInfo> RequestedNugetPackages() => OutputTarget.NugetPackages();

    public IEnumerable<string> GetTargetFrameworks() => Model.TargetFrameworkVersion();

    public override void OnCreated()
    {
        base.OnCreated();
        ExecutionContext.EventDispatcher.Publish(new VisualStudioProjectCreatedEvent(this));
    }

    /// <summary>
    /// This method has been <see langword="sealed"/> to enforce using existing content if it
    /// exists as well as doing a semantic comparison of the result of the xml to avoid
    /// whitespace only changes from occurring. Use <see cref="ApplyAdditionalTransforms"/>
    /// to make changes to the content that was either already existing or generated for the
    /// first time by the <see cref="TransformText"/> method.
    /// </summary>
    public sealed override string RunTemplate()
    {
        var hadExistingContent = TryGetExistingFileContent(out var existingFileContent);

        var content = hadExistingContent
            ? existingFileContent
            : base.RunTemplate().ReplaceLineEndings();

        content = ApplyAdditionalTransforms(content);

        var doc = XDocument.Parse(content);

        var hasChange = ApplySettings(doc);

        return !hadExistingContent || (hasChange && !XmlHelper.IsSemanticallyTheSame(existingFileContent, doc))
            ? doc.ToFormattedProjectString()
            : existingFileContent;
    }

    /// <summary>
    /// Used to return the initial template content if there is no existing file.
    /// </summary>
    /// <remarks>
    /// Do not put any additional logic in your implementation as this method is only called
    /// when there is no existing file, instead do so in <see cref="ApplyAdditionalTransforms"/>
    /// and <see cref="RunTemplate"/> will ensure that it is passed the existing file content if
    /// it exists or otherwise the result of this <see cref="TransformText"/> method if the file
    /// is being generated for the first time.
    /// </remarks>
    public abstract override string TransformText();

    /// <summary>
    /// Override this method if there are additional changes to the output that you wish to perform.
    /// </summary>
    /// <remarks>
    /// This method is called by <see cref="RunTemplate"/> with either the existing file content
    /// if it exists or otherwise the result of <see cref="TransformText"/> being used for the
    /// <paramref name="existingFileOrTransformTextContent"/>.
    /// </remarks>
    protected virtual string ApplyAdditionalTransforms(string existingFileOrTransformTextContent) => existingFileOrTransformTextContent;

    public override ITemplateFileConfig GetTemplateFileConfig()
    {
        return new TemplateFileConfig(
            fileName: Project.Name,
            fileExtension: "csproj"
        );
    }

    /// <summary>
    /// Applies settings from stereotypes in the Visual Studio designer.
    /// </summary>
    /// <returns>Whether there was a change.</returns>
    private bool ApplySettings(XDocument doc)
    {
        if (doc.ResolveProjectScheme() != VisualStudioProjectScheme.Sdk)
        {
            return false;
        }

        var hasChange = false;
        var project = ((IVisualStudioProjectTemplate)this).Project;

        hasChange |= SyncFrameworks(doc);

        var netCoreSettings = project.GetNETCoreSettings();
        if (netCoreSettings != null)
        {
            hasChange |= SyncProperty(doc, "Configurations", netCoreSettings.Configurations());
            hasChange |= SyncProperty(doc, "RuntimeIdentifiers", netCoreSettings.RuntimeIdentifiers());
            hasChange |= SyncProperty(doc, "UserSecretsId", netCoreSettings.UserSecretsId());
            hasChange |= SyncProperty(doc, "RootNamespace", netCoreSettings.RootNamespace());
            hasChange |= SyncProperty(doc, "AssemblyName", netCoreSettings.AssemblyName());
            hasChange |= SyncManageableBooleanProperty(doc, "ImplicitUsings", netCoreSettings.ImplicitUsings().Value);
            hasChange |= SyncManageableBooleanProperty(doc, "GenerateRuntimeConfigurationFiles", netCoreSettings.GenerateRuntimeConfigurationFiles().Value);
            hasChange |= SyncManageableBooleanProperty(doc, "GenerateDocumentationFile", netCoreSettings.GenerateDocumentationFile().Value);
        }

        if (project is CSharpProjectNETModel model &&
            model.HasNETSettings())
        {
            var netSettings = model.GetNETSettings();

            if (doc.Root!.Attribute("Sdk")!.Value != netSettings.SDK().Value)
            {
                doc.Root.Attribute("Sdk")!.Value = netSettings.SDK().Value;
                hasChange = true;
            }

            hasChange |= SyncProperty(doc, "OutputType", netSettings.OutputType().Value switch
            {
                "Class Library" => "Library",
                "Console Application" => "Exe",
                "Windows Application" => "WinExe",
                _ => null
            });
            hasChange |= SyncProperty(doc, "AzureFunctionsVersion", netSettings.AzureFunctionsVersion().Value, true);
            hasChange |= SyncProperty(doc, "Configurations", netSettings.Configurations());
            hasChange |= SyncProperty(doc, "RuntimeIdentifiers", netSettings.RuntimeIdentifiers());
            hasChange |= SyncProperty(doc, "UserSecretsId", netSettings.UserSecretsId());
            hasChange |= SyncProperty(doc, "RootNamespace", netSettings.RootNamespace());
            hasChange |= SyncProperty(doc, "AssemblyName", netSettings.AssemblyName());
            hasChange |= SyncManageableBooleanProperty(doc, "ImplicitUsings", netSettings.ImplicitUsings().Value);
            hasChange |= SyncManageableBooleanProperty(doc, "GenerateRuntimeConfigurationFiles", netSettings.GenerateRuntimeConfigurationFiles().Value);
            hasChange |= SyncManageableBooleanProperty(doc, "GenerateDocumentationFile", netSettings.GenerateDocumentationFile().Value);

            if (netSettings.SuppressWarnings() is null or "$(NoWarn)")
            {
                hasChange |= SyncProperty(doc, "NoWarn", null, removeIfNullOrEmpty: true);
            }
            else
            {
                hasChange |= SyncProperty(doc, "NoWarn", netSettings.SuppressWarnings());
            }

            hasChange |= SyncUsings(doc, model);
        }

        var projectOptions = project.GetCSharpProjectOptions();
        if (projectOptions != null)
        {
            hasChange |= SyncProperty(
                doc: doc,
                propertyName: "LangVersion",
                value: projectOptions.LanguageVersion().IsDefault()
                    ? null
                    : projectOptions.LanguageVersion().Value,
                removeIfNullOrEmpty: true);

            if (projectOptions.Nullable()?.Value == "(unspecified)")
            {
                hasChange |= SyncProperty(doc, "Nullable", null, removeIfNullOrEmpty: true);
            }
            else if (!string.IsNullOrWhiteSpace(projectOptions.Nullable()?.Value))
            {
                hasChange |= SyncProperty(doc, "Nullable", projectOptions.Nullable().Value);
            }
            else if (projectOptions.NullableEnabled())
            {
                // NullableEnabled() was the old property which is just a checkbox, we fall
                // back to it if Nullable() is unset.
                hasChange |= SyncProperty(doc, "Nullable", "enable");
            }
        }

        if (_moduleRequestedProperties.Count == 0)
        {
            return hasChange;
        }

        foreach (var property in _moduleRequestedProperties)
        {
            hasChange |= AddProperty(doc, property.Key, property.Value);
        }

        return hasChange;
    }

    private static bool SyncUsings(XDocument doc, CSharpProjectNETModel model)
    {
        var implicitUsings = model.CustomImplicitUsings?.ImplicitUsings?.Select(x => x.Name).ToArray() ?? [];
        if (implicitUsings.Length == 0)
        {
            return false;
        }

        var hasChange = false;

        var usingGroup = GetUsingItemGroup(doc);
        foreach (var implicitUsing in implicitUsings)
        {
            if (usingGroup.Elements().Any(x => x.Name == "Using" && x.Attribute("Include")?.Value == implicitUsing))
            {
                continue;
            }

            usingGroup.Add(new XElement("Using", new XAttribute("Include", implicitUsing)));
            hasChange = true;
        }

        // Sort alphabetically:
        if (hasChange)
        {
            var elements = usingGroup.Elements().OrderBy(x => x.Name).ThenBy(x => x.Attribute("Include")?.Value).Cast<object>().ToArray();
            usingGroup.RemoveAll();
            usingGroup.Add(elements);
        }

        return hasChange;
    }

    /// <summary>
    /// For when <paramref name="value"/> is one of the following:
    /// <list type="table">
    /// <item>
    /// <term><see langword="null"/></term>
    /// <description>The property's value us "unmanaged" by Intent and should not be changed, added, or removed.</description>
    /// </item>
    /// <item>
    /// <term>"(unspecified)"</term>
    /// <description>The property should be removed from the <c>.csproj</c> file.</description>
    /// </item>
    /// <item>
    /// <term><see langword="false"/> / <c>disable</c></term>
    /// <description>The property's value should be set to <c>false</c>. / <c>disable</c></description>
    /// </item>
    /// <item>
    /// <term><see langword="true"/> / <c>enable</c></term>
    /// <description>The property's value should be set to <c>true</c> / <c>enable</c>.</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <returns>True if there was a change.</returns>
    private static bool SyncManageableBooleanProperty(XDocument doc, string propertyName, string value)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            value is not "(unspecified)" &&
            value is not "enable" &&
            value is not "disable" &&
            value is not "true" &&
            value is not "false")
        {
            throw new ArgumentOutOfRangeException(nameof(value), value);
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return SyncProperty(doc, propertyName, value == "(unspecified)" ? null : value);
    }

    /// <returns>True if there was a change.</returns>
    private static bool SyncProperty(XDocument doc, string propertyName, string value, bool removeIfNullOrEmpty = false)
    {
        var element = GetPropertyGroupElement(doc, propertyName);
        if (string.IsNullOrWhiteSpace(value))
        {
            if (!removeIfNullOrEmpty ||
                element == null)
            {
                return false;
            }

            element.Remove();
            return true;
        }

        if (element == null)
        {
            var propertyGroupElement = GetPropertyGroupElement(doc, "TargetFramework")?.Parent ??
                                       GetPropertyGroupElement(doc, "TargetFrameworks")?.Parent;
            if (propertyGroupElement == null)
            {
                throw new Exception("Could not determine target property group element.");
            }

            element = new XElement(propertyName);
            propertyGroupElement.Add(element);
        }

        if (element.Value == value)
        {
            return false;
        }

        element.Value = value;
        return true;
    }

    /// <returns>True if there was a change.</returns>
    private static bool AddProperty(XDocument doc, string propertyName, string value)
    {
        var element = GetPropertyGroupElement(doc, propertyName);
        if (element != null)
        {
            return false;
        }

        var propertyGroupElement = GetPropertyGroupElement(doc, "TargetFramework")?.Parent ??
                                   GetPropertyGroupElement(doc, "TargetFrameworks")?.Parent;
        if (propertyGroupElement == null)
        {
            throw new Exception("Could not determine target property group element.");
        }

        element = new XElement(propertyName);
        propertyGroupElement.Add(element);
        element.Value = value;

        return true;
    }


    /// <returns>True if there was a change.</returns>
    private bool SyncFrameworks(XDocument doc)
    {
        var targetFrameworks = GetTargetFrameworks().ToArray();
        if (targetFrameworks.Length == 1 && targetFrameworks[0] == "unspecified")
        {
            // User has chosen "(unspecified)" in the Visual Studio designer, useful for
            // scenarios like when a "Directory.Build.props" is being used to set the
            // value.
            return false;
        }

        var element = GetPropertyGroupElement(doc, "TargetFramework") ??
                      GetPropertyGroupElement(doc, "TargetFrameworks");
        if (element == null)
        {
            return false;
        }

        var elementValue = string.Join(";", targetFrameworks.OrderBy(x => x));
        if (element.Value == elementValue)
        {
            return false;
        }

        var elementName = targetFrameworks.Length == 1
            ? "TargetFramework"
            : "TargetFrameworks";

        element.ReplaceWith(XElement.Parse($"<{elementName}>{elementValue}</{elementName}>"));

        return true;
    }

    private void HandleAddUserSecretsEvent(AddUserSecretsEvent @event)
    {
        if (@event.Target.Id != Project.Id || @event.SecretsToAdd == null)
        {
            return;
        }

        var project = ((IVisualStudioProjectTemplate)this).Project;
        if (project is not CSharpProjectNETModel model ||
            !model.HasNETSettings() ||
            !string.IsNullOrEmpty(model.GetNETSettings().UserSecretsId()))
        {
            return;
        }

        CreateUserSecretsFile(@event);
    }

    private void CreateUserSecretsFile(AddUserSecretsEvent @event)
    {
        var userSecretsId = Guid.NewGuid().ToString();
        _moduleRequestedProperties.Add("UserSecretsId", userSecretsId);

        var directory = GetUserSecretsDirectoryName(userSecretsId);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(Path.Combine(directory, "secrets.json"), JsonSerializer.Serialize(@event.SecretsToAdd));
    }

    private static string GetUserSecretsDirectoryName(string userSecretsId)
    {
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var userSecretsDir = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(appDataDir, "Microsoft", "UserSecrets", userSecretsId)
            : Path.Combine(homeDir, ".microsoft", "usersecrets", userSecretsId);
        return userSecretsDir;
    }

    private void HandleAddProjectPropertyEvent(AddProjectPropertyEvent @event)
    {
        if (@event.Target.Id != Project.Id)
        {
            return;
        }
        _moduleRequestedProperties[@event.PropertyName] = @event.PropertyValue;
    }

    private static XElement GetPropertyGroupElement(XDocument doc, string name)
    {
        var (prefix, namespaceManager, _) = doc.GetNamespaceManager();

        return doc.XPathSelectElement($"/{prefix}:Project/{prefix}:PropertyGroup/{prefix}:{name}", namespaceManager);
    }

    private static XElement GetUsingItemGroup(XDocument doc)
    {
        var (prefix, namespaceManager, _) = doc.GetNamespaceManager();

        var usingGroup = doc.XPathSelectElements($"/{prefix}:Project/{prefix}:ItemGroup", namespaceManager)
            .FirstOrDefault(x => x.Descendants("Using").Any());
        if (usingGroup != null)
        {
            return usingGroup;
        }

        usingGroup = new XElement("ItemGroup");

        doc.Elements().First().Add(usingGroup);

        return usingGroup;
    }

    IEnumerable<string> IHasGlobalUsings.GetGlobalUsings()
    {
        var project = ((IVisualStudioProjectTemplate)this).Project;
        var (implicitUsingsIsEnabled, sdk, customImplicitUsings) = GetSettings(project);

        foreach (var customImplicitUsing in customImplicitUsings)
        {
            yield return customImplicitUsing;
        }

        if (TryGetExistingFileContent(out var content))
        {
            var doc = XDocument.Parse(content);
            if (implicitUsingsIsEnabled == null)
            {
                var implicitUsingsValue = doc.Root?.Descendants("ImplicitUsings").FirstOrDefault()?.Value;
                if (string.Equals("enable", implicitUsingsValue, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals("true", implicitUsingsValue, StringComparison.OrdinalIgnoreCase))
                {
                    implicitUsingsIsEnabled = true;
                }
            }

            sdk ??= doc.Root?.Attribute("Sdk")?.Value;

            var usings = doc.Root!
                .Descendants("Using")
                .Select(x => x.Attribute("Include")?.Value)
                .Where(x => x != null);

            foreach (var @using in usings)
            {
                yield return @using;
            }
        }

        if (implicitUsingsIsEnabled == true)
        {
            var implicitUsings = sdk switch
            {
                "Microsoft.NET.Sdk" => ImplicitUsings.ForSdk,
                "Microsoft.NET.Sdk.BlazorWebAssembly" => ImplicitUsings.ForBlazorWebAssemblySdk,
                "Microsoft.NET.Sdk.Web" => ImplicitUsings.ForWebSdk,
                "Microsoft.NET.Sdk.Worker" => ImplicitUsings.ForWorkerSdk,
                _ => Enumerable.Empty<string>()
            };

            foreach (var implicitUsing in implicitUsings)
            {
                yield return implicitUsing;
            }
        }

        yield break;

        static (bool? ImplicitUsingsIsEnabled, string Sdk, IEnumerable<string> CustomImplicitUsings) GetSettings(IVisualStudioProject project)
        {
            if (project.HasNETCoreSettings())
            {
                var settings = project.GetNETCoreSettings();

                var implicitUsings = settings.ImplicitUsings();
                if (implicitUsings.IsEnable())
                {
                    return (true, null, []);
                }

                if (implicitUsings.IsDisable())
                {
                    return (false, null, []);
                }

                return (null, null, []);
            }

            if (project is CSharpProjectNETModel model &&
                model.HasNETSettings())
            {
                var settings = model.GetNETSettings();
                var sdk = settings.SDK().Value;
                var customImplicitUsings = model.CustomImplicitUsings?.ImplicitUsings?.Select(x => x.Name) ?? [];

                var implicitUsings = settings.ImplicitUsings();
                if (implicitUsings.IsEnable())
                {
                    return (true, sdk, customImplicitUsings);
                }

                if (implicitUsings.IsDisable())
                {
                    return (false, sdk, customImplicitUsings);
                }

                return (null, sdk, customImplicitUsings);
            }

            return (null, null, []);
        }
    }
}