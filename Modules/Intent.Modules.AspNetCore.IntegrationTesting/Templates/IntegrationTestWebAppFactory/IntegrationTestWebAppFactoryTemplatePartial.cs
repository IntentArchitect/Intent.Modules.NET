using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.IntegrationTestWebAppFactory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IntegrationTestWebAppFactoryTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.IntegrationTestWebAppFactory";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationTestWebAppFactoryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreMvcTesting(OutputTarget));
            AddNugetDependency(NugetPackages.MicrosoftNETTestSdk(outputTarget));
            AddNugetDependency(NugetPackages.Xunit(outputTarget));
            AddNugetDependency(NugetPackages.XunitRunnerVisualstudio(outputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddAssemblyAttribute("CollectionBehavior", a => a.AddArgument("DisableTestParallelization = true"))
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Hosting")
                .AddUsing("Microsoft.AspNetCore.Mvc.Testing")
                .AddUsing("Microsoft.AspNetCore.TestHost")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddClass($"IntegrationTestWebAppFactory", @class =>
                {
                    var containers = GetRequiredContainers();
                    var template = GetTemplate<IntentTemplateBase>("App.Program");
                    @class.WithBaseType($"WebApplicationFactory<{this.GetTypeName(template)}>");
                    @class.ImplementsInterface("IAsyncLifetime");
                    @class.AddConstructor(ctor =>
                    {
                        foreach (var container in containers)
                        {
                            ctor.AddStatement($"{container.PropertyName} = new {container.TypeName}();");
                        }
                    });

                    foreach (var container in containers)
                    {
                        @class.AddProperty(container.TypeName, container.PropertyName, p => p.ReadOnly());
                    }


                    @class.AddMethod("Task", "InitializeAsync", method =>
                    {
                        method
                            .Async();
                        foreach (var container in containers)
                        {
                            method.AddStatement($"await {container.PropertyName}.InitializeAsync();");
                        }
                    });
                    @class.AddMethod("Task", "DisposeAsync", method =>
                    {
                        method
                            .Async()
                            .IsExplicitImplementationFor("IAsyncLifetime");
                        foreach (var container in containers)
                        {
                            method.AddStatement($"await {container.PropertyName}.DisposeAsync();");
                        }
                    });
                    @class.AddMethod("IHost", "CreateHost", method =>
                    {
                        method
                            .Protected()
                            .Override()
                            .AddParameter("IHostBuilder", "builder")
                            .AddStatement("var result = base.CreateHost(builder);");
                        foreach (var container in containers)
                        {
                            method.AddStatement($"{container.PropertyName}.OnHostCreation(result.Services);");
                        }
                        method.AddStatement("return result;");
                    });
                    @class.AddMethod("void", "ConfigureWebHost", method =>
                    {
                        var servicesLambda = new CSharpLambdaBlock("services");
                        foreach (var container in containers)
                        {
                            servicesLambda.AddStatement($"{container.PropertyName}.ConfigureTestServices(services);");
                        }

                        method
                            .Protected()
                            .Override()
                            .AddParameter("IWebHostBuilder", "builder")
                            .AddStatement(
                                new CSharpInvocationStatement("builder.ConfigureTestServices")
                                    .AddArgument(servicesLambda)
                                );
                    });
                });
        }

        private IEnumerable<RequiredContainer> GetRequiredContainers()
        {
            if (ContainerHelper.RequireCosmosContainer(this))
            {
                yield return new RequiredContainer("CosmosContainerFixture", this.GetCosmosContainerFixtureName());
            }
            if (ContainerHelper.RequireRdbmsEFContainer(this))
            {
                yield return new RequiredContainer("RdbmsFixture", this.GetEFContainerFixtureName());
            }
            if (ContainerHelper.RequireMongoContainer(this))
            {
                yield return new RequiredContainer("MongoDbFixture", this.GetMongoDbContainerFixtureName());
            }
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