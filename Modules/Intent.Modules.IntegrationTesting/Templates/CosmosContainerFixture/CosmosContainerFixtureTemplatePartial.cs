using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IntegrationTesting.Templates.CosmosContainerFixture
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosContainerFixtureTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.IntegrationTesting.CosmosContainerFixture";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosContainerFixtureTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"CosmosContainerFixture", @class =>
                {
                    AddUsing("System.Reflection");
                    AddUsing("DotNet.Testcontainers.Builders");
                    AddUsing("DotNet.Testcontainers.Configurations");
                    AddUsing("Microsoft.Azure.Cosmos");
                    AddUsing("Microsoft.Azure.Cosmos.Fluent");
                    AddUsing("Microsoft.Azure.CosmosRepository.Options");
                    AddUsing("Microsoft.Extensions.DependencyInjection");
                    AddUsing("Microsoft.Extensions.Options");
                    AddUsing("Testcontainers.CosmosDb");
                    AddNugetDependency(NugetPackages.TestcontainersCosmosDb);
                    @class.AddField("string", "_accountEndpoint", f => f.PrivateReadOnly().WithAssignment("\"https://localhost:{0}/\""));
                    @class.AddField("string", "_accountKey", f => f.PrivateReadOnly().WithAssignment("\"C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==\""));
                    @class.AddField("CosmosDbContainer", "_dbContainer", f => f.PrivateReadOnly());
                    @class.AddField("int", "_portNumber", f => f.Private().WithAssignment("0"));

                    @class.AddField("IOutputConsumer", "_consumer", f => f.PrivateReadOnly().WithAssignment("Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream())"));
                    @class.AddField("CosmosClient?", "_cosmosClient", f => f.Private());
                    @class.AddField("Type?", "_cosmosProviderType", f => f.Private());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddStatements(@"_dbContainer = new CosmosDbBuilder()
                .WithImage(""mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator"")
                .WithExposedPort(8081)
                .WithExposedPort(10251)
                .WithExposedPort(10252)
                .WithExposedPort(10253)
                .WithExposedPort(10254)
                .WithPortBinding(8081, true)
                .WithPortBinding(10251, true)
                .WithPortBinding(10252, true)
                .WithPortBinding(10253, true)
                .WithPortBinding(10254, true)
                .WithEnvironment(""AZURE_COSMOS_EMULATOR_PARTITION_COUNT"", ""10"")
                .WithEnvironment(""AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE"", ""127.0.0.1"")
                .WithEnvironment(""AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE"", ""false"")
                .WithOutputConsumer(_consumer)
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilMessageIsLogged(""Started""))
                .Build();".ConvertToStatements());
                    });

