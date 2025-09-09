using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Migrations
{
    public class Migration_05_00_28_Pre_02 : IModuleMigration
    {
        private const string VisualStudioDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string VisualStudioSolutionPackageSpecializationId = "07e7b690-a59d-4b72-8440-4308a121d32c";
        private const string TemplateOutputSpecializationId = "d421c322-7a51-4094-89fa-e5d8a0a97b27";
        private const string RoleSpecializationId = "025e933b-b602-4b6d-95ab-0ec36ae940da";

        private const string TemplateOutputName = "Intent.EntityFrameworkCore.DbMigrationsReadMe";
        private const string RoleName = "Infrastructure";

        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_05_00_28_Pre_02()
        {
        }

        public Migration_05_00_28_Pre_02(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.EntityFrameworkCore";
        [IntentFully]
        public string ModuleVersion => "5.0.28-pre.2";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            if (app == null)
                return;

            var designer = app.GetDesigner(VisualStudioDesignerId);
            var packages = designer.GetPackages().Where(x => x.SpecializationTypeId == VisualStudioSolutionPackageSpecializationId);

            foreach (var package in packages)
            {
                var templateOutput = package.GetElementsOfType(TemplateOutputSpecializationId).FirstOrDefault(to => to.Name == TemplateOutputName);
                if (templateOutput == null ||
                    !TryGetParent(package, templateOutput, out var persistenceFolder) ||
                    persistenceFolder.Name is not "Persistence")
                {
                    continue;
                }

                var roleElement = package.GetElementsOfType(RoleSpecializationId).FirstOrDefault(x => x.Name == RoleName);
                if (roleElement == null ||
                    !TryGetParent(package, roleElement, out var roleParent))
                {
                    continue;
                }

                var migrationsFolder = roleParent.ChildElements.FirstOrDefault(x => x.Name is "Migrations");
                if (migrationsFolder == null)
                {
                    migrationsFolder = new ElementPersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        SpecializationType = persistenceFolder.SpecializationType,
                        SpecializationTypeId = persistenceFolder.SpecializationTypeId,
                        Name = "Migrations",
                        ParentFolderId = roleParent.Id,
                        PackageId = roleParent.PackageId,
                        PackageName = roleParent.PackageName,
                        Package = roleParent.Package,
                        ChildElements = []
                    };

                    roleParent.AddElement(migrationsFolder);
                }

                persistenceFolder.ChildElements.Remove(templateOutput);
                migrationsFolder.AddElement(templateOutput);

                package.Save();
            }
        }

        public void Down()
        {
        }

        private static bool TryGetParent(PackageModelPersistable package, ElementPersistable element, out ElementPersistable parent)
        {
            if (package.TryGetElementById(element.ParentFolderId, out var found))
            {
                parent = (ElementPersistable)found;
                return true;
            }

            parent = element.Parent as ElementPersistable;
            return parent != null;
        }

        private static void SetParent(PackageModelPersistable package, ElementPersistable element, ElementPersistable newParent)
        {
            if (TryGetParent(package, element, out var oldParent))
            {
                oldParent.ChildElements.Remove(element);
            }

            newParent.AddElement(element);
        }
    }
}