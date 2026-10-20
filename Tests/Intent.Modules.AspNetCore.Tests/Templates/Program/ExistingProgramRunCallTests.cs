using Intent.Modules.AspNetCore.Templates.Program;
using Shouldly;
using Xunit;

namespace Intent.Modules.AspNetCore.Tests.Templates.Program
{
    public class ExistingProgramRunCallTests
    {
        // What this template generates for minimal hosting with a Main method
        // (Tests/AspNetCoreNoAuth/AspNetCoreNoAuth.Api/Program.cs).
        private const string IntentGeneratedMinimalHostingMain = """
            using AspNetCoreNoAuth.Api.Configuration;
            using Intent.RoslynWeaver.Attributes;

            [assembly: DefaultIntentManaged(Mode.Fully)]
            [assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

            namespace AspNetCoreNoAuth.Api
            {
                public class Program
                {
                    public static void Main(string[] args)
                    {
                        var builder = WebApplication.CreateBuilder(args);

                        // Add services to the container.
                        builder.Services.ConfigureProblemDetails();

                        var app = builder.Build();

                        // Configure the HTTP request pipeline.
                        app.UseExceptionHandler();
                        app.MapControllers();

                        app.Run();
                    }
                }
            }
            """;

        // What this template generates for minimal hosting with top-level statements and Serilog
        // (Tests/AdvancedMappingCrud.Cosmos.Tests/AdvancedMappingCrud.Cosmos.Tests.Api/Program.cs).
        private const string IntentGeneratedMinimalHostingTopLevelWithSerilog = """
            using Intent.RoslynWeaver.Attributes;
            using Serilog;
            using Serilog.Events;

            [assembly: DefaultIntentManaged(Mode.Fully)]
            [assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

            using var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                app.MapControllers();

                logger.Write(LogEventLevel.Information, "Starting web host");

                app.Run();
            }
            catch (Exception ex)
            {
                logger.Write(LogEventLevel.Fatal, ex, "Unhandled exception");
            }
            """;

        // What this template generates for the generic hosting model with a Main method.
        private const string IntentGeneratedGenericHostingMain = """
            using Intent.RoslynWeaver.Attributes;
            using Microsoft.AspNetCore.Hosting;
            using Microsoft.Extensions.Hosting;

            [assembly: DefaultIntentManaged(Mode.Fully)]
            [assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

            namespace AspNetCoreNoAuth.Api
            {
                public class Program
                {
                    public static void Main(string[] args)
                    {
                        CreateHostBuilder(args).Build().Run();
                    }

                    public static IHostBuilder CreateHostBuilder(string[] args) =>
                        Host.CreateDefaultBuilder(args)
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseStartup<Startup>();
                            });
                }
            }
            """;

        [Fact]
        public void IntentGeneratedMinimalHostingMain_IsRecognisedAsRun()
        {
            var runCall = ExistingProgramRunCall.Detect(IntentGeneratedMinimalHostingMain, usesMinimalHostingModel: true);

            runCall.Kind.ShouldBe(ExistingProgramRunCallKind.Recognised);
            runCall.Statement.ShouldBe("app.Run();");
            runCall.IsAwaited.ShouldBeFalse();
        }

        [Fact]
        public void IntentGeneratedMinimalHostingTopLevelWithSerilog_IsRecognisedAsRun()
        {
            var runCall = ExistingProgramRunCall.Detect(IntentGeneratedMinimalHostingTopLevelWithSerilog, usesMinimalHostingModel: true);

            runCall.Kind.ShouldBe(ExistingProgramRunCallKind.Recognised);
            runCall.Statement.ShouldBe("app.Run();");
            runCall.IsAwaited.ShouldBeFalse();
        }

        [Fact]
        public void IntentGeneratedGenericHostingMain_IsRecognisedAsRun()
        {
            var runCall = ExistingProgramRunCall.Detect(IntentGeneratedGenericHostingMain, usesMinimalHostingModel: false);

            runCall.Kind.ShouldBe(ExistingProgramRunCallKind.Recognised);
            runCall.Statement.ShouldBe("CreateHostBuilder(args).Build().Run();");
            runCall.IsAwaited.ShouldBeFalse();
        }

        [Fact]
        public void SerilogWrappedRunAsync_IsRecognisedAsAwaited()
        {
            var content = IntentGeneratedMinimalHostingTopLevelWithSerilog.Replace("app.Run();", "await app.RunAsync();");

            var runCall = ExistingProgramRunCall.Detect(content, usesMinimalHostingModel: true);

            runCall.Kind.ShouldBe(ExistingProgramRunCallKind.Recognised);
            runCall.Statement.ShouldBe("await app.RunAsync();");
            runCall.IsAwaited.ShouldBeTrue();
        }

