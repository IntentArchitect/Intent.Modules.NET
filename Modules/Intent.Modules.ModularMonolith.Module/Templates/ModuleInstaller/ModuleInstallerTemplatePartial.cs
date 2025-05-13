using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Module.Templates.ModuleInstaller
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ModuleInstallerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.ModularMonolith.Module.ModuleInstallerTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleInstallerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Swashbuckle.AspNetCore.SwaggerGen")
                .AddUsing("MassTransit")
                .AddUsing("Microsoft.AspNetCore.Mvc.ApplicationParts")
                .AddClass($"ModuleInstaller", @class =>
                {
                    this.AddFrameworkDependency("Microsoft.AspNetCore.App");
                    @class.ImplementsInterface(this.GetModuleInstallerInterfaceName());
                    @class.AddMethod("void", "ConfigureContainer", method =>
                    {
                        //Want Project Dependency
                        this.GetTemplate<ICSharpFileBuilderTemplate>("Intent.Infrastructure.DependencyInjection.DependencyInjection");

                        method.AddParameter("IServiceCollection", "services");
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddStatement("Application.DependencyInjection.AddApplication(services, configuration);");
                        method.AddStatement("Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);");
                        method.AddStatement("var builder = services.AddControllers();");
                        method.AddStatement("builder.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(ModuleInstaller).Assembly));");
                    });
                    @class.AddMethod("void", "ConfigureSwagger", method =>
                    {
                        string moduleInstallerTypeName = GetFullTypeName(ModuleInstaller.ModuleInstallerTemplate.TemplateId);

                        method.AddParameter("SwaggerGenOptions", "options");
                        method.AddStatement($"AddCommentFile(options, Path.Combine(AppContext.BaseDirectory, $\"{{typeof({moduleInstallerTypeName}).Assembly.GetName().Name}}.xml\"));");
#warning Would like a better way to get this assembly name Also Can't use name resolution and Careful of Conflict with Host
                        method.AddStatement($"AddCommentFile(options, Path.Combine(AppContext.BaseDirectory, $\"{{typeof({GetFullTypeName("Intent.Application.DependencyInjection.DependencyInjection")}).Assembly.GetName().Name}}.Contracts.xml\"));");

                    });
                    @class.AddMethod("void", "ConfigureIntegrationEventConsumers", method =>
                    {
                        method.AddParameter("IRegistrationConfigurator", "cfg");
                        method.AddStatement($"{GetTypeName("Intent.Eventing.MassTransit.MassTransitConfiguration")}.AddConsumers(cfg);");
                    });

                    @class.AddMethod("void", "AddCommentFile", method =>
                    {
                        method
                            .Private()
                            .Static()
                            .AddParameter("SwaggerGenOptions", "options")
                            .AddParameter("string", "filename");
                        method.AddStatement($"string? docFile = Path.Combine(AppContext.BaseDirectory, filename);");
                        method.AddIfStatement("File.Exists(docFile)", ifs => { ifs.AddStatement("options.IncludeXmlComments(docFile);"); });
                    });


                });
        }
        private string GetFullTypeName(string templateId)
        {
            var template = this.GetTemplate<ICSharpFileBuilderTemplate>(templateId);
            return template.Namespace + "." + template.ClassName;
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