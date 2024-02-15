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

namespace Intent.Modules.AspNetCore.Controllers.Templates.UploadFileFactory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class UploadFileFactoryTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Controllers.UploadFileFactory";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UploadFileFactoryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddClass($"UploadFileFactory", @class =>
                {
                    @class
                        .Internal()
                        .Static()
                        .AddMethod(UseType(this.GetUploadFileName()), "Create", method =>
                        {
                            method
                                .Static()
                                .AddParameter("HttpRequest", "request")
                                .AddParameter("string?", "filename", p => p.WithDefaultValue("default"))
                                .AddStatements(@"
        if (request.ContentType != null &&
                    (request.ContentType == ""application/x-www-form-urlencoded"" || request.ContentType.StartsWith(""multipart/form-data"")) &&
                    request.Form.Files.Any())
        {
            var file = request.Form.Files[0];

            if (file == null || file.Length == 0)
                throw new ArgumentException(""File is empty"");
            return new UploadFile(file.OpenReadStream(), file.FileName, file.ContentType);
        }
        else
        {
            return new UploadFile(request.Body, filename, request.ContentType ?? ""application/octet-stream"");
        }
".ConvertToStatements());
                            ;
                        });
                    ;
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