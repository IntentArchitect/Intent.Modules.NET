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

namespace Intent.Modules.AspNetCore.Controllers.Templates.DownloadFile
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DownloadFileTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Controllers.DownloadFile";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DownloadFileTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.IO")
                .AddClass($"DownloadFile", @class =>
                {
                    @class.AddConstructor(ctor =>
                    {
                        ctor
                            .AddParameter("Stream", "stream")
                            .AddParameter("string?", "filename", p => p.WithDefaultValue("default"))
                            .AddParameter("string", "contentType", p => p.WithDefaultValue("\"application/octet-stream\""))
                            ;
                        ctor
                            .AddStatement("Stream = stream;")
                            .AddStatement("Filename = filename;")
                            .AddStatement("ContentType = contentType;")
                            ;
                    });
                    @class.AddConstructor(ctor =>
                    {
                        ctor
                            .AddParameter("byte[]", "content")
                            .AddParameter("string?", "filename", p => p.WithDefaultValue("default"))
                            .AddParameter("string", "contentType", p => p.WithDefaultValue("\"application/octet-stream\""))
                            ;
                        ctor
                            .AddStatement("Content = content;")
                            .AddStatement("Filename = filename;")
                            .AddStatement("ContentType = contentType;")
                            ;
                    });
                    @class.AddProperty("string?", "Filename", p => p.ReadOnly());
                    @class.AddProperty("string", "ContentType", p => p.ReadOnly());
                    @class.AddProperty("Stream?", "Stream", p => p.ReadOnly());
                    @class.AddProperty("byte[]?", "Content", p => p.ReadOnly());
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