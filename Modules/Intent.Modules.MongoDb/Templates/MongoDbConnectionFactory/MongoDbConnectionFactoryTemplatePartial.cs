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

namespace Intent.Modules.MongoDb.Templates.MongoDbConnectionFactory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbConnectionFactoryTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoDbConnectionFactory";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbConnectionFactoryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("Finbuckle.MultiTenant")
                .AddUsing("MongoDB.Driver")
                .AddUsing("MongoFramework")
                .AddUsing("MongoFramework.Infrastructure.Diagnostics")
                .AddClass($"MongoDbConnectionFactory", @class =>
                {
                    @class.ImplementsInterface("IDisposable");
                    @class.AddField("ConcurrentDictionary<string, CacheableMongoDbConnection>", "_connectionCache", f => f.PrivateReadOnly());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddStatement("_connectionCache = new ConcurrentDictionary<string, CacheableMongoDbConnection>();");
                    });

                    @class.AddMethod("IMongoDbConnection", "GetConnection", method =>
                    {
                        method.AddParameter("ITenantInfo", "tenant");
                        method.AddIfStatement("tenant is null || tenant.Identifier is null", stmt => stmt.AddStatement("throw new ArgumentException(\"Tenant not supplied.\", nameof(tenant));"));
                        method.AddStatement("return _connectionCache.GetOrAdd(tenant.Identifier, id => new CacheableMongoDbConnection( MongoDbConnection.FromConnectionString(tenant.ConnectionString)));");
                    });

                    @class.AddMethod("void", "Dispose", method =>
                    {
                        method.AddForEachStatement("connection", "_connectionCache.Values", stmt => stmt.AddStatement("connection.UnderlyingConnection.Dispose();"));
                    });

                    @class.AddNestedClass("CacheableMongoDbConnection", @class =>
                    {
                        @class
                            .Private()
                            .ImplementsInterface("IMongoDbConnection");
                        @class.AddConstructor(ctor =>
                        {
                            ctor.AddParameter("IMongoDbConnection", "underlyingConnection", p => p.IntroduceProperty(p => p.ReadOnly()));
                        });

                        @class.AddProperty("IMongoClient", "Client", p => p.ReadOnly().Getter.WithExpressionImplementation("UnderlyingConnection.Client"));
                        @class.AddProperty("IDiagnosticListener", "DiagnosticListener", p =>
                        {
                            p.Getter.WithExpressionImplementation("UnderlyingConnection.DiagnosticListener");
                            p.Setter.WithExpressionImplementation("UnderlyingConnection.DiagnosticListener = value");
                        });
                        @class.AddMethod("IMongoDatabase", "GetDatabase", method => method.AddStatement("return UnderlyingConnection.GetDatabase();"));
                        @class.AddMethod("void", "Dispose", method =>
                        {
                            method.AddStatement("//DI is forcing `IMongoDbConnection` to make these scoped, but we want to cache these per tenant");
                            method.AddStatement("//This stops the container from disposing this on every request");
                        });
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && DocumentTemplateHelpers.IsSeparateDatabaseMultiTenancy(ExecutionContext.Settings);
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