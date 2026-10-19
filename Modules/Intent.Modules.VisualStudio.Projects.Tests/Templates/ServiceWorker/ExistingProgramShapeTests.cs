using Intent.Modules.VisualStudio.Projects.Templates.ServiceWorker.ServiceWorkerProgram;
using Shouldly;
using Xunit;

namespace Intent.Modules.VisualStudio.Projects.Tests.Templates.ServiceWorker
{
    public class ExistingProgramShapeTests
    {
        // dotnet/eShop's src/OrderProcessor/Program.cs: a Worker SDK project using top-level statements,
        // WebApplication.CreateBuilder and RunAsync. The file starts with a BOM.
        private const string EShopOrderProcessor = "﻿" + """
            var builder = WebApplication.CreateBuilder(args);

            builder.AddBasicServiceDefaults();
            builder.AddApplicationServices();

            var app = builder.Build();

            app.MapDefaultEndpoints();

            await app.RunAsync();
            """;

        // What this template generates (Tests/WebAndWorker/WebAndWorker.AdditionalWorker/Program.cs).
        private const string IntentGeneratedWorker = """
            using Intent.RoslynWeaver.Attributes;
            using Microsoft.Extensions.DependencyInjection;
            using Microsoft.Extensions.Hosting;

            [assembly: DefaultIntentManaged(Mode.Fully)]
            [assembly: IntentTemplate("Intent.VisualStudio.Projects.ServiceWorker.ServiceWorkerProgram", Version = "1.0")]

            namespace WebAndWorker.AdditionalWorker
            {
                public class Program
                {
                    public static void Main(string[] args)
                    {
                        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

                        builder.Services.AddApplication(builder.Configuration);

                        // Add services to the container.

                        var app = builder.Build();

                        app.Run();
                    }
                }
            }
            """;

        [Fact]
        public void EShopOrderProcessor_IsRecognisedWithItsBuilderLineAndRunAsync()
        {
            var shape = ExistingProgramShape.Detect(EShopOrderProcessor);

            shape.Kind.ShouldBe(ExistingProgramShapeKind.Recognised);
            shape.BuilderStatement.ShouldBe("var builder = WebApplication.CreateBuilder(args);");
            shape.UsesWebApplicationBuilder.ShouldBeTrue();
            shape.RunsAsync.ShouldBeTrue();
            shape.RunCallStatement.ShouldBe("await app.RunAsync();");
        }

        [Fact]
        public void IntentGeneratedOutput_IsRecognisedWithTheHostBuilderAndRun()
        {
            var shape = ExistingProgramShape.Detect(IntentGeneratedWorker);

            shape.Kind.ShouldBe(ExistingProgramShapeKind.Recognised);
            shape.BuilderStatement.ShouldBe(ExistingProgramShape.DefaultBuilderStatement);
            shape.UsesWebApplicationBuilder.ShouldBeFalse();
            shape.RunsAsync.ShouldBeFalse();
            shape.RunCallStatement.ShouldBe("app.Run();");
        }

        [Fact]
        public void CreateDefaultBuilder_IsUnrecognised()
        {
            var shape = ExistingProgramShape.Detect("""
                using Microsoft.Extensions.Hosting;

                IHost host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices(services =>
                    {
                        services.AddHostedService<Worker>();
                    })
                    .Build();

                await host.RunAsync();
                """);

            shape.Kind.ShouldBe(ExistingProgramShapeKind.Unrecognised);
        }

        [Fact]
        public void OtherVariableNames_AreUnrecognised()
        {
            var shape = ExistingProgramShape.Detect("""
                var hostBuilder = Host.CreateApplicationBuilder(args);
                var host = hostBuilder.Build();
                host.Run();
                """);

            shape.Kind.ShouldBe(ExistingProgramShapeKind.Unrecognised);
        }

        [Fact]
        public void TwoRunCalls_AreUnrecognised()
        {
            var shape = ExistingProgramShape.Detect("""
                var builder = Host.CreateApplicationBuilder(args);
                var app = builder.Build();
                app.Run();
                app.Run();
                """);

            shape.Kind.ShouldBe(ExistingProgramShapeKind.Unrecognised);
        }

        [Fact]
        public void CommentedOutStatements_AreIgnored()
        {
            var shape = ExistingProgramShape.Detect("""
                var builder = Host.CreateApplicationBuilder(args);
                // var builder = WebApplication.CreateBuilder(args);
                var app = builder.Build();
                /*
                app.Run();
                */
                await app.RunAsync(); // "app.Run();"
                """);

            shape.Kind.ShouldBe(ExistingProgramShapeKind.Recognised);
            shape.BuilderStatement.ShouldBe("var builder = Host.CreateApplicationBuilder(args);");
            shape.RunsAsync.ShouldBeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void NoFile_IsNone(string content)
        {
            var shape = ExistingProgramShape.Detect(content);

            shape.Kind.ShouldBe(ExistingProgramShapeKind.None);
            shape.BuilderStatement.ShouldBeNull();
            shape.RunCallStatement.ShouldBe("app.Run();");
        }
    }
}
