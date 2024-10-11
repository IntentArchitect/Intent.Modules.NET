using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Google.CloudStorage.Templates.CloudStorageInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CloudStorageInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.Google.CloudStorage.CloudStorageInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CloudStorageInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                
                .AddInterface($"ICloudStorage", @interface =>
                {
                    @interface.AddMethod(UseType($"System.Threading.Tasks.Task<{UseType("System.Uri")}>"), "GetAsync", method =>
                    {
                        method.AddParameter("string", "bucketName")
                            .AddParameter("string", "objectName")
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                            {
                                cancelTokenParam.WithDefaultValue("default");
                            });
                    });

                    @interface.AddMethod(UseType($"System.Collections.Generic.IAsyncEnumerable<{UseType("System.Uri")}>"), "ListAsync", method =>
                    {
                        method.AddParameter("string", "bucketName")
                            .AddParameter("string?", "prefix", prefix => prefix.WithDefaultValue("null"))
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                            {
                                cancelTokenParam.WithDefaultValue("default");
                            });
                    });

                    @interface.AddMethod(UseType($"System.IO.Stream"), "DownloadAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "bucketName")
                            .AddParameter("string", "objectName")
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                            {
                                cancelTokenParam.WithDefaultValue("default");
                            });
                    });

                    @interface.AddMethod(UseType($"System.Threading.Tasks.Task<{UseType("System.Uri")}>"), "UploadAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "bucketName")
                            .AddParameter("string", "objectName")
                            .AddParameter($"{UseType($"System.IO.Stream")}", "dataStream")
                            .AddParameter("string?", "contentType", ct =>
                            {
                                ct.WithDefaultValue("null");
                            })
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                            {
                                cancelTokenParam.WithDefaultValue("default");
                            });
                    });

                    @interface.AddMethod(UseType($"{UseType($"System.Collections.Generic.IAsyncEnumerable<{UseType("System.Uri")}>")}"), "BulkUploadAsync", method =>
                    {
                        method.AddParameter("string", "bucketName")
                        .AddParameter(UseType($"System.Collections.Generic.IEnumerable<{this.GetBulkCloudObjectItemName()}>"), "objects")
                         .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                         {
                             cancelTokenParam.WithDefaultValue("default");
                         });
                    });
                });
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