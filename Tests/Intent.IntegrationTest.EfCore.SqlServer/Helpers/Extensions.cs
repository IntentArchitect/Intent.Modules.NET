namespace Intent.IntegrationTest.EfCore.SqlServer.Helpers;

public static class Extensions
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