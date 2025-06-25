using System;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController.Migrations
{
    public class Migration_04_01_06_Pre_00 : IModuleMigration
    {
        private const string DomainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";
        private const string ServiceDesignerId = "81104ae6-2bc5-4bae-b05a-f987b0372d81";
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_04_01_06_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.AspNetCore.Identity.AccountController";
        [IntentFully]
        public string ModuleVersion => "4.1.6-pre.0";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(DomainDesignerId);
            var package = designer.GetPackages().FirstOrDefault();
            var diagrams = package.GetElementsOfType("4d66fecd-e9b8-436f-aa50-c59040ad0879");

            if (!diagrams.Any(d => d.Name == "Identity Diagram"))
            {
                return;
            }

            var identityDomainRefence = package.References.FirstOrDefault(x => x.Name == "Intent.AspNetCore.Identity.Domain");
            if (identityDomainRefence is not null)
            {
                var serviceDesigner = app.GetDesigner(ServiceDesignerId);

                if (serviceDesigner is not null)
                {
                    var servicePackage = serviceDesigner.GetPackages().FirstOrDefault();

                    if (servicePackage is not null)
                    {
                        servicePackage.AddReference(identityDomainRefence);
                    }
                }
            }

            var identityDiagram = diagrams.First(d => d.Name == "Identity Diagram");
            if (!package.Classes.Any(c => c.Name == "ApplicationIdentityUser"))
            {
                var applicationIdentityUserId = Guid.NewGuid().ToString();
                package.Classes.Add(new ElementPersistable
                {
                    Id = applicationIdentityUserId,
                    SpecializationType = "Class",
                    SpecializationTypeId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10",
                    Name = "ApplicationIdentityUser",
                    Display = "ApplicationIdentityUser: IdentityUser<string>",
                    IsAbstract = false,
                    IsMapped = false,
                    ParentFolderId = package.Id,
                    PackageId = package.Id,
                    PackageName = package.Name,
                    ChildElements = new System.Collections.Generic.List<ElementPersistable>
        {
            new ElementPersistable
            {
                Id = Guid.NewGuid().ToString(),
                SpecializationType = "Attribute",
                SpecializationTypeId = "0090fb93-483e-41af-a11d-5ad2dc796adf",
                ParentFolderId = applicationIdentityUserId,
                PackageId= package.Id,
                PackageName = package.Name,
                Name = "RefreshToken",
                Display = "RefreshToken: string",
                TypeReference = new TypeReferencePersistable
                {
                    Id = Guid.NewGuid().ToString(),
                    TypeId = "d384db9c-a279-45e1-801e-e4e8099625f2",
                    TypePackageName = "Intent.Common.Types",
                    TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67",
                    IsRequired = true,
                    IsNavigable = true,
                    IsNullable = true,
                    IsCollection = false
                },
                Stereotypes = new System.Collections.Generic.List<IArchitect.Agent.Persistence.Model.Common.StereotypePersistable>
                {
                    new IArchitect.Agent.Persistence.Model.Common.StereotypePersistable
                    {
                        DefinitionId = "6347286E-A637-44D6-A5D7-D9BE5789CA7A",
                        Name = "Text CConstraints",
                        DefinitionPackageId = "AF8F3810-745C-42A2-93C8-798860DC45B1",
                        DefinitionPackageName = "Intent.Metadata.RDBMS",
                        Properties = new System.Collections.Generic.List<IArchitect.Agent.Persistence.Model.Common.StereotypePropertyPersistable>
                        {
                            new IArchitect.Agent.Persistence.Model.Common.StereotypePropertyPersistable { DefinitionId = "1288cfcd-ee51-437e-9713-73b80118f026", Name = "SQL Data Type", Value = "DEFAULT", IsActive = true },
                            new IArchitect.Agent.Persistence.Model.Common.StereotypePropertyPersistable { DefinitionId = "A04CC24D-81FB-4EA2-A34A-B3C58E04DCFD", Name = "MaxLength", IsActive = true },
                            new IArchitect.Agent.Persistence.Model.Common.StereotypePropertyPersistable { DefinitionId = "67EC4CF4-7706-4B39-BC7C-DF539EE2B0AF", Name = "IsUnicode", Value = "false", IsActive = true }
                        }
                    }
                }
            },
            new ElementPersistable
            {
                Id = Guid.NewGuid().ToString(),
                SpecializationType = "Attribute",
                SpecializationTypeId = "0090fb93-483e-41af-a11d-5ad2dc796adf",
                ParentFolderId = applicationIdentityUserId,
                PackageId= package.Id,
                PackageName = package.Name,
                Name = "RefreshTokenExpired",
                Display = "RefreshTokenExpired: datetime",
                TypeReference = new TypeReferencePersistable
                {
                    Id = Guid.NewGuid().ToString(),
                    TypeId = "a4107c29-7851-4121-9416-cf1236908f1e",
                    TypePackageName = "Intent.Common.Types",
                    TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67",
                    IsRequired = true,
                    IsNavigable = true,
                    IsNullable = true,
                    IsCollection = false
                }
            }
        }
                });

                var associationVisualId = Guid.NewGuid().ToString();

                package.Associations.Add(new AssociationPersistable
                {
                    AssociationType = "Generalization",
                    AssociationTypeId = "5de35973-3ac7-4e65-b48c-385605aec561",
                    Id = associationVisualId,
                    SourceEnd = new AssociationEndPersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        SpecializationType = "Generalization Source End",
                        SpecializationTypeId = "8190bf43-222c-4b53-8a44-14626efe3574",
                        Display = ": ApplicationIdentityUser",
                        Order = 0,
                        TypeReference = new TypeReferencePersistable
                        {
                            Id = Guid.NewGuid().ToString(),
                            TypeId = applicationIdentityUserId,
                            TypePackageName = package.Name,
                            TypePackageId = package.Id,
                            IsRequired = true,
                            IsNavigable = false,
                            IsNullable = false,
                            IsCollection = false
                        }
                    },
                    TargetEnd = new AssociationEndPersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        SpecializationType = "Generalization Target End",
                        SpecializationTypeId = "4686cc1d-b4d8-4b99-b45b-f77bd5496946",
                        Display = "extends IdentityUser<string>",
                        Order = 0,
                        Name = "base",
                        TypeReference = new TypeReferencePersistable
                        {
                            Id = Guid.NewGuid().ToString(),
                            TypeId = "f6505b15-dced-4beb-9a58-e7b5447a7c73",
                            TypePackageName = "Intent.AspNetCore.Identity.Domain",
                            TypePackageId = "d1f3cf7b-cd9a-431f-af26-a86aec1ace6f",
                            IsRequired = true,
                            IsNavigable = false,
                            IsNullable = false,
                            IsCollection = false,
                            GenericTypeParameters = new System.Collections.Generic.List<TypeReferencePersistable>
                {
                    new TypeReferencePersistable
                    {
                        Id = Guid.NewGuid().ToString(),
                        TypeId = "d384db9c-a279-45e1-801e-e4e8099625f2",
                        TypePackageName = "Intent.Common.Types",
                        TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67",
                        IsRequired = true,
                        IsNavigable = true,
                        IsNullable = false,
                        IsCollection = false,
                        GenericTypeId = "d618eff4-adab-4f6d-a758-6ecad2eb8429"
                    }
                }
                        }
                    }
                });

                identityDiagram.Diagram.ClassVisuals.Add(new IArchitect.Agent.Persistence.Model.Visual.ElementVisualPersistable
                {
                    Id = applicationIdentityUserId,
                    ZIndex = 13,
                    AutoResizeEnabled = true,
                    Position = new IArchitect.Agent.Persistence.Model.Visual.Point { X = 400, Y = 900 },
                    Size = new IArchitect.Agent.Persistence.Model.Visual.Size { Height = 30, Width = 200 }
                });

                identityDiagram.Diagram.AssociationVisuals.Add(new IArchitect.Agent.Persistence.Model.Visual.AssociationVisualPersistable
                {
                    Id = associationVisualId,
                    SourceId = applicationIdentityUserId,
                    TargetId = "f6505b15-dced-4beb-9a58-e7b5447a7c73",
                    TargetPrefPoint = new IArchitect.Agent.Persistence.Model.Visual.Point { X = 100, Y = 310 },
                    ZIndex = 14
                });

                package.Save();

                app.SaveAllChanges();
            }
        }

        public void Down()
        {
        }
    }
}