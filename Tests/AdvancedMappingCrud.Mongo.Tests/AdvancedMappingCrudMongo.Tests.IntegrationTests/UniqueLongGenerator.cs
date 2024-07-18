using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMappingCrudMongo.Tests.IntegrationTests
{
    internal class UniqueLongGenerator
    {
        private static long _lastId = 0;

        public static long GetNextId()
        {
            var result = Interlocked.Increment(ref _lastId);
            return result;
        }
    }
}
