namespace Intent.Modules.Constants;

public static class InfrastructureComponent
{
    public const string SqlServer = nameof(SqlServer);
    public const string PostgreSql = nameof(PostgreSql);
    public const string MySql = nameof(MySql);
    public const string CosmosDb = nameof(CosmosDb);
    public const string MongoDb = nameof(MongoDb);

    public static class ConnectionDetail
    {
        public const string ConnectionStringName = nameof(ConnectionStringName);
        public const string ConnectionStringSettingPath = nameof(ConnectionStringSettingPath);
        public const string DatabaseName = nameof(DatabaseName);
    }
}