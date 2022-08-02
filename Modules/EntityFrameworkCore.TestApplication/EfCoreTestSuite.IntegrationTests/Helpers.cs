using System.Collections.Generic;

namespace EfCoreTestSuite.IntegrationTests;

public static class Helpers
{
    public const string SkipMessage = null;
    //public const string SkipMessage = "CI/CD not ready";
    
    public static ICollection<T> AddRange<T>(this ICollection<T> collection, IEnumerable<T> sequenceToAdd)
    {
        foreach (var element in sequenceToAdd)
        {
            collection.Add(element);
        }

        return collection;
    }
}