        [Theory]
        [InlineData("app.Run();", false)]
        [InlineData("app.Run(\"http://localhost:5000\");", false)]
        [InlineData("await app.RunAsync();", true)]
        [InlineData("await app.RunAsync(url);", true)]
        [InlineData("await app.RunAsync().ConfigureAwait(false);", true)]
        [InlineData("app.RunAsync().GetAwaiter().GetResult();", false)]
        [InlineData("app.RunAsync().Wait();", false)]
        public void MinimalHosting_RecognisedForms_AreReEmittedVerbatim(string statement, bool isAwaited)
        {
            var runCall = ExistingProgramRunCall.Detect(MinimalHostingFile(statement), usesMinimalHostingModel: true);

            runCall.Kind.ShouldBe(ExistingProgramRunCallKind.Recognised);
            runCall.Statement.ShouldBe(statement);
            runCall.IsAwaited.ShouldBe(isAwaited);
        }

        [Theory]
        [InlineData("CreateHostBuilder(args).Build().Run();", false)]
        [InlineData("CreateHostBuilder(args).Build().Run(\"http://localhost:5000\");", false)]
        [InlineData("await CreateHostBuilder(args).Build().RunAsync();", true)]
        [InlineData("await CreateHostBuilder(args).Build().RunAsync(token);", true)]
        [InlineData("await CreateHostBuilder(args).Build().RunAsync().ConfigureAwait(false);", true)]
        [InlineData("CreateHostBuilder(args).Build().RunAsync().GetAwaiter().GetResult();", false)]
        [InlineData("CreateHostBuilder(args).Build().RunAsync().Wait();", false)]
        public void GenericHosting_RecognisedForms_AreReEmittedVerbatim(string statement, bool isAwaited)
        {
            var content = IntentGeneratedGenericHostingMain.Replace("CreateHostBuilder(args).Build().Run();", statement);

            var runCall = ExistingProgramRunCall.Detect(content, usesMinimalHostingModel: false);

            runCall.Kind.ShouldBe(ExistingProgramRunCallKind.Recognised);
            runCall.Statement.ShouldBe(statement);
            runCall.IsAwaited.ShouldBe(isAwaited);
        }

        [Fact]
        public void ByteOrderMark_IsIgnored()
        {
            var runCall = ExistingProgramRunCall.Detect("﻿" + MinimalHostingFile("await app.RunAsync();"), usesMinimalHostingModel: true);

            runCall.Kind.ShouldBe(ExistingProgramRunCallKind.Recognised);
            runCall.Statement.ShouldBe("await app.RunAsync();");
        }

        [Fact]
        public void CommentedOutRunCall_IsIgnored()
        {
            var content = MinimalHostingFile("""
                // app.Run();
                /* app.Run(); */
                await app.RunAsync();
                """);

            var runCall = ExistingProgramRunCall.Detect(content, usesMinimalHostingModel: true);

            runCall.Kind.ShouldBe(ExistingProgramRunCallKind.Recognised);
            runCall.Statement.ShouldBe("await app.RunAsync();");
        }

        [Fact]
        public void TerminalMiddleware_IsNotMistakenForTheRunCall()
        {
            var content = MinimalHostingFile("""
                app.Run(async context => await context.Response.WriteAsync("Hello"));
                app.Run();
                """);

            var runCall = ExistingProgramRunCall.Detect(content, usesMinimalHostingModel: true);

            runCall.Kind.ShouldBe(ExistingProgramRunCallKind.Recognised);
            runCall.Statement.ShouldBe("app.Run();");
        }

        [Fact]
        public void TwoRunCalls_AreUnrecognised()
        {
            var content = MinimalHostingFile("""
                if (args.Length > 0)
                {
                    app.Run(args[0]);
                }
                else
                {
                    app.Run();
                }
                """);

            ExistingProgramRunCall.Detect(content, usesMinimalHostingModel: true).Kind.ShouldBe(ExistingProgramRunCallKind.Unrecognised);
        }

        [Fact]
        public void StartAsyncAndWaitForShutdownAsync_AreUnrecognised()
        {
            var content = MinimalHostingFile("""
                await app.StartAsync();
                await app.WaitForShutdownAsync();
                """);

            ExistingProgramRunCall.Detect(content, usesMinimalHostingModel: true).Kind.ShouldBe(ExistingProgramRunCallKind.Unrecognised);
        }

