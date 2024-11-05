using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Google.CloudStorage.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Common.CSharp.DependencyInjection.ContainerRegistrationRequest;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Google.CloudStorage.Templates.GoogleCloudStorageMultiTenantConnectionFactory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GoogleCloudStorageMultiTenantConnectionFactoryTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Google.CloudStorage.GoogleCloudStorageMultiTenantConnectionFactory";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GoogleCloudStorageMultiTenantConnectionFactoryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("Google.Cloud.Storage.V1")
                .AddUsing("Google.Apis.Auth.OAuth2")
                .AddClass($"GoogleCloudStorageMultiTenantConnectionFactory", @class =>
                {
                    @class.ImplementsInterface("IDisposable");
                    @class.AddField("ConcurrentDictionary<string, StorageClient>", "_connectionCache", f => f.PrivateReadOnly());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddStatement("_connectionCache = new ConcurrentDictionary<string, StorageClient>();");
                    });

                    @class.AddMethod("StorageClient", "GetStorageClient", method =>
                    {
                        method.AddParameter("string", "tenantId");
                        method.AddParameter("string", "connectionJson");
                        method.AddStatement(@"return _connectionCache.GetOrAdd(tenantId, id => 
                    {
                        var googleCredential = GoogleCredential.FromJson(connectionJson);
                        return StorageClient.Create(googleCredential);
                    });");
                    });

                    @class.AddMethod("void", "Dispose", method =>
                    {
                        method.AddForEachStatement("connection", "_connectionCache.Values", stmt => stmt.AddStatement("connection.Dispose();"));
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.Settings.GetMultitenancySettings()?.GoogleCloudStorageDataIsolation()?.IsSeparateStorageAccount() == true;
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