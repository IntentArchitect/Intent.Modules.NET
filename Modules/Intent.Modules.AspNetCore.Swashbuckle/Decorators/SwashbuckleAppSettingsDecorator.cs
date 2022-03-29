using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class SwashbuckleAppSettingsDecorator : AppSettingsDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Swashbuckle.SwashbuckleAppSettingsDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly AppSettingsTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public SwashbuckleAppSettingsDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            appSettings.AddPropertyIfNotExists("Swashbuckle", new
            {
                SwaggerGen = new
                {
                    SwaggerGeneratorOptions = new
                    {
                        SwaggerDocs = new
                        {
                            v1 = new
                            {
                                Title = $"{_template.OutputTarget.Application.Name} API",
                                Version = "v1"
                            }
                        }
                    }
                },
                Swagger = new
                {
                    RouteTemplate = "swagger/{documentName}/swagger.json"
                },
                SwaggerUI = new
                {
                    RoutePrefix = "swagger",
                    ConfigObject = new
                    {
                        Urls = new[]
                        {
                            new {
                                Url = "/swagger/v1/swagger.json",
                                Name = $"{_template.OutputTarget.Application.Name} API V1"
                            }
                        },
                        DeepLinking = true,
                        DisplayOperationId = true,
                        DefaultModelsExpandDepth = -1,
                        DefaultModelExpandDepth = 2,
                        DefaultModelRendering = "model",
                        DocExpansion = "list",
                        Filter = string.Empty,
                        ShowExtensions = true
                    }
                }
            });
        }
    }
}