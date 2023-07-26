namespace Intent.Modules.Constants;

public static class Infrastructure
{
    public static class SqlServer
    {
        public const string Name = nameof(SqlServer);
        public static class Property
        {
            public const string ConnectionStringName = nameof(ConnectionStringName);
            public const string ConnectionStringSettingPath = nameof(ConnectionStringSettingPath);
        }
    }
    
    public static class PostgreSql
    {
        public const string Name = nameof(PostgreSql);
        public static class Property
        {
            public const string ConnectionStringName = nameof(ConnectionStringName);
            public const string ConnectionStringSettingPath = nameof(ConnectionStringSettingPath);
        }
    }
    
    public static class MySql
    {
        public const string Name = nameof(MySql);
        public static class Property
        {
            public const string ConnectionStringName = nameof(ConnectionStringName);
            public const string ConnectionStringSettingPath = nameof(ConnectionStringSettingPath);
        }
    }
    
    public static class CosmosDb
    {
        public const string Name = nameof(CosmosDb);
        public static class Property
        {
            public const string ConnectionStringName = nameof(ConnectionStringName);
            public const string ConnectionStringSettingPath = nameof(ConnectionStringSettingPath);
        }
    }
    
    public static class MongoDb
    {
        public const string Name = nameof(MongoDb);
        public static class Property
        {
            public const string ConnectionStringSettingPath = nameof(ConnectionStringSettingPath);
        }
    }
}