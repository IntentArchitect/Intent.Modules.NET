using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.OutputCaching.Redis.Migrations
{
    public class Migration_01_01_00_Pre_00_cs : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;
        private readonly string CachingPolicyTypeId = "1425f74f-3aa9-4dfd-8676-baa7e8d00a17";
        private readonly string CachingPoliciesTypeId = "4cd00683-9b6e-4340-a4be-6c6a4e36a274";
        

        public Migration_01_01_00_Pre_00_cs(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public string ModuleId => "Intent.AspNetCore.OutputCaching.Redis";

        public string ModuleVersion => "1.1.0-pre.0";

        public void Down()
        {
        }

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var packages = app.GetDesigners().SelectMany(x => x.GetPackages());

            foreach (var package in packages)
            {
                var elements = GetMatchedElements(package.Classes, p => p.SpecializationTypeId == CachingPolicyTypeId);
                if (!elements.Any())
                {
                    continue;
                }

                var policiesElement = GetMatchedElements(package.Classes, p => p.SpecializationTypeId == CachingPoliciesTypeId).FirstOrDefault();
                if (policiesElement == null)
                {
                    policiesElement = new ElementPersistable();
                    policiesElement.Id = Guid.NewGuid().ToString();
                    policiesElement.Name = "Caching Policies";
                    policiesElement.ParentFolderId = package.Id;
                    policiesElement.SpecializationType = "Caching Policies";
                    policiesElement.SpecializationTypeId = CachingPoliciesTypeId;
                    package.Classes.Add(policiesElement);
                }

                foreach (var element in elements)
                {
                    element.ParentFolderId = policiesElement.Id;
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
