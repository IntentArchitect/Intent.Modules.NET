using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.MongoDb.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.MongoDbMultiTenantConnectionFactory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbMultiTenantConnectionFactoryTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoDbMultiTenantConnectionFactory";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbMultiTenantConnectionFactoryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("Finbuckle.MultiTenant")
                .AddUsing("MongoDB.Driver")
                .AddClass($"MongoDbMultiTenantConnectionFactory", @class =>
                {
                    @class.AddField("ConcurrentDictionary<string, CacheableMongoDb>", "_connectionCache", f => f.PrivateReadOnly());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddStatement("_connectionCache = new ConcurrentDictionary<string, CacheableMongoDb>();");
                    });

                    @class.AddMethod("IMongoDatabase", "GetConnection", method =>
                    {
                        method.AddParameter("string", "connectionString");
                        method.AddStatement("var db = new MongoClient(connectionString).GetDatabase(new MongoUrl(connectionString).DatabaseName);");
                        method.AddStatement("return _connectionCache.GetOrAdd(connectionString, id => new CacheableMongoDb(db)).GetDatabase();");
                    });

                    @class.AddNestedClass("CacheableMongoDb", @class =>
                    {
                        @class.Private();
                        @class.AddConstructor(ctor =>
                        {
                            ctor.AddParameter("IMongoDatabase", "underlyingConnection", p => p.IntroduceProperty(p => p.ReadOnly()));
                        });

                        @class.AddProperty("IMongoClient", "Client", p => p.ReadOnly().Getter.WithExpressionImplementation("UnderlyingConnection.Client"));

                        @class.AddMethod("IMongoDatabase", "GetDatabase", method => method.AddStatement("return UnderlyingConnection;"));
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.Settings.GetMultitenancySettings()?.MongoDbDataIsolation()?.IsSeparateDatabase() == true;
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