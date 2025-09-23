using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.Constants.TemplateRoles.Domain;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.Strategies
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

        public bool HasMappingStrategy(ICSharpFileBuilderTemplate template)
        {
            ICSharpFileBuilderTemplate template2 = template;
            List<(IMappingStrategy, int)> list = _strategies.Where<(IMappingStrategy, int)>(((IMappingStrategy Strategy, int Priority) x) => x.Strategy.IsMatch(template2)).ToList();
            if (list.Count > 1)
            {
                Logging.Log.Debug($"Multiple mapping strategies matched for {template2}: [{string.Join(", ", list)}]");
            }

            return list.Count > 0;
        }

        public IMappingStrategy? GetMappingStrategy(ICSharpFileBuilderTemplate template)
        {
            ICSharpFileBuilderTemplate template2 = template;
            List<(IMappingStrategy, int)> list = (from x in _strategies
                                                      orderby x.Priority
                                                      where x.Strategy.IsMatch(template2)
                                                      select x).ToList();
            if (list.Count == 0)
            {
                Logging.Log.Warning($"No mapping strategy matched for {template2}");
                return null;
            }

            if (list.Count > 1)
            {
                Logging.Log.Debug($"Multiple mapping strategies found for {template2}: [{string.Join(", ", list)}]");
            }

            return list[0].Item1;
        }
    }
}
