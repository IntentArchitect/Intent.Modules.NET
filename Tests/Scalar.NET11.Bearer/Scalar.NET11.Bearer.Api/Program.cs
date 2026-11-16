using Intent.RoslynWeaver.Attributes;
using Morcatko.AspNetCore.JsonMergePatch;
using Scalar.AspNetCore;
using Scalar.NET11.Bearer.Api.Configuration;
using Scalar.NET11.Bearer.Api.Filters;
using Scalar.NET11.Bearer.Api.Services;
using Scalar.NET11.Bearer.Application;
using Scalar.NET11.Bearer.Infrastructure;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace Scalar.NET11.Bearer.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers(
                opt =>
                {
                    opt.Filters.Add<ExceptionFilter>();
                    opt.ModelMetadataDetailsProviders.Add(new PatchIgnoreMetadataProvider());
                })
            .AddNewtonsoftJsonMergePatch();
            builder.Services.AddApplication(builder.Configuration);
            builder.Services.ConfigureProblemDetails();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.ConfigureOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.MapScalarApiReference();
            app.MapOpenApi();
            app.MapControllers();

            app.Run();
        }
    }
}