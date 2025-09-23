using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.Constants.TemplateRoles.Domain;

namespace Intent.Modules.Application.DomainInteractions.Strategies
{
    public class MappingStrategyProvider
    {
        private readonly List<(IMappingStrategy Strategy, int Priority)> _strategies = new List<(IMappingStrategy, int)>();

        public static readonly MappingStrategyProvider Instance = new MappingStrategyProvider();

        public void Register(IMappingStrategy strategy)
        {
            Register(strategy, 0);
        }

        public void Register(IMappingStrategy strategy, int priority)
        {
            _strategies.Add((strategy, priority));
        }

        public bool HasMappingStrategy(ICSharpClassMethodDeclaration method)
        {
            ICSharpClassMethodDeclaration method2 = method;
            List<(IMappingStrategy, int)> list = _strategies.Where<(IMappingStrategy, int)>(((IMappingStrategy Strategy, int Priority) x) => x.Strategy.IsMatch(method2)).ToList();
            if (list.Count > 1)
            {
                Logging.Log.Debug($"Multiple mapping strategies matched for {method2}: [{string.Join(", ", list)}]");
            }

            return list.Count > 0;
        }

        public IMappingStrategy? GetMappingStrategy(ICSharpClassMethodDeclaration method)
        {
            ICSharpClassMethodDeclaration method2 = method;
            List<(IMappingStrategy, int)> list = (from x in _strategies
                                                      orderby x.Priority
                                                      where x.Strategy.IsMatch(method2)
                                                      select x).ToList();
            if (list.Count == 0)
            {
                Logging.Log.Warning($"No mapping strategy matched for {method2}");
                return null;
            }

            if (list.Count > 1)
            {
                Logging.Log.Debug($"Multiple mapping strategies found for {method2}: [{string.Join(", ", list)}]");
            }

            return list[0].Item1;
        }
    }
}
