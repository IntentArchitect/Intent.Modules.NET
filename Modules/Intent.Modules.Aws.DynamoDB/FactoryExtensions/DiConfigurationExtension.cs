using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Modules.Aws.DynamoDB.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DiConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Aws.DynamoDB.DiConfigurationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            ConfigureStartupDi(application);
            ConfigureInfrastructureDi(application);
        }

        private static void ConfigureStartupDi(IApplication application)
        {
            var appStartup = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            appStartup.CSharpFile.OnBuild(_ =>
            {
                appStartup.StartupFile.AddAppConfiguration(context =>
                {
                    var @if = new CSharpIfStatement($"{context.Env}.IsDevelopment()");

                    @if.AddStatement($"using var scope = {context.App}.Services.CreateScope();");
                    @if.AddStatement($"var client = scope.ServiceProvider.GetRequiredService<{appStartup.UseType("Amazon.DynamoDBv2.IAmazonDynamoDB")}>();");
                    @if.AddStatement($"{appStartup.GetDynamoDBTableInitializerName()}.Initialize(client).GetAwaiter().GetResult();");

                    return @if;
                });
            });
        }

        private static void ConfigureInfrastructureDi(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Aws.Common.AwsConfiguration");

            template.CSharpFile.OnBuild(file =>
            {
                const string appSetting = "AWS:DynamoDB:UseLocalEmulator";
                ((IntentTemplateBase)template).ApplyAppSetting(appSetting, true);

                file.AddUsing("Amazon.DynamoDBv2");
                file.AddUsing("Amazon.DynamoDBv2.DataModel");
                file.AddUsing("Microsoft.Extensions.Configuration");
                file.AddUsing("Microsoft.Extensions.DependencyInjection");

                var method = file.Classes.SelectMany(x => x.Methods).Single(x => x.Name == "ConfigureAws");
                method.AddIfStatement($"configuration.GetValue<bool>(\"{appSetting}\")", @if =>
                {
                    @if.AddInvocationStatement("services.AddSingleton<IAmazonDynamoDB>", invocation =>
                    {
                        invocation.AddArgument(new CSharpLambdaBlock("_"), lambda =>
                        {
                            lambda.AddObjectInitializerBlock("var clientConfig = new AmazonDynamoDBConfig", initBlock =>
                            {
                                initBlock.AddInitStatement("ServiceURL", "\"http://localhost:8000\"");
                                initBlock.AddInitStatement("UseHttp", "true");
                                initBlock.WithSemicolon();
                            });

                            lambda.AddStatement("return new AmazonDynamoDBClient(\"na\", \"na\", clientConfig);", s => s.SeparatedFromPrevious());
                        });
                    });
                });

                method.AddElseStatement(@else =>
                {
                    @else.AddStatement("services.AddAWSService<IAmazonDynamoDB>();");
                });

                method.AddInvocationStatement("services.AddSingleton<IDynamoDBContext>", invocation =>
                {
                    invocation.SeparatedFromPrevious();
                    invocation.AddArgument(new CSharpLambdaBlock("sp"), lambda =>
                    {
                        lambda.AddStatement("var client = sp.GetRequiredService<IAmazonDynamoDB>();");
                        lambda.AddStatement("return new DynamoDBContextBuilder().WithDynamoDBClient(() => client).Build();");
                    });
                });
            });
        }
    }
}