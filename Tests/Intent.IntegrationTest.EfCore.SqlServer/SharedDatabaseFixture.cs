using System.Data.Common;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Intent.IntegrationTest.EfCore.SqlServer;

// https://stackoverflow.com/q/65360948/802755

[Collection("NoParallelDb")]
public abstract class SharedDatabaseFixture<TDbContext, TTestClass> : IDisposable
    where TDbContext : DbContext
    where TTestClass : SharedDatabaseFixture<TDbContext, TTestClass> // Make sure that test classes get their fresh schema
{
    private static readonly object Lock = new object();
    private static bool _databaseInitialized;
    private static string _DatabaseName = "Database.Server.Local";
    private static IConfigurationRoot _config;
    private static bool _initialized;

    protected SharedDatabaseFixture(ITestOutputHelper outputHelper)
    {
        if (!_initialized)
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", false, true)
                .Build();
        }

        OutputHelper = outputHelper;

        var connectionStringBuilder = new SqlConnectionStringBuilder
        {
            DataSource = _config.GetValue<string>("DataSource"),
            InitialCatalog = _DatabaseName,
            IntegratedSecurity = true,
            Encrypt = false
        };

        var connectionString = connectionStringBuilder.ToString();
        Connection = new SqlConnection(connectionString);

        if (!_initialized)
        {
            CreateEmptyDatabaseAndSeedData();
        }

        Connection.Open();
        _initialized = true;

        DbContext = CreateContext();
        DbContext.Database.Migrate();
    }

    public virtual void Dispose()
    {
        DbContext?.Dispose();
        Connection.Dispose();
    }

    protected ITestOutputHelper OutputHelper { get; private set; }

    protected TDbContext DbContext { get; private set; }

    private DbConnection Connection { get; set; }

    private TDbContext CreateContext(DbTransaction transaction = null)
    {
        var context = (TDbContext)Activator.CreateInstance(typeof(TDbContext),
            new DbContextOptionsBuilder<TDbContext>()
                .LogTo(OutputHelper.WriteLine)
                .UseSqlServer(Connection)
                .EnableSensitiveDataLogging()
                .Options)!;

        if (transaction != null)
        {
            context.Database.UseTransaction(transaction);
        }

        return context;
    }

    private void CreateEmptyDatabaseAndSeedData()
    {
        lock (Lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    DestroyDatabaseRawSql();
                    CreateDatabaseRawSql();
                    context.Database.EnsureCreated();
                }

                _databaseInitialized = true;
            }
        }
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
        DataSource = _config.GetValue<string>("DataSource"),
        InitialCatalog = "master",
        IntegratedSecurity = true,
        Encrypt = false
    };

    private static string Filename =>
        Path.Combine(Path.GetDirectoryName(typeof(SharedDatabaseFixture<TDbContext, TTestClass>).GetTypeInfo().Assembly.Location)!,
            $"{_DatabaseName}.mdf");

    private static string LogFilename =>
        Path.Combine(Path.GetDirectoryName(typeof(SharedDatabaseFixture<TDbContext, TTestClass>).GetTypeInfo().Assembly.Location)!,
            $"{_DatabaseName}_log.ldf");

    private static void CreateDatabaseRawSql()
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

    private static void DestroyDatabaseRawSql()
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
}