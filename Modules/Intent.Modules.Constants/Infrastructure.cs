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
            public const string ConnectionStringName = nameof(ConnectionStringName);
            public const string ConnectionStringSettingPath = nameof(ConnectionStringSettingPath);
        }
    }

    public static class TableStorage
    {
        public const string Name = nameof(TableStorage);
        public static class Property
        {
            public const string ConnectionStringName = nameof(ConnectionStringName);
            public const string ConnectionStringSettingPath = nameof(ConnectionStringSettingPath);
        }
    }

    public static class Oracle
    {
        public const string Name = nameof(Oracle);
        public static class Property
        {
            public const string ConnectionStringName = nameof(ConnectionStringName);
            public const string ConnectionStringSettingPath = nameof(ConnectionStringSettingPath);
        }
    }

    public static class Redis
    {
        public const string Name = nameof(Redis);

        public static class Property
        {
            public const string ConnectionStringName = nameof(ConnectionStringName);
            public const string ConnectionStringSettingPath = nameof(ConnectionStringSettingPath);
        }
    }

    public static class Kafka
    {
        public const string Name = nameof(Kafka);

        public static class Property
        {
            public const string KafkaSettingsPath = nameof(KafkaSettingsPath);
            public const string KafkaSettingsName = nameof(KafkaSettingsName);
            public const string KafkaSchemaRegistryUrl = nameof(KafkaSchemaRegistryUrl);
        }
    }
}