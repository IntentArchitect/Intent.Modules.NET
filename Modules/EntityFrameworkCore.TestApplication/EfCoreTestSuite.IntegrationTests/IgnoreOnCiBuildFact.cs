using System;
using Xunit;

namespace EfCoreTestSuite.IntegrationTests;

public class IgnoreOnCiBuildFact : FactAttribute
{
    public IgnoreOnCiBuildFact()
    {
        if (string.Equals(Environment.GetEnvironmentVariable("TF_BUILD"), true.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            Skip = "Ignore on BI Build";
        }
    }
}