        [Fact]
        public void DifferentReceiver_IsUnrecognised()
        {
            var content = MinimalHostingFile("await webApp.RunAsync();");

            ExistingProgramRunCall.Detect(content, usesMinimalHostingModel: true).Kind.ShouldBe(ExistingProgramRunCallKind.Unrecognised);
        }

        [Fact]
        public void OtherHostingModelsRunCall_IsUnrecognised()
        {
            ExistingProgramRunCall.Detect(IntentGeneratedGenericHostingMain, usesMinimalHostingModel: true)
                .Kind.ShouldBe(ExistingProgramRunCallKind.Unrecognised);
        }

        [Theory]
        [InlineData("app.RunAsync();")]
        [InlineData("await app.Run();")]
        [InlineData("await app.RunAsync().GetAwaiter().GetResult();")]
        [InlineData("app.RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();")]
        [InlineData("app.Run().Wait();")]
        public void InvalidForms_AreUnrecognised(string statement)
        {
            ExistingProgramRunCall.Detect(MinimalHostingFile(statement), usesMinimalHostingModel: true)
                .Kind.ShouldBe(ExistingProgramRunCallKind.Unrecognised);
        }

        [Fact]
        public void IntentGeneratedVoidMain_DoesNotUseAsyncMain()
        {
            var runCall = ExistingProgramRunCall.Detect(IntentGeneratedMinimalHostingMain, usesMinimalHostingModel: true);

            runCall.DeclaresAsyncMain.ShouldBeFalse();
            runCall.UsesAsyncMain.ShouldBeFalse();
        }

        [Fact]
        public void AwaitedRunCall_UsesAsyncMain()
        {
            var content = IntentGeneratedMinimalHostingMain.Replace("app.Run();", "await app.RunAsync();");

            ExistingProgramRunCall.Detect(content, usesMinimalHostingModel: true).UsesAsyncMain.ShouldBeTrue();
        }

        // A developer's own await (e.g. a migration) with the default run call: generating "void" would have
        // Merge mode keep "async" and produce "async void Main", which doesn't compile.
        [Fact]
        public void AsyncTaskMainWithSyncRunCall_KeepsAsyncMain()
        {
            var content = IntentGeneratedMinimalHostingMain
                .Replace("public static void Main", "public static async Task Main")
                .Replace("var app = builder.Build();", "var app = builder.Build();\n await MigrateAsync(app);");

            var runCall = ExistingProgramRunCall.Detect(content, usesMinimalHostingModel: true);

            runCall.Kind.ShouldBe(ExistingProgramRunCallKind.Recognised);
            runCall.IsAwaited.ShouldBeFalse();
            runCall.DeclaresAsyncMain.ShouldBeTrue();
            runCall.UsesAsyncMain.ShouldBeTrue();
        }

        [Fact]
        public void AsyncTaskMainWithUnrecognisedRunCall_KeepsAsyncMain()
        {
            var content = IntentGeneratedMinimalHostingMain
                .Replace("public static void Main", "static async Task Main")
                .Replace("app.Run();", "await app.StartAsync();\n await app.WaitForShutdownAsync();");

            var runCall = ExistingProgramRunCall.Detect(content, usesMinimalHostingModel: true);

            runCall.Kind.ShouldBe(ExistingProgramRunCallKind.Unrecognised);
            runCall.UsesAsyncMain.ShouldBeTrue();
        }

        [Fact]
        public void CommentedOutAsyncMain_IsIgnored()
        {
            var content = IntentGeneratedMinimalHostingMain.Replace("public static void Main", "// public static async Task Main(string[] args)\n public static void Main");

            ExistingProgramRunCall.Detect(content, usesMinimalHostingModel: true).DeclaresAsyncMain.ShouldBeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NoContent_IsNone(string? content)
        {
            ExistingProgramRunCall.Detect(content, usesMinimalHostingModel: true).Kind.ShouldBe(ExistingProgramRunCallKind.None);
        }

        [Theory]
        [InlineData(true, "app.Run();")]
        [InlineData(false, "CreateHostBuilder(args).Build().Run();")]
        public void NotRecognised_FallsBackToTheDefaultStatement(bool usesMinimalHostingModel, string expected)
        {
            ExistingProgramRunCall.None.GetStatementOrDefault(usesMinimalHostingModel).ShouldBe(expected);
            ExistingProgramRunCall.Detect(MinimalHostingFile("await webApp.RunAsync();"), usesMinimalHostingModel)
                .GetStatementOrDefault(usesMinimalHostingModel).ShouldBe(expected);
        }

        private static string MinimalHostingFile(string runStatements) => $"""
            var builder = WebApplication.CreateBuilder(args);

            var app = builder.Build();

            app.MapControllers();

            {runStatements}
            """;
    }
}
