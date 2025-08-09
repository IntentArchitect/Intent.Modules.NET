using System;
using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Application.Identity.Migrations
{
    public class Migration_03_06_00_Pre_00 : IModuleMigration
    {
        private const string VisualStudioDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string VisualStudioSolutionPackageSpecializationId = "07e7b690-a59d-4b72-8440-4308a121d32c";
        private const string TemplateOutputRoleSpecializationId = "d421c322-7a51-4094-89fa-e5d8a0a97b27";
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_03_06_00_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Application.Identity";
        [IntentFully]
        public string ModuleVersion => "3.6.0-pre.0";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            if (app == null)
                return;

            var designer = app.GetDesigner(VisualStudioDesignerId);
            var packages = designer.GetPackages().Where(x => x.SpecializationTypeId == VisualStudioSolutionPackageSpecializationId);


            foreach (var package in packages)
            {
                var templateRole = package.GetElementsOfType(TemplateOutputRoleSpecializationId).FirstOrDefault(to => to.Name == "Intent.Application.Identity.CurrentUserServiceInterface");
                if (templateRole != null)
                {
                    var currentUserInterfaceElement = new ElementPersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        SpecializationType = "Template Output",
                        SpecializationTypeId = TemplateOutputRoleSpecializationId,
                        Name = "Intent.Application.Identity.CurrentUserInterface",
                        ExternalReference = "Intent.Application.Identity",
                        Metadata = new System.Collections.Generic.List<GenericMetadataPersistable> { new GenericMetadataPersistable() { Key = "status", Value = "assigned" } },
                        PackageId = package.Id,
                        PackageName = package.Name

                    };
                    templateRole.Parent.AddElement(currentUserInterfaceElement);
                    package.Save();
                }
            }
        }

        public void Down()
        {
        }
    }
}