using EfCoreSoftDelete.Api.Configuration;
using EfCoreSoftDelete.Api.Filters;
using EfCoreSoftDelete.Application;
using EfCoreSoftDelete.Infrastructure;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace EfCoreSoftDelete.Api
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
                });
            builder.Services.AddApplication(builder.Configuration);
            builder.Services.ConfigureApplicationSecurity(builder.Configuration);

            builder.Services.ConfigureProblemDetails();

            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.ConfigureSwagger(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseSwashbuckle(builder.Configuration);

            app.Run();
        }
    }
}