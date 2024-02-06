using Intent.Modules.Common.CSharp.Builder;
using Intent.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.AspNetCore.IntegrationTests.CRUD.FactoryExtensions.EndpointTestImplementationFactoryExtension;

namespace Intent.Modules.AspNetCore.IntegrationTests.CRUD.FactoryExtensions.TestImplementations
{
    public class DependencySortingHelper
    {

        internal static void SortDependencies(IEnumerable<CrudMap> maps)
        {
            List<string> ordering = new List<string>();
            var processed = new Dictionary<string, CrudMap>();
            var toSort = new List<CrudMap>(maps);
            bool changes = true;
            while (toSort.Count > 0 && changes)
            {
                changes = false;
                int i = 0;
                while (i < toSort.Count)
                {
                    if (!toSort[i].Dependencies.Any())
                    {
                        ordering.Add(toSort[i].Entity.Name);
                        processed.Add(toSort[i].Entity.Name, toSort[i]);
                        toSort.RemoveAt(i);
                        changes = true;
                        continue;
                    }
                    bool allDependenciesMet = true;
                    foreach (var dep in toSort[i].Dependencies)
                    {
                        if (!processed.ContainsKey(dep.EntityName) && dep.EntityName != toSort[i].Entity.Name)
                        {
                            allDependenciesMet = false;
                            break;
                        }
                    }
                    if (allDependenciesMet)
                    {
                        ordering.Add(toSort[i].Entity.Name);
                        processed.Add(toSort[i].Entity.Name, toSort[i]);
                        toSort.RemoveAt(i);
                        changes = true;
                        continue;
                    }
                    i++;
                }
            }
            if (toSort.Count > 0)
            {
                Logging.Log.Info($"CRUD Integration Test Generation : Unable to resolve required dependencies {string.Join(",", toSort.Select(x => $"({x.Entity.Name} -> [{string.Join(",", NotMetDependencies(x.Entity.Name, processed, x.Dependencies))}])"))}");
            }
            foreach (var map in maps)
            {
                map.Dependencies = map.Dependencies.OrderBy(x => ordering.IndexOf(x.EntityName)).ToList();
                foreach (var dep in map.Dependencies)
                {
                    if (processed.ContainsKey(dep.EntityName))
                    {
                        dep.CrudMap = processed[dep.EntityName];
                    }
                }
            }
        }

        private static IEnumerable<string> NotMetDependencies(string entityName, Dictionary<string, CrudMap> processed, List<Dependency> dependencies)
        {
            foreach (var dep in dependencies)
            {
                if (!processed.ContainsKey(dep.EntityName) && dep.EntityName != entityName)
                {
                    yield return dep.EntityName;
                }
            }
        }
    }
}
