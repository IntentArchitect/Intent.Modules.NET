using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class MVCControllerEndpointStartupDecorator : StartupDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.IdentityServer4.UI.MVCControllerEndpointStartupDecorator";

        private readonly StartupTemplate _template;
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge)]
        public MVCControllerEndpointStartupDecorator(StartupTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = 0;
        }

//        public override string Configuration()
//        {
//            if (_template.IsNetCore2App())
//            {
//                return "app.UseMvc();";
//            }

//            return @"
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: ""default"",
//        pattern: ""{controller}/{action=Index}/{id?}"");
//});";
//        }

        public override string EndPointMappings()
        {
            return @"
    endpoints.MapControllerRoute(
        name: ""default"",
        pattern: ""{controller=Home}/{action=Index}/{id?}"");";
        }
    }
}