using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.MongoDb.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Finbuckle.Templates.MongoDbMultiTenancyConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
    public partial class MongoDbMultiTenancyConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.MongoDb.Finbuckle.MongoDbMultiTenancyConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbMultiTenancyConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Finbuckle.MultiTenant")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("MongoPerTenantConnection = MongoFramework.MongoPerTenantConnection")
                .AddUsing("IMongoPerTenantConnection = MongoFramework.IMongoPerTenantConnection")
                .AddClass($"MongoDbMultiTenancyConfiguration")
                .OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "AddMongoDbMultiTenancy", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", param => param.WithThisModifier())
                            .AddParameter("IConfiguration", "configuration");
                        method.AddStatement($"services.AddTransient<IMongoDbContext>(x => x.GetService<{this.GetApplicationMongoDbContextName()}>());");
                        method.AddStatement(new CSharpInvocationStatement("services.AddScoped<ApplicationMongoDbContext>")
                            .AddArgument(new CSharpLambdaBlock("provider")
                                .AddStatement(
                                    $@"var tenantInfo = provider.GetService<MongoPerTenantConnection>() ?? throw new MultiTenantException(""Failed to resolve tenant info."");")
                                .AddStatement($@"var dbContext = new ApplicationMongoDbContext(tenantInfo.Url.Url, tenantInfo.Url.DatabaseName);")
                                .AddStatement($@"return dbContext;"))
                            .WithArgumentsOnNewLines());
                        method.AddStatement(new CSharpInvocationStatement($@"services.AddScoped<MongoPerTenantConnection>")
                            .AddArgument(new CSharpLambdaBlock("x")
                                .AddStatement($@"var tenantInfo = x.GetService<ITenantInfo>();")
                                .AddStatement($@"return tenantInfo == null ? default : new MongoPerTenantConnection(tenantInfo);"))
                            .WithArgumentsOnNewLines());
                        method.AddStatement($@"services.AddTransient<IMongoPerTenantConnection>(x => x.GetService<MongoPerTenantConnection>());");
                        method.AddStatement($@"return services;");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddMongoDbMultiTenancy", ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));
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