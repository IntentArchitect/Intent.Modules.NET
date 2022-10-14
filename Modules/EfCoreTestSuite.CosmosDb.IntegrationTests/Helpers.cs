namespace EfCoreTestSuite.CosmosDb.IntegrationTests;

public static class Helpers
{
    public static ICollection<T> AddRange<T>(this ICollection<T> collection, IEnumerable<T> sequenceToAdd)
    {
        foreach (var element in sequenceToAdd)
        {
            collection.Add(element);
        }

        return collection;
    }
}