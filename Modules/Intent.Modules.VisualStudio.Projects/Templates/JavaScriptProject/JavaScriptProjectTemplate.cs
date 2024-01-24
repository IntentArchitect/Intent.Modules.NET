using System.IO;
using System.Xml;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Templates;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using static Intent.Modules.VisualStudio.Projects.Api.JavaScriptProjectModelStereotypeExtensions;

namespace Intent.Modules.VisualStudio.Projects.Templates.JavaScriptProject
{
    public class JavaScriptProjectTemplate : IntentFileTemplateBase<JavaScriptProjectModel>
    {
        public const string TemplateId = "Intent.VisualStudio.Projects.JavaScriptProject";

        public JavaScriptProjectTemplate(IOutputTarget outputTarget, JavaScriptProjectModel model)
            : base(TemplateId, outputTarget, model)
        {
        }

        public override string TransformText()
        {
            if (!TryGetExistingFileContent(out var content))
            {
                content = null;
            }

            var settings = Model.GetJavaScriptSettings();

            return Generate(content, settings);
        }

        /// <remarks>Internal so that can be unit tested.</remarks>
        internal static string Generate(string content, JavaScriptSettings settings)
        {
            var preserveFormatting = content != null;
            content ??= """
               <Project>
               </Project>
               """;

            var rootElement = ProjectRootElement.Create(
                XmlReader.Create(new StringReader(content)),
                ProjectCollection.GlobalProjectCollection,
                preserveFormatting: preserveFormatting);

            rootElement.Sdk = "Microsoft.VisualStudio.JavaScript.Sdk/0.5.128-alpha";

            if (settings.ShouldRunNpmInstall().IsTrue())
            {
                rootElement.AddProperty("ShouldRunNpmInstall", "true");
            }
            else if (settings.ShouldRunNpmInstall().IsFalse())
            {
                rootElement.AddProperty("ShouldRunNpmInstall", "true");
            }

            if (settings.ShouldRunBuildScript().IsTrue())
            {
                rootElement.AddProperty("ShouldRunBuildScript", "true");
            }
            else if (settings.ShouldRunBuildScript().IsFalse())
            {
                rootElement.AddProperty("ShouldRunBuildScript", "true");
            }

            if (!string.IsNullOrWhiteSpace(settings.BuildCommand()))
            {
                rootElement.AddProperty("BuildCommand", settings.BuildCommand());
            }

            if (!string.IsNullOrWhiteSpace(settings.StartupCommand()))
            {
                rootElement.AddProperty("StartupCommand", settings.StartupCommand());
            }

            if (!string.IsNullOrWhiteSpace(settings.TestCommand()))
            {
                rootElement.AddProperty("TestCommand", settings.TestCommand());
            }

            if (!string.IsNullOrWhiteSpace(settings.CleanCommand()))
            {
                rootElement.AddProperty("CleanCommand", settings.CleanCommand());
            }

            if (!string.IsNullOrWhiteSpace(settings.PublishCommand()))
            {
                rootElement.AddProperty("PublishCommand", settings.PublishCommand());
            }

            return rootElement.RawXml.ReplaceLineEndings();
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: Project.Name,
                fileExtension: "esproj"
            );
        }
    }
}
