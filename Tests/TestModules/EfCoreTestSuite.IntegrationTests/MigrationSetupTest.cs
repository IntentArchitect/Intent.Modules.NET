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

public class MigrationSetupTest : IDisposable
{
    private static readonly object _lock = new object();
    private static bool _databaseInitialized;
    private static string _DatabaseName = "Database.Server.Local";

    public MigrationSetupTest()
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder
        {
            DataSource = "localhost",
            InitialCatalog = _DatabaseName,
            IntegratedSecurity = true,
            MultipleActiveResultSets = true,
            ApplicationName = "Unit Test"
        };

        var connectionString = connectionStringBuilder.ToString();
        Connection = new SqlConnection(connectionString);

        CreateEmptyDatabase();
        Connection.Open();
    }
    
    [Fact]
    public void RunMigrationsTest()
    {
        using (var context = CreateContext())
        {
            context.Database.Migrate();
        }
    }

    private static string Filename =>
        Path.Combine(Path.GetDirectoryName(typeof(MigrationSetupTest).GetTypeInfo().Assembly.Location),
            $"{_DatabaseName}.mdf");

    private static string LogFilename =>
        Path.Combine(Path.GetDirectoryName(typeof(MigrationSetupTest).GetTypeInfo().Assembly.Location),
            $"{_DatabaseName}_log.ldf");

    public DbConnection Connection { get; set; }

    private static SqlConnectionStringBuilder Master => new SqlConnectionStringBuilder
    {
        DataSource = "localhost",
        InitialCatalog = "master",
        IntegratedSecurity = true
    };

    private static void ExecuteSqlCommand(SqlConnectionStringBuilder connectionStringBuilder, string commandText)
    {
        using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.ExecuteNonQuery();
            }
        }
    }
    
    private static List<T> ExecuteSqlQuery<T>(SqlConnectionStringBuilder connectionStringBuilder, string queryText, Func<SqlDataReader, T> read)
    {
        var result = new List<T>();

        using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = queryText;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(read(reader));
                    }
                }
            }
        }

        return result;
    }

    private static void CreateDatabaseRawSQL()
    {
        ExecuteSqlCommand(Master,
            $@"IF(db_id(N'{_DatabaseName}') IS NULL) BEGIN CREATE DATABASE [{_DatabaseName}] ON (NAME = '{_DatabaseName}', FILENAME = '{Filename}') END");
    }
    
    private static void DestroyDatabaseRawSQL()
    {
        var fileNames = ExecuteSqlQuery(Master, $@"SELECT [physical_name] FROM [sys].[master_files] WHERE [database_id] = DB_ID('{_DatabaseName}')", row => (string)row["physical_name"]);

        if (fileNames.Any())
        {
            ExecuteSqlCommand(Master, $@"ALTER DATABASE [{_DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;EXEC sp_detach_db '{_DatabaseName}', 'true'");
            fileNames.ForEach(File.Delete);
        }

        if (File.Exists(Filename))
            File.Delete(Filename);

        if (File.Exists(LogFilename))
            File.Delete(LogFilename);
    }
    
    public ApplicationDbContext CreateContext(DbTransaction transaction = null)
    {
        var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(Connection).Options);

        if (transaction != null)
        {
            context.Database.UseTransaction(transaction);
        }
           
        return context;
    }
    
    private void CreateEmptyDatabase()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    try
                    {
                        DestroyDatabaseRawSQL();
                    }
                    catch (Exception) { }

                    try
                    {
                        CreateDatabaseRawSQL();
                        context.Database.EnsureCreated();
                    }
                    catch (Exception) { }
                }

                _databaseInitialized = true;
            }
        }
    }
    
    public void Dispose()
    {
        Connection.Dispose();
    }
}