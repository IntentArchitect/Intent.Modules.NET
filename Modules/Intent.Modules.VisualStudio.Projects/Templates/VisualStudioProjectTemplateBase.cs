using System.Collections.Generic;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates
{
    public abstract class VisualStudioProjectTemplateBase<TModel> : IntentFileTemplateBase<TModel>, IVisualStudioProjectTemplate
        where TModel : IVisualStudioProject
    {
        private string _fileContent;
        protected VisualStudioProjectTemplateBase(string templateId, IOutputTarget outputTarget, TModel model) : base(templateId, outputTarget, model)
        {
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
            var targetContent = XDocument.Parse(content).ToString();
            var existingContent = LoadContent();

            if (existingContent == targetContent)
            {
                return;
            }

            var change = ExecutionContext.ChangeManager.FindChange(FilePath);
            if (change != null)
            {
                change.ChangeContent(content);
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
                : base.RunTemplate();

            content = ApplyAdditionalTransforms(content);

            return hadExistingContent && XmlHelper.IsSemanticallyTheSame(existingFileContent, content)
                ? existingFileContent
                : content;
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
    }
}