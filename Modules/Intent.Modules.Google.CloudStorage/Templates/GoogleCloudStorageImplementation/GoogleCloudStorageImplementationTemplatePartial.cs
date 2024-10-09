using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Google.CloudStorage.Templates.GoogleCloudStorageImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GoogleCloudStorageImplementationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.Google.CloudStorage.GoogleCloudStorageImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GoogleCloudStorageImplementationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.GoogleCloudStorageV1(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"GoogleCloudStorage", @class =>
                {
                    @class.ImplementsInterface(this.GetCloudStorageInterfaceName());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(UseType("Google.Cloud.Storage.V1.StorageClient"), "client", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod(UseType($"System.Threading.Tasks.Task<{UseType("System.Uri")}>"), "GetAsync", method =>
                    {
                        method.Async();

                        method.AddParameter("string", "bucketName")
                            .AddParameter("string", "objectName")
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                            {
                                cancelTokenParam.WithDefaultValue("default");
                            });

                        method.AddObjectInitStatement("var credential", $"await {UseType("Google.Apis.Auth.OAuth2.GoogleCredential")}.GetApplicationDefaultAsync(cancellationToken);");
                        method.AddObjectInitStatement($"{UseType("Google.Cloud.Storage.V1.UrlSigner")} urlSigner", "UrlSigner.FromCredential(credential);");
                        method.AddObjectInitStatement("var url", $"await urlSigner.SignAsync(bucketName, objectName, TimeSpan.FromMinutes(1));");
                        method.AddReturn("new Uri(url)");
                    });

                    @class.AddMethod(UseType($"System.Collections.Generic.IAsyncEnumerable<{UseType("System.Uri")}>"), "ListAsync", method =>
                    {
                        method.AddParameter("string", "bucketName")
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                            {
                                cancelTokenParam.AddAttribute(UseType("System.Runtime.CompilerServices.EnumeratorCancellation"));
                                cancelTokenParam.WithDefaultValue("default");
                            });

                        method.AddObjectInitStatement("var objects", "_client.ListObjectsAsync(bucketName).AsRawResponses();");
                        method.AddForEachStatement("@object", "objects", itemIteration =>
                        {
                            itemIteration.Await();
                            itemIteration.AddForEachStatement("gcObject", "@object.Items", gcItemIteration =>
                            {
                                gcItemIteration.AddStatement("yield return new Uri(gcObject.SelfLink);");
                            });
                        });
                    });

                    @class.AddMethod(UseType($"System.IO.Stream"), "DownloadAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "bucketName")
                            .AddParameter("string", "objectName")
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                            {
                                cancelTokenParam.WithDefaultValue("default");
                            });

                        method.AddObjectInitStatement("var returnStream", $"new {UseType("System.IO.MemoryStream();")}");
                        method.AddInvocationStatement("var @object = await _client.DownloadObjectAsync", downloadInvoc =>
                        {
                            downloadInvoc.AddArgument("bucketName");
                            downloadInvoc.AddArgument("objectName");
                            downloadInvoc.AddArgument("returnStream");
                            downloadInvoc.AddArgument(new CSharpArgument("cancellationToken"), tokenArg =>
                            {
                                tokenArg.WithName("cancellationToken");
                            });
                        });

                        method.AddStatement("returnStream.Position = 0;");
                        method.AddReturn("returnStream");
                    });

                    @class.AddMethod(UseType($"System.Threading.Tasks.Task<{UseType("System.Uri")}>"), "UploadAsync", method =>
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

                        method.AddInvocationStatement("var uploadResponse = await _client.UploadObjectAsync", uploadInvoc =>
                        {
                            uploadInvoc.AddArgument("bucketName")
                                .AddArgument("objectName")
                                .AddArgument("contentType")
                                .AddArgument("dataStream")
                                .AddArgument(new CSharpArgument("cancellationToken"), tokenArg =>
                                {
                                    tokenArg.WithName("cancellationToken");
                                });
                        });

                        method.AddReturn("new Uri(uploadResponse.SelfLink)");
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