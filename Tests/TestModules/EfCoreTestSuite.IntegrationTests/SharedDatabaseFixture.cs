using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using EfCoreTestSuite.IntentGenerated.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EfCoreTestSuite.IntegrationTests;

// https://stackoverflow.com/q/65360948/802755
// As long as we stick to just testing the migrations in one test we should be fine
public abstract class SharedDatabaseFixture : IDisposable
{
    private static readonly object _lock = new object();
    private static bool _databaseInitialized;
    private static string _DatabaseName = "Database.Server.Local";
    private static IConfigurationRoot config;

    public SharedDatabaseFixture()
    {
        config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", false, true)
            .Build();

        var test = config.GetValue<string>("DataSource");

        var connectionStringBuilder = new SqlConnectionStringBuilder
        {
            DataSource = config.GetValue<string>("DataSource"),
            InitialCatalog = _DatabaseName,
            IntegratedSecurity = true,
        };

        var connectionString = connectionStringBuilder.ToString();
        Connection = new SqlConnection(connectionString);

        CreateEmptyDatabaseAndSeedData();
        Connection.Open();
    }

    public bool ShouldSeedActualData { get; set; } = true;
    public DbConnection Connection { get; set; }

    public ApplicationDbContext CreateContext(DbTransaction transaction = null)
    {
        var context =
            new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(Connection)
                .Options);

        if (transaction != null)
        {
            context.Database.UseTransaction(transaction);
        }

        return context;
    }

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

    private static SqlConnectionStringBuilder Master => new SqlConnectionStringBuilder
    {
        DataSource = config.GetValue<string>("DataSource"),
        InitialCatalog = "master",
        IntegratedSecurity = true
    };

    private static string Filename =>
        Path.Combine(Path.GetDirectoryName(typeof(SharedDatabaseFixture).GetTypeInfo().Assembly.Location),
            $"{_DatabaseName}.mdf");

    private static string LogFilename =>
        Path.Combine(Path.GetDirectoryName(typeof(SharedDatabaseFixture).GetTypeInfo().Assembly.Location),
            $"{_DatabaseName}_log.ldf");

    private static void CreateDatabaseRawSQL()
    {
        ExecuteSqlCommand(Master,
            $@"IF(db_id(N'{_DatabaseName}') IS NULL) BEGIN CREATE DATABASE [{_DatabaseName}] ON (NAME = '{_DatabaseName}', FILENAME = '{Filename}') END");
    }

    private static List<T> ExecuteSqlQuery<T>(SqlConnectionStringBuilder connectionStringBuilder, string queryText,
        Func<SqlDataReader, T> read)
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

    private static void DestroyDatabaseRawSQL()
    {
        var fileNames = ExecuteSqlQuery(Master,
            $@"SELECT [physical_name] FROM [sys].[master_files] WHERE [database_id] = DB_ID('{_DatabaseName}')",
            row => (string)row["physical_name"]);

        if (fileNames.Any())
        {
            ExecuteSqlCommand(Master,
                $@"ALTER DATABASE [{_DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;EXEC sp_detach_db '{_DatabaseName}', 'true'");
            fileNames.ForEach(File.Delete);
        }

        if (File.Exists(Filename))
            File.Delete(Filename);

        if (File.Exists(LogFilename))
            File.Delete(LogFilename);
    }

    private void CreateEmptyDatabaseAndSeedData()
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
                    catch (Exception)
                    {
                    }

                    try
                    {
                        CreateDatabaseRawSQL();
                        context.Database.EnsureCreated();
                    }
                    catch (Exception)
                    {
                    }
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