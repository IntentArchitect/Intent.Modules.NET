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

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.MongoDbContainerFixture
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbContainerFixtureTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.MongoDbContainerFixture";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbContainerFixtureTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"MongoDbContainerFixture", @class =>
                {
                    AddUsing("Microsoft.Extensions.DependencyInjection");
                    AddUsing("MongoFramework");
                    AddUsing("Testcontainers.MongoDb");
                    AddNugetDependency(NugetPackages.TestcontainersMongoDb(outputTarget));
                    @class.AddField("MongoDbContainer", "_dbContainer", f => f.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddStatements(@"_dbContainer = new MongoDbBuilder()
                          .WithImage(""mongo:latest"")
                          .Build();".ConvertToStatements());
                    });


                    @class.AddMethod("void", "ConfigureTestServices", method =>
                    {
                        method.AddParameter("IServiceCollection", "services");
                        string databaseName = "IntegrationTestDb";
                        method.AddStatement($"string connectionString = _dbContainer.GetConnectionString() + \"{databaseName}?authSource=admin\";");
                        method.AddStatement("services.AddSingleton<IMongoDbConnection>((c) => MongoDbConnection.FromConnectionString(connectionString));");

                        @class.AddMethod("void", "OnHostCreation", method =>
                        {
                            method
                                .AddParameter("IServiceProvider", "services");
                        });

                        @class.AddMethod("void", "InitializeAsync", method =>
                        {
                            method
                                .Async()
                                .AddStatement(@"await _dbContainer.StartAsync();");
                        });

                        @class.AddMethod("Task", "DisposeAsync", method =>
                        {
                            method
                                .Async()
                                .AddStatement("await _dbContainer.StopAsync();");
                        });
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ContainerHelper.RequireMongoContainer(this);
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