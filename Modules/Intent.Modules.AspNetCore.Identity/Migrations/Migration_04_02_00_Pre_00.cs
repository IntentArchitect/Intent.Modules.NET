using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Model.Visual;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.Migrations
{
    public class Migration_04_02_00_Pre_00 : IModuleMigration
    {
        private const string DomainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";

        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_04_02_00_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.AspNetCore.Identity";
        [IntentFully]
        public string ModuleVersion => "4.2.0-pre.0";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(DomainDesignerId);

            var package = designer.GetPackages().FirstOrDefault();
            if (package is null)
            {
                return;
            }

            package.Load();

            var identityDomainRefence = package.References.FirstOrDefault(x => x.Name == "Intent.AspNetCore.Identity.Domain");
            if (identityDomainRefence is null)
            {
                return;
            }

            var identityDomainPackage = identityDomainRefence.TryLoadPackage();
            if (identityDomainPackage is null)
            {
                return;
            }

            var diagrams = package.GetElementsOfType("4d66fecd-e9b8-436f-aa50-c59040ad0879");

            var identityDiagramId = Guid.Empty;

            if (diagrams.Count > 0)
            {
                var identityDiagram = diagrams.FirstOrDefault(d => d.Name == "Identity Diagram");

                if (identityDiagram is not null)
                {
                    identityDiagramId = Guid.Parse(identityDiagram.Id);
                }
            }

            if (identityDiagramId == Guid.Empty)
            {
                identityDiagramId = Guid.NewGuid();

                var identityDiagram = identityDomainPackage.Classes.FirstOrDefault(d => d.SpecializationTypeId == "4d66fecd-e9b8-436f-aa50-c59040ad0879");

                List<AssociationVisualPersistable> newAssociationVisuals = new List<AssociationVisualPersistable>();
                List<ElementVisualPersistable> newClassVisuals = new List<ElementVisualPersistable>();

                if (identityDiagram is not null)
                {
                    newClassVisuals.AddRange(identityDiagram.Diagram.ClassVisuals.Select(c => new ElementVisualPersistable
                    {
                        AbsolutePath = c.AbsolutePath,
                        AutoResizeEnabled = c.AutoResizeEnabled,
                        Id = c.Id,
                        Position = c.Position,
                        Size = c.Size,
                        ZIndex = c.ZIndex
                    }));

                    newAssociationVisuals.AddRange(identityDiagram.Diagram.AssociationVisuals
                        .Select(s => new AssociationVisualPersistable
                        {
                            AbsolutePath = s.AbsolutePath,
                            FixedPoints = s.FixedPoints,
                            Id = s.Id,
                            SourceId = s.SourceId,
                            TargetId = s.TargetId,
                            TargetPrefPoint = s.TargetPrefPoint,
                            ZIndex = s.ZIndex
                        }));
                }

                package.Classes.Add(new ElementPersistable
                {
                    Id = identityDiagramId.ToString(),
                    SpecializationType = "Diagram",
                    SpecializationTypeId = "4d66fecd-e9b8-436f-aa50-c59040ad0879",
                    Name = "Identity Diagram",
                    Display = "Identity Diagram",
                    IsAbstract = false,
                    SortChildren = SortChildrenOptions.SortByTypeAndName,
                    IsMapped = false,
                    ParentFolderId = package.Id,
                    PackageId = package.Id,
                    PackageName = package.Name,
                    Diagram = new DiagramPersistable
                    {
                        AssociationVisuals = newAssociationVisuals,
                        ClassVisuals = newClassVisuals,
                        TargetPackageId = package.Id,
                    }
                });
            }

            package.Save();

            app.SaveAllChanges();
        }

        public void Down()
        {
        }
    }
}