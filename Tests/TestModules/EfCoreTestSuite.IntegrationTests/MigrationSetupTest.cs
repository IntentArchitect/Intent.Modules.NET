using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using EfCoreTestSuite.IntentGenerated.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Xunit;

namespace EfCoreTestSuite.IntegrationTests;

public class MigrationSetupTest : SharedDatabaseFixture
{
    [Fact]
    public void RunMigrationsTest()
    {
        using (var context = CreateContext())
        {
            context.Database.Migrate();
        }
    }
}