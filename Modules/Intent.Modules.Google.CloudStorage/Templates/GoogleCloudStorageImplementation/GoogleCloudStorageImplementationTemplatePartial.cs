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
                        ctor.AddParameter(UseType("Microsoft.Extensions.Configuration.IConfiguration"), "configuration", param =>
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

                        method.AddObjectInitStatement($"var urlSigner", "_client.CreateUrlSigner();");
                        method.AddObjectInitStatement("var url", new CSharpInvocationStatement("await urlSigner.SignAsync")
                            .AddArgument("bucketName")
                            .AddArgument("objectName")
                            .AddArgument("_configuration.GetValue<TimeSpan?>(\"GCP:PreSignedUrlExpiry\") ?? TimeSpan.FromMinutes(5)")
                            .AddArgument(new CSharpArgument("cancellationToken"), tokenArg =>
                            {
                                tokenArg.WithName("cancellationToken");
                            }).AddInvocation("ConfigureAwait", ca => ca.AddArgument("false")));
                        method.AddReturn("new Uri(url)");
                    });

                    @class.AddMethod(UseType($"System.Collections.Generic.IAsyncEnumerable<{UseType("System.Uri")}>"), "ListAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "bucketName")
                            .AddParameter("string?", "prefix", prefix => prefix.WithDefaultValue("null"))
                            .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                            {
                                cancelTokenParam.AddAttribute(UseType("System.Runtime.CompilerServices.EnumeratorCancellation"));
                                cancelTokenParam.WithDefaultValue("default");
                            });

                        method.AddObjectInitStatement("var objects", new CSharpInvocationStatement("_client.ListObjectsAsync(bucketName, prefix).AsRawResponses")
                            .AddInvocation("ConfigureAwait", ca => ca.AddArgument("false")));
                        method.AddForEachStatement("@object", "objects", itemIteration =>
                        {
                            itemIteration.Await();
                            itemIteration.AddForEachStatement("gcObject", "@object.Items", gcItemIteration =>
                            {
                                gcItemIteration.AddInvocationStatement("yield return await GetAsync", returnStatement =>
                                {
                                    returnStatement.AddArgument("bucketName")
                                        .AddArgument("gcObject.Name")
                                        .AddArgument("cancellationToken");

                                    returnStatement.AddInvocation("ConfigureAwait", ca => ca.AddArgument("false"));
                                });
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
                        method.AddInvocationStatement("_ = await _client.DownloadObjectAsync", downloadInvoc =>
                        {
                            downloadInvoc.AddArgument("bucketName");
                            downloadInvoc.AddArgument("objectName");
                            downloadInvoc.AddArgument("returnStream");
                            downloadInvoc.AddArgument(new CSharpArgument("cancellationToken"), tokenArg =>
                            {
                                tokenArg.WithName("cancellationToken");
                            });
                            downloadInvoc.AddInvocation("ConfigureAwait", ca => ca.AddArgument("false"));
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

                        method.AddInvocationStatement("_ = await _client.UploadObjectAsync", uploadInvoc =>
                        {
                            uploadInvoc.AddArgument("bucketName")
                                .AddArgument("objectName")
                                .AddArgument("contentType")
                                .AddArgument("dataStream")
                                .AddArgument(new CSharpArgument("cancellationToken"), tokenArg =>
                                {
                                    tokenArg.WithName("cancellationToken");
                                })
                                .AddInvocation("ConfigureAwait", ca => ca.AddArgument("false"));
                        });

                        method.AddInvocationStatement(" return await GetAsync", returnStatement =>
                        {
                            returnStatement.AddArgument("bucketName")
                                .AddArgument("objectName")
                                .AddArgument("cancellationToken")
                                .AddInvocation("ConfigureAwait", ca => ca.AddArgument("false"));
                        });
                    });

                    @class.AddMethod(UseType($"{UseType($"System.Collections.Generic.IAsyncEnumerable<{UseType("System.Uri")}>")}"), "BulkUploadAsync", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "bucketName")
                         .AddParameter(UseType($"System.Collections.Generic.IEnumerable<{this.GetBulkCloudObjectItemName()}>"), "objects")
                         .AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", cancelTokenParam =>
                         {
                             cancelTokenParam.AddAttribute(UseType("System.Runtime.CompilerServices.EnumeratorCancellation"));
                             cancelTokenParam.WithDefaultValue("default");
                         });

                        method.AddForEachStatement("cloudObject", "objects", loopConfig =>
                        {
                            loopConfig.AddInvocationStatement("yield return await UploadAsync", uploadConfig =>
                            {
                                uploadConfig.AddArgument("bucketName")
                                    .AddArgument("cloudObject.Name")
                                    .AddArgument("cloudObject.DataStream")
                                    .AddArgument("cloudObject.ContentType")
                                    .AddArgument(new CSharpArgument("cancellationToken"), tokenArg =>
                                     {
                                         tokenArg.WithName("cancellationToken");
                                     });
                                uploadConfig.AddInvocation("ConfigureAwait", ca => ca.AddArgument("false"));
                            });
                        });
                    });

                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("GCP",
                new { PreSignedUrlExpiry = TimeSpan.FromMinutes(5), CloudStorageAuthFileLocation = "" }));
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