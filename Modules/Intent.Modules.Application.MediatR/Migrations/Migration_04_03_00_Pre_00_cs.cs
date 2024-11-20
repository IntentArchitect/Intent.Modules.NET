using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;

namespace Intent.Modules.Application.MediatR.Migrations
{
    public class Migration_04_03_00_Pre_00_cs : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_04_03_00_Pre_00_cs(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public string ModuleId => "Intent.Application.MediatR";

        public string ModuleVersion => "4.3.0-pre.0";

        public void Down()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var packages = app.GetDesigners().SelectMany(x => x.GetPackages());

            foreach (var package in packages)
            {
                var stereotypes = GetMatchedElements(package.Classes, e => e.SpecializationTypeId is Constants.Elements.Command or Constants.Elements.Query)
                    .SelectMany(e => e.Stereotypes.Where(x => x.DefinitionId == Constants.Stereotypes.Security.DefinitionId))
                    .ToArray();

                if (stereotypes.Length == 0)
                {
                    continue;
                }

                foreach (var stereotype in stereotypes)
                {
                    stereotype.DefinitionId = Constants.Stereotypes.Authorize.DefinitionId;
                    stereotype.DefinitionPackageId = Constants.Stereotypes.Authorize.PackageId;
                    stereotype.DefinitionPackageName = Constants.Stereotypes.Authorize.PackageName;

                    foreach (var property in stereotype.Properties)
                    {
                        property.DefinitionId = property.DefinitionId switch
                        {
                            Constants.Stereotypes.Security.Property.Roles => Constants.Stereotypes.Authorize.Property.Roles,
                            Constants.Stereotypes.Security.Property.Policy => Constants.Stereotypes.Authorize.Property.Policy,
                            Constants.Stereotypes.Security.Property.SecurityRoles => Constants.Stereotypes.Authorize.Property.SecurityRoles,
                            Constants.Stereotypes.Security.Property.SecurityPolicies => Constants.Stereotypes.Authorize.Property.SecurityPolicies,
                            _ => throw new InvalidOperationException($"Unknown property definition id: {property.DefinitionId}")
                        };
                    }
                }

                package.Save(true);
            }
        }

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var packages = app.GetDesigners().SelectMany(x => x.GetPackages());

            foreach (var package in packages)
            {
                var stereotypes = GetMatchedElements(package.Classes, e => e.SpecializationTypeId is Constants.Elements.Command or Constants.Elements.Query)
                    .SelectMany(e => e.Stereotypes.Where(x => x.DefinitionId == Constants.Stereotypes.Authorize.DefinitionId))
                    .ToArray();

                if (stereotypes.Length == 0)
                {
                    continue;
                }

                foreach (var stereotype in stereotypes)
                {
                    stereotype.DefinitionId = Constants.Stereotypes.Security.DefinitionId;
                    stereotype.DefinitionPackageId = Constants.Stereotypes.Security.PackageId;
                    stereotype.DefinitionPackageName = Constants.Stereotypes.Security.PackageName;

                    foreach (var property in stereotype.Properties)
                    {
                        property.DefinitionId = property.DefinitionId switch
                        {
                            Constants.Stereotypes.Authorize.Property.Roles => Constants.Stereotypes.Security.Property.Roles,
                            Constants.Stereotypes.Authorize.Property.Policy => Constants.Stereotypes.Security.Property.Policy,
                            Constants.Stereotypes.Authorize.Property.SecurityRoles => Constants.Stereotypes.Security.Property.SecurityRoles,
                            Constants.Stereotypes.Authorize.Property.SecurityPolicies => Constants.Stereotypes.Security.Property.SecurityPolicies,
                            _ => throw new InvalidOperationException($"Unknown property definition id: {property.DefinitionId}")
                        };
                    }
                }

                package.Save(true);
            }
        }

        private static IEnumerable<ElementPersistable> GetMatchedElements(IList<ElementPersistable> elements, Predicate<ElementPersistable> predicate)
        {
            foreach (var element in elements)
            {
                if (predicate(element))
                {
                    yield return element;
                }

                foreach (var result in GetMatchedElements(element.ChildElements, predicate))
                {
                    yield return result;
                }
            }
        }
    }
}
