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
        public const string TemplateId = "Intent.Google.CloudStorage.CloudStorageInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CloudStorageInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())

                .AddInterface($"ICloudStorage", @interface =>
                {
                    @interface.WithComments(
                    [
                        "/// <summary>",
                        "/// A simplified service interface to access Object Storage.",
                        "/// </summary>"
                    ]);

                    @interface.AddMethod(UseType($"System.Threading.Tasks.Task<{UseType("System.Uri")}>"), "GetAsync", method =>
                    {
                        method.AddParameter("string", "bucketName")
                            .AddParameter("string", "objectName")
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                            {
                                cancelTokenParam.WithDefaultValue("default");
                            });

                        method.WithComments(
                    [
                            "/// <summary>",
                            "/// Retrieves the URI of a specific object from a given bucket.",
                            "/// </summary>",
                            "/// <param name=\"bucketName\">The name of the bucket.</param>",
                            "/// <param name=\"objectName\">The name of the object.</param>",
                            "/// <param name=\"cancellationToken\">An optional token to cancel the asynchronous operation.</param>",
                            "/// <returns>The URI of the object.</returns>",
                        ]);
                    });

                    @interface.AddMethod(UseType($"System.Collections.Generic.IAsyncEnumerable<{UseType("System.Uri")}>"), "ListAsync", method =>
                    {
                        method.AddParameter("string", "bucketName")
                            .AddParameter("string?", "prefix", prefix => prefix.WithDefaultValue("null"))
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                            {
                                cancelTokenParam.WithDefaultValue("default");
                            });

                        method.WithComments(
                    [
                            "/// <summary>",
                            "/// Lists the URIs of all objects in a given bucket.",
                            "/// </summary>",
                            "/// <param name=\"bucketName\">The name of the bucket.</param>",
                            "/// <param name=\"prefix\">The prefix to match. Only objects with names that start with this string will be returned. May be null or empty</param>",
                            "/// <param name=\"cancellationToken\">An optional token to cancel the asynchronous operation.</param>",
                            "/// <returns>An async enumerable of object URIs.</returns>",
                        ]);
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

                        method.WithComments(
                    [
                            "/// <summary>",
                            "/// Downloads data from a specific location in object storage.",
                            "/// </summary>",
                            "/// <param name=\"bucketName\">The name of the bucket.</param>",
                            "/// <param name=\"objectName\">The name of the object.</param>",
                            "/// <param name=\"cancellationToken\">An optional token to cancel the asynchronous operation.</param>",
                            "/// <returns>A stream containing the downloaded data.</returns>",
                        ]);
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

                        method.WithComments(
                    [
                            "/// <summary>",
                            "/// Uploads data to a specific location in object storage.",
                            "/// </summary>",
                            "/// <param name=\"bucketName\">The name of the bucket.</param>",
                            "/// <param name=\"objectName\">The name of the object.</param>",
                            "/// <param name=\"dataStream\">The stream of data to upload.</param>",
                            "/// <param name=\"contentType\">The content type of the object. This should be a MIME type. Can be null.</param>",
                            "/// <param name=\"cancellationToken\">An optional token to cancel the asynchronous operation.</param>",
                            "/// <returns>The URI of the uploaded object.</returns>",
                        ]);
                    });

                    @interface.AddMethod(UseType($"{UseType($"System.Collections.Generic.IAsyncEnumerable<{UseType("System.Uri")}>")}"), "BulkUploadAsync", method =>
                    {
                        method.AddParameter("string", "bucketName")
                        .AddParameter(UseType($"System.Collections.Generic.IEnumerable<{this.GetBulkCloudObjectItemName()}>"), "objects")
                         .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                         {
                             cancelTokenParam.WithDefaultValue("default");
                         });

                        method.WithComments(
                    [
                            "/// <summary>",
                            "/// Performs bulk upload of multiple objects to a specific bucket.",
                            "/// </summary>",
                            "/// <param name=\"bucketName\">The name of the bucket.</param>",
                            "/// <param name=\"objects\">The enumerable of bulk object items to upload.</param>",
                            "/// <param name=\"cancellationToken\">An optional token to cancel the asynchronous operation.</param>",
                            "/// <returns>An async enumerable of object URIs for each uploaded object.</returns>",
                        ]);
                    });

                    @interface.AddMethod(UseType("System.Threading.Tasks.Task"), "DeleteAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "bucketName")
                         .AddParameter(UseType("string"), "objectName")
                         .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                         {
                             cancelTokenParam.WithDefaultValue("default");
                         });

                        method.WithComments(
                    [
                            "/// <summary>",
                            "/// Deletes a specific object in a given bucket.",
                            "/// </summary>",
                            "/// <param name=\"bucketName\">The name of the bucket.</param>",
                            "/// <param name=\"objectName\">The name of the object.</param>",
                            "/// <param name=\"cancellationToken\">An optional token to cancel the asynchronous operation.</param>"
                        ]);
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