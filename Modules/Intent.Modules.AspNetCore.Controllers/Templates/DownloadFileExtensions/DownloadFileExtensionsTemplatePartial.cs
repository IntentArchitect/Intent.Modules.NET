using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.DownloadFileExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DownloadFileExtensionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Controllers.DownloadFileExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DownloadFileExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .AddClass($"DownloadFileExtensions", @class =>
                {
                    @class
                        .Internal()
                        .Static();
                    @class.AddMethod("FileResult", "ToFile", method =>
                    {
                        method
                            .Static()
                            .AddParameter(this.GetDownloadFileName(), "fileTransfer", p => p.WithThisModifier())
                            .AddParameter("string?", "contentType", p => p.WithDefaultValue("default"))
                            .AddParameter("string?", "filename", p => p.WithDefaultValue("default"))
                            .AddStatements(@"
            if (fileTransfer.Content != null)
                return new FileContentResult(fileTransfer.Content, contentType ?? fileTransfer.ContentType) { FileDownloadName = filename?? fileTransfer.Filename } ;
            return new FileStreamResult(fileTransfer.Stream!, contentType ?? fileTransfer.ContentType!){ FileDownloadName = filename ?? fileTransfer.Filename };
".ConvertToStatements());
                        ;
                    });
                });
        }
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && FileTransferHelper.NeedsFileUploadInfrastructure(ExecutionContext.MetadataManager, ExecutionContext.GetApplicationConfig().Id);
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}