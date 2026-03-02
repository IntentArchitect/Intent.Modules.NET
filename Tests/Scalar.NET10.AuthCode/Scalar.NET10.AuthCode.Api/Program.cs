using Intent.RoslynWeaver.Attributes;
using Scalar.AspNetCore;
using Scalar.NET10.AuthCode.Api.Configuration;
using Scalar.NET10.AuthCode.Api.Filters;
using Scalar.NET10.AuthCode.Application;
using Scalar.NET10.AuthCode.Infrastructure;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Program", Version = "1.0")]

namespace Scalar.NET10.AuthCode.Api
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
            builder.Services.ConfigureProblemDetails();
            builder.Services.ConfigureApiVersioning();
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