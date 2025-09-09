using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AI.Blazor.Tasks.Helpers
{
    internal class DictionaryHelper
    {
        public static Dictionary<string, object> MergeDictionaries(
            Dictionary<string, object> baseDict,
            Dictionary<string, object> mergeDict)
        {
            foreach (var kvp in mergeDict)
            {
                if (baseDict.TryGetValue(kvp.Key, out var existingValue) &&
                    existingValue is Dictionary<string, object> existingDict &&
                    kvp.Value is Dictionary<string, object> newDict)
                {
                    MergeDictionaries(existingDict, newDict);
                }
                else
                {
                    baseDict[kvp.Key] = kvp.Value;
                }
            }

            return baseDict;
        }

    }
}
