using System;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.DiffAudit.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private const string DomainDesignerId = "6ab29b31-27af-4f56-a67c-986d82097d63";
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.EntityFrameworkCore.DiffAudit";

        public void OnInstall()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var domainPackages = app.GetDesigner(DomainDesignerId).GetPackages();

            foreach (var domainPackage in domainPackages)
            {
                if (domainPackage.Classes.SelectMany(x => x.Stereotypes).FirstOrDefault(s => s.DefinitionId == "92e433aa-4858-4129-849f-ef0f9f0bf9e6") == null)// Audit Log            
                {
                    var auditLogClassId = Guid.NewGuid().ToString();
                    var auditLogStereotype = new StereotypePersistable
                    {
                        AddedByDefault = true,
                        DefinitionId = "92e433aa-4858-4129-849f-ef0f9f0bf9e6",
                        Name = "Audit Log",
                        DefinitionPackageId = "7269eaca-47f3-4766-adfd-a6f130329d04",
                        DefinitionPackageName = "Intent.EntityFramework.DiffAudit"
                    };

                    var primaryKey = new ElementPersistable()
                    {
                        Id = Guid.NewGuid().ToString(),
                        SpecializationType = "Attribute",
                        SpecializationTypeId = "0090fb93-483e-41af-a11d-5ad2dc796adf",
                        Name = "Id",
                        Display = "Id: int",
                        IsAbstract = false,
                        TypeReference = new TypeReferencePersistable
                        {
                            Id = Guid.NewGuid().ToString(),
                            TypeId = "fb0a362d-e9e2-40de-b6ff-5ce8167cbe74",
                            IsNavigable = true,
                            IsNullable = false,
                            IsCollection = false,
                            IsRequired = true,
                            TypePackageName = "Intent.Common.Types",
                            TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67"
                        },
                        IsMapped = false,
                        ParentFolderId = auditLogClassId,
                        PackageId = domainPackage.Id,
                        PackageName = domainPackage.Name,
                        Metadata = new System.Collections.Generic.List<GenericMetadataPersistable>
                    {
                        new GenericMetadataPersistable
                        {
                            Key = "is-managed-key",
                            Value = "false"
                        }
                    },
                        Stereotypes = new System.Collections.Generic.List<StereotypePersistable>
                    {
                        new StereotypePersistable
                        {
                            AddedByDefault = true,
                            DefinitionId = "b99aac21-9ca4-467f-a3a6-046255a9eed6",
                            Name = "Primary Key",
                            DefinitionPackageId = "AF8F3810-745C-42A2-93C8-798860DC45B1",
                            DefinitionPackageName = "Intent.Metadata.RDBMS",
                            Properties = new System.Collections.Generic.List<StereotypePropertyPersistable>
                            {
                                new StereotypePropertyPersistable
                                {
                                    IsActive = true,
                                    Value = "Default",
                                    DefinitionId = "a7a5e921-18b9-43b4-8078-b4ac4e5dae6f",
                                    Name = "Data source"
                                }
                            }
                        }
                    }
                    };

                    var tableName = CreateStringType("TableName", auditLogClassId, domainPackage.Id, domainPackage.Name);
                    var key = CreateStringType("Key", auditLogClassId, domainPackage.Id, domainPackage.Name);
                    var columnName = CreateStringType("ColumnName", auditLogClassId, domainPackage.Id, domainPackage.Name, true);
                    var oldValue = CreateStringType("OldValue", auditLogClassId, domainPackage.Id, domainPackage.Name, true);
                    var newValue = CreateStringType("NewValue", auditLogClassId, domainPackage.Id, domainPackage.Name, true);
                    var changedBy = CreateStringType("ChangedBy", auditLogClassId, domainPackage.Id, domainPackage.Name);

                    var changedDate = new ElementPersistable()
                    {
                        Id = Guid.NewGuid().ToString(),
                        SpecializationType = "Attribute",
                        SpecializationTypeId = "0090fb93-483e-41af-a11d-5ad2dc796adf",
                        Name = "ChangedDate",
                        Display = "ChangedDate: datetimeoffset",
                        IsAbstract = false,
                        TypeReference = new TypeReferencePersistable
                        {
                            Id = Guid.NewGuid().ToString(),
                            TypeId = "f1ba4df3-a5bc-427e-a591-4f6029f89bd7",
                            IsNavigable = true,
                            IsNullable = false,
                            IsCollection = false,
                            IsRequired = true,
                            TypePackageName = "Intent.Common.Types",
                            TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67"
                        },
                        IsMapped = false,
                        ParentFolderId = auditLogClassId,
                        PackageId = domainPackage.Id,
                        PackageName = domainPackage.Name,
                    };

                    var auditLogType = new ElementPersistable()
                    {
                        Id = auditLogClassId,
                        Name = domainPackages.Count > 1 ? $"{domainPackage.Name}AuditLog" : "AuditLog",
                        PackageId = domainPackage.Id,
                        ParentFolderId = domainPackage.Id,
                        Display = "AuditLog",
                        IsAbstract = false,
                        IsMapped = false,
                        Stereotypes = new[] { auditLogStereotype }.ToList(),
                        SortChildren = SortChildrenOptions.SortByTypeThenManually,
                        SpecializationType = "Class",
                        SpecializationTypeId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10",
                        ChildElements = new System.Collections.Generic.List<ElementPersistable>
                        {
                            primaryKey, tableName, key, columnName, oldValue, newValue, changedBy, changedDate
                        }
                    };

                    domainPackage.Classes.Add(auditLogType);
                }
            }
            app.SaveAllChanges();
        }

        private ElementPersistable CreateStringType(string attributeName, string parentFolderId, string packageId, string packageName, bool isNullable = false)
        {
            return new ElementPersistable()
            {
                Id = Guid.NewGuid().ToString(),
                SpecializationType = "Attribute",
                SpecializationTypeId = "0090fb93-483e-41af-a11d-5ad2dc796adf",
                Name = attributeName,
                Display = $"{attributeName}: string",
                IsAbstract = false,
                TypeReference = new TypeReferencePersistable
                {
                    Id = Guid.NewGuid().ToString(),
                    TypeId = "d384db9c-a279-45e1-801e-e4e8099625f2",
                    IsNavigable = true,
                    IsNullable = isNullable,
                    IsCollection = false,
                    IsRequired = true,
                    TypePackageName = "Intent.Common.Types",
                    TypePackageId = "870ad967-cbd4-4ea9-b86d-9c3a5d55ea67"
                },
                IsMapped = false,
                ParentFolderId = parentFolderId,
                PackageId = packageId,
                PackageName = packageName,
                Stereotypes = new System.Collections.Generic.List<StereotypePersistable>
                    {
                        new StereotypePersistable
                        {
                            AddedByDefault = true,
                            DefinitionId = "6347286E-A637-44D6-A5D7-D9BE5789CA7A",
                            Name = "Text Constraints",
                            DefinitionPackageId = "AF8F3810-745C-42A2-93C8-798860DC45B1",
                            DefinitionPackageName = "Intent.Metadata.RDBMS",
                            Properties = new System.Collections.Generic.List<StereotypePropertyPersistable>
                            {
                                new StereotypePropertyPersistable
                                {
                                    IsActive = true,
                                    Value = "DEFAULT",
                                    Name = "SQL Data Type",
                                    DefinitionId = "1288cfcd-ee51-437e-9713-73b80118f026"
                                },
                                new StereotypePropertyPersistable
                                {
                                    IsActive = true,
                                    Name = "MaxLength",
                                    DefinitionId = "A04CC24D-81FB-4EA2-A34A-B3C58E04DCFD"
                                },
                                new StereotypePropertyPersistable
                                {
                                    IsActive = true,
                                    Value = "false",
                                    Name = "IsUnicode",
                                    DefinitionId = "67EC4CF4-7706-4B39-BC7C-DF539EE2B0AF"
                                }
                            }
                        }
                    }
            };
        }
    }
}