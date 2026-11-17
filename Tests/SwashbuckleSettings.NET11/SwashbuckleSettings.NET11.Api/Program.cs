using Intent.RoslynWeaver.Attributes;
using Morcatko.AspNetCore.JsonMergePatch;
using SwashbuckleSettings.NET11.Api.Configuration;
using SwashbuckleSettings.NET11.Api.Filters;
using SwashbuckleSettings.NET11.Api.Services;
using SwashbuckleSettings.NET11.Application;
using SwashbuckleSettings.NET11.Infrastructure;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace SwashbuckleSettings.NET11.Api
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
            builder.Services.ConfigureApiVersioning();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.ConfigureSwagger(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
            app.UseSwashbuckle(builder.Configuration);

            app.Run();
        }
    }
}