#warning get these
                    string databaseId = "CosmosIntegrationTest";
                    string containerId = "Container";

                    @class.AddMethod("void", "ConfigureTestServices", method =>
                    {
                        method.AddParameter("IServiceCollection", "services");
                        method.AddStatements(@$"var updatedEndpoint = string.Format(_accountEndpoint, _portNumber);

            var descriptor = services.SingleOrDefault(s => s.ServiceType == typeof(IOptions<RepositoryOptions>));
            if (descriptor is not null)
            {{
                services.Remove(descriptor);
            }}

            services.AddSingleton<IOptions<RepositoryOptions>>(new OptionsMock(new RepositoryOptions() {{ CosmosConnectionString = $""AccountEndpoint={{updatedEndpoint}};AccountKey={{_accountKey}}"", DatabaseId = ""{databaseId}"", ContainerId = ""{containerId}"" }}));

            var cosmosClientBuilder = new CosmosClientBuilder(updatedEndpoint, _accountKey);
            cosmosClientBuilder.WithHttpClientFactory(() =>
            {{
                HttpMessageHandler httpMessageHandler = new HttpClientHandler
                {{
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }};

                return new HttpClient(new FixRequestLocationHandler(_portNumber, httpMessageHandler));
            }});
            cosmosClientBuilder.WithConnectionModeGateway();
            cosmosClientBuilder.WithSerializerOptions(new CosmosSerializationOptions
            {{
                IgnoreNullValues = false,
                Indented = false,
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }});

            _cosmosClient = cosmosClientBuilder.Build();
            _cosmosProviderType = services.Single(s => s.ServiceType.Name == ""ICosmosClientProvider"").ServiceType;
".ConvertToStatements());

                        @class.AddMethod("void", "OnHostCreation", method =>
                        {
                            method
                                .AddParameter("IServiceProvider", "services")
                                .AddStatement("SwapCosmosClientToTestVersion(services);");
                        });

                        @class.AddMethod("void", "SwapCosmosClientToTestVersion", method =>
                        {
                            method
                                .Private()
                                .AddParameter("IServiceProvider", "services")
                                .AddStatement("var cosmosClientProvider = services.GetRequiredService(_cosmosProviderType!);")
                                .AddStatement("var privateField = cosmosClientProvider.GetType().GetField(\"_lazyCosmosClient\", BindingFlags.NonPublic | BindingFlags.Instance);")
                                .AddStatement("if (privateField is null) throw new Exception(\"Unable to inject testing CosmosClient. Can't find _lazyCosmosClient\");")
                                .AddStatement("privateField.SetValue(cosmosClientProvider, new Lazy<CosmosClient>(_cosmosClient!));");
                        });

                        @class.AddMethod("void", "InitializeAsync", method =>
                        {
                            method
                                .Async()
                                .AddStatements(@"await _dbContainer.StartAsync();

            // everything below here is about creating the test database and container
            var mappedPort = _dbContainer.GetMappedPublicPort(8081);
            _portNumber = mappedPort;
            var updated = string.Format(_accountEndpoint, mappedPort);
            var cosmosClientBuilder = new CosmosClientBuilder(updated, _accountKey);
            cosmosClientBuilder.WithHttpClientFactory(() =>
            {
                HttpMessageHandler httpMessageHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                return new HttpClient(new FixRequestLocationHandler(mappedPort, httpMessageHandler));
            });
            cosmosClientBuilder.WithConnectionModeGateway();

            var cosmosClient = cosmosClientBuilder.Build();".ConvertToStatements());
                        });

                        @class.AddMethod("Task", "DisposeAsync", method =>
                        {
                            method
                                .Async()
                                .AddStatement("await _dbContainer.StopAsync();");
                        });
                    });
                })
                .AddClass("OptionsMock", options =>
                {
                    options.ImplementsInterface("IOptions<RepositoryOptions>");
                    options.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("RepositoryOptions", "options", p => p.IntroduceReadonlyField());
                    });
                    options.AddProperty("RepositoryOptions", "Value", p => p.WithoutSetter().Getter.WithExpressionImplementation("_options"));
                })
                .AddClass("FixRequestLocationHandler", fix =>
                {
                    fix.WithBaseType("DelegatingHandler");
                    fix.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("int", "portNumber", p => p.IntroduceReadonlyField());
                        ctor.AddParameter("HttpMessageHandler", "innerHandler");
                        ctor.CallsBase(c => c.AddArgument("innerHandler"));
                    });

                    fix.AddMethod("Task<HttpResponseMessage>", "SendAsync", method =>
                    {

                        method
                            .Async()
                            .Protected()
                            .Override()
                            .AddParameter("HttpRequestMessage", "request")
                            .AddParameter("CancellationToken", "cancellationToken")
                            .AddStatement("request.RequestUri = new Uri($\"https://localhost:{_portNumber}{request.RequestUri.PathAndQuery}\");")
                            .AddStatement("HttpResponseMessage response = await base.SendAsync(request, cancellationToken);")
                            .AddStatement("return response;");
                        method.WithComments(@"
/// <summary>
/// Override of the SendAsync method to allow for reconstruction of the request uri to point to the dynamic testcontainer
/// port number. This needs to be done as otherwise it defaults back to 8081. I assume there is some hard coded port in
/// the emulator somewhere. If this is not done then the requests are not successful.
/// </summary>
/// <param name=""request""></param>
/// <param name=""cancellationToken""></param>
/// <returns></returns>");
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ContainerHelper.RequireCosmosContainer(this);
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